namespace ModbusPilot.UI.Common
{
    partial class F_SmartImport
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            panelTop = new Panel();
            btnHelp = new Button();
            lblFile = new Label();
            btnOpenFile = new Button();
            numHeaderRow = new NumericUpDown();
            lblHeaderRow = new Label();
            panelBottom = new Panel();
            lblStatus = new Label();
            btnCancel = new Button();
            btnImport = new Button();
            dgvPreview = new DataGridView();
            lblGuide = new Label();
            panelTop.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numHeaderRow).BeginInit();
            panelBottom.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvPreview).BeginInit();
            SuspendLayout();
            // 
            // panelTop
            // 
            panelTop.Controls.Add(btnHelp);
            panelTop.Controls.Add(lblFile);
            panelTop.Controls.Add(btnOpenFile);
            panelTop.Controls.Add(numHeaderRow);
            panelTop.Controls.Add(lblHeaderRow);
            panelTop.Dock = DockStyle.Top;
            panelTop.Location = new Point(0, 0);
            panelTop.Name = "panelTop";
            panelTop.Size = new Size(1058, 50);
            panelTop.TabIndex = 0;
            // 
            // btnHelp
            // 
            btnHelp.BackColor = Color.Snow;
            btnHelp.Dock = DockStyle.Right;
            btnHelp.FlatAppearance.BorderSize = 0;
            btnHelp.Font = new Font("Microsoft YaHei UI", 13F, FontStyle.Bold);
            btnHelp.ForeColor = Color.Orange;
            btnHelp.Location = new Point(900, 0);
            btnHelp.Name = "btnHelp";
            btnHelp.Size = new Size(158, 50);
            btnHelp.TabIndex = 4;
            btnHelp.Text = "💡 操作指引 >>";
            btnHelp.UseVisualStyleBackColor = false;
            // 
            // lblFile
            // 
            lblFile.AutoSize = true;
            lblFile.ForeColor = Color.Gray;
            lblFile.Location = new Point(300, 20);
            lblFile.Name = "lblFile";
            lblFile.Size = new Size(44, 17);
            lblFile.TabIndex = 3;
            lblFile.Text = "未选择";
            // 
            // btnOpenFile
            // 
            btnOpenFile.Location = new Point(12, 15);
            btnOpenFile.Name = "btnOpenFile";
            btnOpenFile.Size = new Size(100, 26);
            btnOpenFile.TabIndex = 2;
            btnOpenFile.Text = "📂 选择文件";
            btnOpenFile.UseVisualStyleBackColor = true;
            // 
            // numHeaderRow
            // 
            numHeaderRow.Location = new Point(230, 17);
            numHeaderRow.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            numHeaderRow.Name = "numHeaderRow";
            numHeaderRow.Size = new Size(50, 23);
            numHeaderRow.TabIndex = 1;
            numHeaderRow.Value = new decimal(new int[] { 1, 0, 0, 0 });
            // 
            // lblHeaderRow
            // 
            lblHeaderRow.AutoSize = true;
            lblHeaderRow.Location = new Point(130, 20);
            lblHeaderRow.Name = "lblHeaderRow";
            lblHeaderRow.Size = new Size(91, 17);
            lblHeaderRow.TabIndex = 0;
            lblHeaderRow.Text = "表头所在行(第):";
            // 
            // panelBottom
            // 
            panelBottom.Controls.Add(lblStatus);
            panelBottom.Controls.Add(btnCancel);
            panelBottom.Controls.Add(btnImport);
            panelBottom.Dock = DockStyle.Bottom;
            panelBottom.Location = new Point(0, 539);
            panelBottom.Name = "panelBottom";
            panelBottom.Size = new Size(1058, 50);
            panelBottom.TabIndex = 1;
            // 
            // lblStatus
            // 
            lblStatus.AutoSize = true;
            lblStatus.Location = new Point(12, 17);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(56, 17);
            lblStatus.TabIndex = 2;
            lblStatus.Text = "状态提示";
            // 
            // btnCancel
            // 
            btnCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnCancel.DialogResult = DialogResult.Cancel;
            btnCancel.Location = new Point(968, 10);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(80, 30);
            btnCancel.TabIndex = 1;
            btnCancel.Text = "取消";
            btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnImport
            // 
            btnImport.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnImport.DialogResult = DialogResult.OK;
            btnImport.Font = new Font("Microsoft YaHei UI", 9F, FontStyle.Bold);
            btnImport.Location = new Point(838, 10);
            btnImport.Name = "btnImport";
            btnImport.Size = new Size(120, 30);
            btnImport.TabIndex = 0;
            btnImport.Text = "🚀 开始导入";
            btnImport.UseVisualStyleBackColor = true;
            // 
            // dgvPreview
            // 
            dgvPreview.AllowUserToAddRows = false;
            dgvPreview.AllowUserToDeleteRows = false;
            dgvPreview.BackgroundColor = Color.White;
            dgvPreview.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvPreview.Dock = DockStyle.Fill;
            dgvPreview.Location = new Point(0, 50);
            dgvPreview.Name = "dgvPreview";
            dgvPreview.ReadOnly = true;
            dgvPreview.Size = new Size(1058, 489);
            dgvPreview.TabIndex = 2;
            // 
            // lblGuide
            // 
            lblGuide.AutoSize = true;
            lblGuide.BackColor = SystemColors.ControlLightLight;
            lblGuide.Font = new Font("Microsoft YaHei UI", 15F, FontStyle.Regular, GraphicsUnit.Pixel);
            lblGuide.Location = new Point(381, 266);
            lblGuide.Name = "lblGuide";
            lblGuide.Size = new Size(243, 40);
            lblGuide.TabIndex = 3;
            lblGuide.Text = "第一步：点击左上角 [📂 选择文件]\r\n支持 .xlsx, .xls, .csv 格式";
            lblGuide.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // F_SmartImport
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1058, 589);
            Controls.Add(lblGuide);
            Controls.Add(dgvPreview);
            Controls.Add(panelBottom);
            Controls.Add(panelTop);
            Name = "F_SmartImport";
            StartPosition = FormStartPosition.CenterParent;
            Text = "智能导入向导 (Smart Import Wizard)";
            panelTop.ResumeLayout(false);
            panelTop.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numHeaderRow).EndInit();
            panelBottom.ResumeLayout(false);
            panelBottom.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dgvPreview).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        private System.Windows.Forms.Panel panelTop;
        private System.Windows.Forms.Button btnOpenFile;
        private System.Windows.Forms.NumericUpDown numHeaderRow;
        private System.Windows.Forms.Label lblHeaderRow;
        private System.Windows.Forms.Label lblFile;
        private System.Windows.Forms.Panel panelBottom;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnImport;
        private System.Windows.Forms.DataGridView dgvPreview;
        private Label lblGuide;
        private Label lblStatus;
        private Button btnHelp;
    }
}