using System.Globalization;
using System.Reflection;

namespace ModbusPilot.Core
{
    [Obfuscation(Exclude = true, ApplyToMembers = true)]
    public static class LangProvider
    {
        // 当前语言状态
        public static string CurrentLang { get; set; } =
            CultureInfo.CurrentUICulture.Name.StartsWith("zh", StringComparison.OrdinalIgnoreCase) ? "zh" : "en";

        // 字典存储
        private static readonly Dictionary<string, (string cn, string en)> _dict = new();

        // 【静态构造函数】：类加载时自动调用，且只调用一次
        static LangProvider()
        {
            InitGeneral();      // 通用 & 标题
            InitMainMenu();     // 顶部菜单栏
            InitMainToolbar();  // 顶部工具栏
            InitTreeControls(); // 左侧资源树区
            InitDashboard();    // 仪表盘区
            InitMessages();     // 弹窗与提示信息
            InitQuickGuide();   // 操作指引信息
            InitThemes();

            InitChannelWindow();
            InitDeviceMonitor(); // 【新增】调用初始化
            InitLogMonitor();
            InitWidgetSelector(); // 【新增】
            InitDashboardWidgets();

            InitAddrManager(); // 【新增】
            InitAboutWindow(); // 【新增】
        }

        // 辅助添加方法
        private static void Add(string key, string cn, string en)
        {
            if (!_dict.ContainsKey(key)) _dict.Add(key, (cn, en));
        }

        /// <summary>
        /// 获取翻译文本
        /// </summary>
        /// <param name="key">字典键名</param>
        /// <param name="defaultValue">代码中的默认文本（选填）</param>
        public static string Get(string key, string defaultValue = null)
        {
            // 1. 尝试从你辛苦维护的元组大字典里找
            if (_dict.TryGetValue(key, out var translation))
            {
                // 找到了：根据当前语言返回对应的部分
                return (CurrentLang == "en") ? translation.en : translation.cn;
            }

            // 2. 没找到（说明你还没在 InitGeneral 等函数里 Add 它）：
            // 直接返回你在代码里写的 defaultValue（通常是中文）
            // 这样你就不用急着去改 LangProvider 了！
            return defaultValue ?? key;
        }

        // =============================================================
        //  以下为拆分的初始化子函数
        // =============================================================

        #region 1. 通用与标题 (General)
        private static void InitGeneral()
        {
            // 窗口标题 (注意：这里只存基础标题，文件名动态拼接)
            Add("App_Title_Base", "Modbus Pilot - 控制台", "Modbus Pilot - Console");
            Add("App_Title_Free", "ModbusPilot V0.9 (免费版)", "ModbusPilot V0.9 (Free)");
            Add("App_Title_Untitled", "[未命名]", "[Untitled]");

            // 【新增】默认名称
            Add("Def_NewChannel", "新建通道", "New Channel");
            Add("Def_NewDevice", "新设备", "New Device");
            Add("Def_NewTag", "新变量", "New Tag");
            Add("Def_InsertTag", "插入变量", "Inserted Tag");
        
        }
        #endregion

        #region 2. 顶部菜单栏 (Main Menu)
        private static void InitMainMenu()
        {
            // --- 文件 ---
            Add("Menu_File", "文件(&F)", "File(&F)");
            Add("Menu_Open", "📂 打开项目", "📂 Open Project");
            Add("Menu_Save", "💾 保存项目", "💾 Save Project");
            Add("Menu_SaveAs", "💾 另存为(&A)...", "💾 Save As(&A)...");

            // --- 语言 ---
            Add("Menu_Lang", "语言(&L)", "Language(&L)");
            Add("Lang_Cn", "简体中文", "Simplified Chinese");
            Add("Lang_En", "English", "English");

            // --- 帮助 ---
            Add("Menu_Help", "帮助(&H)", "Help(&H)");
            Add("Help_Docs", "📖 在线文档 (&D)", "📖 Online Docs (&D)");
            Add("Help_Tips", "💡 操作指引 (&T)", "💡 Quick Tips (&T)");
            Add("Help_Bug", "🐞 问题反馈 (&Q)", "🐞 Report Issue (&Q)");
            Add("Help_About", "ℹ️ 关于 ModbusPilot (&A)", "ℹ️ About ModbusPilot (&A)");
        }
        #endregion

        #region 3. 顶部工具栏 (Main Toolbar)
        private static void InitMainToolbar()
        {
            // 按钮文字
            Add("Tsb_Open", "📂 打开项目", "📂 Open");
            Add("Tsb_Save", "💾 保存项目", "💾 Save");
            Add("Tsb_SaveAs", "💾 另存为", "💾 Save As");
            Add("Tsb_Log", "📟 报文监视器", "📟 Traffic Monitor");

            // 标签
            Add("Lbl_Theme", "卡片主题", "Theme");
            Add("Btn_Help", "💡 操作指引", "💡 Quick Tips");

            // 鼠标悬停提示 (ToolTip)
            Add("Tip_Open", "加载工程文件 (Ctrl+O)", "Load Project (Ctrl+O)");
            Add("Tip_Save", "保存当前工程 (Ctrl+S)", "Save Project (Ctrl+S)");
            Add("Tip_SaveAs", "另存为新文件", "Save As New File");
            Add("Tip_Log", "打开全局报文监视窗口", "Open Global Traffic Monitor");
        }
        #endregion

        #region 4. 资源树区域 (Tree View)
        private static void InitTreeControls()
        {
            // GroupBox 标题
            Add("Grp_Explorer", "资源管理器", "Explorer");

            // 树节点右键菜单
            Add("Ctx_Edit", "⚙️ 修改配置...", "⚙️ Edit Config...");
            Add("Ctx_Del", "🗑 删除", "🗑 Remove");

            // 树工具栏 (Tree Toolbar) - 英文尽量简短，防止换行太丑
            Add("Tree_Start", "▶ 启动", "▶ Start");
            Add("Tree_Stop", "⏹ 停止", "⏹ Stop");
            Add("Tree_Config", "⚙ 参数", "⚙ Config"); // 英文用了简写
            Add("Tree_AddCh", "➕ 通道", "➕ Ch");     // Channel 简写
            Add("Tree_AddDev", "➕ 设备", "➕ Dev");    // Device 简写
            Add("Tree_Del", "🗑️ 删除", "🗑️ Del");
        }
        #endregion

        #region 5. 消息提示框 (Messages & Dialogs)
        private static void InitMessages()
        {
            // 标题类
            Add("Title_Error", "错误", "Error");
            Add("Title_Warning", "警告", "Warning");
            Add("Title_Info", "提示", "Info");
            Add("Title_Confirm", "确认", "Confirm");
            Add("Title_DelConfirm", "删除确认", "Delete Confirmation");

            // 内容类
            Add("Msg_SaveSucc", "项目已成功保存至：\r\n{0}", "Project saved successfully to:\r\n{0}");
            Add("Msg_SaveFail", "保存失败: {0}", "Save failed: {0}");
            Add("Msg_LoadCover", "加载新项目将覆盖当前配置，是否继续？", "Loading a new project will overwrite current config. Continue?");
            Add("Msg_LoadFail", "加载失败: {0}", "Load failed: {0}");

            Add("Msg_DelNode", "确定要删除 [{0}] 吗？\r\n此操作不可恢复。", "Are you sure you want to delete [{0}]?\r\nThis cannot be undone.");
            Add("Msg_SelChannel", "请先选中一个 [通道] 节点！", "Please select a [Channel] node first!");

            Add("Msg_StartFail", "启动失败: {0}", "Start failed: {0}");
            Add("Msg_NoMonitor", "请先启动通道，才能监控数据。", "Please start the channel before monitoring.");

            // 节点状态
            Add("Node_Connecting", " [连接中...]", " [Connecting...]");
            Add("Node_Open", " [已打开]", " [Running]");

            // 通信状态
            Add("Status_Normal", "通信正常", "Comm OK");
            Add("Status_Error", "通信异常: {0}", "Comm Error: {0}");

            Add("Msg_OpenUrlErr", "无法打开浏览器: {0}", "Unable to open browser: {0}");
        }
        #endregion

        #region 6. 仪表盘相关 (Dashboard)
        private static void InitDashboard()
        {
            // 如果以后有仪表盘通用的文字放这里
            Add("Dash_Waiting", "等待数据...", "Waiting...");
        }
        #endregion

        #region 7. 操作指引 (Quick Guide Panel)
        private static void InitQuickGuide()
        {
            string cn = @"
<div style='padding:12px; line-height:1.8; font-family: ""Microsoft YaHei UI"", ""Arial"";'>
    <!-- 标题区 -->
    <div style='color:#FFD700; font-size:11pt; font-weight:bold; margin-bottom:12px; border-bottom:1px solid #555; padding-bottom:5px;'>
        ★ 快速操作指南 ★
    </div>

    <!-- 内容区 -->
    <div style='color:#A0A0A0; font-size:9pt;'>
        <div style='margin-bottom:4px;'>• 管理设备：<b style='color:#00BFFF;'>右键点击</b> 左侧树节点</div>
        <div style='margin-bottom:4px;'>• 地址配置：<b style='color:#00BFFF;'>右键点击</b> 左侧树节点</div>
        <div style='margin-bottom:4px;'>• 打开监控：<b style='color:#32CD32;'>双击</b> 设备节点</div>
        <div style='margin-bottom:4px;'>• 创建卡片：<span style='color:#32CD32;'>监控窗口</span> &rarr; <b style='color:#FFA500;'>拖拽</b> 变量</div>
        
        <!-- 新增功能指引 -->
        <div style='margin-bottom:4px;'>• 调整布局：<b style='color:#00BFFF;'>左键按住</b> 卡片即可自由拖拽重排</div>
        <div style='margin-bottom:4px;'>• 全屏模式：键盘按下 <b style='color:#32CD32;'>F11</b> 进入全屏沉浸模式</div>
        <div style='margin-bottom:4px;'>• 快捷管理：仪表盘背景 <b style='color:#FFA500;'>右键</b> &rarr; 清理失效/清空</div>
        <div style='margin-bottom:4px;'>• 移除卡片：卡片上方 <b style='color:#FFA500;'>右键</b> &rarr; 移除</div>
    </div>
    
    <!-- 底部小贴士 (可选，增加精致感) -->
    <div style='margin-top:10px; padding-top:5px; border-top:1px dashed #444; color:#666; font-size:8pt;'>
        提示：指向变量名可查看设备等更多信息
    </div>
</div>";

            // 英文版示例 (同步更新)
            string en = @"
<div style='padding:12px; line-height:1.8;'>
    <div style='color:#FFD700; font-size:11pt; font-weight:bold; margin-bottom:12px; border-bottom:1px solid #555; padding-bottom:5px;'>
        ★ Quick Start Guide ★
    </div>
    <div style='color:#A0A0A0; font-size:9pt;'>
        • Device: <b style='color:#00BFFF;'>Right Click</b> Tree Node<br/>
        • Monitor: <b style='color:#32CD32;'>Double Click</b> Device Node<br/>
        • Widget: <span style='color:#32CD32;'>Monitor</span> &rarr; <b style='color:#FFA500;'>Drag</b> Tags<br/>
        • Layout: <b style='color:#00BFFF;'>Click & Hold</b> cards to reorder<br/>
        • Screen: Press <b style='color:#32CD32;'>F11</b> for Full Screen mode<br/>
        • Manage: <b style='color:#FFA500;'>Right Click</b> Dashboard to Clear<br/>
        • Remove: <b style='color:#FFA500;'>Right Click</b> card to delete
    </div>
</div>";

            Add("Guide_Html", cn, en);
        }
        #endregion

        #region 8. 主题名称 (Themes)
        private static void InitThemes()
        {
            Add("Theme_White", "简约白", "Simple White");
            Add("Theme_Blue", "科技蓝", "Tech Blue");
            Add("Theme_Dark", "极客黑", "Cyberpunk");
            Add("Theme_Ind", "工业灰", "Industrial");
        }
        #endregion

        // ==========================================
        // 9. 通道配置窗口 (F_ChannelConfig)
        // ==========================================
        private static void InitChannelWindow()
        {
            // 标题
            Add("Ch_Title_Add", "新增通道", "New Channel");
            Add("Ch_Title_Edit", "修改通道配置", "Edit Channel Config");

            // 基础标签
            Add("Ch_Lbl_Name", "通道名称:", "Channel Name:");
            Add("Ch_Lbl_Interval", "轮询间隔(ms):", "Interval(ms):");

            // 选项卡
            Add("Ch_Tab_Serial", "串口 (RTU)", "Serial (RTU)");
            Add("Ch_Tab_Tcp", "网口 (TCP)", "Ethernet (TCP)");

            // 串口参数
            Add("Ch_Lbl_Port", "端口号:", "Port:");
            Add("Ch_Lbl_Baud", "波特率:", "Baud Rate:");
            Add("Ch_Lbl_DataBits", "数据位:", "Data Bits:");
            Add("Ch_Lbl_StopBits", "停止位:", "Stop Bits:");
            Add("Ch_Lbl_Parity", "校验位:", "Parity:");

            // TCP参数
            Add("Ch_Lbl_Ip", "IP地址:", "IP Address:");
            // 端口号复用 Ch_Lbl_Port
            Add("Ch_Chk_Mbap", "使用 Modbus TCP (MBAP)", "Use Modbus TCP (MBAP)");

            // 按钮 (如果通用里没定义，这里定义一份)
            Add("Btn_OK", "确定", "OK");
            Add("Btn_Cancel", "取消", "Cancel");

            // 错误提示消息
            Add("Msg_NameEmpty", "通道名称不能为空！", "Channel name cannot be empty!");
            Add("Msg_NameExist", "通道名称 [{0}] 已存在，请换一个名字。", "Channel name [{0}] already exists.");
            Add("Msg_PortInvalid", "TCP 端口必须是数字！", "TCP Port must be numeric!");
        }

        // ==========================================
        // 11. 设备数据监控窗口 (F_DeviceMonitor)
        // ==========================================
        private static void InitDeviceMonitor()
        {
            // 窗口标题格式： "设备监控 - {0} (ID: {1})"
            Add("Mon_Title_Fmt", "设备监控 - {0} (ID: {1})", "Device Monitor - {0} (ID: {1})");

            // 表格列名
            Add("Mon_Col_Name", "变量名称", "Tag Name");
            Add("Mon_Col_Unit", "单位", "Unit");
            Add("Mon_Col_Value", "当前值", "Value");
            Add("Mon_Col_Input", "写入数值", "Write Value");
            Add("Mon_Col_Btn", "操作", "Action");

            // 按钮文字 & 状态
            Add("Mon_Btn_Write", "写入", "Write");
            Add("Mon_Btn_Disabled", "禁用", "Disabled"); // 灰色文字

            // 提示信息 (ToolTip / Value)
            Add("Mon_Val_ReadOnly", "(只读)", "(Read-Only)");
            Add("Mon_Val_NotSup", "(不支持)", "(N/A)");
            Add("Mon_Msg_Empty", "请输入要写入的数值", "Please enter a value to write.");

            // 精度修正弹窗
            Add("Mon_Fix_Title", "数值修正", "Value Correction");
            Add("Mon_Fix_Msg", "【精度修正提示】\r\n\r\n由于当前系数 ({0}) 的限制，设备无法存储 {1}。\r\n最接近的有效值为: {2}\r\n\r\n点击 [确定] 将自动修正输入值并写入。\r\n点击 [取消] 放弃操作。",
                                    "[Precision Check]\r\n\r\nThe device cannot store {1} due to the scaling factor ({0}).\r\nThe closest valid value is: {2}\r\n\r\nClick [OK] to auto-correct and write.\r\nClick [Cancel] to abort.");

            // 写入结果
            Add("Mon_Write_Fail", "写入失败: {0}", "Write Failed: {0}");
        }

        // ==========================================
        // 12. 全局报文监视器 (F_LogMonitor)
        // ==========================================
        private static void InitLogMonitor()
        {
            Add("Log_Title", "通讯报文监视器", "Traffic Monitor");

            // 工具栏
            Add("Log_Lbl_Filter", "通道筛选:", "Channel Filter:");
            Add("Log_Filter_All", "所有通道 (All)", "All Channels");

            Add("Log_Btn_Pause", "⏸ 暂停滚屏", "⏸ Pause");
            Add("Log_Btn_Resume", "▶ 继续滚屏", "▶ Resume");
            Add("Log_Btn_Clear", "🗑 清空", "🗑 Clear");
            Add("Log_Btn_Export", "💾 导出", "💾 Export");

            // 复选框
            Add("Log_Chk_Tx", "发送 (TX)", "Sent (TX)");
            Add("Log_Chk_Rx", "接收 (RX)", "Recv (RX)");
            Add("Log_Chk_Err", "错误 (Err)", "Error (Err)");
            Add("Log_Chk_Info", "信息 (Info)", "Info (Sys)");

            // 表格列头
            Add("Log_Col_Time", "时间", "Time");
            Add("Log_Col_Ch", "通道", "Channel");
            Add("Log_Col_Type", "类型", "Type");
            Add("Log_Col_Hex", "报文 (Hex)", "Packet (Hex)");
            Add("Log_Col_Msg", "信息/错误", "Message/Error");

            // 提示信息
            Add("Log_Msg_ExportSucc", "导出成功！", "Export Successful!");
        }

        // ==========================================
        // 13. 卡片选择弹窗 (F_WidgetSelector)
        // ==========================================
        private static void InitWidgetSelector()
        {
            Add("Sel_Title", "选择卡片类型", "Select Widget Type");

            // 格式： "点位: {0} ({1})" -> "Point: {0} ({1})"
            Add("Sel_PointInfo", "点位: {0} ({1})", "Point: {0} ({1})");

            // 按钮选项
            Add("Sel_Btn_Monitor", "📊 数值/状态监视", "📊 Value Monitor");
            Add("Sel_Btn_Switch", "🔘 开关控制", "🔘 Toggle Switch");
            Add("Sel_Btn_Control", "📝 数值写入", "📝 Value Writer");
        }

        // ==========================================
        // 14. 仪表盘卡片 (Dashboard Widgets)
        // ==========================================
        private static void InitDashboardWidgets()
        {
            // 右键菜单
            Add("Card_Ctx_Remove", "移除卡片", "Remove Widget");

            // 写入按钮 (由于按钮很小，英文尽量简短)
            Add("Card_Btn_Set", "设", "Set");

            // 标签前缀 (用于 Control 模式)
            Add("Card_Lbl_Curr", "当前: ", "Cur: ");

            // 错误提示
            Add("Msg_ChNotStarted", "该通道尚未启动，无法下发指令。", "Channel not started. Command aborted.");
            Add("Title_NotConn", "未连接", "Not Connected");

            // 精度修正弹窗 (复用 F_DeviceMonitor 的 key 或者新建)
            // 建议新建一组，或者确认 InitDeviceMonitor 已经初始化了 Mon_Fix_Msg
            // 为了安全，这里单独定义一组 Card_ 前缀的，内容一样
            Add("Card_Fix_Title", "数值修正", "Value Correction");
            Add("Card_Fix_Msg", "【精度修正提示】\r\n\r\n由于当前系数 ({0}) 的限制，设备无法存储 {1}。\r\n最接近的有效值为: {2}\r\n\r\n点击 [确定] 将自动修正并写入。\r\n点击 [取消] 放弃操作。",
                                    "[Precision Check]\r\n\r\nCannot store {1} due to factor ({0}).\r\nClosest valid value: {2}\r\n\r\n[OK] Auto-correct and write.\r\n[Cancel] Abort.");

            Add("Card_Write_Err", "写入错误: {0}", "Write Error: {0}");
        }

        // ==========================================
        // 15. 地址表管理器 (F_ModbusAddrManager)
        // ==========================================
        private static void InitAddrManager()
        {
            Add("Addr_Title", "设备地址表编辑器", "Device Address Editor");

            // 头部区域
            Add("Addr_Lbl_DevName", "设备名称:", "Device Name:");
            Add("Addr_Lbl_SlaveId", "站号 (Slave ID):", "Slave ID:");

            // 工具栏按钮
            Add("Addr_Btn_Confirm", "✅ 确定并关闭", "✅ Confirm & Close");
            Add("Addr_Btn_Export", "📤 导出标准表格", "📤 Export Standard Excel");
            Add("Addr_Btn_Import", "📥 导入标准表格", "📥 Import Standard Excel");
            Add("Addr_Grp_Tags", "功能点位表 (右击批量操作)", "Point Map (Right-click for Batch Ops)");

            Add("Addr_Btn_Hex", "🔢 Hex", "🔢 Hex");
            Add("Addr_Btn_Dec", "🔢 Dec", "🔢 Dec");
            Add("Addr_Btn_Add", "➕ 新增变量", "➕ Add Tag");
            Add("Addr_Btn_Insert", "📥 插入变量", "📥 Insert");
            Add("Addr_Btn_Del", "➖ 删除选中", "➖ Delete");

            // 分组框标题
            Add("Addr_Grp_Cmds", "2. 指令预览 (自动生成)", "2. Command Preview (Auto Generated)");

            // 表格列名 (左侧 - 标签表)
            Add("Addr_Col_Name", "变量名称", "Tag Name");
            Add("Addr_Col_Unit", "单位", "Unit");
            Add("Addr_Col_Zone", "存储区", "Zone");
            Add("Addr_Col_Addr", "地址", "Address");
            Add("Addr_Col_Type", "类型", "Data Type");
            Add("Addr_Col_Bit", "位", "Bit");
            Add("Addr_Col_Format", "字节序", "Byte Order");
            Add("Addr_Col_Factor", "系数", "Factor");
            Add("Addr_Col_Offset", "偏移", "Offset");
            Add("Addr_Col_Note", "备注说明", "Note");

            // 表格列名 (右侧 - 指令表)
            Add("Addr_Col_Func", "功能码", "Function"); // FC
            Add("Addr_Col_Hex", "报文 (Hex)", "Packet (Hex)");

            // 存储区下拉框选项
            Add("Zone_0x", "线圈(RW)-(DO/0x)", "Coils (RW)-(0x)");
            Add("Zone_1x", "离散输入(RO)-(DI/1x)", "Discrete Inputs (RO)-(1x)");
            Add("Zone_3x", "输入寄存器(RO)-(AI/3x)", "Input Regs (RO)-(3x)");
            Add("Zone_4x", "保持寄存器(RW)-(Data/4x)", "Holding Regs (RW)-(4x)");

            // 字节序下拉框选项
            Add("Fmt_ABCD", "ABCD (标准)", "ABCD (Big Endian)");
            Add("Fmt_CDAB", "CDAB (字交换)", "CDAB (Word Swap)");
            Add("Fmt_BADC", "BADC (字节交换)", "BADC (Byte Swap)");
            Add("Fmt_DCBA", "DCBA (反转)", "DCBA (Little Endian)");

            // 右键菜单 (列显示)
            Add("Addr_Ctx_ShowFmt", "显示: 字节序 (ABCD/CDAB...)", "Show: Byte Order");
            Add("Addr_Ctx_ShowScl", "显示: 线性变换 (系数/偏移)", "Show: Linear Scaling");

            // 消息提示
            Add("Addr_Msg_SlaveConflict", "该通道下已存在站号为 [{0}] 的设备！\r\nModbus 总线要求同一通道下站号唯一。", "Slave ID [{0}] already exists in this channel!\r\nSlave IDs must be unique per channel.");
            Add("Addr_Msg_Copied", "指令报文已复制到剪贴板！", "Command copied to clipboard!");
            Add("Addr_Msg_ImportErr", "导入失败: {0}", "Import Failed: {0}");
        }

        // ==========================================
        // 16. 关于窗口 (F_About)
        // ==========================================
        private static void InitAboutWindow()
        {
            // 标题
            Add("About_Title", "关于 ModbusPilot", "About ModbusPilot");

            // 描述文字 (支持换行)
            Add("About_Desc", "一款专为自动化流程设计的现代化 Modbus 上位机与轻量组态工具。\r\n基于 .NET 8 与 WinForms 构建。",
                                    "A professional Modbus master & SCADA configuration tool designed for automation workflow.\r\nPowered by .NET 8 & WinForms.");

            // 链接文字 (可选)
            Add("About_Link", "访问官网 / GitHub", "Visit Website / GitHub");

            // 按钮 (如果通用里没定义)
            Add("Btn_OK", "确定", "OK");
        }
    }
}