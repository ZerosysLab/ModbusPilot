using ModbusPilot.Core;
using ModbusPilot.Core.Models;
using System;
using System.IO.Ports;
using System.Windows.Forms;

namespace ModbusPilot.UI.Common
{
    public partial class F_ChannelConfig : F_BaseForm
    {
        public ChannelConfig Config { get; private set; }

        // 【新增】黑名单列表（用于查重）
        public List<string> ForbiddenNames { get; set; } = new List<string>();

        public F_ChannelConfig(ChannelConfig editConfig = null)
        {
            InitializeComponent();

            InitData();

            // 【新增】应用语言设置
            ApplyUIText();

            if (editConfig != null)
            {
                LoadFromConfig(editConfig);
            }
            else
            {
                Config = new ChannelConfig(); // 新建默认
                txtName.Text = LangProvider.Get("Def_NewChannel");
            }
        }



        private void InitData()
        {
            // 填充下拉框数据
            cboPort.Items.AddRange(SerialPort.GetPortNames());
            if (cboPort.Items.Count > 0) cboPort.SelectedIndex = 0;

            cboBaud.Items.AddRange(new object[] { 9600, 19200, 38400, 57600, 115200 });
            cboBaud.SelectedItem = 9600;

            cboDataBits.Items.AddRange(new object[] { 7, 8 });
            cboDataBits.SelectedItem = 8;

            cboStopBits.DataSource = Enum.GetValues(typeof(StopBits));
            cboStopBits.SelectedItem = StopBits.One;

            cboParity.DataSource = Enum.GetValues(typeof(Parity));
            cboParity.SelectedItem = Parity.None;
        }

        private void LoadFromConfig(ChannelConfig cfg)
        {
            Config = cfg;
            txtName.Text = cfg.ChannelName;

            if (cfg.Type == CommType.Serial)
            {
                tabType.SelectedTab = pageSerial;
                cboPort.Text = cfg.PortName;
                cboBaud.SelectedItem = cfg.BaudRate;
                cboDataBits.SelectedItem = cfg.DataBits;
                cboStopBits.SelectedItem = cfg.StopBits;
                cboParity.SelectedItem = cfg.Parity;
            }
            else
            {
                tabType.SelectedTab = pageTcp;
                txtIp.Text = cfg.IpAddress;
                txtTcpPort.Text = cfg.TcpPort.ToString();
                // set checkbox state if exists
                foreach (Control c in pageTcp.Controls)
                {
                    if (c is CheckBox cb && cb.Text.Contains("Modbus TCP"))
                    {
                        cb.Checked = cfg.UseModbusTcp;
                        break;
                    }
                }
            }
            numInterval.Value = cfg.MinInterval; // 回显
        }

        private void BtnOk_Click(object sender, EventArgs e)
        {
            // 1. 获取输入并去空格
            string newName = txtName.Text.Trim();

            // 2. 基础非空校验
            if (string.IsNullOrEmpty(newName))
            {
                MessageBox.Show(
                    LangProvider.Get("Msg_NameEmpty"),
                    LangProvider.Get("Title_Warning"), // 需要在 InitMessages 里确保有 Title_Warning
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                ); 
                return;
            }

            // 3. 【新增】重名校验逻辑
            // 如果是新建：Config.ChannelName 为空，直接检查 ForbiddenNames 是否包含 newName
            // 如果是修改：只有当名字发生变化时 (Config.ChannelName != newName)，才检查 ForbiddenNames
            if (Config.ChannelName != newName && ForbiddenNames.Contains(newName))
            {
                // 使用 string.Format 填充参数
                string msg = string.Format(LangProvider.Get("Msg_NameExist"), newName);
                MessageBox.Show(
                    msg,
                    LangProvider.Get("Title_Error"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                ); 
                return;
            }

            // --- 校验通过，开始赋值 ---

            if (Config == null) Config = new ChannelConfig();

            Config.ChannelName = newName; // 使用处理过的名字
            Config.Type = tabType.SelectedTab == pageSerial ? CommType.Serial : CommType.Tcp;
            Config.MinInterval = (int)numInterval.Value;

            if (Config.Type == CommType.Serial)
            {
                Config.PortName = cboPort.Text;
                Config.BaudRate = (int)cboBaud.SelectedItem;
                Config.DataBits = (int)cboDataBits.SelectedItem;
                Config.StopBits = (StopBits)cboStopBits.SelectedItem;
                Config.Parity = (Parity)cboParity.SelectedItem;
            }
            else
            {
                Config.IpAddress = txtIp.Text;
                // 增加简单的端口校验
                if (!int.TryParse(txtTcpPort.Text, out int port))
                {
                    MessageBox.Show(LangProvider.Get("Msg_PortInvalid"));
                    return;
                }
                Config.TcpPort = port;

                foreach (Control c in pageTcp.Controls)
                {
                    if (c is CheckBox cb && cb.Text.Contains("Modbus TCP"))
                    {
                        Config.UseModbusTcp = cb.Checked;
                        break;
                    }
                }
            }

            // 4. 设置窗口结果并关闭
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        // 辅助 UI 方法
        private void AddLabel(Control parent, string text, int top)
        {
            parent.Controls.Add(new Label { Text = text, Location = new System.Drawing.Point(20, top + 5), AutoSize = true });
        }
        private ComboBox AddCombo(Control parent, int top)
        {
            var cbo = new ComboBox { Location = new System.Drawing.Point(100, top), Width = 150, DropDownStyle = ComboBoxStyle.DropDownList };
            parent.Controls.Add(cbo);
            return cbo;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        // ==========================================
        // 【核心】实现界面文字替换
        // ==========================================
        private void ApplyUIText()
        {
            // 1. 窗口标题 (根据 Config 是否有 ID 或名字来判断是新增还是修改)
            // 简单的逻辑：如果名字为空或者等于默认值，视为新增
            bool isEdit = !string.IsNullOrEmpty(Config?.ChannelName) && Config.ChannelName != LangProvider.Get("Def_NewChannel"); 
            this.Text = isEdit ? LangProvider.Get("Ch_Title_Edit") : LangProvider.Get("Ch_Title_Add");



            // 2. 基础控件
            //txtName.Text = LangProvider.Get("Def_NewChannel");
            lblName.Text = LangProvider.Get("Ch_Lbl_Name");
            lblInterval.Text = LangProvider.Get("Ch_Lbl_Interval");
            btnOk.Text = LangProvider.Get("Btn_OK");
            btnCancel.Text = LangProvider.Get("Btn_Cancel");

            // 3. 选项卡名称
            pageSerial.Text = LangProvider.Get("Ch_Tab_Serial");
            pageTcp.Text = LangProvider.Get("Ch_Tab_Tcp");

            // 4. 串口页标签
            lblPort.Text = LangProvider.Get("Ch_Lbl_Port");
            lblBaud.Text = LangProvider.Get("Ch_Lbl_Baud");
            lblDataBits.Text = LangProvider.Get("Ch_Lbl_DataBits");
            lblStopBits.Text = LangProvider.Get("Ch_Lbl_StopBits");
            lblParity.Text = LangProvider.Get("Ch_Lbl_Parity");

            // 5. TCP页标签
            lblIp.Text = LangProvider.Get("Ch_Lbl_Ip");
            lblTcpPort.Text = LangProvider.Get("Ch_Lbl_Port"); // 复用端口号翻译
            chkModbusTcp.Text = LangProvider.Get("Ch_Chk_Mbap");
        }
    }
}