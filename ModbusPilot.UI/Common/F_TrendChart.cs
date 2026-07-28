using ModbusPilot.Core;
using ModbusPilot.Core.Driver;
using ModbusPilot.Core.Models;
using ModbusPilot.Core.Services;
using ScottPlot;
using ScottPlot.WinForms;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing; // WinForms 颜色
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WinFormsTimer = System.Windows.Forms.Timer;
using System.Threading.Tasks;

namespace ModbusPilot.UI.Common
{
    public partial class F_TrendChart : Form
    {
        // === 单例模式 ===
        private static F_TrendChart _instance;
        public static bool HasInstance => _instance != null && !_instance.IsDisposed;
        public static F_TrendChart Instance
        {
            get
            {
                if (_instance == null || _instance.IsDisposed)
                {
                    _instance = new F_TrendChart();
                }
                return _instance;
            }
        }
        // 状态枚举
        private enum RecState { Stopped, Running, Paused }
        private RecState _currentState = RecState.Stopped;

        // 核心数据
        private List<TrendCurve> _curves = new List<TrendCurve>();
        //private ModbusMaster _master;

        // 定时器
        private WinFormsTimer _sampleTimer;
        private WinFormsTimer _renderTimer;

        // ScottPlot 控件
        private FormsPlot _chartPlot;

        // 渲染参数
        private int _maxRenderPoints = 1000; // 默认显示范围
        private bool _autoScroll = true; // 默认开启自动跟随

        // 暗黑主题配色
        private readonly System.Drawing.Color clrBackground = System.Drawing.Color.FromArgb(30, 30, 30);
        private readonly System.Drawing.Color clrPanel = System.Drawing.Color.FromArgb(45, 45, 48);
        private readonly System.Drawing.Color clrText = System.Drawing.Color.WhiteSmoke;

        private int _targetCapacity = 2000; // 计算出的目标缓存点数 (默认为免费版限制)
        private const int SAFE_HARD_LIMIT = 500000; // 程序安全上限 (50万点约占用 8MB 内存/条，防崩溃)
        private const int FREE_MAX_MINUTES = 30;    // 免费版最大时长 (分钟)
        // === 新增：安全限制与控件 ===
        // 50万点 x 8字节(double) ≈ 4MB，加上时间对象，单条曲线约占用 10-15MB 内存
        // 4条曲线合计占用 < 100MB，非常安全。再大容易造成 GC 压力卡顿。
        private const int SAFE_POINT_LIMIT = 500000;

        private F_TrendChart()
        {
            InitializeComponent();
            //_master = master;

            InitDarkTheme();
            InitChart();
            InitTimers();
            InitDragDropGlobal();

            // 绑定事件
            dgvCurves.CurrentCellDirtyStateChanged += DgvCurves_CurrentCellDirtyStateChanged;
            dgvCurves.CellValueChanged += DgvCurves_CellValueChanged;
            dgvCurves.CellClick += DgvCurves_CellClick;

            // 【新增】绑定 Range 滑动条事件
            trackRange.ValueChanged += TrackRange_ValueChanged;

            // 初始化界面显示
            UpdateRangeLabel();

            this.FormClosing += TrendChart_FormClosing;
        }
        private void TrendChart_FormClosing(object? sender, FormClosingEventArgs e)
        {

            // 只有在正在运行 (Running) 或 暂停 (Paused) 状态下才提示
            // 如果已经是 Stopped 状态，直接让它关掉，体验更好
            if (_currentState != RecState.Stopped)
            {
                var result = MessageBox.Show("正在采集数据，退出将丢失当前未导出的波形。\n确认退出吗？",
                    "退出确认", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (result == DialogResult.No)
                {
                    e.Cancel = true; // 拦截掉，不让关
                    return;
                }
            }

            // --- 如果走到这里，说明用户选了 Yes 或者当前没在采集 ---

            // 关键点：不要写 this.Close()！

            // 彻底释放资源 (清理定时器，防止后台继续跑)
            _sampleTimer?.Stop();
            _renderTimer?.Stop();
        }

        private void InitDarkTheme()
        {
            this.BackColor = clrBackground;
            this.ForeColor = clrText;

            // 表格样式
            dgvCurves.BackgroundColor = clrPanel;
            dgvCurves.DefaultCellStyle.BackColor = clrPanel;
            dgvCurves.DefaultCellStyle.ForeColor = clrText;
            dgvCurves.DefaultCellStyle.SelectionBackColor = System.Drawing.Color.FromArgb(60, 60, 60);
            dgvCurves.ColumnHeadersDefaultCellStyle.BackColor = System.Drawing.Color.Black;
            dgvCurves.ColumnHeadersDefaultCellStyle.ForeColor = System.Drawing.Color.White;
            dgvCurves.EnableHeadersVisualStyles = false;
            dgvCurves.GridColor = System.Drawing.Color.FromArgb(60, 60, 60);

            // 按钮样式基础设置
            foreach (Control c in pnlBottom.Controls)
            {
                if (c is Button btn)
                {
                    btn.FlatStyle = FlatStyle.Flat;
                    btn.BackColor = System.Drawing.Color.FromArgb(60, 60, 60);
                    btn.ForeColor = System.Drawing.Color.White;
                    btn.FlatAppearance.BorderColor = System.Drawing.Color.Gray;
                    btn.FlatAppearance.BorderSize = 1;
                    btn.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(80, 80, 80);
                    btn.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(40, 40, 40);
                }
            }

            // 初始化按钮状态 (三色灯逻辑)
            UpdateButtonState();
        }

        private void InitChart()
        {
            _chartPlot = new FormsPlot();
            _chartPlot.Dock = DockStyle.Fill;
            pnlChart.Controls.Add(_chartPlot);

            // --- 背景色 ---
            _chartPlot.Plot.FigureBackground.Color = ScottPlot.Color.FromHex("#1E1E1E");
            _chartPlot.Plot.DataBackground.Color = ScottPlot.Color.FromHex("#252526");
            _chartPlot.Plot.Grid.MajorLineColor = ScottPlot.Color.FromHex("#333333");

            // --- 字体与轴颜色 ---
            ScottPlot.Color axisColor = ScottPlot.Color.FromHex("#D0D0D0");
            string fontName = "Microsoft YaHei";

            foreach (var axis in _chartPlot.Plot.Axes.GetAxes())
            {
                axis.Label.FontName = fontName;
                axis.Label.ForeColor = axisColor;
                axis.TickLabelStyle.ForeColor = axisColor;
                axis.TickLabelStyle.FontName = fontName;
                axis.FrameLineStyle.Color = axisColor;
            }

            _chartPlot.Plot.Axes.Bottom.Label.Text = "Time";
            _chartPlot.Plot.Axes.Left.Label.Text = "Value";

            // 设置 X 轴为时间格式
            //_chartPlot.Plot.Axes.Bottom.TickGenerator = new ScottPlot.TickGenerators.DateTimeAutomatic();
            ScottPlot.TickGenerators.NumericAutomatic tickGen = new();
            tickGen.LabelFormatter = (value) =>
            {
                // 将 double (OADate) 转回 DateTime
                try
                {
                    return DateTime.FromOADate(value).ToString("HH:mm:ss");
                }
                catch
                {
                    return "";
                }
            };
            _chartPlot.Plot.Axes.Bottom.TickGenerator = tickGen;

            // --- 图例样式 ---
            _chartPlot.Plot.HideLegend();

            _chartPlot.MouseDown += (s, e) => _autoScroll = false; // 用户一点，就停止跟随
            _chartPlot.MouseWheel += (s, e) => _autoScroll = false; // 用户一滚，就停止跟随
            _chartPlot.DoubleClick += (s, e) =>
            {
                _autoScroll = true;
                _chartPlot.Plot.Axes.AutoScale(); // 立即归位
            };
        }

        private void InitTimers()
        {
            _sampleTimer = new WinFormsTimer { Interval = 1000 };
            _sampleTimer.Tick += SampleTimer_Tick;

            _renderTimer = new WinFormsTimer { Interval = 500 }; // 刷新频率固定，不受采样影响
            _renderTimer.Tick += (s, e) => RefreshChart();
        }

        // ====================================================================
        // 核心采样逻辑
        // ====================================================================
        private void SampleTimer_Tick(object sender, EventArgs e)
        {
            var now = DateTime.Now;

            foreach (var curve in _curves)
            {
                double value = 0;
                object rawValue = curve.Point.CurrentValue; // 拿到原始值
                if (rawValue != null)
                {
                    // --- 核心修复逻辑 ---
                    if (rawValue is bool b)
                    {
                        value = b ? 1.0 : 0.0; // 如果是 bool，直接转 1/0
                    }
                    else
                    {
                        // 如果是数字类型，尝试解析
                        double.TryParse(rawValue.ToString(), out value);
                    }
                }

                curve.TimeData.Add(now);
                curve.ValueData.Add(value);

                // 更新当前值缓存
                curve.CurrentValue = value;
            }
        }

        // ====================================================================
        // 核心渲染逻辑 (滑动窗口)
        // ====================================================================
        private void RefreshChart()
        {
            try
            {
                _chartPlot.Plot.Clear();
                bool hasRightCurve = false;

                // 遍历曲线
                for (int i = 0; i < _curves.Count; i++)
                {
                    var curve = _curves[i];

                    // 1. 更新表格里的当前值
                    if (i < dgvCurves.Rows.Count)
                    {
                        var row = dgvCurves.Rows[i];
                        bool isVisible = Convert.ToBoolean(row.Cells[colVisible.Index].Value);


                        if (curve.Point.DataType == DataType.Bool)
                        {
                            row.Cells[colValue.Index].Value = curve.CurrentValue > 0.5 ? "1" : "0";
                        }
                        else
                        {
                            row.Cells[colValue.Index].Value = curve.CurrentValue.ToString("F2");
                        }


                        if (!isVisible) continue; // 不勾选就不画图
                    }

                    if (curve.TimeData.Count == 0) continue;

                    // 2. 准备绘图数据 (应用滑动窗口 _maxRenderPoints)
                    // -----------------------------------------------------
                    var allTimes = curve.TimeData.GetAll();
                    var allValues = curve.ValueData.GetAll();

                    int totalCount = curve.TimeData.Count;
                    int skipCount = 0;

                    // 如果数据太多，跳过前面的，只取最后 N 个
                    if (totalCount > _maxRenderPoints)
                    {
                        skipCount = totalCount - _maxRenderPoints;
                    }

                    // 使用 Skip().Take() 切片
                    double[] xs = allTimes.Skip(skipCount).Select(t => t.ToOADate()).ToArray();
                    double[] ys = allValues.Skip(skipCount).ToArray();
                    // -----------------------------------------------------

                    var scatter = _chartPlot.Plot.Add.Scatter(xs, ys);
                    scatter.Label = curve.DisplayName;
                    scatter.Color = ScottPlot.Color.FromColor(curve.LineColor);
                    scatter.LineWidth = 2;

                    // 【新增】显示离散点 (Marker)
                    scatter.MarkerSize = 5;
                    // scatter.MarkerShape = MarkerShape.FilledCircle; // 默认就是圆点

                    if (curve.AxisSide == YAxisSide.Right)
                    {
                        scatter.Axes.YAxis = _chartPlot.Plot.Axes.Right;
                        hasRightCurve = true;
                    }
                }

                // 右轴显示控制
                if (hasRightCurve)
                {
                    _chartPlot.Plot.Axes.Right.TickLabelStyle.IsVisible = true;
                    _chartPlot.Plot.Axes.Right.FrameLineStyle.IsVisible = true;
                }

                // 只有在开启自动跟随的时候，才强制缩放
                if (_autoScroll)
                {
                    _chartPlot.Plot.Axes.AutoScale();
                }
                _chartPlot.Refresh();
            }
            catch { }

            // 更新底部状态栏 (取第一条曲线的点数)
            int totalPoints = _curves.Count > 0 ? _curves[0].TimeData.Count : 0;

            // 估算时长
            double intervalSec = (double)numInterval.Value / 1000.0;
            TimeSpan span = TimeSpan.FromSeconds(totalPoints * intervalSec);

            // 显示已采集点数和时长
            //lblStatus.Text = $"已采: {totalPoints} pts | 时长: {span:hh\\:mm\\:ss}";
            // 自动适配显示格式 (hh:mm:ss 或者 d.hh:mm:ss)
            string timeStr = span.ToString(@"dd\.hh\:mm\:ss"); // 例如 00.00:05:30
            if (span.TotalDays < 1) timeStr = span.ToString(@"hh\:mm\:ss"); // 例如 00:05:30

            lblStatus.Text = $"已采: {totalPoints} pts | 总时长: {timeStr}";
        }

        // ====================================================================
        // 按钮状态逻辑 (三色交通灯)
        // ====================================================================
        private void UpdateButtonState()
        {
            System.Drawing.Color colorGreen = System.Drawing.Color.FromArgb(30, 160, 80);
            System.Drawing.Color colorYellow = System.Drawing.Color.FromArgb(200, 140, 0);
            System.Drawing.Color colorRed = System.Drawing.Color.FromArgb(180, 60, 60);

            // 【修改】调亮禁用背景色，解决文字看不清的问题
            System.Drawing.Color colorDisabled = System.Drawing.Color.FromArgb(80, 80, 80);
            System.Drawing.Color textDisabled = System.Drawing.Color.Gray;
            System.Drawing.Color textNormal = System.Drawing.Color.White;

            switch (_currentState)
            {
                case RecState.Stopped:
                    SetBtnStyle(btnStart, true, colorGreen, textNormal, "▶️ 开始");
                    SetBtnStyle(btnPause, false, colorDisabled, textDisabled, "⏸️ 暂停");
                    SetBtnStyle(btnClear, false, colorDisabled, textDisabled, "⏹️ 停止");
                    numInterval.Enabled = true; // 允许改周期
                    break;

                case RecState.Running:
                    SetBtnStyle(btnStart, false, colorDisabled, textDisabled, "▶️ 运行中...");
                    SetBtnStyle(btnPause, true, colorYellow, textNormal, "⏸️ 暂停");
                    SetBtnStyle(btnClear, true, colorRed, textNormal, "⏹️ 停止");
                    numInterval.Enabled = false; // 禁止改周期
                    break;

                case RecState.Paused:
                    SetBtnStyle(btnStart, true, colorGreen, textNormal, "▶️ 继续");
                    SetBtnStyle(btnPause, false, colorDisabled, textDisabled, "⏸️ 已暂停");
                    SetBtnStyle(btnClear, true, colorRed, textNormal, "⏹️ 停止");
                    numInterval.Enabled = false; // 暂停时也禁止改周期
                    break;
            }
        }

        private void SetBtnStyle(Button btn, bool enabled, System.Drawing.Color backColor, System.Drawing.Color foreColor, string text)
        {
            btn.Enabled = enabled;
            btn.BackColor = backColor;
            btn.ForeColor = foreColor;
            btn.Text = text;
            btn.FlatAppearance.BorderSize = enabled ? 1 : 0;
        }

        // ====================================================================
        // Range 滑动条逻辑
        // ====================================================================
        private void TrackRange_ValueChanged(object sender, EventArgs e)
        {
            UpdateRangeLabel();
            // 立即刷新图表，让用户看到视野缩放的效果
            RefreshChart();
        }

        private void UpdateRangeLabel()
        {
            _maxRenderPoints = trackRange.Value;

            // 估算这个 Range 代表的时间窗口
            double intervalSec = (double)numInterval.Value / 1000.0;
            double timeWindowSec = _maxRenderPoints * intervalSec;

            string timeStr = "";
            if (timeWindowSec < 60) timeStr = $"{timeWindowSec:F0}s";
            else timeStr = $"{timeWindowSec / 60:F1}m";

            //lblRangeValue.Text = $"{_maxRenderPoints} pts (~{timeStr})";
            // 修改文案，强调这是“显示范围”
            lblRangeValue.Text = $"视野: {_maxRenderPoints} 点 (最近 {timeStr})";
        }

        private void BtnStart_Click(object sender, EventArgs e)
        {
            if (_currentState == RecState.Stopped)
            {
                // =========================================================
                // 1. 算账：根据用户输入计算需要多少个点
                // =========================================================
                int intervalMs = (int)numInterval.Value;
                int durationMin = (int)numDuration.Value;

                // 2. 授权拦截：如果是免费版，强制锁定最大时长
                // -------------------------------------------------------------
                // 2. 授权拦截 (License Guard)
                // -------------------------------------------------------------
                // 调用你提供的静态方法。如果 Check 返回 false，说明用户取消了支付/不是Pro版
                // 此时强制回滚到免费版上限 (30分钟)
                if (!LicenseGuard.CanSetTrendDuration(durationMin))
                {
                    // 既然被拦截了，就强制改为免费版上限
                    durationMin = LicenseGuard.MAX_FREE_TREND_MINUTES;
                    numDuration.Value = durationMin; // 界面回显修正，告诉用户“你只能用这么多”
                }

                // 3. 计算目标点数 = (分钟 * 60 * 1000) / 毫秒
                long calculatedPoints = (long)durationMin * 60 * 1000 / intervalMs;

                // 4. 安全熔断：防止数值溢出或内存爆炸
                if (calculatedPoints > SAFE_POINT_LIMIT)
                {
                    calculatedPoints = SAFE_POINT_LIMIT;
                    // 反算能录多久
                    double safeMinutes = (double)SAFE_POINT_LIMIT * intervalMs / 1000.0 / 60.0;
                    MessageBox.Show($"基于当前程序性能保护设置，最大缓存点数限制为 {SAFE_POINT_LIMIT:N0}。\n" +
                           $"当前频率下，最大录制时长已自动调整为约 {safeMinutes:F1} 分钟。",
                           "性能安全提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }

                _targetCapacity = (int)calculatedPoints;

                // 5. 应用设置
                _sampleTimer.Interval = intervalMs;

                // 确保数据清空
                // 遍历所有曲线，按照计算出的容量 "重塑" 容器
                foreach (var curve in _curves)
                {
                    curve.ResizeBuffer(_targetCapacity);
                }

                // 更新状态栏，让用户知道当前的限制
                lblStatus.Text = $"运行中 | 缓存深度: {_targetCapacity} 点 ({(_targetCapacity * intervalMs / 1000 / 60)} 分钟)";
            }

            _currentState = RecState.Running;
            _sampleTimer.Start();
            _renderTimer.Start();
          
            // 锁定输入框，防止运行时修改导致逻辑混乱
            numInterval.Enabled = false;
            numDuration.Enabled = false;

            UpdateButtonState();
        }

        private void BtnPause_Click(object sender, EventArgs e)
        {
            _currentState = RecState.Paused;
            _sampleTimer.Stop();
            UpdateButtonState();
        }

        private void BtnClear_Click(object sender, EventArgs e) // Stop 按钮
        {
            if (_currentState != RecState.Stopped)
            {
                if (MessageBox.Show("停止将清除当前波形数据。\n确定要停止吗？", "停止确认", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No)
                    return;
            }

            _currentState = RecState.Stopped;
            _sampleTimer.Stop();
            _renderTimer.Stop();

            // 强制清空
            foreach (var c in _curves) { c.TimeData.Clear(); c.ValueData.Clear(); }
            _chartPlot.Plot.Clear();
            _chartPlot.Refresh();

            lblStatus.Text = "数据点: 0 | 时长: 00:00:00";

            // 【新增】解锁输入框
            numInterval.Enabled = true;
            numDuration.Enabled = true; // 解锁时长设置

            UpdateButtonState();
        }

        // ====================================================================
        // 拖拽与曲线管理
        // ====================================================================
        private void InitDragDropGlobal()
        {
            this.AllowDrop = true;
            BindDragEvents(this);
        }

        private void BindDragEvents(Control ctl)
        {
            ctl.AllowDrop = true;
            ctl.DragEnter += (s, e) => {
                if (e.Data.GetDataPresent(typeof(TrendDragData))) e.Effect = DragDropEffects.Copy;
                else e.Effect = DragDropEffects.None;
            };
            ctl.DragDrop += (s, e) => {
                var data = e.Data.GetData(typeof(TrendDragData)) as TrendDragData;
                if (data != null) AddCurve(data);
            };
            foreach (Control c in ctl.Controls) BindDragEvents(c);
        }

        private void AddCurve(TrendDragData data)
        {
            // 1. 【新增】首先检查是否已经重复
            if (_curves.Any(c => c.DeviceName == data.DeviceName && c.Point.Name == data.Point.Name))
            {
                // 增加明确的弹窗提示
                MessageBox.Show($"点位 [{data.Point.Name}] 已经在曲线列表中，请勿重复添加。",
                                "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (!LicenseGuard.CanAddTrendCurve(_curves.Count)) return;


            // 3. 授权通过且不重复，执行添加
            var curve = new TrendCurve(_targetCapacity)
            {
                DeviceName = data.DeviceName,
                SlaveId = data.SlaveId,
                Point = data.Point,
                LineColor = GetNextNeonColor(),
                AxisSide = YAxisSide.Left
            };

            _curves.Add(curve);
            UpdateCurveList();
        }

        private void UpdateCurveList()
        {
            dgvCurves.Rows.Clear();
            foreach (var curve in _curves)
            {
                int idx = dgvCurves.Rows.Add();
                var row = dgvCurves.Rows[idx];

                row.Cells[colVisible.Index].Value = true;
                row.Cells[colColor.Index].Style.BackColor = curve.LineColor;
                row.Cells[colColor.Index].Style.SelectionBackColor = curve.LineColor;
                row.Cells[colName.Index].Value = curve.DisplayName;
                row.Cells[colAxis.Index].Value = (curve.AxisSide == YAxisSide.Left) ? "左轴" : "右轴";
                row.Tag = curve;
            }

            lblCurveCount.Text = $"曲线列表 ({_curves.Count}/4)";
            lblDragTip.Visible = (_curves.Count == 0);
            _chartPlot.Visible = (_curves.Count > 0);
        }

        private void RemoveCurve(Guid curveId)
        {
            _curves.RemoveAll(c => c.Id == curveId);
            UpdateCurveList();
            RefreshChart();
        }

        // ====================================================================
        // 表格交互
        // ====================================================================
        private void DgvCurves_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex == colAxis.Index)
            {
                dgvCurves.BeginEdit(true);
                if (dgvCurves.EditingControl is ComboBox combo) combo.DroppedDown = true;
            }
            if (e.RowIndex >= 0 && dgvCurves.Columns[e.ColumnIndex] == colDelete)
            {
                var curve = dgvCurves.Rows[e.RowIndex].Tag as TrendCurve;
                if (curve != null) RemoveCurve(curve.Id);
            }
        }
        // ====================================================================
        // 补充缺失的事件实现
        // ====================================================================
        private void DgvCurves_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // 防止点击表头触发
            if (e.RowIndex < 0) return;

            // 判断是否点击了“删除”按钮列 (colDelete)
            if (dgvCurves.Columns[e.ColumnIndex] == colDelete)
            {
                // 获取绑定的曲线对象
                var curve = dgvCurves.Rows[e.RowIndex].Tag as TrendCurve;
                if (curve != null)
                {
                    // 执行删除逻辑
                    RemoveCurve(curve.Id);
                }
            }
            // 处理复选框的立即提交 (让 CheckBox 点了马上生效，不用点别处)
            else if (dgvCurves.Columns[e.ColumnIndex] == colVisible)
            {
                dgvCurves.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }
        }
        private void DgvCurves_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;
            var row = dgvCurves.Rows[e.RowIndex];
            if (row.Tag is TrendCurve curve)
            {
                if (e.ColumnIndex == colVisible.Index) RefreshChart();
                else if (e.ColumnIndex == colAxis.Index)
                {
                    var val = row.Cells[colAxis.Index].Value?.ToString();
                    curve.AxisSide = (val == "右轴") ? YAxisSide.Right : YAxisSide.Left;
                    RefreshChart();
                }
            }
        }

        private void DgvCurves_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (dgvCurves.IsCurrentCellDirty) dgvCurves.CommitEdit(DataGridViewDataErrorContexts.Commit);
        }

        // ====================================================================
        // 导出功能
        // ====================================================================
        private async void BtnExport_Click(object sender, EventArgs e)
        {
            // 2. 授权拦截 (保留你的逻辑)
            if (!LicenseGuard.CanExportData()) return;

            // 1. 简单校验
            int totalPoints = _curves.Sum(c => c.TimeData.Count);
            if (totalPoints == 0)
            {
                MessageBox.Show("当前没有数据可导出。", "提示");
                return;
            }


            // 3. 选择保存路径 (必须在 UI 线程完成)
            string filePath = "";
            using (var sfd = new SaveFileDialog())
            {
                sfd.Filter = "CSV文件|*.csv";
                sfd.FileName = $"Trend_{DateTime.Now:yyyyMMdd_HHmm}.csv";
                if (sfd.ShowDialog() != DialogResult.OK) return;
                filePath = sfd.FileName;
            }

            // 4. 进入忙碌状态
            btnExport.Enabled = false;
            btnExport.Text = "导出中...";
            this.Cursor = Cursors.WaitCursor; // 鼠标变转圈圈

            try
            {
                var curvesCopy = _curves.ToList();
                // 5. 【核心】放到后台线程执行，解放 UI
                await Task.Run(() => ExportToCsvLogic(filePath, curvesCopy));

                MessageBox.Show($"导出成功！\n共导出约 {totalPoints} 个数据点。", "完成", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("导出失败: " + ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                // 6. 恢复 UI 状态
                btnExport.Enabled = true;
                btnExport.Text = "💾 导出";
                this.Cursor = Cursors.Default;
            }
        }

        private void ExportToCsv()
        {
            using (var sfd = new SaveFileDialog())
            {
                sfd.Filter = "CSV文件|*.csv";
                sfd.FileName = $"Trend_{DateTime.Now:yyyyMMdd_HHmm}.csv";

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        var sb = new StringBuilder();
                        sb.AppendLine("Time," + string.Join(",", _curves.Select(c => c.DisplayName)));

                        var allTimes = _curves.SelectMany(c => c.TimeData.GetAll())
                                             .Distinct().OrderBy(t => t).ToList();

                        foreach (var time in allTimes)
                        {
                            sb.Append(time.ToString("yyyy-MM-dd HH:mm:ss.fff")); // 毫秒精度
                            foreach (var curve in _curves)
                            {
                                var times = curve.TimeData.GetAll().ToList();
                                var values = curve.ValueData.GetAll().ToList();
                                int idx = times.IndexOf(time);
                                sb.Append("," + (idx >= 0 ? values[idx].ToString() : ""));
                            }
                            sb.AppendLine();
                        }
                        System.IO.File.WriteAllText(sfd.FileName, sb.ToString(), Encoding.UTF8);
                        MessageBox.Show("导出成功！");
                    }
                    catch (Exception ex) { MessageBox.Show("导出失败: " + ex.Message); }
                }
            }
        }

        // 纯逻辑方法，运行在后台线程
        private void ExportToCsvLogic(string filePath, List<TrendCurve> curvesCopy)
        {
            // 1. 先把所有曲线的数据“快照”取出来，存进局部变量
            // 这样在后面的耗时循环中，即便 UI 删除了曲线或定时器加了点，也不会报错
            var curveSnapshots = curvesCopy.Select(c => new {
                c.DisplayName,
                Times = c.TimeData.ToArray(), // 拿到快照数组
                Values = c.ValueData.ToArray()
            }).ToList();


            // 使用 StreamWriter 流式写入，内存占用极低，速度极快
            using (var sw = new System.IO.StreamWriter(filePath, false, Encoding.UTF8))
            {
                // 1. 写表头
                var header = "Time," + string.Join(",", _curves.Select(c => c.DisplayName));
                sw.WriteLine(header);

                // 2. 整理时间轴 (合并所有曲线的时间点，去重并排序)
                // 注意：这里可能会比较耗时，但在后台线程跑没关系
                var allTimes = _curves.SelectMany(c => c.TimeData.GetAll())
                                      .Distinct()
                                      .OrderBy(t => t)
                                      .ToList();

                // 3. 逐行写入数据
                foreach (var time in allTimes)
                {
                    var sb = new StringBuilder();
                    sb.Append("\t" + time.ToString("yyyy-MM-dd HH:mm:ss.fff")); // 毫秒精度

                    foreach (var snap in curveSnapshots)
                    {
                        // 在快照中查找，绝对不会报错
                        int idx = Array.IndexOf(snap.Times, time);
                        sb.Append(",");
                        if (idx >= 0) sb.Append(snap.Values[idx]);
                    }
                    sw.WriteLine(sb.ToString());
                }
            }
        }
        
        private int _colorSeed = 0; // 类成员变量，用于确保颜色分配的唯一性
        private System.Drawing.Color GetNextNeonColor()
        {
            // 精选 20 种荧光色，避开了深蓝色和暗色，确保在黑底上清晰可见
            System.Drawing.Color[] neonPalette = {
                System.Drawing.Color.Cyan,          // 青色
                System.Drawing.Color.Lime,          // 荧光绿
                System.Drawing.Color.Magenta,       // 洋红
                System.Drawing.Color.Yellow,        // 纯黄
                System.Drawing.Color.Orange,        // 橙色
                System.Drawing.Color.DeepPink,      // 深粉
                System.Drawing.Color.SpringGreen,   // 春绿
                System.Drawing.Color.RebeccaPurple,// 电力紫 (#9F00FF)
                System.Drawing.Color.Turquoise,     // 绿松石色
                System.Drawing.Color.Gold,           // 金色
                System.Drawing.Color.Aqua,           // 水色
                System.Drawing.Color.Chartreuse,     // 查特酒绿 (黄绿)
                System.Drawing.Color.DeepSkyBlue,    // 深天蓝
                System.Drawing.Color.HotPink,        // 亮粉
                System.Drawing.Color.MediumSpringGreen, // 中春绿
                System.Drawing.Color.YellowGreen,    // 黄绿色
                System.Drawing.Color.Orchid,         // 兰花紫
                System.Drawing.Color.Coral,          // 珊瑚色
                System.Drawing.Color.LightSeaGreen,  // 浅海绿
                System.Drawing.Color.Tomato          // 番茄红
            };

            // 使用 _colorSeed 而不是 _curves.Count
            // 理由：如果你加了3条线，删掉第2条再加1条，Count还是3，用Count会导致新线和第3条线重色。
            // 用 Seed 能保证即便频繁增删，颜色也会循环轮替。
            return neonPalette[_colorSeed++ % neonPalette.Length];
        }
    }
}