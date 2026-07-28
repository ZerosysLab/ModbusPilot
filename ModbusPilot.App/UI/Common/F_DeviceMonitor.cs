using ModbusPilot.Core;
using ModbusPilot.Core.Driver;
using ModbusPilot.Core.Models;
using ModbusPilot.Core.Services; // 引用 ValueEncoder
using System;
using System.Drawing;
using System.Windows.Forms;

namespace ModbusPilot.UI.Common
{
    public partial class F_DeviceMonitor : F_BaseForm
    {
        public DeviceConfig Device { get; private set; }
        private ModbusMaster _master;

        // 缓存常用的样式对象，避免频繁创建
        private readonly DataGridViewCellStyle _styleOn;
        private readonly DataGridViewCellStyle _styleOff;
        private readonly DataGridViewCellStyle _styleBtnDisabled;

        private Rectangle _dragBoxFromMouseDown;
        private int _rowIndexFromMouseDown;

        // 地址格式状态
        private bool _showHexAddress = false;
        private bool _showPlcAddress = true;
        // --- 在类顶部添加字段 ---
        private ContextMenuStrip _ctxMenu;
        private ToolStripMenuItem _tsmiBatchWrite;

        // 【新增】用于存储当前所属通道名
        public string CurrentChannelName { get; set; }

        public F_DeviceMonitor(DeviceConfig device, ModbusMaster master)
        {
            InitializeComponent();

            this.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);

            Device = device;
            _master = master;

            this.Text = $"设备监控 - {device.DeviceName} (ID: {device.SlaveId})";

            // 初始化样式
            _styleOn = new DataGridViewCellStyle { BackColor = Color.LimeGreen, ForeColor = Color.White, Font = new Font("Consolas", 10, FontStyle.Bold), Alignment = DataGridViewContentAlignment.MiddleCenter };
            _styleOff = new DataGridViewCellStyle { BackColor = Color.White, ForeColor = Color.LightGray, Alignment = DataGridViewContentAlignment.MiddleCenter };

            // 禁用按钮样式
            _styleBtnDisabled = new DataGridViewCellStyle { BackColor = Color.LightGray, ForeColor = Color.Gray, SelectionBackColor = Color.LightGray, SelectionForeColor = Color.Gray };

            // 【新增 1】开启多选模式
            dgv.MultiSelect = true;

            // 【新增 2】设置为整行选择 (体验更好，点任意单元格都选中整行)
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;


            InitDragDrop();
            InitContextMenu();

            // 【新增】应用语言设置
            ApplyUIText();

            LoadPoints();
            RefreshData();

            // 绑定事件
            dgv.CellContentClick += Dgv_CellContentClick;

            // 【关键】必须绑定这个事件才能让 ButtonColumn 实现“禁用”效果（视觉上）
            dgv.CellPainting += Dgv_CellPainting;

        }

        private void InitDragDrop()
        {
            dgv.MouseDown += Dgv_MouseDown;
            dgv.MouseMove += Dgv_MouseMove;
            dgv.MouseUp += Dgv_MouseUp;
            dgv.CellMouseClick += dgv_CellMouseClick;
        }
        // --- 新增初始化方法 ---
        private void InitContextMenu()
        {
            _ctxMenu = new ContextMenuStrip();
            _tsmiBatchWrite = new ToolStripMenuItem("批量写入");
            // 增加图标更美观 (可选)
            _tsmiBatchWrite.Image = SystemIcons.Shield.ToBitmap();
            _tsmiBatchWrite.Click += TsmiBatchWrite_Click;
            _ctxMenu.Items.Add(_tsmiBatchWrite);
        }
        private void LoadPoints()
        {
            dgv.Rows.Clear();
            foreach (var p in Device.Points)
            {
                int idx = dgv.Rows.Add();
                var row = dgv.Rows[idx];

                row.Cells[colName.Name].Value = p.Name;
                row.Cells[colAddr.Name].Value = FormatAddress(p.Address, p.Zone);
                row.Cells[colUnit.Name].Value = p.Unit;
                row.Cells[colValue.Name].Value = "-";
                row.Tag = p;

                // 【修改这里】定义什么是"不可写"
                bool isReadOnlyZone = (p.Zone == StorageZone.InputStatus_1x || p.Zone == StorageZone.InputRegister_3x);
                bool isRegBit = (p.Zone == StorageZone.HoldingRegister_4x && p.DataType == DataType.Bool);

                // 如果是只读区，或者是不支持的寄存器位 -> 禁用
                if (isReadOnlyZone || isRegBit)
                {
                    row.Cells[colInput.Name].ReadOnly = true;
                    row.Cells[colInput.Name].Style.BackColor = Color.WhiteSmoke;

                    // 提示文字差异化
                    if (isReadOnlyZone)
                        row.Cells[colInput.Name].Value = LangProvider.Get("Mon_Val_ReadOnly");
                    else
                        row.Cells[colInput.Name].Value = LangProvider.Get("Mon_Val_NotSup");
                }
            }
        }

        public void RefreshData()
        {
            if (this.IsDisposed || !this.Visible) return;

            foreach (DataGridViewRow row in dgv.Rows)
            {
                if (row.Tag is ModbusPoint p)
                {
                    object rawVal = p.CurrentValue;
                    string displayVal = "-";
                    bool isBool = p.DataType == DataType.Bool;

                    // 1. 格式化数值
                    if (rawVal != null)
                    {
                        if (isBool)
                        {
                            bool b = Convert.ToBoolean(rawVal);
                            displayVal = b ? "1" : "0";
                        }
                        else if (rawVal is double || rawVal is float)
                        {
                            displayVal = Convert.ToDouble(rawVal).ToString("0.##"); // 保留2位小数
                        }
                        else
                        {
                            displayVal = rawVal.ToString();
                        }
                    }

                    // 2. 更新单元格值 (仅变化时)
                    var cell = row.Cells[colValue.Name];
                    if (cell.Value?.ToString() != displayVal)
                    {
                        cell.Value = displayVal;
                    }

                    // 3. 【核心需求】Bool 状态变色
                    if (isBool && rawVal != null)
                    {
                        bool b = Convert.ToBoolean(rawVal);
                        // 如果是 1 (True) -> 绿色背景 + 白色字
                        // 如果是 0 (False) -> 白色背景 + 灰色字
                        cell.Style = b ? _styleOn : _styleOff;
                    }
                    else if (!isBool)
                    {
                        // 非 Bool 类型恢复默认蓝色样式 (Designer 里设的那个)
                        cell.Style = colValue.DefaultCellStyle;
                    }
                }
            }
        }

        // 处理点击
        private void Dgv_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || dgv.Columns[e.ColumnIndex].Name != colBtn.Name) return;

            var row = dgv.Rows[e.RowIndex];
            var p = row.Tag as ModbusPoint;

            if (p == null) return;

            // 拦截不支持的类型
            bool isRegisterBool = (p.Zone == StorageZone.HoldingRegister_4x && p.DataType == DataType.Bool);
            bool isReadOnlyZone = (p.Zone == StorageZone.InputStatus_1x || p.Zone == StorageZone.InputRegister_3x);

            if (isRegisterBool || isReadOnlyZone) return;

            DoWrite(row);
        }
        private void dgv_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            // 只处理右键，且点击的是有效行（不是表头）
            if (e.Button == MouseButtons.Right && e.RowIndex >= 0)
            {
                // 1. 检查右键点击的这一行，之前是否已经被选中了？
                bool isAlreadySelected = dgv.Rows[e.RowIndex].Selected;

                if (!isAlreadySelected)
                {
                    // 情况 A：点击了一个新行 -> 清除旧选中，只选中当前行
                    dgv.ClearSelection();
                    dgv.Rows[e.RowIndex].Selected = true;
                    dgv.CurrentCell = dgv.Rows[e.RowIndex].Cells[e.ColumnIndex];
                }
                else
                {
                    // 情况 B：点击的是已选中的群组之一 -> 啥都不干，保持多选状态！
                    // 这样用户选了10行后，在其中任意一行右键，10行依然被选中
                }

                // 2. 动态更新菜单文字 (显示选中了几个)
                int count = dgv.SelectedRows.Count;
                if (count > 1)
                {
                    _tsmiBatchWrite.Text = $"批量写入 ({count} 个点位)";
                    _tsmiBatchWrite.Enabled = true;
                }
                else
                {
                    _tsmiBatchWrite.Text = "写入数值 (单个)"; // 单选时也可以用这个菜单
                    _tsmiBatchWrite.Enabled = true;
                }

                // 3. 弹出菜单
                _ctxMenu.Show(Cursor.Position);
            }
        }
        private void DoWrite(DataGridViewRow row)
        {
            var p = row.Tag as ModbusPoint;
            // 获取用户输入的原始字符串
            string inputStr = row.Cells[colInput.Name].Value?.ToString();

            if (string.IsNullOrWhiteSpace(inputStr))
            {
                MessageBox.Show(LangProvider.Get("Mon_Msg_Empty"));
                return;
            }

            try
            {
                // =================================================================
                // 1. 量化误差预检查 (Quantization Pre-check)
                // =================================================================
                // 只有【保持寄存器】且【非浮点数】且【系数!=1】时，才会有分辨率丢失的问题
                bool isIntegerReg = p.Zone == StorageZone.HoldingRegister_4x &&
                                    p.DataType != DataType.Float &&
                                    p.DataType != DataType.Double &&
                                    p.DataType != DataType.Bool;

                // 如果系数接近 1 (比如 0.99999 或 1.00001)，通常忽略此检查
                bool hasScaling = Math.Abs(p.Factor - 1.0) > 0.000001;

                if (isIntegerReg && hasScaling)
                {
                    if (double.TryParse(inputStr, out double userVal))
                    {
                        // A. 模拟写入：逆向计算 + 取整 (模拟 PLC 的整数存储)
                        // (Val - Offset) / Factor
                        double rawCalc = (userVal - p.Offset) / p.Factor;
                        long rawInt = (long)Math.Round(rawCalc, MidpointRounding.AwayFromZero);

                        // B. 模拟读取：正向计算 (回算出 PLC 真正代表的物理值)
                        // (Raw * Factor) + Offset
                        double actualVal = (rawInt * p.Factor) + p.Offset;

                        // C. 比较差异
                        // 如果用户输入 120，回算出来是 125，差异 > 0.001
                        if (Math.Abs(userVal - actualVal) > 0.001)
                        {
                            // 【修改】使用格式化字符串弹窗
                            string msgFormat = LangProvider.Get("Mon_Fix_Msg");
                            string msg = string.Format(msgFormat, p.Factor, userVal, actualVal);
                            string title = LangProvider.Get("Mon_Fix_Title");

                            //string msg = $"【精度修正提示】\r\n\r\n" +
                            //             $"由于当前系数 ({p.Factor}) 的限制，设备无法存储 {userVal}。\r\n" +
                            //             $"最接近的有效值为: {actualVal}\r\n\r\n" +
                            //             $"点击 [确定] 将自动修正输入值并写入。\r\n" +
                            //             $"点击 [取消] 放弃操作。";

                            if (MessageBox.Show(msg, title, MessageBoxButtons.OKCancel, MessageBoxIcon.Information) == DialogResult.OK)
                            {
                                // 修正输入框的显示
                                inputStr = actualVal.ToString();
                                row.Cells[colInput.Name].Value = inputStr; // 更新界面
                            }
                            else
                            {
                                return; // 用户取消
                            }
                        }
                    }
                }
                // =================================================================

                // 2. 正常下发指令 (此时 inputStr 已经是修正后的值，或者是用户确认过的值)
                _master.WritePoint(p, inputStr, Device.SlaveId);

                // 3. 视觉反馈
                row.Cells[colInput.Name].Style.BackColor = Color.LightGreen;
            }
            catch (Exception ex)
            {
                string errMsg = string.Format(LangProvider.Get("Mon_Write_Fail"), ex.Message);
                MessageBox.Show(errMsg);
                row.Cells[colInput.Name].Style.BackColor = Color.MistyRose;
            }
        }

        // 高级：绘制“假”禁用按钮 (让不可用的按钮看起来是灰色的)
        private void Dgv_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            // 确保处理的是按钮列，且不是表头
            if (e.RowIndex >= 0 && dgv.Columns[e.ColumnIndex].Name == colBtn.Name)
            {
                var row = dgv.Rows[e.RowIndex];
                if (row.Tag is ModbusPoint p)
                {
                    // 【修改这里】同样的禁用逻辑
                    bool isReadOnlyZone = (p.Zone == StorageZone.InputStatus_1x || p.Zone == StorageZone.InputRegister_3x);
                    bool isRegBit = (p.Zone == StorageZone.HoldingRegister_4x && p.DataType == DataType.Bool);

                    if (isReadOnlyZone || isRegBit)
                    {
                        // 1. 擦除默认的按钮绘制
                        e.Paint(e.CellBounds, DataGridViewPaintParts.All & ~DataGridViewPaintParts.ContentForeground);

                        // 2. 绘制灰色文字 "禁用"
                        Color grayColor = SystemColors.GrayText;
                        string disabledText = LangProvider.Get("Mon_Btn_Disabled");
                        TextRenderer.DrawText(e.Graphics, disabledText, e.CellStyle.Font, e.CellBounds, grayColor,
                            TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);

                        // 3. 告诉 Grid 我画完了，你别画了
                        e.Handled = true;
                    }
                }
            }
        }

        private void Dgv_MouseDown(object sender, MouseEventArgs e)
        {
            // 获取鼠标点击的行
            var hit = dgv.HitTest(e.X, e.Y);
            _rowIndexFromMouseDown = -1;

            if (hit.RowIndex != -1 && e.Button == MouseButtons.Left)
            {
                // 1. 记录点击的行号
                _rowIndexFromMouseDown = hit.RowIndex;

                // 2. 创建一个“拖拽敏感矩形”
                // 只有鼠标移出了这个小矩形范围，才算真正的拖拽开始
                // 这样可以避免普通的“点击选择”被误判为“拖拽”
                Size dragSize = SystemInformation.DragSize;
                _dragBoxFromMouseDown = new Rectangle(
                    new Point(e.X - (dragSize.Width / 2), e.Y - (dragSize.Height / 2)),
                    dragSize);

                System.Diagnostics.Debug.WriteLine($"[Drag] MouseDown at Row: {_rowIndexFromMouseDown}");
            }
            else
            {
                // 重置矩形
                _dragBoxFromMouseDown = Rectangle.Empty;
            }
        }

        private void Dgv_MouseMove(object sender, MouseEventArgs e)
        {
            // 只有当左键按下时才检测
            if ((e.Button & MouseButtons.Left) == MouseButtons.Left)
            {
                // 检查矩形是否有效，并且鼠标是否移出了矩形范围
                if (_dragBoxFromMouseDown != Rectangle.Empty && !_dragBoxFromMouseDown.Contains(e.X, e.Y))
                {
                    if (_rowIndexFromMouseDown >= 0 && _rowIndexFromMouseDown < dgv.Rows.Count)
                    {
                        System.Diagnostics.Debug.WriteLine("[Drag] Trigger DoDragDrop!");

                        // 获取数据
                        var row = dgv.Rows[_rowIndexFromMouseDown];
                        var point = row.Tag as ModbusPoint;

                        if (point != null)
                        {
                            // 封装拖拽数据（包含设备信息）
                            var dragData = new TrendDragData
                            {
                                DeviceName = Device.DeviceName,
                                SlaveId = Device.SlaveId,
                                ChannelName = this.CurrentChannelName,
                                Point = point
                            };

                            // 3. 正式开始拖拽 (这是一个阻塞操作，直到松开鼠标)
                            DragDropEffects effect = dgv.DoDragDrop(dragData, DragDropEffects.Copy);

                            System.Diagnostics.Debug.WriteLine($"[Drag] Finished with effect: {effect}");
                        }
                    }
                    // 拖拽结束后重置
                    _dragBoxFromMouseDown = Rectangle.Empty;
                }
            }
        }

        private void Dgv_MouseUp(object sender, MouseEventArgs e)
        {
            // 鼠标抬起，重置状态
            _dragBoxFromMouseDown = Rectangle.Empty;
        }
        // 2. 菜单点击事件
        private void TsmiBatchWrite_Click(object sender, EventArgs e)
        {
            var selectedPoints = new System.Collections.Generic.List<ModbusPoint>();

            // 收集选中的有效点位
            foreach (DataGridViewRow row in dgv.SelectedRows)
            {
                if (row.Tag is ModbusPoint p)
                {
                    // 过滤掉只读点位
                    bool isReadOnly = (p.Zone == StorageZone.InputStatus_1x || p.Zone == StorageZone.InputRegister_3x);
                    // 过滤掉寄存器位操作 (通常不支持批量写位)
                    bool isRegBit = (p.Zone == StorageZone.HoldingRegister_4x && p.DataType == DataType.Bool);

                    if (!isReadOnly && !isRegBit)
                    {
                        selectedPoints.Add(p);
                    }
                }
            }

            if (selectedPoints.Count == 0)
            {
                MessageBox.Show("未选中任何可写的点位！");
                return;
            }

            // 简单检查类型一致性 (以第一个为准)
            bool isFirstBool = selectedPoints[0].DataType == DataType.Bool;
            bool hasConflict = selectedPoints.Any(p => (p.DataType == DataType.Bool) != isFirstBool);

            if (hasConflict)
            {
                MessageBox.Show("批量写入不支持混合类型（同时包含布尔量和数值）。\r\n请重新选择。", "类型冲突", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 弹出输入框
            string typeStr = isFirstBool ? "Bool" : "Value";
            string title = $"批量写入 ({selectedPoints.Count} 个)";

            // 使用刚才新建的 F_InputValue 窗口
            F_InputValue frm = new F_InputValue(title, "", typeStr);

            if (frm.ShowDialog() == DialogResult.OK)
            {
                PerformBatchWrite(selectedPoints, frm.InputValue);
            }
        }

        // 3. 执行批量写入
        private void PerformBatchWrite(System.Collections.Generic.List<ModbusPoint> points, string valStr)
        {
            // --- 阶段 1: 预检 (Simulation) ---
            var errorList = new System.Collections.Generic.List<string>();
            int errorCount = 0;

            foreach (var p in points)
            {
                if (CheckQuantizationError(p, valStr, out double actualVal, out string reason))
                {
                    errorCount++;
                    // 只记录前 5 条详细信息，防止弹窗爆炸
                    if (errorCount <= 5)
                    {
                        errorList.Add($"• {p.Name}: 输入 {valStr} -> 将变为 {actualVal} [{reason}]");
                    }
                }
            }

            // --- 阶段 2: 决策 (Decision) ---
            if (errorCount > 0)
            {
                string msg = $"检测到 {errorCount} 个点位存在精度丢失或取整情况！\r\n\r\n";
                msg += string.Join("\r\n", errorList);

                if (errorCount > 5) msg += $"\r\n... 以及其他 {errorCount - 5} 个点位。";

                msg += "\r\n\r\n是否仍要继续写入修正后的值？";

                DialogResult dr = MessageBox.Show(msg, "精度丢失警告", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2);

                if (dr != DialogResult.OK)
                {
                    return; // 用户取消操作
                }
            }

            // --- 阶段 3: 执行 (Execution) ---
            int success = 0;
            int fail = 0;

            foreach (var p in points)
            {
                try
                {
                    // 注意：这里我们传入原始的 valStr
                    // 因为 CheckQuantizationError 只是检查，真正的修正逻辑
                    // 应该是在 WritePoint 内部或者 ValueEncoder 里再次执行，或者你可以传 actualVal

                    // 更稳妥的做法是：如果刚才 Check 发现了问题，这里应该传 actualVal
                    // 但因为我们是循环处理，需要再次计算一次 actualVal 或者不仅是检查，而是直接把修正后的值存个字典

                    // 为了简单且代码复用，我们这里直接传 valStr。
                    // 前提是：底层的 _master.WritePoint 也会做同样的 (Val-Offset)/Factor + Round 操作。
                    // 这样最终写入的就是刚才提示用户看到的那个 actualVal。

                    _master.WritePoint(p, valStr, Device.SlaveId);
                    success++;
                }
                catch (Exception ex)
                {
                    fail++;
                    LogHub.Write(p.Name, LogType.Error, $"批量写入失败 [{p.Name}]: {ex.Message}");
                }
            }

            MessageBox.Show($"写入完成！\r\n成功: {success}\r\n失败: {fail}", "结果", MessageBoxButtons.OK, fail > 0 ? MessageBoxIcon.Warning : MessageBoxIcon.Information);
        }
        // ==========================================
        // 【核心】实现界面文字替换
        // ==========================================
        private void ApplyUIText()
        {
            // 1. 窗口标题
            // 格式： "设备监控 - {0} (ID: {1})"
            string fmt = LangProvider.Get("Mon_Title_Fmt");
            this.Text = string.Format(fmt, Device.DeviceName, Device.SlaveId);

            // 2. DataGridView 列头
            colName.HeaderText = LangProvider.Get("Mon_Col_Name");
            colUnit.HeaderText = LangProvider.Get("Mon_Col_Unit");
            colValue.HeaderText = LangProvider.Get("Mon_Col_Value");
            colInput.HeaderText = LangProvider.Get("Mon_Col_Input");
            colBtn.HeaderText = LangProvider.Get("Mon_Col_Btn");

            // 3. 按钮列的默认文本 (非常重要！否则所有按钮都是 "写入")
            colBtn.Text = LangProvider.Get("Mon_Btn_Write");
            colBtn.UseColumnTextForButtonValue = true; // 确保它使用这一列的 Text 属性

            // 4. 初始化筛选控件
            InitFilterControls();
        }

        // ==========================================
        // 筛选控件初始化
        // ==========================================
        private void InitFilterControls()
        {
            // 初始化筛选器的存储区下拉框
            var filterZoneSource = new List<EnumItem> { new EnumItem { Text = "全部 (All)", Value = -1 } };
            filterZoneSource.Add(new EnumItem { Text = LangProvider.Get("Zone_0x"), Value = StorageZone.CoilStatus_0x });
            filterZoneSource.Add(new EnumItem { Text = LangProvider.Get("Zone_1x"), Value = StorageZone.InputStatus_1x });
            filterZoneSource.Add(new EnumItem { Text = LangProvider.Get("Zone_3x"), Value = StorageZone.InputRegister_3x });
            filterZoneSource.Add(new EnumItem { Text = LangProvider.Get("Zone_4x"), Value = StorageZone.HoldingRegister_4x });
            cmbFilterZone.DisplayMember = "Text";
            cmbFilterZone.ValueMember = "Value";
            cmbFilterZone.DataSource = filterZoneSource;

            // 类型下拉框
            var filterTypeSource = new System.Collections.Generic.List<EnumItem> { new EnumItem { Text = "全部 (All)", Value = -1 } };
            foreach (DataType t in Enum.GetValues(typeof(DataType)))
            {
                filterTypeSource.Add(new EnumItem { Text = t.ToString(), Value = t });
            }
            cmbFilterType.DisplayMember = "Text";
            cmbFilterType.ValueMember = "Value";
            cmbFilterType.DataSource = filterTypeSource;

            // 绑定事件
            txtSearch.TextChanged += (s, e) => ExecuteFilter();
            cmbFilterType.SelectedIndexChanged += (s, e) => ExecuteFilter();
            cmbFilterZone.SelectedIndexChanged += (s, e) => ExecuteFilter();
            btnClearFilter.Click += (s, e) =>
            {
                txtSearch.Text = "";
                cmbFilterType.SelectedIndex = 0;
                cmbFilterZone.SelectedIndex = 0;
                ExecuteFilter(); // 恢复全部显示
            };
            btnAddrFormat.Click += (s, e) => ToggleAddressFormat();
        }

        // ==========================================
        // 地址格式切换
        // ==========================================
        private void ToggleAddressFormat()
        {
            if (!_showHexAddress && !_showPlcAddress)
            {
                _showHexAddress = true;
                btnAddrFormat.Text = "🔢 Hex";
            }
            else if (_showHexAddress && !_showPlcAddress)
            {
                _showHexAddress = false;
                _showPlcAddress = true;
                btnAddrFormat.Text = "🏭 PLC";
            }
            else
            {
                _showHexAddress = false;
                _showPlcAddress = false;
                btnAddrFormat.Text = "🔢 Dec";
            }

            RefreshAddressDisplay();
        }

        private void RefreshAddressDisplay()
        {
            foreach (DataGridViewRow row in dgv.Rows)
            {
                if (row.Tag is ModbusPoint p)
                {
                    row.Cells[colAddr.Name].Value = FormatAddress(p.Address, p.Zone);
                }
            }
        }

        private string FormatAddress(int protocolAddr, StorageZone zone)
        {
            string prefix = zone switch
            {
                StorageZone.CoilStatus_0x => "0x",
                StorageZone.InputStatus_1x => "1x",
                StorageZone.InputRegister_3x => "3x",
                StorageZone.HoldingRegister_4x => "4x",
                _ => "?x"
            };

            if (_showPlcAddress)
            {
                int plcAddr = protocolAddr;
                switch (zone)
                {
                    case StorageZone.CoilStatus_0x: plcAddr += 1; break;
                    case StorageZone.InputStatus_1x: plcAddr += 10001; break;
                    case StorageZone.InputRegister_3x: plcAddr += 30001; break;
                    case StorageZone.HoldingRegister_4x: plcAddr += 40001; break;
                }
                return $"{prefix}-{plcAddr}";
            }
            else if (_showHexAddress)
            {
                return $"{prefix}-0x{protocolAddr:X4}";
            }
            else
            {
                return $"{prefix}-{protocolAddr}";
            }
        }

        // 放在 F_DeviceMonitor 类里，或者单独的工具类里
        private bool CheckQuantizationError(ModbusPoint p, string inputStr, out double actualVal, out string reason)
        {
            actualVal = 0;
            reason = "";

            // 1. 基础检查：非数值类型跳过
            if (p.DataType == DataType.Bool) return false; // Bool 没有精度问题
            if (p.DataType == DataType.Float || p.DataType == DataType.Double) return false; // 浮点数通常接受任意值

            // 2. 解析用户输入
            if (!double.TryParse(inputStr, out double userVal)) return false;

            // 3. 检查系数是否接近 1
            // 如果系数是 1 且 Offset 是 0，且是整数类型，通常直接转 Int 即可，不用逆向算
            if (Math.Abs(p.Factor - 1.0) < 0.000001 && Math.Abs(p.Offset) < 0.000001)
            {
                // 唯一的风险是用户输入了小数给整数寄存器
                if (Math.Abs(userVal - Math.Round(userVal)) > 0.001)
                {
                    actualVal = Math.Round(userVal);
                    reason = "取整";
                    return true;
                }
                return false;
            }

            // 4. 核心逻辑：模拟 PLC 的存储过程
            // A. 逆向计算 (物理值 -> 寄存器原始值)
            double rawCalc = (userVal - p.Offset) / p.Factor;

            // B. 强制取整 (模拟寄存器只能存整数)
            long rawInt = (long)Math.Round(rawCalc, MidpointRounding.AwayFromZero);

            // C. 正向回算 (寄存器原始值 -> 实际物理值)
            actualVal = (rawInt * p.Factor) + p.Offset;

            // D. 比较差异
            // 如果差异超过 0.001 (或者业务允许的误差范围)，则认为发生了精度丢失
            if (Math.Abs(userVal - actualVal) > 0.001)
            {
                reason = $"分辨率限制 (Factor={p.Factor})";
                return true;
            }

            return false;
        }

        // 筛选逻辑
        private void ExecuteFilter()
        {
            string keyword = txtSearch.Text.Trim();

            // 获取筛选条件 (与你之前的逻辑一致)
            StorageZone? targetZone = cmbFilterZone.SelectedIndex > 0 ? (StorageZone?)cmbFilterZone.SelectedValue : null;
            DataType? targetType = cmbFilterType.SelectedIndex > 0 ? (DataType?)cmbFilterType.SelectedValue : null;
            //bool onlyShowError = chkOnlyError.Checked; // 假设你加了一个“只看错误”的勾选框

            dgv.CurrentCell = null;
            dgv.SuspendLayout();

            // 依然使用你之前的 CurrencyManager 挂起方式，这是对的
            CurrencyManager cm = (CurrencyManager)BindingContext[dgv.DataSource ?? dgv.Rows];
            cm.SuspendBinding();

            try
            {
                foreach (DataGridViewRow row in dgv.Rows)
                {
                    if (row.IsNewRow) continue;
                    var pt = row.Tag as ModbusPoint;
                    if (pt == null) continue;

                    // --- 1. 存储区与类型筛选 ---
                    bool matchZone = !targetZone.HasValue || pt.Zone == targetZone.Value;
                    bool matchType = !targetType.HasValue || pt.DataType == targetType.Value;

                    // --- 2. 通讯质量筛选 (监控特有) ---
                    // 假设你在 Point 或 Row 中记录了上次通讯是否成功
                    //bool matchStatus = !onlyShowError || pt.LastStatus == PointStatus.Error;

                    // --- 3. 文本关键字匹配 (解决地址坑) ---
                    // --- C. 文本关键字匹配 (针对带前缀地址优化) ---
                    bool matchText = true;
                    if (!string.IsNullOrEmpty(keyword))
                    {
                        string name = pt.Name ?? "";
                        string displayAddr = row.Cells[colAddr.Index].Value?.ToString() ?? ""; // 例如 "4x-0010"

                        // 1. 名称匹配：包含即可
                        bool nameMatch = name.Contains(keyword, StringComparison.OrdinalIgnoreCase);

                        // 2. 地址匹配：
                        bool addrMatch = false;

                        // 情况 A: 直接匹配前缀（例如输入 "4x-"）
                        if (displayAddr.StartsWith(keyword, StringComparison.OrdinalIgnoreCase))
                        {
                            addrMatch = true;
                        }
                        else
                        {
                            // 情况 B: 剥离前缀匹配数字（例如输入 "10" 匹配 "4x-0010"）
                            int dashIdx = displayAddr.IndexOf('-');
                            if (dashIdx >= 0)
                            {
                                // 拿到 "-" 之后的数字部分，比如 "0010"
                                string pureAddrPart = displayAddr.Substring(dashIdx + 1);

                                // 只要数字部分以 keyword 开头，就算中 (支持搜 "10" 或 "001")
                                if (pureAddrPart.StartsWith(keyword, StringComparison.OrdinalIgnoreCase))
                                {
                                    addrMatch = true;
                                }
                                // 进阶：如果用户搜 "10"，而显示是 "0010"，去掉前导零再比对一次
                                else if (pureAddrPart.TrimStart('0').StartsWith(keyword.TrimStart('0'), StringComparison.OrdinalIgnoreCase))
                                {
                                    addrMatch = true;
                                }
                            }
                        }

                        matchText = nameMatch || addrMatch;
                    }

                    // --- 4. 综合判定 ---
                    row.Visible = matchZone && matchType && matchText;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("筛选异常: " + ex.Message);
            }
            finally
            {
                cm.ResumeBinding();
                dgv.ResumeLayout();
            }
        }

        public class EnumItem { public string Text { get; set; } public object Value { get; set; } }
    }
}
