namespace ModbusPilot.UI.Common
{
    partial class F_ImportGuide
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
            rtbGuide = new RichTextBox();
            SuspendLayout();
            // 
            // rtbGuide
            // 
            rtbGuide.BackColor = Color.FromArgb(240, 248, 255);
            rtbGuide.BorderStyle = BorderStyle.None;
            rtbGuide.Dock = DockStyle.Fill;
            rtbGuide.Location = new Point(10, 10);
            rtbGuide.Name = "rtbGuide";
            rtbGuide.ReadOnly = true;
            rtbGuide.ScrollBars = RichTextBoxScrollBars.Vertical;
            rtbGuide.Size = new Size(492, 673);
            rtbGuide.TabIndex = 0;
            rtbGuide.Text = "";
            // 
            // F_ImportGuide
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(240, 248, 255);
            ClientSize = new Size(512, 693);
            Controls.Add(rtbGuide);
            FormBorderStyle = FormBorderStyle.SizableToolWindow;
            Name = "F_ImportGuide";
            Padding = new Padding(10);
            StartPosition = FormStartPosition.CenterParent;
            Text = "📖 智能导入操作指南";
            ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RichTextBox rtbGuide;
    }
}