namespace ModbusPilot.App
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            menuStrip = new MenuStrip();
            menuProject = new ToolStripMenuItem();
            menuLoad = new ToolStripMenuItem();
            menuSave = new ToolStripMenuItem();
            menuSaveAs = new ToolStripMenuItem();
            menuView = new ToolStripMenuItem();
            tsmiLogMonitorMenu = new ToolStripMenuItem();
            tsmiTrendChartMenu = new ToolStripMenuItem();
            tsmiFullScreen = new ToolStripMenuItem();
            menuLanguage = new ToolStripMenuItem();
            tsmiCn = new ToolStripMenuItem();
            tsmiEn = new ToolStripMenuItem();
            menuHelp = new ToolStripMenuItem();
            tsmiOnlineHelp = new ToolStripMenuItem();
            tsmiShowTips = new ToolStripMenuItem();
            tsmiBugReport = new ToolStripMenuItem();
            tsmiRegistration = new ToolStripMenuItem();
            tsmiAbout = new ToolStripMenuItem();
            splitMain = new SplitContainer();
            grpTree = new GroupBox();
            treeView = new TreeView();
            ctxMenuTree = new ContextMenuStrip(components);
            itemEdit = new ToolStripMenuItem();
            itemDelete = new ToolStripMenuItem();
            toolStripTree = new ToolStrip();
            btnStart = new ToolStripButton();
            btnStop = new ToolStripButton();
            sep1 = new ToolStripSeparator();
            btnConfig = new ToolStripButton();
            sep2 = new ToolStripSeparator();
            btnAddChannel = new ToolStripButton();
            btnAddDevice = new ToolStripButton();
            btnRemove = new ToolStripButton();
            flowDashboard = new FlowLayoutPanel();
            toolStripMain = new ToolStrip();
            tsbOpen = new ToolStripButton();
            tsbSave = new ToolStripButton();
            tsbSaveAs = new ToolStripButton();
            toolStripSeparator1 = new ToolStripSeparator();
            tsbLogMonitor = new ToolStripButton();
            tsbTrendChart = new ToolStripButton();
            toolStripSeparator2 = new ToolStripSeparator();
            toolStripLabel1 = new ToolStripLabel();
            cmbTheme = new ToolStripComboBox();
            statusStrip = new StatusStrip();
            tssStatus = new ToolStripStatusLabel();
            tssStats = new ToolStripStatusLabel();
            tssProject = new ToolStripStatusLabel();
            tssTime = new ToolStripStatusLabel();
            itemToggleEnable = new ToolStripMenuItem();
            menuStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitMain).BeginInit();
            splitMain.Panel1.SuspendLayout();
            splitMain.Panel2.SuspendLayout();
            splitMain.SuspendLayout();
            grpTree.SuspendLayout();
            ctxMenuTree.SuspendLayout();
            toolStripTree.SuspendLayout();
            toolStripMain.SuspendLayout();
            statusStrip.SuspendLayout();
            SuspendLayout();
            // 
            // menuStrip
            // 
            menuStrip.Items.AddRange(new ToolStripItem[] { menuProject, menuView, menuLanguage, menuHelp });
            menuStrip.Location = new Point(0, 0);
            menuStrip.Name = "menuStrip";
            menuStrip.Size = new Size(1184, 25);
            menuStrip.TabIndex = 0;
            menuStrip.Text = "menuStrip1";
            // 
            // menuProject
            // 
            menuProject.DropDownItems.AddRange(new ToolStripItem[] { menuLoad, menuSave, menuSaveAs });
            menuProject.Name = "menuProject";
            menuProject.Size = new Size(58, 21);
            menuProject.Text = "文件(&F)";
            // 
            // menuLoad
            // 
            menuLoad.Name = "menuLoad";
            menuLoad.ShortcutKeys = Keys.Control | Keys.O;
            menuLoad.Size = new Size(191, 22);
            menuLoad.Text = "📂 打开项目";
            // 
            // menuSave
            // 
            menuSave.Name = "menuSave";
            menuSave.ShortcutKeys = Keys.Control | Keys.S;
            menuSave.Size = new Size(191, 22);
            menuSave.Text = "💾 保存项目";
            // 
            // menuSaveAs
            // 
            menuSaveAs.Name = "menuSaveAs";
            menuSaveAs.Size = new Size(191, 22);
            menuSaveAs.Text = "💾 另存为(&A)...";
            // 
            // menuView
            // 
            menuView.DropDownItems.AddRange(new ToolStripItem[] { tsmiLogMonitorMenu, tsmiTrendChartMenu, tsmiFullScreen });
            menuView.Name = "menuView";
            menuView.Size = new Size(60, 21);
            menuView.Text = "视图(&V)";
            // 
            // tsmiLogMonitorMenu
            // 
            tsmiLogMonitorMenu.Name = "tsmiLogMonitorMenu";
            tsmiLogMonitorMenu.Size = new Size(176, 22);
            tsmiLogMonitorMenu.Text = "📟 报文监视器";
            // 
            // tsmiTrendChartMenu
            // 
            tsmiTrendChartMenu.Name = "tsmiTrendChartMenu";
            tsmiTrendChartMenu.Size = new Size(176, 22);
            tsmiTrendChartMenu.Text = "📈 实时趋势图";
            // 
            // tsmiFullScreen
            // 
            tsmiFullScreen.Name = "tsmiFullScreen";
            tsmiFullScreen.Size = new Size(176, 22);
            tsmiFullScreen.Text = "📺 全屏模式 (F11)";
            // 
            // menuLanguage
            // 
            menuLanguage.DropDownItems.AddRange(new ToolStripItem[] { tsmiCn, tsmiEn });
            menuLanguage.Name = "menuLanguage";
            menuLanguage.Size = new Size(58, 21);
            menuLanguage.Text = "语言(&L)";
            // 
            // tsmiCn
            // 
            tsmiCn.Name = "tsmiCn";
            tsmiCn.Size = new Size(124, 22);
            tsmiCn.Text = "简体中文";
            // 
            // tsmiEn
            // 
            tsmiEn.Name = "tsmiEn";
            tsmiEn.Size = new Size(124, 22);
            tsmiEn.Text = "English";
            // 
            // menuHelp
            // 
            menuHelp.DropDownItems.AddRange(new ToolStripItem[] { tsmiOnlineHelp, tsmiShowTips, tsmiBugReport, tsmiRegistration, tsmiAbout });
            menuHelp.Name = "menuHelp";
            menuHelp.Size = new Size(61, 21);
            menuHelp.Text = "帮助(&H)";
            // 
            // tsmiOnlineHelp
            // 
            tsmiOnlineHelp.Name = "tsmiOnlineHelp";
            tsmiOnlineHelp.Size = new Size(218, 22);
            tsmiOnlineHelp.Text = "📖 在线文档 (&D)";
            // 
            // tsmiShowTips
            // 
            tsmiShowTips.Name = "tsmiShowTips";
            tsmiShowTips.Size = new Size(218, 22);
            tsmiShowTips.Text = "💡  操作指引 (&T)";
            // 
            // tsmiBugReport
            // 
            tsmiBugReport.Name = "tsmiBugReport";
            tsmiBugReport.Size = new Size(218, 22);
            tsmiBugReport.Text = "🐞 问题反馈 (&Q)";
            // 
            // tsmiRegistration
            // 
            tsmiRegistration.Name = "tsmiRegistration";
            tsmiRegistration.Size = new Size(218, 22);
            tsmiRegistration.Text = "💎 激活专业版 (&L)";
            // 
            // tsmiAbout
            // 
            tsmiAbout.Name = "tsmiAbout";
            tsmiAbout.Size = new Size(218, 22);
            tsmiAbout.Text = "ℹ️ 关于 ModbusPilot (&A)";
            // 
            // splitMain
            // 
            splitMain.Dock = DockStyle.Fill;
            splitMain.FixedPanel = FixedPanel.Panel1;
            splitMain.Location = new Point(0, 50);
            splitMain.Name = "splitMain";
            // 
            // splitMain.Panel1
            // 
            splitMain.Panel1.Controls.Add(grpTree);
            // 
            // splitMain.Panel2
            // 
            splitMain.Panel2.Controls.Add(flowDashboard);
            splitMain.Panel2.Padding = new Padding(0, 7, 0, 0);
            splitMain.Size = new Size(1184, 711);
            splitMain.SplitterDistance = 350;
            splitMain.TabIndex = 1;
            // 
            // grpTree
            // 
            grpTree.Controls.Add(treeView);
            grpTree.Controls.Add(toolStripTree);
            grpTree.Dock = DockStyle.Fill;
            grpTree.Location = new Point(0, 0);
            grpTree.Name = "grpTree";
            grpTree.Size = new Size(350, 711);
            grpTree.TabIndex = 0;
            grpTree.TabStop = false;
            grpTree.Text = "资源管理器";
            // 
            // treeView
            // 
            treeView.BorderStyle = BorderStyle.None;
            treeView.ContextMenuStrip = ctxMenuTree;
            treeView.Dock = DockStyle.Fill;
            treeView.FullRowSelect = true;
            treeView.HideSelection = false;
            treeView.ItemHeight = 24;
            treeView.Location = new Point(3, 43);
            treeView.Name = "treeView";
            treeView.Size = new Size(344, 665);
            treeView.TabIndex = 1;
            // 
            // ctxMenuTree
            // 
            ctxMenuTree.Items.AddRange(new ToolStripItem[] { itemEdit, itemDelete, itemToggleEnable });
            ctxMenuTree.Name = "ctxMenuTree";
            ctxMenuTree.Size = new Size(154, 70);
            // 
            // itemEdit
            // 
            itemEdit.Name = "itemEdit";
            itemEdit.Size = new Size(153, 22);
            itemEdit.Text = "⚙️ 修改配置...";
            // 
            // itemDelete
            // 
            itemDelete.Name = "itemDelete";
            itemDelete.Size = new Size(153, 22);
            itemDelete.Text = "🗑 删除";
            // 
            // toolStripTree
            // 
            toolStripTree.GripStyle = ToolStripGripStyle.Hidden;
            toolStripTree.Items.AddRange(new ToolStripItem[] { btnStart, btnStop, sep1, btnConfig, sep2, btnAddChannel, btnAddDevice, btnRemove });
            toolStripTree.LayoutStyle = ToolStripLayoutStyle.Flow;
            toolStripTree.Location = new Point(3, 19);
            toolStripTree.Name = "toolStripTree";
            toolStripTree.Size = new Size(344, 24);
            toolStripTree.TabIndex = 0;
            toolStripTree.Text = "toolStrip1";
            // 
            // btnStart
            // 
            btnStart.ImageTransparentColor = Color.Magenta;
            btnStart.Name = "btnStart";
            btnStart.Size = new Size(50, 21);
            btnStart.Text = "▶ 启动";
            // 
            // btnStop
            // 
            btnStop.ImageTransparentColor = Color.Magenta;
            btnStop.Name = "btnStop";
            btnStop.Size = new Size(56, 21);
            btnStop.Text = "⏹ 停止";
            // 
            // sep1
            // 
            sep1.Name = "sep1";
            sep1.Size = new Size(6, 23);
            // 
            // btnConfig
            // 
            btnConfig.ImageTransparentColor = Color.Magenta;
            btnConfig.Name = "btnConfig";
            btnConfig.Size = new Size(56, 21);
            btnConfig.Text = "⚙ 参数";
            // 
            // sep2
            // 
            sep2.Name = "sep2";
            sep2.Size = new Size(6, 23);
            // 
            // btnAddChannel
            // 
            btnAddChannel.ImageTransparentColor = Color.Magenta;
            btnAddChannel.Name = "btnAddChannel";
            btnAddChannel.Size = new Size(56, 21);
            btnAddChannel.Text = "➕ 通道";
            // 
            // btnAddDevice
            // 
            btnAddDevice.ImageTransparentColor = Color.Magenta;
            btnAddDevice.Name = "btnAddDevice";
            btnAddDevice.Size = new Size(56, 21);
            btnAddDevice.Text = "➕ 设备";
            // 
            // btnRemove
            // 
            btnRemove.Alignment = ToolStripItemAlignment.Right;
            btnRemove.ImageTransparentColor = Color.Magenta;
            btnRemove.Name = "btnRemove";
            btnRemove.Size = new Size(56, 21);
            btnRemove.Text = "🗑️ 删除";
            // 
            // flowDashboard
            // 
            flowDashboard.AutoScroll = true;
            flowDashboard.BackColor = Color.White;
            flowDashboard.Dock = DockStyle.Fill;
            flowDashboard.Location = new Point(0, 7);
            flowDashboard.Name = "flowDashboard";
            flowDashboard.Size = new Size(830, 704);
            flowDashboard.TabIndex = 1;
            // 
            // toolStripMain
            // 
            toolStripMain.GripStyle = ToolStripGripStyle.Hidden;
            toolStripMain.Items.AddRange(new ToolStripItem[] { tsbOpen, tsbSave, tsbSaveAs, toolStripSeparator1, tsbLogMonitor, tsbTrendChart, toolStripSeparator2, toolStripLabel1, cmbTheme });
            toolStripMain.Location = new Point(0, 25);
            toolStripMain.Name = "toolStripMain";
            toolStripMain.RenderMode = ToolStripRenderMode.Professional;
            toolStripMain.Size = new Size(1184, 25);
            toolStripMain.TabIndex = 1;
            toolStripMain.Text = "toolStrip1";
            // 
            // tsbOpen
            // 
            tsbOpen.Name = "tsbOpen";
            tsbOpen.Size = new Size(80, 22);
            tsbOpen.Text = "📂 打开项目";
            tsbOpen.ToolTipText = "加载工程文件 (Ctrl+O)";
            // 
            // tsbSave
            // 
            tsbSave.Name = "tsbSave";
            tsbSave.Size = new Size(80, 22);
            tsbSave.Text = "💾 保存项目";
            tsbSave.ToolTipText = "保存当前工程 (Ctrl+S)";
            // 
            // tsbSaveAs
            // 
            tsbSaveAs.Name = "tsbSaveAs";
            tsbSaveAs.Size = new Size(68, 22);
            tsbSaveAs.Text = "💾 另存为";
            tsbSaveAs.ToolTipText = "另存为新文件";
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new Size(6, 25);
            // 
            // tsbLogMonitor
            // 
            tsbLogMonitor.Name = "tsbLogMonitor";
            tsbLogMonitor.Size = new Size(92, 22);
            tsbLogMonitor.Text = "📟 报文监视器";
            tsbLogMonitor.ToolTipText = "打开全局报文监视窗口";
            // 
            // tsbTrendChart
            // 
            tsbTrendChart.Name = "tsbTrendChart";
            tsbTrendChart.Size = new Size(68, 22);
            tsbTrendChart.Text = "📈 趋势图";
            tsbTrendChart.ToolTipText = "打开趋势图窗口";
            // 
            // toolStripSeparator2
            // 
            toolStripSeparator2.Name = "toolStripSeparator2";
            toolStripSeparator2.Size = new Size(6, 25);
            // 
            // toolStripLabel1
            // 
            toolStripLabel1.Name = "toolStripLabel1";
            toolStripLabel1.Size = new Size(56, 22);
            toolStripLabel1.Text = "卡片主题";
            // 
            // cmbTheme
            // 
            cmbTheme.Name = "cmbTheme";
            cmbTheme.Size = new Size(121, 25);
            // 
            // statusStrip
            // 
            statusStrip.Items.AddRange(new ToolStripItem[] { tssStatus, tssStats, tssProject, tssTime });
            statusStrip.Location = new Point(0, 735);
            statusStrip.Name = "statusStrip";
            statusStrip.Size = new Size(1184, 26);
            statusStrip.TabIndex = 2;
            statusStrip.Text = "statusStrip1";
            // 
            // tssStatus
            // 
            tssStatus.Name = "tssStatus";
            tssStatus.Size = new Size(921, 21);
            tssStatus.Spring = true;
            tssStatus.Text = "就绪";
            tssStatus.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // tssStats
            // 
            tssStats.BorderSides = ToolStripStatusLabelBorderSides.Left | ToolStripStatusLabelBorderSides.Right;
            tssStats.BorderStyle = Border3DStyle.Etched;
            tssStats.Name = "tssStats";
            tssStats.Size = new Size(124, 21);
            tssStats.Text = "TX: 0 | RX: 0 | Err: 0";
            // 
            // tssProject
            // 
            tssProject.ForeColor = Color.DimGray;
            tssProject.Name = "tssProject";
            tssProject.Size = new Size(68, 21);
            tssProject.Text = "未加载工程";
            // 
            // tssTime
            // 
            tssTime.Name = "tssTime";
            tssTime.Size = new Size(56, 21);
            tssTime.Text = "00:00:00";
            // 
            // itemToggleEnable
            // 
            itemToggleEnable.Name = "itemToggleEnable";
            itemToggleEnable.Size = new Size(153, 22);
            itemToggleEnable.Text = "启用/禁用";
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1184, 761);
            Controls.Add(statusStrip);
            Controls.Add(splitMain);
            Controls.Add(toolStripMain);
            Controls.Add(menuStrip);
            Icon = (Icon)resources.GetObject("$this.Icon");
            MainMenuStrip = menuStrip;
            Name = "MainForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Modbus Pilot - 控制台";
            menuStrip.ResumeLayout(false);
            menuStrip.PerformLayout();
            splitMain.Panel1.ResumeLayout(false);
            splitMain.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitMain).EndInit();
            splitMain.ResumeLayout(false);
            grpTree.ResumeLayout(false);
            grpTree.PerformLayout();
            ctxMenuTree.ResumeLayout(false);
            toolStripTree.ResumeLayout(false);
            toolStripTree.PerformLayout();
            toolStripMain.ResumeLayout(false);
            toolStripMain.PerformLayout();
            statusStrip.ResumeLayout(false);
            statusStrip.PerformLayout();
            ResumeLayout(false);
            PerformLayout();




        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.ToolStripMenuItem menuProject;
        private System.Windows.Forms.ToolStripMenuItem menuLoad;
        private System.Windows.Forms.ToolStripMenuItem menuSave;
        private System.Windows.Forms.ToolStripMenuItem tsmiCn;
        private System.Windows.Forms.ToolStripMenuItem tsmiEn;
        private System.Windows.Forms.ToolStripMenuItem tsmiOnlineHelp;
        private System.Windows.Forms.ToolStripMenuItem tsmiShowTips;
        private System.Windows.Forms.ToolStripMenuItem tsmiBugReport;
        private System.Windows.Forms.ToolStripMenuItem tsmiAbout;

        private System.Windows.Forms.SplitContainer splitMain;
        private System.Windows.Forms.GroupBox grpTree;
        private System.Windows.Forms.TreeView treeView;
        private System.Windows.Forms.ContextMenuStrip ctxMenuTree;
        private System.Windows.Forms.ToolStripMenuItem itemEdit;
        private System.Windows.Forms.ToolStrip toolStripTree;
        private System.Windows.Forms.ToolStripButton btnStart;
        private System.Windows.Forms.ToolStripButton btnStop;
        private System.Windows.Forms.ToolStripSeparator sep1;
        private System.Windows.Forms.ToolStripButton btnConfig;
        private System.Windows.Forms.ToolStripSeparator sep2;
        private System.Windows.Forms.ToolStripButton btnAddChannel;
        private System.Windows.Forms.ToolStripButton btnAddDevice;
        private System.Windows.Forms.ToolStripButton btnRemove;
        private System.Windows.Forms.FlowLayoutPanel flowDashboard;
        private System.Windows.Forms.ToolStrip toolStripMain;
        private System.Windows.Forms.ToolStripButton tsbSave;
        private System.Windows.Forms.ToolStripButton tsbOpen;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton tsbLogMonitor;
        private ToolStripMenuItem 删除ToolStripMenuItem;
        private ToolStripMenuItem itemDelete;
        private System.Windows.Forms.ToolStripMenuItem menuSaveAs; // 菜单项
        private System.Windows.Forms.ToolStripButton tsbSaveAs;    // 工具栏按钮
        private ToolStripComboBox cmbTheme;
        private ToolStripSeparator toolStripSeparator2;
        private ToolStripLabel toolStripLabel1;
        private ToolStripMenuItem menuHelp;
        private ToolStripMenuItem menuLanguage;
        private ToolStripButton tsbTrendChart;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel tssStatus; // 就绪/提示
        private System.Windows.Forms.ToolStripStatusLabel tssStats;  // TX/RX 统计
        private System.Windows.Forms.ToolStripStatusLabel tssProject; // 工程文件路径
        private System.Windows.Forms.ToolStripStatusLabel tssTime;   // 系统时间
        private System.Windows.Forms.ToolStripMenuItem menuView;        // 新增：视图菜单
        private System.Windows.Forms.ToolStripMenuItem tsmiLogMonitorMenu; // 新增：菜单里的日志
        private System.Windows.Forms.ToolStripMenuItem tsmiTrendChartMenu; // 新增：菜单里的趋势
        private System.Windows.Forms.ToolStripMenuItem tsmiFullScreen;   // 新增：全屏
        private ToolStripMenuItem tsmiRegistration;
        private ToolStripMenuItem itemToggleEnable;
    }
}
