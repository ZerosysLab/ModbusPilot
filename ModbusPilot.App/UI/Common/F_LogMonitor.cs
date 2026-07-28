using ModbusPilot.Core;
using ModbusPilot.Core.Models;
using ModbusPilot.Core.Services;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace ModbusPilot.UI.Common
{
    public partial class F_LogMonitor : F_BaseForm
    {
        // === 单例模式 ===
        private static F_LogMonitor _instance;
        public static bool HasInstance => _instance != null && !_instance.IsDisposed;
        public static F_LogMonitor Instance
        {
            get
            {
                if (_instance == null || _instance.IsDisposed)
                {
                    _instance = new F_LogMonitor();
                }
                return _instance;
            }
        }

        // UI 缓存队列
        private ConcurrentQueue<LogEntry> _uiQueue = new ConcurrentQueue<LogEntry>();
        private const int MAX_ROWS = 1000; // 最多保留 1000 行
        private HashSet<string> _knownChannels = new HashSet<string>(); // 记录已知的通道名

        private F_LogMonitor()
        {
            InitializeComponent();


            // 初始化筛选框 (这里先不加 Item，移到 ApplyUIText 里加，或者加了后再改)
            cmbChannels.Items.Add(LangProvider.Get("Log_Filter_All"));
            cmbChannels.SelectedIndex = 0;

            InitAutoLogButton();

            // 绑定事件
            BindEvents();

            // 订阅全局日志
            LogHub.OnLog += LogHub_OnLog;

            // 启动定时刷新
            timerFlush.Start();

            ApplyUIText();
        }

        private void InitAutoLogButton()
        {
            // 绑定点击事件 (开关)
            btnAutoLog.ButtonClick += (s, e) => ToggleAutoLog();

            // 绑定下拉菜单项事件 (策略选择)
            itemPolicyError.Click += (s, e) => SetPolicy(AutoLogPolicy.ErrorsOnly);
            itemPolicyAll.Click += (s, e) => SetPolicy(AutoLogPolicy.All);

            // 初始化状态
            UpdateAutoLogUI();
        }
        private void ToggleAutoLog()
        {
            if (AutoLogService.Instance.IsEnabled)
            {
                // 关闭
                AutoLogService.Instance.Stop();
                MessageBox.Show("已停止后台自动存盘。");
            }
            else
            {
                // 开启前检查授权
                if (!LicenseGuard.CanUseAutoLogging()) return;

                AutoLogService.Instance.Start();
                MessageBox.Show("后台自动存盘已启动。\n日志将存储在 /AutoLogs 目录下。");
            }
            UpdateAutoLogUI();
        }
        private void SetPolicy(AutoLogPolicy policy)
        {
            AutoLogService.Instance.Policy = policy;
            // 更新菜单勾选状态 (UI逻辑略)
            UpdateAutoLogUI();
        }
        private void UpdateAutoLogUI()
        {
            bool on = AutoLogService.Instance.IsEnabled;
            btnAutoLog.Text = on ? "🟢 存盘中" : "⚪ 自动存盘";
            btnAutoLog.ToolTipText = $"当前策略: {AutoLogService.Instance.Policy}";

            // 菜单勾选状态同步
            itemPolicyError.Checked = (AutoLogService.Instance.Policy == AutoLogPolicy.ErrorsOnly);
            itemPolicyAll.Checked = (AutoLogService.Instance.Policy == AutoLogPolicy.All);
        }
        public void ApplyUIText()
        {
            // 1. 窗口标题
            this.Text = LangProvider.Get("Log_Title");

            // 2. 工具栏
            lblFilter.Text = LangProvider.Get("Log_Lbl_Filter");

            // 下拉框第一项 "All Channels" 需要更新
            if (cmbChannels.Items.Count > 0)
            {
                cmbChannels.Items[0] = LangProvider.Get("Log_Filter_All");
            }

            // 暂停按钮 (需要根据当前状态判断显示什么)
            btnPause.Text = btnPause.Checked
                ? LangProvider.Get("Log_Btn_Resume")
                : LangProvider.Get("Log_Btn_Pause");

            btnClear.Text = LangProvider.Get("Log_Btn_Clear");
            btnExport.Text = LangProvider.Get("Log_Btn_Export");

            // 3. 复选框
            chkShowTx.Text = LangProvider.Get("Log_Chk_Tx");
            chkShowRx.Text = LangProvider.Get("Log_Chk_Rx");
            chkShowErr.Text = LangProvider.Get("Log_Chk_Err");
            chkShowInfo.Text = LangProvider.Get("Log_Chk_Info");

            // 4. 表格列头
            colTime.HeaderText = LangProvider.Get("Log_Col_Time");
            colCh.HeaderText = LangProvider.Get("Log_Col_Ch");
            colDir.HeaderText = LangProvider.Get("Log_Col_Type");
            colHex.HeaderText = LangProvider.Get("Log_Col_Hex");
            colMsg.HeaderText = LangProvider.Get("Log_Col_Msg");
        }
        private void BindEvents()
        {
            btnClear.Click += (s, e) => { dgvLog.Rows.Clear(); };
            btnPause.Click += (s, e) =>
            {
                btnPause.Text = btnPause.Checked
                    ? LangProvider.Get("Log_Btn_Resume")
                    : LangProvider.Get("Log_Btn_Pause");
            };
            btnExport.Click += (s, e) => ExportLog();

            // 窗口关闭时，不要销毁，只是隐藏 (可选，或者彻底销毁下次重建)
            // 这里我们选择标准模式：销毁。下次 Instance 会重建。
            this.FormClosing += (s, e) =>
            {
                LogHub.OnLog -= LogHub_OnLog; // 必须取消订阅，防止内存泄漏
                timerFlush.Stop();
            };
        }

        // --- 1. 接收日志 (后台线程) ---
        private void LogHub_OnLog(LogEntry entry)
        {
            // 只要不暂停，就往队列里塞
            if (!btnPause.Checked)
            {
                _uiQueue.Enqueue(entry);
            }
        }

        // --- 2. 刷新 UI (UI 线程 - 定时器) ---
        private void TimerFlush_Tick(object sender, EventArgs e)
        {
            if (_uiQueue.IsEmpty) return;

            dgvLog.SuspendLayout();
            int count = 0;

            // 每次最多处理 50 条，防止卡顿
            while (!_uiQueue.IsEmpty && count < 50)
            {
                if (_uiQueue.TryDequeue(out LogEntry log))
                {
                    ProcessLogEntry(log);
                    count++;
                }
            }

            // 自动滚动到底部
            if (dgvLog.Rows.Count > 0 && !btnPause.Checked)
            {
                dgvLog.FirstDisplayedScrollingRowIndex = dgvLog.Rows.Count - 1;
            }

            // 限制总行数
            while (dgvLog.Rows.Count > MAX_ROWS)
            {
                dgvLog.Rows.RemoveAt(0);
            }

            dgvLog.ResumeLayout();
        }

        private void ProcessLogEntry(LogEntry log)
        {
            // A. 更新通道列表
            if (!_knownChannels.Contains(log.ChannelName))
            {
                _knownChannels.Add(log.ChannelName);
                cmbChannels.Items.Add(log.ChannelName);
            }

            // B. 应用筛选条件

            // 1. 通道筛选
            string selectedCh = cmbChannels.SelectedItem?.ToString();
            if (selectedCh != LangProvider.Get("Log_Filter_All") && selectedCh != log.ChannelName)
            {
                return; // 不显示
            }

            // 2. 类型筛选
            if (log.Type == LogType.Send && !chkShowTx.Checked) return;
            if (log.Type == LogType.Receive && !chkShowRx.Checked) return;
            if (log.Type == LogType.Error && !chkShowErr.Checked) return;
            if (log.Type == LogType.Info && !chkShowInfo.Checked) return;

            // C. 添加行
            int idx = dgvLog.Rows.Add();
            var row = dgvLog.Rows[idx];

            row.Cells[colTime.Index].Value = log.Time.ToString("HH:mm:ss.fff");
            row.Cells[colCh.Index].Value = log.ChannelName;

            // 设置类型列和颜色
            var cellType = row.Cells[colDir.Index];
            var cellHex = row.Cells[colHex.Index];
            var cellMsg = row.Cells[colMsg.Index];

            // 数据内容
            cellHex.Value = log.GetDataHex();

            // --- 核心修改：调用解释器 ---
            cellHex.Value = log.GetDataHex();

            // 如果日志本身有消息（比如 Error 报错信息），优先显示原始消息
            // 如果是 TX/RX 且 Data 不为空，则尝试进行协议解析
            if (!string.IsNullOrEmpty(log.Message) && log.Type != LogType.Send && log.Type != LogType.Receive)
            {
                cellMsg.Value = log.Message;
            }
            else if (log.Data != null && log.Data.Length > 0)
            {
                // 使用 LogEntry 携带的准确属性
                bool isTcp = log.IsTcp;

                string interpretation = ModbusPilot.Core.Utils.ProtocolInterpreter.Interpret(
                    log.Data,
                    log.Type == LogType.Send,
                    isTcp
                );
                cellMsg.Value = interpretation;
            }
            else
            {
                cellMsg.Value = log.Message;
            }
            // ---------------------------


            switch (log.Type)
            {
                case LogType.Send:
                    cellType.Value = "TX";
                    cellType.Style.ForeColor = Color.Green;
                    cellHex.Style.ForeColor = Color.Green;
                    break;
                case LogType.Receive:
                    cellType.Value = "RX";
                    cellType.Style.ForeColor = Color.Black;
                    cellHex.Style.ForeColor = Color.Black;
                    break;
                case LogType.Error:
                    cellType.Value = "ERR";
                    row.DefaultCellStyle.ForeColor = Color.Red;
                    row.DefaultCellStyle.BackColor = Color.MistyRose;
                    break;
                case LogType.Warning:
                    cellType.Value = "WARN";
                    row.DefaultCellStyle.ForeColor = Color.DarkOrange;
                    break;
                case LogType.Info:
                    cellType.Value = "SYS";
                    row.DefaultCellStyle.ForeColor = Color.Blue;
                    break;
            }
        }

        private void ExportLog()
        {
            using (SaveFileDialog sfd = new SaveFileDialog() { Filter = "Text File|*.txt", FileName = $"ModbusLog_{DateTime.Now:yyyyMMdd_HHmm}.txt" })
            {
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine("Time\t\tChannel\tType\tHex\t\t\tMessage");
                    foreach (DataGridViewRow row in dgvLog.Rows)
                    {
                        sb.Append($"{row.Cells[colTime.Index].Value}\t");
                        sb.Append($"{row.Cells[colCh.Index].Value}\t");
                        sb.Append($"{row.Cells[colDir.Index].Value}\t");
                        sb.Append($"{row.Cells[colHex.Index].Value}\t");
                        sb.AppendLine($"{row.Cells[colMsg.Index].Value}");
                    }
                    File.WriteAllText(sfd.FileName, sb.ToString());
                    // 【修改】使用字典提示
                    MessageBox.Show(LangProvider.Get("Log_Msg_ExportSucc"));
                }
            }
        }
    }
}