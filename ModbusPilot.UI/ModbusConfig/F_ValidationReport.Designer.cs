namespace ModbusPilot.UI.Common
{
    partial class F_ValidationReport
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
            this.pnlTop = new System.Windows.Forms.FlowLayoutPanel();
            this.lblValid = new System.Windows.Forms.Label();
            this.lblWarning = new System.Windows.Forms.Label();
            this.lblCritical = new System.Windows.Forms.Label();
            this.pnlBottom = new System.Windows.Forms.Panel();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnContinue = new System.Windows.Forms.Button();
            this.rtbLog = new System.Windows.Forms.RichTextBox();
            this.pnlTop.SuspendLayout();
            this.pnlBottom.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlTop
            // 
            this.pnlTop.Controls.Add(this.lblValid);
            this.pnlTop.Controls.Add(this.lblWarning);
            this.pnlTop.Controls.Add(this.lblCritical);
            this.pnlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlTop.Location = new System.Drawing.Point(0, 0);
            this.pnlTop.Name = "pnlTop";
            this.pnlTop.Padding = new System.Windows.Forms.Padding(10);
            this.pnlTop.Size = new System.Drawing.Size(584, 80);
            this.pnlTop.TabIndex = 0;
            // 
            // lblValid
            // 
            this.lblValid.AutoSize = true;
            this.lblValid.Font = new System.Drawing.Font("Microsoft YaHei UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblValid.Location = new System.Drawing.Point(13, 10);
            this.lblValid.Margin = new System.Windows.Forms.Padding(3, 0, 20, 0);
            this.lblValid.Name = "lblValid";
            this.lblValid.Size = new System.Drawing.Size(125, 19);
            this.lblValid.TabIndex = 0;
            this.lblValid.Text = "✅ 有效数据: 0 行";
            // 
            // lblWarning
            // 
            this.lblWarning.AutoSize = true;
            this.lblWarning.Font = new System.Drawing.Font("Microsoft YaHei UI", 10F);
            this.lblWarning.Location = new System.Drawing.Point(161, 10);
            this.lblWarning.Margin = new System.Windows.Forms.Padding(3, 0, 20, 0);
            this.lblWarning.Name = "lblWarning";
            this.lblWarning.Size = new System.Drawing.Size(193, 20);
            this.lblWarning.TabIndex = 1;
            this.lblWarning.Text = "⚠️ 警告 (类型未知/默认): 0 行";
            // 
            // lblCritical
            // 
            this.lblCritical.AutoSize = true;
            this.lblCritical.Font = new System.Drawing.Font("Microsoft YaHei UI", 10F);
            this.lblCritical.Location = new System.Drawing.Point(13, 40);
            this.lblCritical.Margin = new System.Windows.Forms.Padding(3, 10, 20, 0);
            this.lblCritical.Name = "lblCritical";
            this.lblCritical.Size = new System.Drawing.Size(193, 20);
            this.lblCritical.TabIndex = 2;
            this.lblCritical.Text = "⛔ 严重错误 (将跳过): 0 行";
            // 
            // pnlBottom
            // 
            this.pnlBottom.Controls.Add(this.btnCancel);
            this.pnlBottom.Controls.Add(this.btnContinue);
            this.pnlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlBottom.Location = new System.Drawing.Point(0, 411);
            this.pnlBottom.Name = "pnlBottom";
            this.pnlBottom.Size = new System.Drawing.Size(584, 50);
            this.pnlBottom.TabIndex = 1;
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(340, 10);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(90, 30);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "返回修改";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnContinue
            // 
            this.btnContinue.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnContinue.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnContinue.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnContinue.Location = new System.Drawing.Point(440, 10);
            this.btnContinue.Name = "btnContinue";
            this.btnContinue.Size = new System.Drawing.Size(130, 30);
            this.btnContinue.TabIndex = 0;
            this.btnContinue.Text = "忽略警告并导入";
            this.btnContinue.UseVisualStyleBackColor = true;
            // 
            // rtbLog
            // 
            this.rtbLog.BackColor = System.Drawing.Color.White;
            this.rtbLog.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.rtbLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtbLog.Font = new System.Drawing.Font("Consolas", 9F);
            this.rtbLog.Location = new System.Drawing.Point(0, 80);
            this.rtbLog.Name = "rtbLog";
            this.rtbLog.ReadOnly = true;
            this.rtbLog.Size = new System.Drawing.Size(584, 331);
            this.rtbLog.TabIndex = 2;
            this.rtbLog.Text = "";
            // 
            // F_ValidationReport
            // 
            this.AcceptButton = this.btnContinue;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(584, 461);
            this.Controls.Add(this.rtbLog);
            this.Controls.Add(this.pnlBottom);
            this.Controls.Add(this.pnlTop);
            this.Name = "F_ValidationReport";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "数据预检报告";
            this.pnlTop.ResumeLayout(false);
            this.pnlTop.PerformLayout();
            this.pnlBottom.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel pnlTop;
        private System.Windows.Forms.Label lblValid;
        private System.Windows.Forms.Label lblWarning;
        private System.Windows.Forms.Label lblCritical;
        private System.Windows.Forms.Panel pnlBottom;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnContinue;
        private System.Windows.Forms.RichTextBox rtbLog;
    }
}