namespace ModbusPilot.UI.Common
{
    partial class UC_WidgetControl
    {
        /// <summary> 
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 组件设计器生成的代码

        /// <summary> 
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            lblCurrent = new Label();
            txtInput = new TextBox();
            btnSet = new Button();
            pnlHeader.SuspendLayout();
            pnlContent.SuspendLayout();
            SuspendLayout();
            // 
            // pnlHeader
            // 
            pnlHeader.Size = new Size(185, 26);
            // 
            // lblName
            // 
            lblName.Size = new Size(140, 26);
            // 
            // lblUnit
            // 
            lblUnit.Location = new Point(140, 0);
            lblUnit.Size = new Size(45, 26);
            // 
            // pnlContent
            // 
            pnlContent.Controls.Add(btnSet);
            pnlContent.Controls.Add(txtInput);
            pnlContent.Controls.Add(lblCurrent);
            pnlContent.Location = new Point(0, 26);
            pnlContent.Size = new Size(185, 84);
            // 
            // lblCurrent
            // 
            lblCurrent.AutoSize = true;
            lblCurrent.Font = new Font("Consolas", 10F);
            lblCurrent.Location = new Point(10, 5);
            lblCurrent.Name = "lblCurrent";
            lblCurrent.Size = new Size(64, 17);
            lblCurrent.TabIndex = 2;
            lblCurrent.Text = "当前: -";
            // 
            // txtInput
            // 
            txtInput.Location = new Point(10, 35);
            txtInput.Name = "txtInput";
            txtInput.Size = new Size(100, 23);
            txtInput.TabIndex = 3;
            // 
            // btnSet
            // 
            btnSet.Location = new Point(115, 33);
            btnSet.Name = "btnSet";
            btnSet.Size = new Size(50, 27);
            btnSet.TabIndex = 4;
            btnSet.Text = "设";
            btnSet.Click += BtnSet_Click;
            // 
            // UC_WidgetControl
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            Name = "UC_WidgetControl";
            Size = new Size(185, 110);
            pnlHeader.ResumeLayout(false);
            pnlContent.ResumeLayout(false);
            pnlContent.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private Label lblCurrent;
        private TextBox txtInput;
        private Button btnSet;
    }
}
