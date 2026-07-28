namespace ModbusPilot.UI.Common
{
    partial class F_Registration
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            pnlLeft = new Panel();
            htmlCompare = new TheArtOfDev.HtmlRenderer.WinForms.HtmlPanel();
            pnlRight = new Panel();
            lblStatus = new Label();
            lblPrompt = new Label();
            lnkBuy = new LinkLabel();
            btnActivate = new Button();
            label3 = new Label();
            txtLicenseKey = new TextBox();
            btnCopy = new Button();
            label2 = new Label();
            txtMachineCode = new TextBox();
            label1 = new Label();
            pnlLeft.SuspendLayout();
            pnlRight.SuspendLayout();
            SuspendLayout();
            // 
            // pnlLeft
            // 
            pnlLeft.BackColor = Color.FromArgb(30, 30, 30);
            pnlLeft.Controls.Add(htmlCompare);
            pnlLeft.Dock = DockStyle.Left;
            pnlLeft.Location = new Point(0, 0);
            pnlLeft.Name = "pnlLeft";
            pnlLeft.Size = new Size(420, 681);
            pnlLeft.TabIndex = 0;
            // 
            // htmlCompare
            // 
            htmlCompare.AutoScroll = true;
            htmlCompare.BackColor = Color.FromArgb(30, 30, 30);
            htmlCompare.BaseStylesheet = null;
            htmlCompare.Dock = DockStyle.Fill;
            htmlCompare.IsContextMenuEnabled = false;
            htmlCompare.IsSelectionEnabled = false;
            htmlCompare.Location = new Point(0, 0);
            htmlCompare.Name = "htmlCompare";
            htmlCompare.Size = new Size(420, 681);
            htmlCompare.TabIndex = 0;
            htmlCompare.Text = null;
            // 
            // pnlRight
            // 
            pnlRight.BackColor = Color.FromArgb(45, 45, 48);
            pnlRight.Controls.Add(lblStatus);
            pnlRight.Controls.Add(lblPrompt);
            pnlRight.Controls.Add(lnkBuy);
            pnlRight.Controls.Add(btnActivate);
            pnlRight.Controls.Add(label3);
            pnlRight.Controls.Add(txtLicenseKey);
            pnlRight.Controls.Add(btnCopy);
            pnlRight.Controls.Add(label2);
            pnlRight.Controls.Add(txtMachineCode);
            pnlRight.Controls.Add(label1);
            pnlRight.Dock = DockStyle.Fill;
            pnlRight.Location = new Point(420, 0);
            pnlRight.Name = "pnlRight";
            pnlRight.Size = new Size(364, 681);
            pnlRight.TabIndex = 1;
            // 
            // lblStatus
            // 
            lblStatus.AutoSize = true;
            lblStatus.ForeColor = Color.DarkGray;
            lblStatus.Location = new Point(23, 385);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(128, 17);
            lblStatus.TabIndex = 9;
            lblStatus.Text = "当前状态：基础免费版";
            // 
            // lblPrompt
            // 
            lblPrompt.Font = new Font("Microsoft YaHei UI", 9F, FontStyle.Bold);
            lblPrompt.ForeColor = Color.Orange;
            lblPrompt.Location = new Point(20, 56);
            lblPrompt.Name = "lblPrompt";
            lblPrompt.Size = new Size(322, 45);
            lblPrompt.TabIndex = 8;
            lblPrompt.Text = "专业版可解锁设备数量限制并开启数据导出功能。";
            // 
            // lnkBuy
            // 
            lnkBuy.ActiveLinkColor = Color.Lime;
            lnkBuy.AutoSize = true;
            lnkBuy.LinkColor = Color.FromArgb(0, 192, 192);
            lnkBuy.Location = new Point(23, 222);
            lnkBuy.Name = "lnkBuy";
            lnkBuy.Size = new Size(128, 17);
            lnkBuy.TabIndex = 7;
            lnkBuy.TabStop = true;
            lnkBuy.Text = "还没有激活码？去获取";
            lnkBuy.LinkClicked += lnkBuy_LinkClicked;
            // 
            // btnActivate
            // 
            btnActivate.BackColor = Color.FromArgb(0, 122, 204);
            btnActivate.FlatAppearance.BorderSize = 0;
            btnActivate.FlatStyle = FlatStyle.Flat;
            btnActivate.Font = new Font("Microsoft YaHei UI", 10.5F, FontStyle.Bold);
            btnActivate.ForeColor = Color.White;
            btnActivate.Location = new Point(20, 327);
            btnActivate.Name = "btnActivate";
            btnActivate.Size = new Size(322, 40);
            btnActivate.TabIndex = 6;
            btnActivate.Text = "立即激活专业版";
            btnActivate.UseVisualStyleBackColor = false;
            btnActivate.Click += btnActivate_Click;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.ForeColor = Color.White;
            label3.Location = new Point(20, 248);
            label3.Name = "label3";
            label3.Size = new Size(80, 17);
            label3.TabIndex = 5;
            label3.Text = "输入激活码：";
            // 
            // txtLicenseKey
            // 
            txtLicenseKey.BackColor = Color.FromArgb(30, 30, 30);
            txtLicenseKey.BorderStyle = BorderStyle.FixedSingle;
            txtLicenseKey.ForeColor = Color.Lime;
            txtLicenseKey.Location = new Point(20, 270);
            txtLicenseKey.Multiline = true;
            txtLicenseKey.Name = "txtLicenseKey";
            txtLicenseKey.ScrollBars = ScrollBars.Vertical;
            txtLicenseKey.Size = new Size(322, 51);
            txtLicenseKey.TabIndex = 4;
            // 
            // btnCopy
            // 
            btnCopy.BackColor = Color.FromArgb(63, 63, 70);
            btnCopy.FlatAppearance.BorderSize = 0;
            btnCopy.FlatStyle = FlatStyle.Flat;
            btnCopy.ForeColor = Color.White;
            btnCopy.Location = new Point(267, 185);
            btnCopy.Name = "btnCopy";
            btnCopy.Size = new Size(75, 25);
            btnCopy.TabIndex = 3;
            btnCopy.Text = "复制";
            btnCopy.UseVisualStyleBackColor = false;
            btnCopy.Click += btnCopy_Click;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.ForeColor = Color.White;
            label2.Location = new Point(20, 163);
            label2.Name = "label2";
            label2.Size = new Size(133, 17);
            label2.TabIndex = 2;
            label2.Text = "您的机器码 (Unique)：";
            // 
            // txtMachineCode
            // 
            txtMachineCode.BackColor = Color.FromArgb(30, 30, 30);
            txtMachineCode.BorderStyle = BorderStyle.FixedSingle;
            txtMachineCode.Font = new Font("Consolas", 10F);
            txtMachineCode.ForeColor = Color.White;
            txtMachineCode.Location = new Point(20, 186);
            txtMachineCode.Name = "txtMachineCode";
            txtMachineCode.ReadOnly = true;
            txtMachineCode.Size = new Size(241, 23);
            txtMachineCode.TabIndex = 1;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Microsoft YaHei UI", 15F, FontStyle.Bold);
            label1.ForeColor = Color.White;
            label1.Location = new Point(15, 18);
            label1.Name = "label1";
            label1.Size = new Size(92, 27);
            label1.TabIndex = 0;
            label1.Text = "软件激活";
            // 
            // F_Registration
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(784, 681);
            Controls.Add(pnlRight);
            Controls.Add(pnlLeft);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "F_Registration";
            ShowIcon = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "ModbusPilot - 升级专业版";
            pnlLeft.ResumeLayout(false);
            pnlRight.ResumeLayout(false);
            pnlRight.PerformLayout();
            ResumeLayout(false);
        }

        private Panel pnlLeft;
        private Panel pnlRight;
        private TheArtOfDev.HtmlRenderer.WinForms.HtmlPanel htmlCompare;
        private Label label1;
        private Label label2;
        private TextBox txtMachineCode;
        private Button btnCopy;
        private Label label3;
        private TextBox txtLicenseKey;
        private Button btnActivate;
        private LinkLabel lnkBuy;
        private Label lblPrompt;
        private Label lblStatus;
    }
}