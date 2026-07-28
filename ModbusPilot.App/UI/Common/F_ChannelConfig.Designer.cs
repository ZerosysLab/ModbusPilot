namespace ModbusPilot.UI.Common
{
    partial class F_ChannelConfig
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            lblName = new Label();
            txtName = new TextBox();
            tabType = new TabControl();
            pageSerial = new TabPage();
            lblParity = new Label();
            cboParity = new ComboBox();
            lblStopBits = new Label();
            cboStopBits = new ComboBox();
            lblDataBits = new Label();
            cboDataBits = new ComboBox();
            lblBaud = new Label();
            cboBaud = new ComboBox();
            lblPort = new Label();
            cboPort = new ComboBox();
            pageTcp = new TabPage();
            chkModbusTcp = new CheckBox();
            lblTcpPort = new Label();
            txtTcpPort = new TextBox();
            lblIp = new Label();
            txtIp = new TextBox();
            btnOk = new Button();
            btnCancel = new Button();
            lblInterval = new Label();
            numInterval = new NumericUpDown();
            tabType.SuspendLayout();
            pageSerial.SuspendLayout();
            pageTcp.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numInterval).BeginInit();
            SuspendLayout();
            // 
            // lblName
            // 
            lblName.AutoSize = true;
            lblName.Location = new Point(20, 25);
            lblName.Name = "lblName";
            lblName.Size = new Size(59, 17);
            lblName.TabIndex = 0;
            lblName.Text = "通道名称:";
            // 
            // txtName
            // 
            txtName.Location = new Point(100, 22);
            txtName.Name = "txtName";
            txtName.Size = new Size(200, 23);
            txtName.TabIndex = 1;
            txtName.Text = "新建通道";
            // 
            // tabType
            // 
            tabType.Controls.Add(pageSerial);
            tabType.Controls.Add(pageTcp);
            tabType.Location = new Point(20, 60);
            tabType.Name = "tabType";
            tabType.SelectedIndex = 0;
            tabType.Size = new Size(340, 280);
            tabType.TabIndex = 2;
            // 
            // pageSerial
            // 
            pageSerial.Controls.Add(lblParity);
            pageSerial.Controls.Add(cboParity);
            pageSerial.Controls.Add(lblStopBits);
            pageSerial.Controls.Add(cboStopBits);
            pageSerial.Controls.Add(lblDataBits);
            pageSerial.Controls.Add(cboDataBits);
            pageSerial.Controls.Add(lblBaud);
            pageSerial.Controls.Add(cboBaud);
            pageSerial.Controls.Add(lblPort);
            pageSerial.Controls.Add(cboPort);
            pageSerial.Location = new Point(4, 26);
            pageSerial.Name = "pageSerial";
            pageSerial.Padding = new Padding(3);
            pageSerial.Size = new Size(332, 250);
            pageSerial.TabIndex = 0;
            pageSerial.Text = "串口 (RTU)";
            pageSerial.UseVisualStyleBackColor = true;
            // 
            // lblParity
            // 
            lblParity.AutoSize = true;
            lblParity.Location = new Point(20, 185);
            lblParity.Name = "lblParity";
            lblParity.Size = new Size(47, 17);
            lblParity.TabIndex = 8;
            lblParity.Text = "校验位:";
            // 
            // cboParity
            // 
            cboParity.DropDownStyle = ComboBoxStyle.DropDownList;
            cboParity.FormattingEnabled = true;
            cboParity.Location = new Point(100, 180);
            cboParity.Name = "cboParity";
            cboParity.Size = new Size(150, 25);
            cboParity.TabIndex = 9;
            // 
            // lblStopBits
            // 
            lblStopBits.AutoSize = true;
            lblStopBits.Location = new Point(20, 145);
            lblStopBits.Name = "lblStopBits";
            lblStopBits.Size = new Size(47, 17);
            lblStopBits.TabIndex = 6;
            lblStopBits.Text = "停止位:";
            // 
            // cboStopBits
            // 
            cboStopBits.DropDownStyle = ComboBoxStyle.DropDownList;
            cboStopBits.FormattingEnabled = true;
            cboStopBits.Location = new Point(100, 140);
            cboStopBits.Name = "cboStopBits";
            cboStopBits.Size = new Size(150, 25);
            cboStopBits.TabIndex = 7;
            // 
            // lblDataBits
            // 
            lblDataBits.AutoSize = true;
            lblDataBits.Location = new Point(20, 105);
            lblDataBits.Name = "lblDataBits";
            lblDataBits.Size = new Size(47, 17);
            lblDataBits.TabIndex = 4;
            lblDataBits.Text = "数据位:";
            // 
            // cboDataBits
            // 
            cboDataBits.DropDownStyle = ComboBoxStyle.DropDownList;
            cboDataBits.FormattingEnabled = true;
            cboDataBits.Location = new Point(100, 100);
            cboDataBits.Name = "cboDataBits";
            cboDataBits.Size = new Size(150, 25);
            cboDataBits.TabIndex = 5;
            // 
            // lblBaud
            // 
            lblBaud.AutoSize = true;
            lblBaud.Location = new Point(20, 65);
            lblBaud.Name = "lblBaud";
            lblBaud.Size = new Size(47, 17);
            lblBaud.TabIndex = 2;
            lblBaud.Text = "波特率:";
            // 
            // cboBaud
            // 
            cboBaud.DropDownStyle = ComboBoxStyle.DropDownList;
            cboBaud.FormattingEnabled = true;
            cboBaud.Location = new Point(100, 60);
            cboBaud.Name = "cboBaud";
            cboBaud.Size = new Size(150, 25);
            cboBaud.TabIndex = 3;
            // 
            // lblPort
            // 
            lblPort.AutoSize = true;
            lblPort.Location = new Point(20, 25);
            lblPort.Name = "lblPort";
            lblPort.Size = new Size(47, 17);
            lblPort.TabIndex = 0;
            lblPort.Text = "端口号:";
            // 
            // cboPort
            // 
            cboPort.DropDownStyle = ComboBoxStyle.DropDownList;
            cboPort.FormattingEnabled = true;
            cboPort.Location = new Point(100, 20);
            cboPort.Name = "cboPort";
            cboPort.Size = new Size(150, 25);
            cboPort.TabIndex = 1;
            // 
            // pageTcp
            // 
            pageTcp.Controls.Add(chkModbusTcp);
            pageTcp.Controls.Add(lblTcpPort);
            pageTcp.Controls.Add(txtTcpPort);
            pageTcp.Controls.Add(lblIp);
            pageTcp.Controls.Add(txtIp);
            pageTcp.Location = new Point(4, 26);
            pageTcp.Name = "pageTcp";
            pageTcp.Padding = new Padding(3);
            pageTcp.Size = new Size(332, 250);
            pageTcp.TabIndex = 1;
            pageTcp.Text = "网口 (TCP)";
            pageTcp.UseVisualStyleBackColor = true;
            // 
            // chkModbusTcp
            // 
            chkModbusTcp.AutoSize = true;
            chkModbusTcp.Checked = true;
            chkModbusTcp.CheckState = CheckState.Checked;
            chkModbusTcp.Location = new Point(100, 95);
            chkModbusTcp.Name = "chkModbusTcp";
            chkModbusTcp.Size = new Size(177, 21);
            chkModbusTcp.TabIndex = 4;
            chkModbusTcp.Text = "使用 Modbus TCP (MBAP)";
            chkModbusTcp.UseVisualStyleBackColor = true;
            // 
            // lblTcpPort
            // 
            lblTcpPort.AutoSize = true;
            lblTcpPort.Location = new Point(20, 65);
            lblTcpPort.Name = "lblTcpPort";
            lblTcpPort.Size = new Size(47, 17);
            lblTcpPort.TabIndex = 2;
            lblTcpPort.Text = "端口号:";
            // 
            // txtTcpPort
            // 
            txtTcpPort.Location = new Point(100, 55);
            txtTcpPort.Name = "txtTcpPort";
            txtTcpPort.Size = new Size(150, 23);
            txtTcpPort.TabIndex = 3;
            txtTcpPort.Text = "502";
            // 
            // lblIp
            // 
            lblIp.AutoSize = true;
            lblIp.Location = new Point(20, 25);
            lblIp.Name = "lblIp";
            lblIp.Size = new Size(46, 17);
            lblIp.TabIndex = 0;
            lblIp.Text = "IP地址:";
            // 
            // txtIp
            // 
            txtIp.Location = new Point(100, 15);
            txtIp.Name = "txtIp";
            txtIp.Size = new Size(150, 23);
            txtIp.TabIndex = 1;
            txtIp.Text = "127.0.0.1";
            // 
            // btnOk
            // 
            btnOk.Location = new Point(180, 400);
            btnOk.Name = "btnOk";
            btnOk.Size = new Size(75, 28);
            btnOk.TabIndex = 5;
            btnOk.Text = "确定";
            btnOk.UseVisualStyleBackColor = true;
            btnOk.Click += BtnOk_Click;
            // 
            // btnCancel
            // 
            btnCancel.DialogResult = DialogResult.Cancel;
            btnCancel.Location = new Point(280, 400);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(75, 28);
            btnCancel.TabIndex = 6;
            btnCancel.Text = "取消";
            btnCancel.UseVisualStyleBackColor = true;
            btnCancel.Click += btnCancel_Click;
            // 
            // lblInterval
            // 
            lblInterval.AutoSize = true;
            lblInterval.Location = new Point(20, 360);
            lblInterval.Name = "lblInterval";
            lblInterval.Size = new Size(84, 17);
            lblInterval.TabIndex = 3;
            lblInterval.Text = "轮询间隔(ms):";
            // 
            // numInterval
            // 
            numInterval.Location = new Point(120, 357);
            numInterval.Maximum = new decimal(new int[] { 5000, 0, 0, 0 });
            numInterval.Name = "numInterval";
            numInterval.Size = new Size(80, 23);
            numInterval.TabIndex = 4;
            numInterval.Value = new decimal(new int[] { 50, 0, 0, 0 });
            // 
            // F_ChannelConfig
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(400, 450);
            Controls.Add(numInterval);
            Controls.Add(lblInterval);
            Controls.Add(btnCancel);
            Controls.Add(btnOk);
            Controls.Add(tabType);
            Controls.Add(txtName);
            Controls.Add(lblName);
            Name = "F_ChannelConfig";
            StartPosition = FormStartPosition.CenterParent;
            Text = "F_ChannelConfig";
            tabType.ResumeLayout(false);
            pageSerial.ResumeLayout(false);
            pageSerial.PerformLayout();
            pageTcp.ResumeLayout(false);
            pageTcp.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numInterval).EndInit();
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblName;
        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.TabControl tabType;
        private System.Windows.Forms.TabPage pageSerial;
        private System.Windows.Forms.TabPage pageTcp;
        private System.Windows.Forms.Label lblPort;
        private System.Windows.Forms.ComboBox cboPort;
        private System.Windows.Forms.Label lblBaud;
        private System.Windows.Forms.ComboBox cboBaud;
        private System.Windows.Forms.Label lblDataBits;
        private System.Windows.Forms.ComboBox cboDataBits;
        private System.Windows.Forms.Label lblStopBits;
        private System.Windows.Forms.ComboBox cboStopBits;
        private System.Windows.Forms.Label lblParity;
        private System.Windows.Forms.ComboBox cboParity;
        private System.Windows.Forms.Label lblIp;
        private System.Windows.Forms.TextBox txtIp;
        private System.Windows.Forms.Label lblTcpPort;
        private System.Windows.Forms.TextBox txtTcpPort;
        private System.Windows.Forms.CheckBox chkModbusTcp;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label lblInterval;
        private System.Windows.Forms.NumericUpDown numInterval;
    }
}