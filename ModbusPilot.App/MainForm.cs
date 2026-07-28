using ModbusPilot.Core;
using ModbusPilot.Core.Driver;
using ModbusPilot.Core.Models;
using ModbusPilot.Core.Services;
using ModbusPilot.Core.Utils;
using ModbusPilot.UI.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Windows.Forms;
using TheArtOfDev.HtmlRenderer.WinForms;



namespace ModbusPilot.App
{
    public partial class MainForm : Form
    {
        // === 字段定义区域 ===
        private List<ChannelConfig> _projectData = new List<ChannelConfig>();
        private Dictionary<ChannelConfig, ModbusMaster> _runningMasters = new Dictionary<ChannelConfig, ModbusMaster>();
        private Dictionary<DeviceConfig, F_DeviceMonitor> _openMonitors = new Dictionary<DeviceConfig, F_DeviceMonitor>();

        // 增加一个字段，记录当前打开的文件路径
        private string _currentFilePath = null;

        // 声明一个全局变量
        private DragHelper _ghostForm = null;
        private ContextMenuStrip _ctxDashboard; // 仪表盘全局菜单
        System.Windows.Forms.Timer timerUI;

        // 全局计数器 (建议放在 LogHub 或 Transport 层，这里为了演示先放这)
        private long _totalTx = 0;
        private long _totalRx = 0;
        private long _totalErr = 0;
        // 声明变量
        private HtmlPanel htmlPanelTips;

        // 在类里定义一个计数器，混淆后它就是一个普通的 int 字段
        private int _heartbeatCount = 0;
        private bool _isPiratied = false; // 标记是否为盗版

        // === 构造函数 ===
        public MainForm()
        {
            InitializeComponent();

            // 在 MainForm 构造函数中
            Task.Run(async () =>
            {
                await LinkManager.SyncConfigAsync();

                // 此时 LinkManager.BetaExpiryDate 已经更新
                this.Invoke(new Action(() => {

                    if (CheckUpdate() == false) return;

                    UpdateAnnouncement();
                    
                    UpdateTitle();
                    
                    // 项目已开源转为免费软件，不再有"公测到期回退免费版"的强制限制。
                    if (false)
                    {
                        // 1. 弹出提示，明确告知去下载正式版
                        var result = MessageBox.Show(
                            "公测活动已结束，当前预览版本已回退至基础免费版。\n\n" +
                            "请前往官网下载最新的【正式版】软件以解锁全部功能。\n\n" +
                            "是否立即前往下载页面？",
                            "公测结束提醒",
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Information);

                        // 2. 如果点击“是”，直接打开你在 LinkManager 里配好的 DownloadUrl
                        if (result == DialogResult.Yes)
                        {
                            LinkManager.Open(LinkManager.DownloadUrl);
                        }

                        // 3. 执行强制截断/限制逻辑，保护商业价值
                        //EnforceLicenseLimits();

                        // 2. 强制关闭已经打开的专业版专属窗口 (如果有的话)
                        if (F_LogMonitor.HasInstance)
                        {
                            F_LogMonitor.Instance.Close(); // 自动存盘日志窗口是专业版的，关掉
                        }

                        // 自动打开上次的项目
                        LoadLastOpenPath();

                        // 4. 强制退出全屏 (如果当前正在全屏展示)
                        if (_isFullScreen)
                        {
                            ApplyFullScreenUI(false);
                        }

                        // 5. 刷新标题栏和状态文字
                        UpdateTitle();
                    }
                }));
            });          

            this.KeyPreview = true; // 开启键盘预览，确保快捷键有效

            // 强行开启 TreeView 双缓冲，解决闪烁问题
            EnableDoubleBuffered(treeView);

            InitializeDashboard();

            InitHelpButton();

            InitTipsSystem();

            InitDashboardMenu();

            treeView.HideSelection = true;

            BindEvents();

            BindHelpMenuEvents();

            InitThemeSelector();

            // 【新增】暂时隐藏语言菜单
            menuLanguage.Visible = false;

            // 【新增】将新菜单项绑定到现有方法
            BindMenuEvents();

            // 自动打开上次的项目
            LoadLastOpenPath();

            // 【新增】应用语言设置
            ApplyUIText();           // 应用文字
            UpdateLanguageMenuState(); // 【新增】初始化打钩状态

            // 在 UI 项目的某个初始化位置执行：
            LicenseGuard.LicenseRequestHandler = (reason) =>
            {
                // 这里是 UI 项目，可以自由访问 F_Registration
                using (var frm = new F_Registration($"{reason}\n升级专业版即可解锁无限制体验。"))
                {
                    return frm.ShowDialog() == DialogResult.OK;
                }
            };

            timerUI = new System.Windows.Forms.Timer();
            timerUI.Interval = 1000;
            timerUI.Enabled = true;
            timerUI.Tick += TimerUI_Tick;
            timerUI.Start();

            this.Load += LoadFormState;
            this.FormClosing += MainForm_FormClosing;
        }
        // --- 新增方法 ---
        private void UpdateAnnouncement()
        {
            string currentAnn = LinkManager.Announcement;

            // 1. 如果公告内容为空，直接跳过
            if (string.IsNullOrWhiteSpace(currentAnn)) return;

            // 2. 读取上次看过的公告内容
            string lastAnn = Properties.Settings.Default.LastAnnouncement;

            // 3. 【核心逻辑】如果内容有变化才弹窗
            if (currentAnn != lastAnn)
            {
                // 弹出公告窗口
                using (var frm = new F_Notice(currentAnn))
                {
                    frm.ShowDialog(this);
                }

                // 4. 用户点击确定后，记录当前内容到本地记忆
                Properties.Settings.Default.LastAnnouncement = currentAnn;
                Properties.Settings.Default.Save();
            }

           
        }
        private void UpdateTitle()
        {
            // 项目已开源转为免费软件，标题栏不再显示授权状态文案。
            this.Text = "ModbusPilot";
        }
        private bool CheckUpdate()
        {
            // 1. 直接获取检查结果
            var updateInfo = LinkManager.CheckUpdate();

            // 标记是否有更新 (给你后面的公测逻辑用)
            bool hasUpdate = updateInfo.HasUpdate;

            if (hasUpdate)
            {
                string msg = $"发现新版本 {updateInfo.Version}！\n\n更新内容：\n{updateInfo.Log}\n\n是否前往下载？";

                if (updateInfo.IsForce)
                {
                    MessageBox.Show("当前版本已停用，请立即更新！", "强制更新", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                    // 打开浏览器并退出
                    LinkManager.Open(LinkManager.DownloadUrl);
                    // 2. 彻底杀死进程 (不会触发任何“是否退出”的询问)
                    Environment.Exit(0);
                    return false; // 阻断后续代码执行
                }
                else
                {
                    if (MessageBox.Show(msg, "版本更新", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                    {
                        LinkManager.Open(LinkManager.DownloadUrl);
                    }
                }
            }

            return true ;
        }
        private void MainForm_FormClosing(object? sender, FormClosingEventArgs e)
        {
            // 1. 保存窗口大小/位置 (你之前做好的)
            SaveFormState();

            // 2. 【新增】退出确认
            // 只有当有数据时才提示 (比如 _projectData 有数据)
            if (_projectData != null && _projectData.Count > 0)
            {
                var result = MessageBox.Show("正在关闭软件。\r\n是否保存对当前工程的修改？", "退出确认",
                    MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);

                if (result == DialogResult.Cancel)
                {
                    e.Cancel = true; // 取消关闭
                    return;
                }

                if (result == DialogResult.Yes)
                {
                    // 执行保存逻辑
                    tsbSave.PerformClick();
                }
            }

            // 3. 停止所有线程/释放资源 (StopAllAndClear)
            // 确保进程能彻底杀掉，不会残留在后台
            StopAllAndClear();
        }

        private void TimerUI_Tick(object? sender, EventArgs e)
        {
            // 1. 计数器累加
            _heartbeatCount++;
            // 2. 触发检查：假设定时器 1000ms 一次，300次 就是 5分钟
            // 这个数字你可以随便定，别太短，太短黑客容易测试出来
            if (_heartbeatCount > 300)
            {
                _heartbeatCount = 0; // 重置

                // 执行反盗版检查
                CheckAntiPiracy();
            }

            // 项目已开源转为免费软件，移除随机崩溃式反盗版惩罚逻辑。
            // if (_isPiratied) { ... Environment.FailFast(null); ... }



            // 1. 更新系统时间 (右下角)
            tssTime.Text = DateTime.Now.ToString("HH:mm:ss");

            tssTime.Text = DateTime.Now.ToString("HH:mm:ss");

            long sumTx = 0, sumErr = 0;
            long maxLastDelay = 0;   // 记录所有通道中最慢的那个实时延迟
            double maxRecentErrRate = 0; // 记录最严重的近期丢包率
            int maxConsecutiveErr = 0;
            int onlineChannels = 0;

            foreach (var master in _runningMasters.Values)
            {
                sumTx += master.TxCount;
                sumErr += master.ErrCount;

                // --- 核心改动：取实时值 ---
                if (master.LastResponseTimeMs > maxLastDelay)
                    maxLastDelay = master.LastResponseTimeMs;

                if (master.RecentErrorRate > maxRecentErrRate)
                    maxRecentErrRate = master.RecentErrorRate;

                if (master.ConsecutiveErrors > maxConsecutiveErr)
                    maxConsecutiveErr = master.ConsecutiveErrors;

                if (master.IsOnline) onlineChannels++;
            }

            // 更新状态栏统计：显示实时延迟和近期丢包率
            // 这样只要一断线，丢包率会在 20 秒内迅速冲向 100%，恢复后也会迅速降回 0%
            tssStats.Text = $"TX: {sumTx} | 近期丢包: {maxRecentErrRate:F0}% | 实时延迟: {maxLastDelay}ms";

            // 颜色逻辑改为根据“近期丢包率”判定
            if (maxRecentErrRate > 50) tssStats.ForeColor = Color.Red;
            else if (maxRecentErrRate > 5) tssStats.ForeColor = Color.DarkOrange;
            else tssStats.ForeColor = Color.Black;

            // ================================================================
            // 4. 动态更新状态提示与警报 (tssStatus - 左下角)
            // ================================================================
            if (_runningMasters.Count > 0)
            {
                // 判定：如果存在严重的连续错误（比如连续3次以上失败）
                if (maxConsecutiveErr >= 3)
                {
                    tssStatus.Text = $"⚠️ 通讯异常：检测到设备连续 {maxConsecutiveErr} 次无响应！";
                    tssStatus.ForeColor = Color.Red;
                }
                else
                {
                    // 正常运行时显示：在线通道/总通道
                    tssStatus.Text = $"正在运行 ... [在线通道: {onlineChannels}/{_runningMasters.Count}]";
                    tssStatus.ForeColor = Color.DarkGreen;
                }
            }
            else
            {
                // 停止时显示：就绪 (或者当前加载的工程名)
                string projectName = string.IsNullOrEmpty(_currentFilePath) ? "未命名工程" : Path.GetFileName(_currentFilePath);
                tssStatus.Text = $"就绪 - {projectName}";
                tssStatus.ForeColor = Color.Black;
            }

            // ================================================================
            // 5. 刷新卡片状态 (离线变灰检查)
            // ================================================================
            foreach (Control ctrl in flowDashboard.Controls)
            {
                if (ctrl is UC_WidgetBase widget)
                {
                    widget.UpdateConnectionState();
                }
            }
        }
        private void CheckAntiPiracy()
        {
            // 1. 先看有没有豁免权 (特权阶级)
            // 如果是专业版 OR 公测期，直接放行，不管他有多少设备
            bool hasPrivilege = LicenseGuard.IsProUser() || LicenseGuard.IsBetaMode;
            if (hasPrivilege) return; // 尊贵用户，请便

            // ---------------------------------------------------------
            // 能走到这里，说明用户现在的身份只能是【免费版】
            // 接下来检查他的资产是否“越界”
            // ---------------------------------------------------------
            bool isHacked = false;

            int totalChannelCount = 0;
            int totalDeviceCount = 0;
            int totalPointCount = 0;
            foreach (var channel in _runningMasters.Keys)
            {
                totalChannelCount +=1;
                totalDeviceCount += channel.Devices.Count;
                foreach (var device in channel.Devices)
                {
                    totalPointCount += device.Points.Count;
                }
            }
            if (totalChannelCount > LicenseGuard.MAX_FREE_CHANNELS) isHacked = true;
            if (totalDeviceCount > LicenseGuard.MAX_FREE_DEVICES) isHacked = true;
            if (totalPointCount > LicenseGuard.MAX_FREE_TAGS) isHacked = true;

            // 检查 3: 仪表盘卡片数量是否超标？
            // (假设免费版只允许 20 个)
            int currentWidgetCount = this.flowDashboard.Controls.Count;
            if (currentWidgetCount > LicenseGuard.MAX_FREE_WIDGETS)
            {
                isHacked = true;
            }

            // ---------------------------------------------------------
            // 审判时刻
            // ---------------------------------------------------------
            if (isHacked)
            {
                // 标记为盗版，等待下一次循环触发“暴毙”或“数据归零”
                _isPiratied = true;

                // 也可以记录一下日志，方便自己排查（可选）
                // System.Diagnostics.Debug.WriteLine("检测到非法越权：免费身份使用了过多资源");
            }
        }
        // === 事件绑定 ===
        private void BindMenuEvents()
        {
            // 视图菜单逻辑绑定
            tsmiLogMonitorMenu.Click += (s, e) => tsbLogMonitor.PerformClick();
            tsmiTrendChartMenu.Click += (s, e) => tsbTrendChart.PerformClick();
            tsmiFullScreen.Click += (s, e) => ToggleFullScreen();
        }
        private void BindEvents()
        {
            // 1. 绑定右键删除事件
            // 直接复用我们写好的 RemoveNode 方法
            itemDelete.Click += (s, e) => RemoveNode();

            // 顶部菜单 - 连接真实的 Save/Load 方法
            menuSave.Click += (s, e) => SaveProject();
            // 【新增】菜单栏另存为 (假设你叫 menuSaveAs)
            if (this.menuSaveAs != null) menuSaveAs.Click += (s, e) => SaveProjectAs();
            menuLoad.Click += (s, e) => LoadProject();

            // 1. 文件操作区
            tsbSave.Click += (s, e) => SaveProject();
            if (this.tsbSaveAs != null) tsbSaveAs.Click += (s, e) => SaveProjectAs();
            tsbOpen.Click += (s, e) => LoadProject();

            //2. 语言切换区
            tsmiCn.Click += (s, e) => tsmiCn_Click(s,e);
            tsmiEn.Click += (s, e) => tsmiEn_Click(s,e);

            // 2. 监视区
            tsbLogMonitor.Click += (s, e) =>
            {
                F_LogMonitor.Instance.Show();
                F_LogMonitor.Instance.BringToFront();
            };

            // 3. 趋势图
            tsbTrendChart.Click += (s, e) => OpenTrendChart();

            // 树形菜单事件
            treeView.AfterSelect += (s, e) => UpdateToolbarState();

            // 右键菜单 -> 修改配置
            itemEdit.Click += (s, e) => EditSelectedNode();
            itemToggleEnable.Click += (s, e) => ToggleDeviceEnable();
            // 3. 【关键】在菜单弹出前，根据选中项动态改变文字 (Enable 还是 Disable?)
            // 找到右键菜单的 Opening 事件
            if (itemDelete.Owner is ContextMenuStrip ctxMenu)
            {
                ctxMenu.Opening += (s, e) =>
                {
                    var node = treeView.SelectedNode;

                    // 只有选中的是“设备”时，才显示这个菜单项
                    bool isDevice = node != null && node.Tag is DeviceConfig;

                    itemToggleEnable.Visible = isDevice;

                    if (isDevice)
                    {
                        var dev = node.Tag as DeviceConfig;
                        // 动态改名
                        itemToggleEnable.Text = dev.IsEnabled ? "⛔ 禁用此设备 (Disable)" : "✅ 启用此设备 (Enable)";
                    }
                };
            }
            // 双击 -> 打开监控 (只针对设备)
            treeView.NodeMouseDoubleClick += (s, e) =>
            {
                if (e.Node.Tag is DeviceConfig dev) OpenDeviceMonitor(dev);
                else EditSelectedNode();
            };

            // 左侧工具栏按钮绑定
            btnAddChannel.Click += (s, e) => CreateChannel();
            btnAddDevice.Click += (s, e) => CreateDevice();
            btnRemove.Click += (s, e) => RemoveNode();
            btnConfig.Click += (s, e) => EditSelectedNode();

            btnStart.Click += (s, e) => ToggleChannelState(true);
            btnStop.Click += (s, e) => ToggleChannelState(false);

            // 点击空白区域取消选中
            treeView.MouseDown += (s, e) =>
            {
                var hit = treeView.HitTest(e.Location);
                if (hit.Location == TreeViewHitTestLocations.None || hit.Location == TreeViewHitTestLocations.RightOfLabel)
                {
                    treeView.SelectedNode = null;
                    UpdateToolbarState();
                }
            };
            treeView.NodeMouseClick += (s, e) =>
            {
                if (e.Button == MouseButtons.Right)
                {
                    // 强制选中被点击的节点
                    treeView.SelectedNode = e.Node;

                    // (可选) 根据节点类型控制菜单项的可用性
                    // 比如：根节点不能删？或者某些状态下不能删？
                    // itemDelete.Enabled = (e.Node.Tag != null); 
                }
            };
        }
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            // 1. Ctrl + S 保存
            if (keyData == (Keys.Control | Keys.S))
            {
                SaveProject();
                return true; // 表示已处理
            }

            // 2. Ctrl + O 打开
            if (keyData == (Keys.Control | Keys.O))
            {
                LoadProject();
                return true;
            }

            // 3. F11 全屏切换
            if (keyData == Keys.F11)
            {              
                ToggleFullScreen();
                return true;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }
        private void TsmiEn_Click(object? sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void TsmiCn_Click(object? sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void BindHelpMenuEvents()
        {
            // 在线文档
            tsmiOnlineHelp.Click += (s, e) =>
            {
                try
                {
                    LinkManager.Open(LinkManager.DocUrl); // <--- 动态获取
                }
                catch (Exception ex)
                {
                    MessageBox.Show("无法打开浏览器: " + ex.Message);
                }
            };

            // 问题反馈
            tsmiBugReport.Click += (s, e) =>
            {
                try
                {
                    LinkManager.Open(LinkManager.IssueUrl); // <--- 动态获取
                }
                catch { }
            };

            // 2. 操作指引 (复用之前的逻辑)
            tsmiShowTips.Click += (s, e) =>
            {
                // 模拟点击工具栏那个 Help 按钮，或者直接显示 Panel
                if (pnlTips != null)
                {
                    // 让它显示在屏幕中央或者某个显眼位置
                    pnlTips.Location = new Point(
                        (this.ClientSize.Width - pnlTips.Width) / 2,
                        (this.ClientSize.Height - pnlTips.Height) / 2
                    );
                    pnlTips.Visible = true;
                    pnlTips.BringToFront();
                }
            };

            // 3. 激活注册
            tsmiRegistration.Click += (s, e) =>
            {
                using (var frm = new F_Registration())
                {
                    // 使用 ShowDialog 模态显示，用户必须关掉它才能操作主界面
                    frm.ShowDialog(this);
                }
            };

            // 3. 关于窗口
            tsmiAbout.Click += (s, e) =>
            {
                using (var frm = new F_About1())
                {
                    // 使用 ShowDialog 模态显示，用户必须关掉它才能操作主界面
                    frm.ShowDialog(this);
                }
            };
        }
        // 修改标题栏的辅助方法
       

        #region 主题卡片
        private void InitThemeSelector()
        {
            cmbTheme.Items.Add(UITheme.DefaultWhite);
            cmbTheme.Items.Add(UITheme.TechBlue);   // 新增
            cmbTheme.Items.Add(UITheme.DarkMode);
            cmbTheme.Items.Add(UITheme.Industrial);

            cmbTheme.ComboBox.DisplayMember = "Name";  // 显示 "极客黑" 等名字
            cmbTheme.SelectedIndexChanged += CmbTheme_SelectedIndexChanged;

            // 【新增】读取保存的主题设置
            string savedThemeName = Properties.Settings.Default.CardTheme;
            // 查找并选中
            bool found = false;
            foreach (var item in cmbTheme.Items)
            {
                if (item is UITheme theme && theme.Name == savedThemeName)
                {
                    cmbTheme.SelectedItem = item; // 这会触发 SelectedIndexChanged 应用主题
                    found = true;
                    break;
                }
            }

            // 如果没存过或者找不到，默认选第一个
            if (!found && cmbTheme.Items.Count > 0)
            {
                cmbTheme.SelectedIndex = 0;
            }
        }
        private UITheme _currentTheme = UITheme.DefaultWhite;
        private void CmbTheme_SelectedIndexChanged(object sender, EventArgs e)
        {
            var theme = cmbTheme.SelectedItem as UITheme;
            if (theme == null) return;

            _currentTheme = theme; // 记录下来

            // 1. 改变背景色
            flowDashboard.BackColor = theme.DashboardBack;

            // 2. 遍历所有卡片应用主题
            // 暂时挂起布局逻辑，防止闪烁
            flowDashboard.SuspendLayout();

            foreach (Control ctrl in flowDashboard.Controls)
            {
                if (ctrl is UC_WidgetBase widget)
                {
                    widget.ApplyTheme(theme);
                }
            }

            flowDashboard.ResumeLayout();

            // 【新增】立即保存到用户设置
            Properties.Settings.Default.CardTheme = theme.Name;
            Properties.Settings.Default.Save();
        }

        #endregion

        #region 操作提示

        // 定义控件
        private Panel pnlTips;
     
        // 1. 在类成员位置声明变量
        private ToolStripButton btnHelp;

        // 2. 初始化方法
        private void InitHelpButton()
        {
            btnHelp = new ToolStripButton();

            // --- 核心设置 ---
            btnHelp.Name = "btnHelp";
            btnHelp.Text = LangProvider.Get("Btn_Help"); // 这里用 Emoji 当图标，后面跟文字
                                      // 如果只想显示灯泡，就写: btnHelp.Text = "💡"; 

            // 设置显示模式为纯文本 (因为我们用字符代替了 Image)
            btnHelp.DisplayStyle = ToolStripItemDisplayStyle.Text;

            // --- 视觉微调 ---
            // 使用稍大一点的字体，或者专门支持 Emoji 的字体，让灯泡看起来更清晰
            // Segoe UI Emoji 是 Win10/11 自带的 Emoji 字体，如果没有会自动回退
            btnHelp.Font = new Font("Microsoft YaHei UI", 9F, FontStyle.Regular);

            // 鼠标悬停时的原生提示 (作为双重保障)
            btnHelp.AutoToolTip = false;
            btnHelp.ToolTipText = "";
            //btnHelp.ToolTipText = "查看常用操作快捷键";

            // --- 布局设置 ---
            // (可选) 如果你想让帮助按钮靠右对齐，显得更像系统功能
            // btnHelp.Alignment = ToolStripItemAlignment.Right; 

            // --- 加入工具栏 ---
            // 假设你的工具栏控件名叫 toolStrip1
            if (toolStripMain != null)
            {
                // 加个分隔符，跟前面的功能区隔开，显得稍微正式点
                toolStripMain.Items.Add(new ToolStripSeparator());
                toolStripMain.Items.Add(btnHelp);
            }
        }

        private void InitTipsSystem()
        {
            pnlTips = new Panel
            {
                Size = new Size(320, 420),
                Visible = false,
                BackColor = Color.FromArgb(45, 45, 48), // 稍微浅一点的深灰色，与卡片区分
                Padding = new Padding(1), // 给边框留出 1 像素
                Name = "pnlTips"
            };

            htmlPanelTips = new HtmlPanel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(30, 30, 30), // 内容区背景
                IsContextMenuEnabled = false,
                IsSelectionEnabled = false,
            };

            // 【关键：字体栈】加上 'Segoe UI Emoji' 解决图标变方块的问题
            htmlPanelTips.BaseStylesheet = @"
        body { 
            font-family: 'Segoe UI Emoji', 'Microsoft YaHei UI', 'Arial'; 
            font-size: 7pt; 
            color: #DCDCDC; 
            margin: 0; 
            padding: 0; 
        }
        b { color: #FFFFFF; }";

            pnlTips.Controls.Add(htmlPanelTips);
            this.Controls.Add(pnlTips);
            pnlTips.BringToFront();

            FillHelpContent();
            BindHoverEvents();
        }

        // 辅助方法：填充富文本内容
        private void FillHelpContent()
        {
            // 一行搞定，再也不用管各种 SelectionColor 切换了
            htmlPanelTips.Text = LangProvider.Get("Guide_Html");
        }

       
        private void BindHoverEvents()
        {
            // 假设你的工具栏按钮叫 btnHelp
            // 注意：如果是 ToolStripButton，它没有 MouseEnter 事件，只有 MouseHover
            // 但 ToolStripItem 有 MouseEnter

            btnHelp.MouseEnter += (s, e) =>
            {
                // 动态计算位置：按钮正下方
                // 注意：btnHelp 是 ToolStripItem，要用 Owner 获取 ToolStrip 的位置
                var strip = btnHelp.Owner;
                if (strip == null) return;

                Point btnLoc = btnHelp.Bounds.Location;
                // 转换为屏幕坐标再转回 Form 坐标，或者直接计算
                int x = strip.Location.X + btnLoc.X;
                // 稍微往左偏一点，防止超出屏幕右边界
                if (x + pnlTips.Width > this.Width) x = this.Width - pnlTips.Width - 10;

                int y = strip.Location.Y + strip.Height;

                pnlTips.Location = new Point(x, y);
                pnlTips.Visible = true;
                pnlTips.BringToFront();
            };

            // 逻辑：离开按钮时，如果没进入 Panel，就隐藏
            btnHelp.MouseLeave += (s, e) =>
            {
                // 延迟一丢丢判断，防止鼠标移动过快产生的缝隙问题
                Task.Delay(50).ContinueWith(t =>
                {
                    this.Invoke((System.Windows.Forms.MethodInvoker)delegate
                    {
                        Point cursor = this.PointToClient(Cursor.Position);
                        if (!pnlTips.Bounds.Contains(cursor))
                        {
                            pnlTips.Visible = false;
                        }
                    });
                });
            };

            // 逻辑：离开 Panel 时隐藏
            pnlTips.MouseLeave += (s, e) => pnlTips.Visible = false;
            htmlPanelTips.MouseLeave += (s, e) => pnlTips.Visible = false; // RTB 也要绑，因为它遮住了 Panel
        }

        #endregion

        #region 1. 资源管理器逻辑 (增删改)

        private void CreateChannel()
        {
            // 【埋雷点 1】
            if (!LicenseGuard.CanAddChannel(_projectData.Count)) return;

            var frm = new F_ChannelConfig();

            // 【新增】传入当前已有的所有通道名，用于查重
            frm.ForbiddenNames = _projectData.Select(c => c.ChannelName).ToList();

            if (frm.ShowDialog() == DialogResult.OK)
            {
                var newChannel = frm.Config;
                _projectData.Add(newChannel);

                var node = treeView.Nodes.Add($"📂 {newChannel.ChannelName} ({newChannel.Type})");
                node.Tag = newChannel;
                treeView.SelectedNode = node;
            }
        }

        private void CreateDevice()
        {
            var selNode = treeView.SelectedNode;
            // 必须选中通道节点
            if (selNode == null || !(selNode.Tag is ChannelConfig))
            {
                MessageBox.Show(LangProvider.Get("Msg_SelChannel"));
                return;
            }

            var channel = selNode.Tag as ChannelConfig;

            // 【埋雷点 2】
            if (!LicenseGuard.CanAddDevice(channel.Devices.Count)) return;

            // 智能计算默认 ID (最大值 + 1)
            byte nextId = (byte)((channel.Devices.Count > 0 ? channel.Devices.Max(d => d.SlaveId) : 0) + 1);

            var newDevice = new DeviceConfig
            {
                //DeviceName = "新设备",
                DeviceName = LangProvider.Get("Def_NewDevice"),
                SlaveId = nextId
            };

            // 弹出配置窗口
            var frm = new F_ModbusAddrManager(newDevice);

            // 【新增】传入该通道下现有的所有 ID
            frm.ForbiddenSlaveIds = channel.Devices.Select(d => d.SlaveId).ToList();

            if (frm.ShowDialog() == DialogResult.OK)
            {
                // 保存数据
                channel.Devices.Add(frm.CurrentDevice);

                var devNode = selNode.Nodes.Add($"📄 {newDevice.DeviceName} (ID:{newDevice.SlaveId})");
                devNode.Tag = newDevice;
                selNode.Expand();

                // 【新增】清洗孤岛
                CleanUpGhostWidgets();
            }
        }

        private void EditSelectedNode()
        {
            var node = treeView.SelectedNode;
            if (node == null) return;

            if (node.Tag is ChannelConfig ch)
            {
                var frm = new F_ChannelConfig(ch);
                frm.ForbiddenNames = _projectData.Select(c => c.ChannelName).ToList();
                if (frm.ShowDialog() == DialogResult.OK)
                    node.Text = $"📂 {ch.ChannelName} ({ch.Type})";
            }
            else if (node.Tag is DeviceConfig dev)
            {
                var frm = new F_ModbusAddrManager(dev);
                if (node.Parent != null && node.Parent.Tag is ChannelConfig parentChannel)
                {
                    frm.ForbiddenSlaveIds = parentChannel.Devices.Select(d => d.SlaveId).ToList();
                }

                if (frm.ShowDialog() == DialogResult.OK)
                {
                    // 统计当前项目总点位
                    int totalTags = _projectData.Sum(c => c.Devices.Sum(d => d.Points.Count));

                    if (!LicenseGuard.CanSupportTagCount(totalTags))
                    {
                        // 如果用户在弹出的注册窗里也没激活，我们可以采取“软截断”策略
                        MessageBox.Show($"由于未获得专业版授权，系统将仅处理前 {LicenseGuard.MAX_FREE_TAGS} 个变量点位，超出部分将不会参与轮询更新。",
                                        "点位数量受限", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }

                    // 更新节点显示
                    node.Text = $"📄 {dev.DeviceName} (ID:{dev.SlaveId})";

                    // 热更新指令
                    ReloadMasterCommands(dev);
                    // 【新增】清洗孤岛
                    CleanUpGhostWidgets();
                }
            }
        }

        private void RemoveNode()
        {
            var node = treeView.SelectedNode;
            if (node == null) return;

            string msg = string.Format(LangProvider.Get("Msg_DelNode"), node.Text);
            string title = LangProvider.Get("Title_DelConfirm");
            // 二次确认
            if (MessageBox.Show(msg, title, MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
            {
                return;
            }

            // === 场景 A: 删除通道 ===
            if (node.Tag is ChannelConfig ch)
            {
                // 1. 停止并移除驱动
                if (_runningMasters.ContainsKey(ch))
                {
                    var master = _runningMasters[ch];
                    master.Stop();
                    master.Dispose();
                    _runningMasters.Remove(ch);
                }

                // 2. 清理该通道下所有设备的资源
                foreach (var dev in ch.Devices)
                {
                    // 关闭监控窗口
                    if (_openMonitors.ContainsKey(dev))
                    {
                        _openMonitors[dev].Close();
                        _openMonitors.Remove(dev);
                    }
                    // 【新增】清理仪表盘上的相关卡片
                    RemoveWidgetsByDevice(dev);
                }

                // 3. 移除数据
                _projectData.Remove(ch);

                // 4. 移除界面节点
                node.Remove();
            }
            // === 场景 B: 删除设备 ===
            else if (node.Tag is DeviceConfig dev)
            {
                // 1. 关闭监控窗口
                if (_openMonitors.ContainsKey(dev))
                {
                    _openMonitors[dev].Close();
                    _openMonitors.Remove(dev);
                }

                // 2. 【新增】清理仪表盘上的相关卡片
                RemoveWidgetsByDevice(dev);

                // 3. 从父通道中移除数据
                // 需要找到父节点对应的通道配置
                if (node.Parent != null && node.Parent.Tag is ChannelConfig parentCh)
                {
                    parentCh.Devices.Remove(dev);

                    // 4. 热更新：如果通道正在运行，需要更新轮询列表
                    // (如果不更新，驱动还会继续请求这个设备的地址，导致超时)
                    if (_runningMasters.ContainsKey(parentCh))
                    {
                        //ReloadMasterCommands(dev); // 这里传入 dev 其实是为了找通道，逻辑通用
                        // 或者直接调用一个针对通道的刷新方法
                        RefreshChannelCommands(parentCh); // 建议封装一个这样的方法
                    }
                }

                // 5. 移除界面节点
                node.Remove();
            }

            // 【新增】清洗孤岛
            CleanUpGhostWidgets();
            // 刷新按钮状态
            UpdateToolbarState();
        }
        private void ToggleDeviceEnable()
        {
            var node = treeView.SelectedNode;
            if (node == null || !(node.Tag is DeviceConfig dev)) return;

            // 1. 切换数据状态
            dev.IsEnabled = !dev.IsEnabled;

            // 2. 【核心修复】立即刷新 UI (不管通不通道是否在运行)
            RefreshDeviceNodeVisual(node, dev);

            // 3. 仅当通道正在运行时，才去通知驱动更新指令
            if (node.Parent != null && node.Parent.Tag is ChannelConfig ch)
            {
                if (_runningMasters.ContainsKey(ch))
                {
                    RefreshChannelCommands(ch);
                }
            }
        }
        /// <summary>
        /// 通用的单节点视觉刷新方法
        /// </summary>
        private void RefreshDeviceNodeVisual(TreeNode node, DeviceConfig dev)
        {
            // 1. 清洗图标
            string cleanText = node.Text
                .Replace("🟢 ", "")
                .Replace("🔴 ", "")
                .Replace("⛔ ", "")
                .Trim();

            // 2. 判断状态
            if (!dev.IsEnabled)
            {
                // 禁用状态：优先显示禁用标
                node.Text = $"⛔ {cleanText}";
                node.ForeColor = Color.Gray;
                node.ToolTipText = "设备已禁用 (Disabled)";
            }
            else
            {
                // 启用状态：
                // 这里有个小细节：如果通道正在运行，这里暂时变回黑色，
                // 等下一次轮询结果回来(几十毫秒后)，它会自动变红或变绿。
                // 如果通道没运行，就直接变黑。
                node.Text = cleanText;
                node.ForeColor = Color.Black;
                node.ToolTipText = "";
            }
        }
        // 辅助：通用样式更新
        private void UpdateNodeStyle(TreeNode node)
        {
            if (node.Tag is DeviceConfig dev)
            {
                if (!dev.IsEnabled)
                {
                    node.ForeColor = Color.Gray;
                    // 如果你想做得更细，可以把字体改成斜体
                    // node.NodeFont = new Font(treeView.Font, FontStyle.Italic);
                }
                else
                {
                    node.ForeColor = Color.Black;
                    // node.NodeFont = new Font(treeView.Font, FontStyle.Regular);
                }
            }
        }
        private void RefreshChannelCommands(ChannelConfig ch)
        {
            if (!_runningMasters.ContainsKey(ch)) return;

            var master = _runningMasters[ch];

            // 重新打包该通道下 剩余所有设备 的点位
            var allCommands = new List<ModbusCommand>();
            foreach (var d in ch.Devices)
            {
                // 【新增】关键改动：如果设备被禁用，直接跳过，不生成指令
                if (!d.IsEnabled) continue;

                var cmds = CommandPacker.Pack(d.Points);
                foreach (var c in cmds) c.SlaveId = d.SlaveId;
                allCommands.AddRange(cmds);
            }

            // 下发新列表 (Master 内部是线程安全的)
            master.SetReadCommands(allCommands);
        }
        private void RemoveWidgetsByDevice(DeviceConfig dev)
        {
            // 倒序遍历，因为我们要从集合中移除元素
            for (int i = flowDashboard.Controls.Count - 1; i >= 0; i--)
            {
                if (flowDashboard.Controls[i] is UC_WidgetBase widget)
                {
                    // 判断卡片绑定的点位是否属于该设备
                    if (dev.Points.Contains(widget.BoundPoint))
                    {
                        flowDashboard.Controls.RemoveAt(i);
                        widget.Dispose(); // 释放资源
                    }
                }
            }
        }

        #endregion

        #region 2. 运行控制逻辑 (Start/Stop)

        private void ToggleChannelState(bool start)
        {
            var node = treeView.SelectedNode;
            if (node == null) return;

            // 智能查找：如果选中设备，则找其父通道
            var ch = node.Tag as ChannelConfig;
            if (ch == null && node.Tag is DeviceConfig)
            {
                ch = node.Parent?.Tag as ChannelConfig;
                node = node.Parent;
            }

            if (ch == null) return;

            if (start)
            {
                if (!_runningMasters.ContainsKey(ch))
                {
                    node.Text += LangProvider.Get("Node_Connecting"); 
                    node.ForeColor = Color.DarkOrange;
                    Application.DoEvents();

                    StartChannel(ch, node);
                }
            }
            else
            {
                if (_runningMasters.ContainsKey(ch))
                {
                    var master = _runningMasters[ch];
                    // 1. 先从运行列表中移除！
                    // 这样后续飞回来的 OnMasterResponse 在 if check 时就会失败，从而被拦截
                    _runningMasters.Remove(ch);

                    // 2. 停止驱动 (虽然 Remove 了，但引用还在 master 变量里)
                    // 建议：Master 内部最好加一个标志位，比如 IsRunning = false
                    master.Stop();

                    // 【新增】停止后，通知卡片："Master 挂了，别发指令了"
                    SyncDashboardMasters(ch, null);

                    SetNodeRunningState(node, false);

                    // 5. 释放资源
                    master.Dispose();
                }
            }
            UpdateToolbarState();
        }

        private void StartChannel(ChannelConfig ch, TreeNode node)
        {
            try
            {
                ITransport transport = null;
                IModbusCodec codec = null;

                // 传入 ChannelName 以便在日志中区分
                if (ch.Type == CommType.Serial)
                {
                    transport = new RtuTransport(ch.ChannelName, ch.PortName, ch.BaudRate, ch.DataBits, ch.StopBits, ch.Parity);
                    codec = new ModbusRtuCodec();
                }
                else
                {
                    transport = new TcpTransport(ch.ChannelName, ch.IpAddress, ch.TcpPort);
                    codec = new ModbusTcpCodec();
                }

                var master = new ModbusMaster(transport, codec);
                master.Interval = ch.MinInterval;

                // 【修复】只绑定 OnResponseReceived，不再绑定 OnMessageCached
                master.OnResponseReceived += (cmd) => OnMasterResponse(cmd, ch);

                master.Start();

                // 生成并下发初始指令
                var allCommands = new List<ModbusCommand>();
                foreach (var dev in ch.Devices)
                {
                    var cmds = CommandPacker.Pack(dev.Points);
                    foreach (var c in cmds) c.SlaveId = dev.SlaveId;
                    allCommands.AddRange(cmds);
                }
                master.SetReadCommands(allCommands);

                _runningMasters.Add(ch, master);

                // 【新增】启动成功后，通知仪表盘上的卡片："兄弟们，我有 Master 了！"
                SyncDashboardMasters(ch, master);

                SetNodeRunningState(node, true);
            }
            catch (Exception ex)
            {
                MessageBox.Show("启动失败: " + ex.Message);
                SetNodeRunningState(node, false);
            }
        }

        private void ReloadMasterCommands(DeviceConfig dev)
        {
            ChannelConfig targetCh = null;
            foreach (var ch in _projectData) if (ch.Devices.Contains(dev)) targetCh = ch;

            if (targetCh != null && _runningMasters.ContainsKey(targetCh))
            {
                var master = _runningMasters[targetCh];
                // 1. 暂停 Master (这会断开连接，但不销毁线程)
                // 注意：ModbusMaster 需要增加一个 Restart() 或 Pause/Resume 方法
                // 或者简单粗暴点：
                master.Stop(); // 先停掉 (这会释放 Socket)

                var allCommands = new List<ModbusCommand>();
                foreach (var d in targetCh.Devices)
                {
                    var cmds = CommandPacker.Pack(d.Points);
                    foreach (var c in cmds) c.SlaveId = d.SlaveId;
                    allCommands.AddRange(cmds);
                }
                master.SetReadCommands(allCommands);

                // 4. 【关键】同步仪表盘卡片的引用（让卡片持有的点位对象也变成最新的）
                SyncDashboardPoints(targetCh);

                // 3. 重新启动
                // 因为 Stop() 会结束线程，所以 Start() 会重新创建线程和连接
                master.Start();

                // [日志]
                LogHub.Write(targetCh.ChannelName, LogType.Info, "配置变更，连接已重置");
            
            if (_openMonitors.ContainsKey(dev))
                {
                    _openMonitors[dev].Close();
                    _openMonitors.Remove(dev);
                }
            }
        }
        private void SyncDashboardPoints(ChannelConfig ch)
        {
            foreach (Control ctrl in flowDashboard.Controls)
            {
                if (ctrl is UC_WidgetBase widget)
                {
                    // 1. 【安全检查】只同步属于这个通道的卡片
                    if (widget.ChannelName != ch.ChannelName) continue;

                    // 2. 【核心修复】利用 Identity 里的 SlaveId 精准锁定设备
                    // 不再用 SelectMany 全局盲搜，而是去它该去的那个设备里找
                    var targetDev = ch.Devices.FirstOrDefault(d => d.SlaveId == widget.SlaveId);

                    if (targetDev != null)
                    {
                        // 3. 在这个特定设备里，利用 Equals 找到对应点位
                        var livePoint = targetDev.Points.FirstOrDefault(p => p.Equals(widget.BoundPoint));

                        if (livePoint != null)
                        {
                            // 引用重连
                            widget.BoundPoint = livePoint;
                        }
                    }
                }
            }
        }
        #endregion

        #region 3. 数据分发逻辑 (核心)

        // 此方法在后台线程运行 (由 ModbusMaster 触发)
        private void OnMasterResponse(ModbusCommand cmd, ChannelConfig ch)
        {
            // 1. 更新主界面的设备状态 (红/绿灯)
            this.BeginInvoke(new Action(() =>
            {
                // =================================================================
                // 【核心修复】防“诈尸”逻辑
                // 如果 _runningMasters 里已经没有这个通道了，说明用户点击了停止。
                // 此时收到的任何回调（无论成功失败）都是由于网络延迟造成的“遗言”。
                // 直接丢弃，防止它把已经重置为黑色的 UI 又刷成红色/绿色。
                // =================================================================
                if (!_runningMasters.ContainsKey(ch))
                {
                    return;
                }

                TreeNode channelNode = FindChannelNode(ch);
                if (channelNode != null)
                {
                    bool isSuccess = (cmd.ResultStatus == CommStatus.Success);
                    UpdateDeviceStatus(channelNode, cmd.SlaveId, isSuccess, cmd.ErrorMessage);
                }

                // 如果通讯失败，就不解析数据了
                if (cmd.ResultStatus != CommStatus.Success) return;

                //广播给打开的监控窗口
                foreach (var kvp in _openMonitors)
                {
                    var dev = kvp.Key;
                    var frm = kvp.Value;

                    if (ch.Devices.Contains(dev) && dev.SlaveId == cmd.SlaveId)
                    {
                        if (!frm.IsDisposed) frm.RefreshData();
                    }
                }

                // 【新增】刷新仪表盘上的卡片
                foreach (Control ctrl in flowDashboard.Controls)
                {
                    // 【修改】只识别基类 UC_WidgetBase
                    if (ctrl is UC_WidgetBase widget)
                    {
                        if (widget.ChannelName == ch.ChannelName && widget.SlaveId == cmd.SlaveId)
                        {
                            if (cmd.RelatedPoints.Contains(widget.BoundPoint))
                            {
                                string valStr = widget.BoundPoint.CurrentValue?.ToString() ?? "-";
                                // 统一在这里处理一下 Bool 的显示字符，或者交给子类去处理
                                // 这里直接传原始 String，子类自己会判断
                                widget.UpdateValue(valStr);
                            }
                        }
                    }
                }

                // 【新增】更新统计数据
                _totalTx++; // 每次请求算一次 TX
                if (cmd.ResultStatus == CommStatus.Success)
                {
                    _totalRx++;
                }
                else
                {
                    _totalErr++;
                }
            }));
        }

        private TreeNode FindChannelNode(ChannelConfig targetCh)
        {
            foreach (TreeNode node in treeView.Nodes)
            {
                if (node.Tag == targetCh) return node;
            }
            return null;
        }

        #endregion

        #region 4. 辅助界面逻辑

        private void OpenDeviceMonitor(DeviceConfig dev)
        {
            ChannelConfig parentCh = null;
            foreach (var c in _projectData) if (c.Devices.Contains(dev)) parentCh = c;

            if (parentCh == null) return;

            ModbusMaster master = null;
            if (_runningMasters.ContainsKey(parentCh)) master = _runningMasters[parentCh];

            if (master == null)
            {
                MessageBox.Show(LangProvider.Get("Msg_NoMonitor"));
                return;
            }

            if (_openMonitors.ContainsKey(dev))
            {
                var exist = _openMonitors[dev];
                if (exist.IsDisposed) _openMonitors.Remove(dev);
                else { exist.Activate(); return; }
            }

            var frm = new F_DeviceMonitor(dev, master);
            frm.CurrentChannelName = parentCh.ChannelName;
            frm.Show();
            frm.FormClosed += (s, e) => _openMonitors.Remove(dev);
            _openMonitors.Add(dev, frm);
        }

        private void OpenTrendChart()
        {
            var master = _runningMasters.Values.FirstOrDefault();
            if (master == null)
            {
                MessageBox.Show("请先启动至少一个通道", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            F_TrendChart.Instance.Show();
            F_TrendChart.Instance.BringToFront();
        }

        private void UpdateToolbarState()
        {
            var node = treeView.SelectedNode;
            if (node == null)
            {
                btnStart.Enabled = false;
                btnStop.Enabled = false;
                btnConfig.Enabled = false;
                btnAddDevice.Enabled = false;
                return;
            }

            bool isChannel = node?.Tag is ChannelConfig;
            bool isDevice = node?.Tag is DeviceConfig;

            ChannelConfig ch = null;
            if (isChannel) ch = node.Tag as ChannelConfig;
            else if (isDevice) ch = node.Parent.Tag as ChannelConfig;

            bool isRunning = ch != null && _runningMasters.ContainsKey(ch);

            btnStart.Enabled = (isChannel || isDevice) && !isRunning;
            btnStop.Enabled = (isChannel || isDevice) && isRunning;
            btnConfig.Enabled = isChannel && !isRunning;
            btnAddDevice.Enabled = isChannel;
        }

        private void SetNodeRunningState(TreeNode node, bool isOpen)
        {
            if (node == null) return;
            string cleanName = "";
            if (node.Tag is ChannelConfig ch) cleanName = $"📂 {ch.ChannelName} ({ch.Type})";

            //string rawText = node.Text
            //    .Replace(" [已打开]", "")
            //    .Replace(" [连接中...]", "")
            //    .Replace(" [停止]", "");

            if (isOpen)
            {
                node.ForeColor = Color.DarkGreen;
                node.NodeFont = new Font(treeView.Font, FontStyle.Bold);
                //node.Text = rawText + " [已打开]";
                node.Text = cleanName + LangProvider.Get("Node_Open");
                node.Expand();
            }
            else
            {
                node.ForeColor = Color.Black;
                node.NodeFont = new Font(treeView.Font, FontStyle.Regular);
                //node.Text = rawText;
                node.Text = cleanName;

                foreach (TreeNode child in node.Nodes)
                {
                    ResetDeviceNodeStyle(child);
                }
            }
        }

        private void ResetDeviceNodeStyle(TreeNode devNode)
        {
            if (devNode.Tag is DeviceConfig dev)
            {
                // 1. 先把所有可能的图标都清洗掉，只留纯文本
                string cleanText = devNode.Text
                    .Replace("🟢 ", "")
                    .Replace("🔴 ", "")
                    .Replace("⛔ ", "")
                    .Trim(); // 去掉可能的空格

                // 2. 根据当前的“禁用状态”决定显示什么
                if (!dev.IsEnabled)
                {
                    // 如果是禁用的，停止后依然要显示禁用状态
                    devNode.Text = $"⛔ {cleanText}";
                    devNode.ForeColor = Color.Gray;
                    devNode.ToolTipText = "设备已禁用";
                }
                else
                {
                    // 如果是启用的，停止后恢复成普通黑色文本
                    devNode.Text = cleanText;
                    devNode.ForeColor = Color.Black;
                    devNode.ToolTipText = "";
                }

                // 3. 恢复字体样式（防止之前变粗了）
                devNode.NodeFont = new Font(treeView.Font, FontStyle.Regular);
            }
        }

        private void UpdateDeviceStatus(TreeNode channelNode, byte slaveId, bool isSuccess, string msg)
        {
            if (channelNode == null) return;

            foreach (TreeNode devNode in channelNode.Nodes)
            {
                // 【新增】如果设备被禁用了，界面上什么都别动，保持灰色
                if (devNode.Tag is DeviceConfig dev && !dev.IsEnabled) continue;

                if (devNode.Tag is DeviceConfig targetDev && targetDev.SlaveId == slaveId)
                {
                    string rawName = devNode.Text.Replace("🟢 ", "").Replace("🔴 ", "");
                    string newText = isSuccess ? $"🟢 {rawName}" : $"🔴 {rawName}";
                    Color newColor = isSuccess ? Color.DarkGreen : Color.Red;                   
                    string newTip = isSuccess
    ? LangProvider.Get("Status_Normal")
    : string.Format(LangProvider.Get("Status_Error"), msg);

                    if (devNode.Text != newText) devNode.Text = newText;
                    if (devNode.ForeColor != newColor) devNode.ForeColor = newColor;
                    if (devNode.ToolTipText != newTip) devNode.ToolTipText = newTip;

                    break;
                }
            }
        }

        private void EnableDoubleBuffered(Control control)
        {
            typeof(Control).InvokeMember("DoubleBuffered",
                System.Reflection.BindingFlags.SetProperty |
                System.Reflection.BindingFlags.Instance |
                System.Reflection.BindingFlags.NonPublic,
                null, control, new object[] { true });

            // ================================================================
            // 【核心修复】利用反射开启 FlowLayoutPanel 的双缓冲，彻底消除闪烁
            // ================================================================
            Type type = flowDashboard.GetType();
            PropertyInfo pi = type.GetProperty("DoubleBuffered",
                BindingFlags.Instance | BindingFlags.NonPublic);

            if (pi != null)
            {
                pi.SetValue(flowDashboard, true, null);
            }
        }


        #endregion

        #region 5.项目加载与保存

        // --- 保存项目 ---
        private void SaveProject()
        {
            // 1. 如果从来没保存过 (是新建的项目)，转到"另存为"
            if (string.IsNullOrEmpty(_currentFilePath))
            {
                SaveProjectAs();
                return;
            }

            // 2. 如果已经有路径，直接静默保存
            try
            {
                // 收集仪表盘布局
                var layout = CollectDashboardLayout();

                // 保存文件
                ProjectManager.SaveProject(_currentFilePath, _projectData, layout);

                // 更新标题 (防止文件名变了没刷新)
                UpdateTitle();

                string msg = string.Format(LangProvider.Get("Msg_SaveSucc"), _currentFilePath);
                MessageBox.Show(msg, LangProvider.Get("Title_Info"), MessageBoxButtons.OK, MessageBoxIcon.Information);
                
            }
            catch (Exception ex)
            {
                string errMsg = string.Format(LangProvider.Get("Msg_SaveFail"), ex.Message);
                MessageBox.Show(errMsg, LangProvider.Get("Title_Error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // --- 另存为 (Save As) ---
        private void SaveProjectAs()
        {
            using (var sfd = new SaveFileDialog())
            {
                sfd.Filter = "ModbusPilot Project (*.mpp)|*.mpp|All Files (*.*)|*.*";
                sfd.FileName = $"Project_{DateTime.Now:MMdd}";

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        var layout = CollectDashboardLayout();
                        ProjectManager.SaveProject(sfd.FileName, _projectData, layout);

                        // 【关键】更新当前路径和标题
                        _currentFilePath = sfd.FileName;
                        UpdateTitle();

                        // 记录到最近打开，方便下次自动加载
                        SaveLastOpenPath(_currentFilePath);

                        tssProject.Text = sfd.FileName;

                        MessageBox.Show("保存成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"保存失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        // --- 加载项目 ---
        private void LoadProject()
        {
            using (var ofd = new OpenFileDialog())
            {
                ofd.Filter = "ModbusPilot Project (*.mpp)|*.mpp|Json Files (*.json)|*.json";

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    // 1. 确认是否覆盖
                    if (_projectData.Count > 0)
                    {
                        if (MessageBox.Show("加载新项目将覆盖当前配置，是否继续？", "确认", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                        {
                            return;
                        }
                    }

                    // 2. 调用核心加载方法 (不要在这里写重复逻辑！)
                    DoLoadProject(ofd.FileName);
                }
            }
        }

        // --- 核心加载逻辑 (供 LoadProject 和 启动自动加载 共用) ---
        private void DoLoadProject(string path)
        {
            try
            {
                // 1. 停止一切并清空
                StopAllAndClear();

                // 2. 读取文件
                var profile = ProjectManager.LoadProject(path);
                _projectData = profile.Channels;

                // 3. 重建树形菜单
                RebuildTree();

                // 4. 重建仪表盘
                // 注意：ProjectManager 加载时如果 DashboardLayout 为空会自动 new 一个空的 List，不会报错
                RestoreDashboard(profile.DashboardLayout);

                // 5. 更新状态
                _currentFilePath = path;
                UpdateTitle();
                SaveLastOpenPath(path); // 记录成功加载的路径

                tssProject.Text = path;
                tssProject.ToolTipText = path; // 鼠标放上去显示全路径

                // 仅在手动加载时提示，自动加载不需要弹窗
                // (可以通过判断调用来源来决定是否弹窗，这里简单处理：始终不弹窗，或者仅在 LoadProject 里弹窗)
                // 建议：DoLoadProject 保持静默，由调用者决定是否提示成功

                int totalCount = _projectData.Sum(ch => ch.Devices.Sum(dev => dev.Points.Count));

                // 仅仅是提醒，不强制关闭软件，体现“软着陆”
                LicenseGuard.CanSupportTagCount(totalCount);

            }
            catch (Exception ex)
            {
                MessageBox.Show($"加载失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);

                // 如果加载失败（比如文件被删了），清空记录，免得下次启动一直报错
                SaveLastOpenPath("");
            }
        }

        private void SaveLastOpenPath(string path)
        {
            Properties.Settings.Default.LastProjectPath = path;
            Properties.Settings.Default.Save();
        }
        private void LoadLastOpenPath()
        {
            try
            {
                string path = Properties.Settings.Default.LastProjectPath;
                if (!string.IsNullOrEmpty(path) && System.IO.File.Exists(path))
                {
                    DoLoadProject(path);
                }
            }
            catch
            {
                // 自动加载出错忽略即可，不要弹窗吓用户
            }
        }

        // 辅助：停止一切并清空
        private void StopAllAndClear()
        {
            // 停止所有 Master
            foreach (var kvp in _runningMasters)
            {
                kvp.Value.Stop();
                kvp.Value.Dispose();
            }
            _runningMasters.Clear();

            // 关闭所有监控窗口
            foreach (var kvp in _openMonitors)
            {
                kvp.Value.Close();
            }
            _openMonitors.Clear();

            // 清空树
            treeView.Nodes.Clear();
            _projectData.Clear();

            // 刷新按钮状态
            UpdateToolbarState();
        }

        // 辅助：根据 _projectData 重建 TreeView
        private void RebuildTree()
        {
            treeView.BeginUpdate();
            treeView.Nodes.Clear();

            bool hasChannelTruncated = false;
            bool hasDeviceTruncated = false;

            int chCount = 0;
            foreach (var ch in _projectData)
            {
                // 1. 【授权拦截：通道数量】
                if (!LicenseGuard.CanAddChannelSilent(chCount))
                {
                    hasChannelTruncated = true;
                    break; // 达到通道上限，不再显示后续通道
                }

                // 创建通道节点
                var chNode = treeView.Nodes.Add($"📂 {ch.ChannelName} ({ch.Type})");
                chNode.Tag = ch;

                int devCount = 0;
                foreach (var dev in ch.Devices)
                {
                    // 2. 【授权拦截：设备数量】
                    if (!LicenseGuard.CanAddDeviceSilent(devCount))
                    {
                        hasDeviceTruncated = true;
                        // 达到该通道下的设备上限，不再加载更多设备，但继续处理下一个通道
                        break;
                    }

                    var devNode = chNode.Nodes.Add($"📄 {dev.DeviceName} (ID:{dev.SlaveId})");
                    devNode.Tag = dev;
                    // 【新增】直接复用上面的刷新逻辑，保证图标一致
                    RefreshDeviceNodeVisual(devNode, dev);
                    devCount++;
                }

                chNode.Expand();
                chCount++;
            }

            treeView.EndUpdate();

            // 3. 【统一提示】
            if (hasChannelTruncated || hasDeviceTruncated)
            {
                // 这里可以根据需要决定是否弹窗，或者只是在状态栏显示一个红点/提示
                string msg = "当前授权等级有限，部分通道或设备未能加载：\n" +
                             $"- 通道上限: {LicenseGuard.MAX_FREE_CHANNELS}\n" +
                             $"- 每通道设备上限: {LicenseGuard.MAX_FREE_DEVICES}";

                SystemLogger.WriteLog(msg, "LICENSE");
                // 如果想让用户明显感觉到，可以放开下面的弹窗
                 MessageBox.Show(msg, "加载受限", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        // 刷新仪表盘上指定通道的所有卡片
        private void SyncDashboardMasters(ChannelConfig ch, ModbusMaster master)
        {
            // 遍历所有卡片
            foreach (Control ctrl in flowDashboard.Controls)
            {
                if (ctrl is UC_WidgetBase widget)
                {
                    if (widget.ChannelName == ch.ChannelName)
                    {
                        // 启动时 master 是新实例，停止时 master 是 null
                        widget.UpdateMaster(master);
                    }
                }
            }
        }

        private void SaveFormState()
        {
            // 1. 如果最小化了，千万别保存！
            // 否则坐标会变成 (-32000, -32000)，下次启动软件就“消失”了
            if (this.WindowState == FormWindowState.Minimized) return;

            // 2. 记录是否最大化
            Properties.Settings.Default.IsMaximized = (this.WindowState == FormWindowState.Maximized);

            // 3. 智能保存坐标
            if (this.WindowState == FormWindowState.Normal)
            {
                Properties.Settings.Default.WindowSize = this.Size;
                Properties.Settings.Default.WindowLocation = this.Location;
            }
            else
            {
                // 如果当前是最大化，我们要保存“还原后的尺寸”，而不是全屏尺寸
                Properties.Settings.Default.WindowSize = this.RestoreBounds.Size;
                Properties.Settings.Default.WindowLocation = this.RestoreBounds.Location;
            }

            // 4. 保存分割条位置
            // 加个保护，防止保存成负数
            if (splitMain.SplitterDistance > 0)
            {
                Properties.Settings.Default.SplitDistance = splitMain.SplitterDistance;
            }

            Properties.Settings.Default.Save();
        }

        private void LoadFormState(object? sender, EventArgs e)
        {
            // 1. 读取尺寸
            // 检查是否有有效值 (宽或高不能为0)
            if (Properties.Settings.Default.WindowSize.Width > 100 &&
                Properties.Settings.Default.WindowSize.Height > 100)
            {
                this.Size = Properties.Settings.Default.WindowSize;
                this.Location = Properties.Settings.Default.WindowLocation;
            }

            // 2. 防跑偏检查 (重要！)
            // 如果上次在副屏关闭，现在拔了副屏，坐标可能在屏幕外
            // 简单检查：看当前坐标是否在任何一个屏幕的工作区内
            bool isVisible = false;
            foreach (Screen screen in Screen.AllScreens)
            {
                if (screen.WorkingArea.IntersectsWith(this.Bounds))
                {
                    isVisible = true;
                    break;
                }
            }
            // 如果跑到屏幕外了，强制居中
            if (!isVisible)
            {
                this.StartPosition = FormStartPosition.CenterScreen;
            }

            // 3. 恢复最大化
            if (Properties.Settings.Default.IsMaximized)
            {
                this.WindowState = FormWindowState.Maximized;
            }

            // 4. 恢复分割条
            int splitDist = Properties.Settings.Default.SplitDistance;
            if (splitDist > 50 && splitDist < this.Width - 50)
            {
                splitMain.SplitterDistance = splitDist;
            }

           //SystemLogger.WriteLog(HardwareHelper.GetMachineCode()); // 启动时获取一次机器码
        }
        #endregion

        #region 6.数据拖拽操作
        private void InitializeDashboard()
        {
            flowDashboard.AutoScroll = true;

            // 背景色微调：不要纯白，用一点点灰，突出白色卡片
            flowDashboard.BackColor = Color.FromArgb(245, 245, 245);

            // 设置控件之间的间距 (Margin)
            flowDashboard.Padding = new Padding(0); // 整体内边距

            flowDashboard.AllowDrop = true;

            // 绑定事件
            flowDashboard.DragEnter += FlowDashboard_DragEnter;
            flowDashboard.DragOver += flowDashboard_DragOver;
            flowDashboard.DragDrop += FlowDashboard_DragDrop;
            flowDashboard.DragLeave += flowDashboard_DragLeave;
        }

        // A. 鼠标拖进来了，检查是不是 ModbusPoint
        private void FlowDashboard_DragEnter(object sender, DragEventArgs e)
        {
            // 情况A: 外部拖入新变量 (TrendDragData) -> 复制模式
            if (e.Data.GetDataPresent(typeof(TrendDragData)))
            {
                e.Effect = DragDropEffects.Copy;
            }
            // 情况B: 内部卡片重排 (UC_WidgetBase) -> 移动模式
            // 注意：GetDataPresent 参数是字符串或Type，这里检查基类类型
            else if (e.Data.GetDataPresent(typeof(UC_WidgetMonitor)) ||
                     e.Data.GetDataPresent(typeof(UC_WidgetSwitch)) ||
                     e.Data.GetDataPresent(typeof(UC_WidgetControl)))
            {
                e.Effect = DragDropEffects.Move;
                // 【新增】创建幽灵
                UC_WidgetBase draggedCard = GetDraggedWidget(e); // 提取控件的方法
                if (draggedCard != null)
                {
                    // A. 生成幽灵跟随鼠标 (你已有的逻辑)
                    if (_ghostForm == null)
                    {
                        _ghostForm = new DragHelper(draggedCard);
                        _ghostForm.Show();
                    }

                    // B. 【新增】本体隐身，变成“透明占位符”
                    draggedCard.SetPlaceholderMode(true); // <-- 加上这句
                }
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }
        // 2. 【核心】DragOver: 实现实时排序效果
        private void flowDashboard_DragOver(object sender, DragEventArgs e)
        {
            // 只处理内部重排
            if (e.Effect == DragDropEffects.Move)
            {
                // A. 移动幽灵窗口跟随鼠标
                if (_ghostForm != null)
                {
                    _ghostForm.MoveTo(Cursor.Position);
                }

                // 获取隐身的本体
                UC_WidgetBase draggedCard = GetDraggedWidget(e);

                if (draggedCard != null)
                {
                    Point pt = flowDashboard.PointToClient(new Point(e.X, e.Y));
                    Control targetCard = flowDashboard.GetChildAtPoint(pt);

                    // 让这个“看不见的坑”在布局里移动
                    if (targetCard != null && targetCard != draggedCard)
                    {
                        int targetIndex = flowDashboard.Controls.GetChildIndex(targetCard);
                        flowDashboard.Controls.SetChildIndex(draggedCard, targetIndex);
                    }
                }
            }
        }
        // B. 鼠标松开了，开始生成卡片
        private void FlowDashboard_DragDrop(object sender, DragEventArgs e)
        {
            

            RecoverDraggedCard(e);
            DestroyGhost();

            // 1. 直接获取数据包
            var data = e.Data.GetData(typeof(TrendDragData)) as TrendDragData;
            if (data == null || data.Point == null) return;

            // 1. OfType<UC_WidgetBase>() 快速筛选出所有的卡片控件
            // 2. 依次比对 ChannelName, SlaveId 和 BoundPoint (使用 Point 重写的 Equals)
            bool isExist = flowDashboard.Controls.OfType<UC_WidgetBase>()
                           .Any(widget =>
                               widget.ChannelName == data.ChannelName &&
                               widget.SlaveId == data.SlaveId &&
                               widget.BoundPoint != null &&
                               widget.BoundPoint.Equals(data.Point) // 调用你重写的 Equals (Zone, Address, BitIndex)
                           );

            if (isExist)
            {
                MessageBox.Show($"仪表盘已存在该点位的监控卡片：\n{data.ChannelName} - ID:{data.SlaveId} - {data.Point.Name}",
                                "重复添加", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // 【埋雷点 3】
            if (!LicenseGuard.CanAddWidget(flowDashboard.Controls.Count)) return;

            // 直接用数据包里的点位，它就是此时此刻最准确的引用
            ModbusPoint point = data.Point;

            // 2. 模式预判 (基于点位本身的属性)
            var availableModes = new List<WidgetMode>();
            availableModes.Add(WidgetMode.Monitor);

            if (point.DataType == DataType.Bool)
            {
                if (point.Zone == StorageZone.CoilStatus_0x || point.Zone == StorageZone.HoldingRegister_4x)
                    availableModes.Add(WidgetMode.Switch);
            }
            else if (point.Zone == StorageZone.HoldingRegister_4x)
            {
                availableModes.Add(WidgetMode.Control);
            }

            // 3. 决策：选哪种卡片
            WidgetMode selectedMode = WidgetMode.Monitor;
            if (availableModes.Count > 1)
            {
                using (var frm = new F_WidgetSelector(point))
                {
                    if (frm.ShowDialog() != DialogResult.OK) return;
                    selectedMode = frm.SelectedMode;
                }
            }
            else { selectedMode = availableModes[0]; }

            // 4. 获取对应的 Master 实例 (这个还是得通过 ChannelName 找一下，因为卡片需要驱动)
            ModbusMaster targetMaster = null;
            var targetCh = _projectData.FirstOrDefault(c => c.ChannelName == data.ChannelName);
            if (targetCh != null && _runningMasters.ContainsKey(targetCh))
            {
                targetMaster = _runningMasters[targetCh];
            }

            // 5. 创建控件 (直接把整个 data 包塞进去，卡片里啥都有了)
            UC_WidgetBase widget = null;
            switch (selectedMode)
            {
                case WidgetMode.Monitor:
                    widget = new UC_WidgetMonitor(data, targetMaster);
                    break;
                case WidgetMode.Switch:
                    widget = new UC_WidgetSwitch(data, targetMaster);
                    break;
                case WidgetMode.Control:
                    widget = new UC_WidgetControl(data, targetMaster);
                    break;
            }

            if (widget != null)
            {
                widget.ApplyTheme(_currentTheme);

                flowDashboard.Controls.Add(widget);
            }
        }
        private void flowDashboard_DragLeave(object sender, EventArgs e)
        {
            RecoverDraggedCard((DragEventArgs)e); // 视情况可选
            DestroyGhost();
        }
        // 【辅助方法】恢复本体显示
        private void RecoverDraggedCard(DragEventArgs e)
        {
            // 这里没法直接从 e 拿数据，因为 DragLeave 时可能拿不到
            // 我们可以遍历所有卡片，或者用一个全局变量 _currentDraggingCard 记录
            // 简单粗暴法：把面板里所有隐藏的卡片都显示出来 (反正正常情况下只有一个被隐藏)
            foreach (Control c in flowDashboard.Controls)
            {
                if (c is UC_WidgetBase w)
                {
                    // 如果你刚才设置了标志位，这里可以判断一下，或者无脑恢复
                    // 建议给 Widget 加个属性 IsPlaceholder public get
                    // 或者直接全量恢复，反正开销不大
                    w.SetPlaceholderMode(false);
                    w.ApplyTheme(_currentTheme); // 重新上色，确保背景色对
                }
            }
        }
        // 辅助：销毁幽灵
        private void DestroyGhost()
        {
            if (_ghostForm != null)
            {
                _ghostForm.Close();
                _ghostForm.Dispose();
                _ghostForm = null;
            }
        }
        // 辅助：从 Data 中提取控件
        private UC_WidgetBase GetDraggedWidget(DragEventArgs e)
        {
            foreach (string format in e.Data.GetFormats())
            {
                var obj = e.Data.GetData(format);
                if (obj is UC_WidgetBase w) return w;
            }
            return null;
        }
        private List<DashboardWidgetConfig> CollectDashboardLayout()
        {
            var layout = new List<DashboardWidgetConfig>();

            foreach (Control ctrl in flowDashboard.Controls)
            {
                // 1. 只处理我们的卡片基类
                if (ctrl is UC_WidgetBase widget)
                {
                    // 2. 判定卡片模式 (0:Monitor, 1:Switch, 2:Control)
                    int mode = 0;
                    if (ctrl is UC_WidgetSwitch) mode = 1;
                    else if (ctrl is UC_WidgetControl) mode = 2;

                    // 3. 直接从卡片固化的属性中提取信息，100% 准确
                    layout.Add(new DashboardWidgetConfig
                    {
                        // 直接使用卡片自带的频道名和设备ID
                        ChannelName = widget.ChannelName,
                        SlaveId = widget.SlaveId,

                        // 物理地址信息
                        PointAddress = widget.BoundPoint.Address,
                        Zone = (int)widget.BoundPoint.Zone,
                        BitIndex = widget.BoundPoint.BitIndex,

                        Mode = mode
                    });
                }
            }
            return layout;
        }
     
        private void RestoreDashboard(List<DashboardWidgetConfig> layout)
        {
            // 1. 【开始】挂起面板布局
            flowDashboard.SuspendLayout();
            bool hasTruncated = false;
            try
            {
                // 1. 清空现有
                flowDashboard.Controls.Clear();

                if (layout == null) return;
              
                // 2. 遍历配置
                foreach (var cfg in layout)
                {
                    // A. 找通道
                    var ch = _projectData.FirstOrDefault(c => c.ChannelName == cfg.ChannelName);
                    if (ch == null) continue; // 通道都不见了，跳过

                    // B. 找设备
                    var dev = ch.Devices.FirstOrDefault(d => d.SlaveId == cfg.SlaveId);
                    if (dev == null) continue;

                    // 3. 找点位 (【核心修复】：必须同时匹配 Address 和 Zone)
                    var point = dev.Points.FirstOrDefault(p =>
                        p.Address == cfg.PointAddress &&
                        (int)p.Zone == cfg.Zone &&
                        p.BitIndex == cfg.BitIndex // 加上这个就绝对唯一了！
                    );

                    if (point == null) continue;

                    // D. 找 Master (如果还没启动就是 null，没关系，卡片支持 null Master)
                    ModbusMaster master = null;
                    if (_runningMasters.ContainsKey(ch)) master = _runningMasters[ch];

                    if (!LicenseGuard.CanAddWidgetSilent(flowDashboard.Controls.Count))
                    {
                        // 记录一下：有数据被截断了
                        hasTruncated = true;
                        break; // 跳过这个，或者 break 直接结束
                    }

                    // --- 3. 创建控件 ---
                    // 这里复用之前的工厂逻辑，或者直接 new
                    UC_WidgetBase widget = null;
                    WidgetMode mode = (WidgetMode)cfg.Mode; // int 转 enum

                    switch (mode)
                    {
                        case WidgetMode.Monitor:
                            widget = new UC_WidgetMonitor(new TrendDragData(ch.ChannelName,dev.DeviceName,dev.SlaveId,point), master);
                            break;
                        case WidgetMode.Switch:
                            widget = new UC_WidgetSwitch(new TrendDragData(ch.ChannelName, dev.DeviceName, dev.SlaveId, point), master);
                            break;
                        case WidgetMode.Control:
                            widget = new UC_WidgetControl(new TrendDragData(ch.ChannelName, dev.DeviceName, dev.SlaveId, point), master);
                            break;
                    }

                    if (widget != null)
                    {
                        widget.ApplyTheme(_currentTheme); // 还原时也要应用
                        flowDashboard.Controls.Add(widget);
                    }
                }
                }
            finally
            {
                // 2. 【结束】恢复面板布局 (放在 finally 块保底)
                flowDashboard.ResumeLayout();

                if (hasTruncated)
                {
                    MessageBox.Show("当前为免费版，部分保存的仪表盘卡片未能加载。\n(上限 20 个)",
                                    "加载受限", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        /// <summary>
        /// 精准清洗：只有在所属设备中找不到物理地址完全匹配的点位时，才移除卡片
        /// </summary>
        private void CleanUpGhostWidgets()
        {
            int removedCount = 0;
            flowDashboard.SuspendLayout();

            // 倒序遍历
            for (int i = flowDashboard.Controls.Count - 1; i >= 0; i--)
            {
                if (flowDashboard.Controls[i] is UC_WidgetBase widget)
                {
                    // --- 精准定位：按路径找 ---

                    // 1. 找通道
                    var channel = _projectData.FirstOrDefault(c => c.ChannelName == widget.ChannelName);

                    // 2. 找设备
                    var device = channel?.Devices.FirstOrDefault(d => d.SlaveId == widget.SlaveId);

                    // 3. 检查这个点位是否还在该设备中存在
                    // 利用重写的 Equals 判断物理身份（地址、存储区、位索引）
                    bool exists = device != null && device.Points.Any(p => p.Equals(widget.BoundPoint));

                    // 如果路径断了（通道删了、设备删了）或者点位在该设备里消失了
                    if (!exists)
                    {
                        flowDashboard.Controls.RemoveAt(i);
                        widget.Dispose(); // 彻底释放
                        removedCount++;
                    }
                }
            }

            flowDashboard.ResumeLayout();

            if (removedCount > 0)
            {
                System.Diagnostics.Debug.WriteLine($"[清理] 已自动移除 {removedCount} 个失效卡片。");
            }
        }

        private void InitDashboardMenu()
        {
            _ctxDashboard = new ContextMenuStrip();

            // --- 1. 全屏模式 (为下一步做准备) ---
            var itemFullScreen = new ToolStripMenuItem("📺 进入全屏模式 (Full Screen)");
            itemFullScreen.Click += (s, e) => ToggleFullScreen(); // 稍后实现这个方法

            // --- 2. 清空所有 ---
            var itemClear = new ToolStripMenuItem("🗑️ 清空仪表盘 (Clear All)");
            itemClear.ForeColor = Color.Red; // 标红示警
            itemClear.Click += (s, e) =>
            {
                if (flowDashboard.Controls.Count == 0) return;

                if (MessageBox.Show("确定要移除所有卡片吗？", "清空确认",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    flowDashboard.SuspendLayout();
                    while (flowDashboard.Controls.Count > 0)
                    {
                        var ctrl = flowDashboard.Controls[0];
                        flowDashboard.Controls.RemoveAt(0);
                        ctrl.Dispose();
                    }
                    flowDashboard.ResumeLayout();
                }
            };

            // --- 3. 清理无效数据 ---
            var itemCleanGhost = new ToolStripMenuItem("🧹 清理失效卡片 (Clean Invalid)");
            itemCleanGhost.Click += (s, e) =>
            {
                CleanUpGhostWidgets();
                MessageBox.Show("清理完成。", "提示");
            };

            // 添加到菜单
            _ctxDashboard.Items.Add(itemFullScreen);
            _ctxDashboard.Items.Add(new ToolStripSeparator());
            _ctxDashboard.Items.Add(itemCleanGhost);
            _ctxDashboard.Items.Add(itemClear);

            // 【关键】绑定到 FlowLayoutPanel
            flowDashboard.ContextMenuStrip = _ctxDashboard;
        }

        private bool _isFullScreen = false;
        private FormWindowState _lastState;
        private FormBorderStyle _lastStyle;

        

        // 2. 带有“先尝后买”逻辑的触发方法
        private async void ToggleFullScreen()
        {
            // A. 如果是准备“退出”全屏，直接退出，不需要任何拦截
            if (_isFullScreen)
            {
                ApplyFullScreenUI(false);
                return;
            }

            // B. 如果是准备“进入”全屏：

            // 1. 先直接进入（Teaser 模式），让用户感受震撼效果
            ApplyFullScreenUI(true);

            // 2. 判断当前是否有专业版权限（如果是公测期或已激活，直接结束，留在全屏）
            // 注意：这里需要调用一个只返回 bool、不弹窗的判断方法，比如 LicenseGuard.IsProUser()
          
            if (!LicenseGuard.CanUseFullScreen())
            {
                // 3. 让用户爽 2 秒钟
                await Task.Delay(2000);

                // 4. 2秒后，弹出那个带有“功能对比表”的高颜值激活窗口
                // 这里调用你原来的拦截方法，它内部会判断授权并弹窗
                if (!LicenseGuard.CanUseFullScreen())
                {
                    // 5. 如果用户点击了“取消”或者关闭了激活窗，强制将其踢出全屏
                    ApplyFullScreenUI(false);
                }
            }
        }
        // 1. 抽离出来的纯 UI 逻辑，不包含任何授权检查
        private void ApplyFullScreenUI(bool enter)
        {
            this.SuspendLayout();
            _isFullScreen = enter;

            if (_isFullScreen)
            {
                // === 进入全屏 ===
                _lastState = this.WindowState;
                _lastStyle = this.FormBorderStyle;

                menuStrip.Visible = false;
                toolStripMain.Visible = false;
                statusStrip.Visible = false;
                splitMain.Panel1Collapsed = true;

                this.FormBorderStyle = FormBorderStyle.None;
                this.WindowState = FormWindowState.Maximized;

                _ctxDashboard.Items[0].Text = "🔙 退出全屏 (Exit Full Screen)";
            }
            else
            {
                // === 退出全屏 ===
                this.FormBorderStyle = _lastStyle;
                this.WindowState = _lastState;

                menuStrip.Visible = true;
                toolStripMain.Visible = true;
                statusStrip.Visible = true;
                splitMain.Panel1Collapsed = false;

                _ctxDashboard.Items[0].Text = "📺 进入全屏模式 (Full Screen)";
            }

            this.ResumeLayout(true);
        }

        #endregion

        #region 7. 语言切换

        // 1. 在 MainForm 类中添加这个方法
        // 在 MainForm 类中

        private void ApplyUIText()
        {
            // === 1. 更新标题栏 (需要重新计算文件名) ===
            UpdateTitle();

            // === 2. 菜单栏 ===
            menuProject.Text = LangProvider.Get("Menu_File");
            menuLoad.Text = LangProvider.Get("Menu_Open");
            menuSave.Text = LangProvider.Get("Menu_Save");
            menuSaveAs.Text = LangProvider.Get("Menu_SaveAs");

            menuLanguage.Text = LangProvider.Get("Menu_Lang");
            tsmiCn.Text = LangProvider.Get("Lang_Cn");
            tsmiEn.Text = LangProvider.Get("Lang_En");

            menuHelp.Text = LangProvider.Get("Menu_Help");
            tsmiOnlineHelp.Text = LangProvider.Get("Help_Docs");
            tsmiShowTips.Text = LangProvider.Get("Help_Tips");
            tsmiBugReport.Text = LangProvider.Get("Help_Bug");
            tsmiAbout.Text = LangProvider.Get("Help_About");

            // === 3. 资源树区域 ===
            grpTree.Text = LangProvider.Get("Grp_Explorer");
            itemEdit.Text = LangProvider.Get("Ctx_Edit");
            itemDelete.Text = LangProvider.Get("Ctx_Del");

            // 树工具栏
            btnStart.Text = LangProvider.Get("Tree_Start");
            btnStop.Text = LangProvider.Get("Tree_Stop");
            btnConfig.Text = LangProvider.Get("Tree_Config");
            btnAddChannel.Text = LangProvider.Get("Tree_AddCh");
            btnAddDevice.Text = LangProvider.Get("Tree_AddDev");
            btnRemove.Text = LangProvider.Get("Tree_Del");

            // === 4. 主工具栏 ===
            tsbOpen.Text = LangProvider.Get("Tsb_Open");
            tsbSave.Text = LangProvider.Get("Tsb_Save");
            tsbSaveAs.Text = LangProvider.Get("Tsb_SaveAs");
            tsbLogMonitor.Text = LangProvider.Get("Tsb_Log");
            toolStripLabel1.Text = LangProvider.Get("Lbl_Theme");
            btnHelp.Text = LangProvider.Get("Btn_Help");

            // 工具栏提示
            tsbOpen.ToolTipText = LangProvider.Get("Tip_Open");
            tsbSave.ToolTipText = LangProvider.Get("Tip_Save");
            tsbSaveAs.ToolTipText = LangProvider.Get("Tip_SaveAs");
            tsbLogMonitor.ToolTipText = LangProvider.Get("Tip_Log");
            

            //帮助窗口的切换
            FillHelpContent();

            // 1. 直接修改内存中对象的 Name 属性
            // (注意：这里假设你的 InitThemeSelector 是按顺序添加的)
            if (cmbTheme.Items.Count >= 4)
            {
                if (cmbTheme.Items[0] is UITheme t0) t0.Name = LangProvider.Get("Theme_White");
                if (cmbTheme.Items[1] is UITheme t1) t1.Name = LangProvider.Get("Theme_Blue");
                if (cmbTheme.Items[2] is UITheme t2) t2.Name = LangProvider.Get("Theme_Dark");
                if (cmbTheme.Items[3] is UITheme t3) t3.Name = LangProvider.Get("Theme_Ind");
            }

            // 2. 强制刷新显示 (这可能会触发 SelectedIndexChanged，但没关系，让它触发好了)
            // 记录位置
            int currentIdx = cmbTheme.SelectedIndex;

            // 切换 DisplayMember 迫使控件重绘文本
            cmbTheme.ComboBox.DisplayMember = "";
            cmbTheme.ComboBox.DisplayMember = "Name";

            // 还原选中项 (这一步非常关键，防止刷新后变成空白)
            if (currentIdx >= 0)
            {
                cmbTheme.SelectedIndex = currentIdx;
            }

            // 【修正版】刷新仪表盘上已有的卡片
            // ============================================
            foreach (Control ctrl in flowDashboard.Controls)
            {
                // 统一识别为基类 UC_WidgetBase
                if (ctrl is UC_WidgetBase widget)
                {
                    // 魔法就在这里：
                    // 虽然变量类型是 Base，但如果这个实例实际上是 UC_WidgetControl，
                    // 它会自动调用 UC_WidgetControl.ApplyUIText()
                    // 从而把 "设"、"当前" 等特有文字也刷新了。
                    //widget.ApplyUIText();
                }
            }
        }
        // 1. 绑定点击事件 (可以在设计器里双击那两个菜单项生成，然后填入内容)

        // 点击“简体中文”
        private void tsmiCn_Click(object sender, EventArgs e)
        {
            SwitchLanguage("zh");
        }

        // 点击“English”
        private void tsmiEn_Click(object sender, EventArgs e)
        {
            SwitchLanguage("en");
        }

        // 2. 核心切换逻辑
        private void SwitchLanguage(string langCode)
        {
            // 如果当前已经是这个语言，就不折腾了
            if (LangProvider.CurrentLang == langCode) return;

            // 修改全局语言状态
            LangProvider.CurrentLang = langCode;

            // 刷新界面文字
            ApplyUIText();

            // 更新菜单打钩状态
            UpdateLanguageMenuState();

            // (可选) 提示用户
            // MessageBox.Show(LangProvider.Get("Msg_LangRestart"));

            if (F_LogMonitor.HasInstance) // 你需要在 F_LogMonitor 加个 HasInstance 属性
            {
                F_LogMonitor.Instance.ApplyUIText();
            }
        }

        // 3. 管理打钩状态 (让菜单看起来很专业)
        private void UpdateLanguageMenuState()
        {
            string current = LangProvider.CurrentLang;

            // 互斥打钩
            tsmiCn.Checked = (current == "zh");
            tsmiEn.Checked = (current == "en");
        }
        #endregion

        
    }
}