namespace ModbusPilot.UI.Common
{
    partial class UC_WidgetBase
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        private void InitializeComponent()
        {
            this.pnlHeader = new System.Windows.Forms.Panel();
            this.lblUnit = new System.Windows.Forms.Label();
            this.lblName = new System.Windows.Forms.Label();
            this.pnlContent = new System.Windows.Forms.Panel();
            this.lblDevice = new System.Windows.Forms.Label();
            this.pnlHeader.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlHeader
            // 
            this.pnlHeader.BackColor = System.Drawing.Color.Transparent;
            this.pnlHeader.Controls.Add(this.lblUnit);
            this.pnlHeader.Controls.Add(this.lblName);
            this.pnlHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlHeader.Height = 26;
            this.pnlHeader.Name = "pnlHeader";
            this.pnlHeader.TabIndex = 0;
            // 
            // lblName
            // 
            this.lblName.AutoEllipsis = true; // 开启自动省略号
            this.lblName.Dock = System.Windows.Forms.DockStyle.Left;
            this.lblName.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblName.ForeColor = System.Drawing.Color.DimGray;
            this.lblName.Location = new System.Drawing.Point(0, 0);
            this.lblName.Name = "lblName";
            this.lblName.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
            this.lblName.Size = new System.Drawing.Size(120, 26);
            this.lblName.TabIndex = 0;
            this.lblName.Text = "变量名称";
            this.lblName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblUnit
            // 
            this.lblUnit.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblUnit.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.lblUnit.ForeColor = System.Drawing.Color.Gray;
            this.lblUnit.Location = new System.Drawing.Point(120, 0);
            this.lblUnit.Name = "lblUnit";
            this.lblUnit.Padding = new System.Windows.Forms.Padding(0, 0, 5, 0);
            this.lblUnit.Size = new System.Drawing.Size(78, 26);
            this.lblUnit.TabIndex = 1;
            this.lblUnit.Text = "[单位]";
            this.lblUnit.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // pnlContent
            // 
            this.pnlContent.BackColor = System.Drawing.Color.Transparent;
            this.pnlContent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlContent.Location = new System.Drawing.Point(1, 27); // 留出 Header
            this.pnlContent.Name = "pnlContent";
            this.pnlContent.Padding = new System.Windows.Forms.Padding(5);
            this.pnlContent.Size = new System.Drawing.Size(198, 65);
            this.pnlContent.TabIndex = 1;
            // 
            // lblDevice
            // 
            this.lblDevice.BackColor = System.Drawing.Color.Transparent;
            this.lblDevice.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.lblDevice.Font = new System.Drawing.Font("Segoe UI", 7F); // 小字体
            this.lblDevice.ForeColor = System.Drawing.Color.Silver; // 淡色
            this.lblDevice.Location = new System.Drawing.Point(1, 92);
            this.lblDevice.Name = "lblDevice";
            this.lblDevice.Size = new System.Drawing.Size(198, 16);
            this.lblDevice.TabIndex = 2;
            this.lblDevice.Text = "Device 1";
            this.lblDevice.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // UC_WidgetBase
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.pnlContent);
            this.Controls.Add(this.pnlHeader);
            this.Controls.Add(this.lblDevice); // 底部设备名
            this.Name = "UC_WidgetBase";
            this.Padding = new System.Windows.Forms.Padding(1); // 边框
            this.Size = new System.Drawing.Size(200, 110);
            this.pnlHeader.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        protected System.Windows.Forms.Panel pnlHeader;
        protected System.Windows.Forms.Label lblName;
        protected System.Windows.Forms.Label lblUnit;
        protected System.Windows.Forms.Label lblDevice; // 新增
        public System.Windows.Forms.Panel pnlContent;
    }
}