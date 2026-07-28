using ModbusPilot.Core;
using ModbusPilot.Core.Driver;
using ModbusPilot.Core.Models;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace ModbusPilot.UI.Common
{
    public partial class UC_WidgetBase : UserControl
    {
        // 1. 【身份信息】这些是原始数据，一旦创建就固定死，用于“找回”点位
        // 包含：通道名、SlaveId、存储区、偏移地址、位索引
        public string ChannelName { get; private set; }
        public byte SlaveId { get; private set; }

        // 2. 【实时引用】这是唯一的 ModbusPoint 引用
        // UI 界面所有的 Text、Color 绑定都只看这个对象
        public ModbusPoint BoundPoint { get; set; }
        public ModbusMaster Master { get; private set; }


        // 样式颜色
        protected Color _currentBorderColor;
        protected Color _currentAccentColor;
        protected Color CurrentTextColor = Color.Black;

        // 状态色
        protected Color StatusOnBack = Color.LimeGreen;
        protected Color StatusOnText = Color.White;
        protected Color StatusOffBack = Color.LightGray;
        protected Color StatusOffText = Color.Gray;

        // 交互状态
        private bool _isHovering = false;
        private ToolStripMenuItem _ctxRemoveItem;
        private ToolTip _toolTip;
        // 1. 在类成员里加个标志位
        private bool _isPlaceholder = false;
        private bool _isOffline = false; // 记录当前状态，防止重复刷新
        private UITheme _currTheme = UITheme.DarkMode;

        public UC_WidgetBase()
        {
            InitializeComponent();
            // 开启双缓冲 (至关重要，防止绘图闪烁)
            this.SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
        }

        public void Init(TrendDragData data, ModbusMaster master)
        {
            // 固化身份（就像身份证号）
            this.ChannelName = data.ChannelName;
            this.SlaveId = data.SlaveId;

            // 挂载当前引用
            this.BoundPoint = data.Point;

            Master = master;

            lblName.Text = BoundPoint.Name;
            lblUnit.Text = string.IsNullOrEmpty(BoundPoint.Unit) ? "" : $"[{BoundPoint.Unit}]";
            if (lblDevice != null)
            {
                lblDevice.Text = data.DeviceName; // 赋值还是要赋的，为了 ToolTip 能取到值
                lblDevice.Visible = false;   // 【确保隐藏】
            }

            // 默认颜色
            _currentBorderColor = Color.LightGray;
            _currentAccentColor = Color.DodgerBlue;

            // 【关键优化 1】固定尺寸和边距，绝不动态修改
            this.Size = new Size(200, 100);
            this.Margin = new Padding(4); // 固定间距
            this.Padding = new Padding(4); // 内部内容避让边框
            this.Cursor = Cursors.Hand;

            // 右键菜单
            var ctx = new ContextMenuStrip();
            _ctxRemoveItem = new ToolStripMenuItem("移除卡片");
            _ctxRemoveItem.Click += (s, e) => {
                this.Parent?.Controls.Remove(this);
                this.Dispose();
            };
            ctx.Items.Add(_ctxRemoveItem);
            this.ContextMenuStrip = ctx;

            // ToolTip
            _toolTip = new ToolTip { AutoPopDelay = 10000, InitialDelay = 500 };
            string tipText = $"通道: {ChannelName}\r\n设备: {data.DeviceName} (ID:{SlaveId})\r\n变量: {BoundPoint.Name}\r\n地址: {GetAddressDesc(BoundPoint)}\r\n描述: {BoundPoint.Note ?? "-"}";
            _toolTip.SetToolTip(this, tipText);
            _toolTip.SetToolTip(lblName, tipText);
            _toolTip.SetToolTip(pnlHeader, tipText);
            _toolTip.SetToolTip(pnlContent, tipText);

            // 绑定事件
            BindInteractiveEvents(this);
        }
        // 1. 重写 OnLoad 方法
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            // 确保无论何时添加了控件，加载时都把事件绑上去
            // 放在这里比放在 Init 里更保险，因为它能覆盖所有子类 InitializeComponent 生成的控件
            BindInteractiveEvents(this);
        }
        // 2. 新增一个公开方法，切换占位模式
        public void SetPlaceholderMode(bool enable)
        {
            _isPlaceholder = enable;

            // A. 隐藏/显示所有子控件 (文字、数值等)
            foreach (Control c in this.Controls)
            {
                // 【核心修复】恢复显示时(enable=false)，如果是 lblDevice，永远保持隐藏！
                if (!enable && c == lblDevice)
                {
                    c.Visible = false;
                    continue;
                }
            }

            // B. 改变背景色
            if (enable)
            {
                this.BackColor = Color.FromArgb(240, 240, 240); // 变成底色 (看起来像空的)
                                                                // 或者 Color.Transparent (如果父容器支持)
            }
            else
            {
                // 恢复正常背景 (如果你有主题变量 _currentTheme，最好重新 ApplyTheme)
                // 这里暂时设回白色，或者调用 ApplyTheme
                this.BackColor = Color.White; // 简单恢复
            }

            // C. 触发重绘 (改变边框样式)
            this.Invalidate();
        }
        // =============================================================
        // 【核心修复 2】丝滑的悬浮判定逻辑
        // =============================================================
        private void BindInteractiveEvents(Control ctrl)
        {
            // 跳过交互型控件 (按钮、输入框)，否则没法点击了
            if (ctrl is Button || ctrl is TextBox || ctrl is CheckBox) return;

            // 解绑旧事件防止重复绑定 (可选，更严谨)
            ctrl.MouseDown -= OnAnyMouseDown;
            ctrl.MouseEnter -= OnAnyMouseEnter;
            ctrl.MouseLeave -= OnAnyMouseLeave;

            // 绑定新事件
            ctrl.MouseDown += OnAnyMouseDown;
            ctrl.MouseEnter += OnAnyMouseEnter;
            ctrl.MouseLeave += OnAnyMouseLeave;

            // 递归绑定所有子控件 (包括 Header, Label, ContentPanel 等)
            foreach (Control c in ctrl.Controls)
            {
                BindInteractiveEvents(c);
            }
        }
        // 3. 统一的 MouseDown 处理
        private void OnAnyMouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                // 【关键】不管点的是 Label 还是 Panel，都把 'this' (整个卡片) 传出去
                this.DoDragDrop(this, DragDropEffects.Move);
            }
        }

        // 4. 统一的 MouseEnter 处理
        private void OnAnyMouseEnter(object sender, EventArgs e)
        {
            if (!_isHovering)
            {
                _isHovering = true;
                this.Invalidate();
            }
        }

        // 5. 统一的 MouseLeave 处理
        private void OnAnyMouseLeave(object sender, EventArgs e)
        {
            Point p = this.PointToClient(Cursor.Position);
            if (!this.ClientRectangle.Contains(p))
            {
                _isHovering = false;
                this.Invalidate();
            }
        }
        // =============================================================
        // 【核心修复 3】视觉欺骗绘图法
        // =============================================================
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;

            if (_isPlaceholder)
            {
                // === 占位模式：画虚线框 ===
                using (Pen p = new Pen(Color.Gray, 2))
                {
                    p.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash; // 虚线
                    Rectangle rect = new Rectangle(1, 1, this.Width - 2, this.Height - 2);
                    g.DrawRectangle(p, rect);
                }
                return; // 画完直接返回，不画正常的边框了
            }


            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None; // 锐利线条

            // 定义两个矩形：
            // rectFull: 填满整个控件的矩形
            Rectangle rectFull = new Rectangle(0, 0, this.Width - 1, this.Height - 1);

            // rectShrink: 向内缩 2px 的矩形 (模拟常态)
            // 这样切换时，视觉上就像卡片变大了，实际上控件没动
            Rectangle rectShrink = new Rectangle(2, 2, this.Width - 5, this.Height - 5);

            if (_isHovering)
            {
                // === 悬浮状态 (变大) ===
                // 1. 绘制边框：使用主题色(Accent)，贴着边缘画，这就显得大了
                using (Pen p = new Pen(_currentAccentColor, 2)) // 线宽2更醒目
                {
                    // 线宽>1时需向内偏移
                    g.DrawRectangle(p, 1, 1, this.Width - 2, this.Height - 2);
                }
            }
            else
            {
                // === 普通状态 (变小) ===
                // 1. 绘制边框：使用浅灰色，向内缩 2px 画
                // 这样留出的空白区域(Padding)就模拟了"未浮起"的距离感
                using (Pen p = new Pen(_currentBorderColor, 1))
                {
                    g.DrawRectangle(p, rectShrink);
                }
            }
        }

        public virtual void ApplyTheme(UITheme theme)
        {
            _currTheme = theme;
            if (_isOffline)
            {
                // 立即使用【新主题】定义的离线颜色重新渲染
                ApplyOfflineVisuals();
                return; // 结束，不再执行下面的在线样式赋值
            }
            if (_isPlaceholder) return; // 如果是占位符，别上色

            this.BackColor = theme.CardBack;
            _currentBorderColor = theme.Border;
            _currentAccentColor = theme.Border;
            CurrentTextColor = theme.TextPrimary;
            StatusOnBack = theme.StatusOnBack;
            StatusOnText = theme.StatusOnText;
            StatusOffBack = theme.StatusOffBack;
            StatusOffText = theme.StatusOffText;

            lblName.ForeColor = theme.TextSecondary;
            lblUnit.ForeColor = theme.TextSecondary;

            this.Invalidate();
        }

        private string GetAddressDesc(ModbusPoint p)
        {
            string prefix = "4x";
            int baseAddr = 40001;
            if (p.Zone == StorageZone.CoilStatus_0x) { prefix = "0x"; baseAddr = 1; }
            else if (p.Zone == StorageZone.InputStatus_1x) { prefix = "1x"; baseAddr = 10001; }
            else if (p.Zone == StorageZone.InputRegister_3x) { prefix = "3x"; baseAddr = 30001; }
            return $"{prefix}{p.Address} (PLC:{p.Address + baseAddr})";
        }

        // 对外公开的方法，供主界面定时调用
        public void UpdateConnectionState()
        {
            // 1. 判断当前连接状态
            bool currentlyOnline = (this.Master != null && this.Master.IsOnline);

            // 2. 状态反转时才触发 UI 变更 (减少重绘)
            bool currentlyOffline = !currentlyOnline;

            if (_isOffline != currentlyOffline)
            {
                _isOffline = currentlyOffline;
                ApplyOfflineVisuals(); // 应用视觉效果
            }
        }
        // 应用离线/在线视觉效果
        private void ApplyOfflineVisuals()
        {
            if (_currTheme == null)
            {
                this.BackColor = _isOffline ? Color.LightGray : Color.White;
                return;
            }

            if (_isOffline)
            {
                // === 变身：离线状态 (手动变灰) ===

                // 1. 背景：暗黑模式下变深黑，浅色模式下变浅灰
                this.BackColor = _currTheme.OfflineBack;

                // 2. 文字：变暗
                lblName.ForeColor = _currTheme.OfflineText;
                lblUnit.ForeColor = _currTheme.OfflineText;
                this.ForeColor = _currTheme.OfflineText; // 级联给子控件

                // 3. 绘图颜色：边框变暗
                _currentBorderColor = _currTheme.OfflineBorder;
                _currentAccentColor = _currTheme.OfflineBorder; // 悬浮时也是暗色

                foreach (Control c in this.pnlContent.Controls)
                {
                    if (c is Label) c.ForeColor = _currTheme.OfflineText;
                    // 2. Button: 背景和文字都要变，且必须是 Flat 样式颜色才生效
                    if (c is Button btn)
                    {
                        btn.BackColor = _currTheme.OfflineBack;
                        btn.ForeColor = _currTheme.OfflineText;
                        // 如果按钮本来不是 Flat，设置颜色可能无效，建议强制统一
                        btn.FlatStyle = FlatStyle.Flat;
                        btn.FlatAppearance.BorderColor = _currTheme.OfflineBorder;
                    }

                    // 3. TextBox: 背景变暗，文字也要变暗
                    if (c is TextBox txt)
                    {
                        txt.BackColor = _currTheme.OfflineBack;
                        // 【漏了这句】输入框里的字也要变暗，不然黑底白字太亮了
                        txt.ForeColor = _currTheme.OfflineText;
                        txt.BorderStyle = BorderStyle.FixedSingle; // 配合扁平化
                    }
                    if (c is CheckBox chk)
                    {
                        chk.ForeColor = _currTheme.OfflineText;
                        chk.BackColor = _currTheme.OfflineBack;
                    }
                }
            }
            else
            {
                // === 恢复：在线状态 ===

                // 直接应用缓存的主题，恢复背景、字体、边框的所有颜色
                if (_currTheme != null)
                {
                    ApplyTheme(_currTheme);
                }
                else
                {
                    _currTheme = UITheme.DarkMode;
                    ApplyTheme(_currTheme);                  
                }
            }

            // 触发重绘 (让 OnPaint 使用新的 BorderColor 画框)
            this.Invalidate();
        }
        public void UpdateMaster(ModbusMaster newMaster) => this.Master = newMaster;
        public virtual void UpdateValue(string val) { }
    }
}