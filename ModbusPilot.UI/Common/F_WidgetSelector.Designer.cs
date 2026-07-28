namespace ModbusPilot.UI.Common
{
    partial class F_WidgetSelector
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
            lblPointName = new Label();
            panelButtons = new FlowLayoutPanel();
            SuspendLayout();
            // 
            // lblPointName
            // 
            lblPointName.Dock = DockStyle.Top;
            lblPointName.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblPointName.Location = new Point(0, 0);
            lblPointName.Name = "lblPointName";
            lblPointName.Padding = new Padding(10);
            lblPointName.Size = new Size(300, 40);
            lblPointName.TabIndex = 1;
            lblPointName.Text = "Point Name";
            // 
            // panelButtons
            // 
            panelButtons.Dock = DockStyle.Fill;
            panelButtons.FlowDirection = FlowDirection.TopDown;
            panelButtons.Location = new Point(0, 40);
            panelButtons.Name = "panelButtons";
            panelButtons.Padding = new Padding(10);
            panelButtons.Size = new Size(300, 208);
            panelButtons.TabIndex = 0;
            // 
            // F_WidgetSelector
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(300, 248);
            Controls.Add(panelButtons);
            Controls.Add(lblPointName);
            FormBorderStyle = FormBorderStyle.FixedToolWindow;
            Name = "F_WidgetSelector";
            StartPosition = FormStartPosition.CenterParent;
            Text = "F_WidgetSelector";
            ResumeLayout(false);
        }

        #endregion

        private Label lblPointName;
        private FlowLayoutPanel panelButtons;
    }
}