namespace ModbusPilot.App
{
    partial class F_About
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(F_About));
            pictureBoxLogo = new PictureBox();
            lblProductName = new Label();
            lblVersion = new Label();
            lblCopyright = new Label();
            lblCompany = new Label();
            txtDescription = new TextBox();
            btnOk = new Button();
            linkWeb = new LinkLabel();
            panelDivider = new Panel();
            ((System.ComponentModel.ISupportInitialize)pictureBoxLogo).BeginInit();
            SuspendLayout();
            // 
            // pictureBoxLogo
            // 
            pictureBoxLogo.BorderStyle = BorderStyle.FixedSingle;
            pictureBoxLogo.Image = (Image)resources.GetObject("pictureBoxLogo.Image");
            pictureBoxLogo.Location = new Point(20, 20);
            pictureBoxLogo.Name = "pictureBoxLogo";
            pictureBoxLogo.Size = new Size(100, 100);
            pictureBoxLogo.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBoxLogo.TabIndex = 0;
            pictureBoxLogo.TabStop = false;
            // 
            // lblProductName
            // 
            lblProductName.AutoSize = true;
            lblProductName.Font = new Font("Microsoft YaHei UI", 14.25F, FontStyle.Bold);
            lblProductName.Location = new Point(140, 18);
            lblProductName.Name = "lblProductName";
            lblProductName.Size = new Size(189, 26);
            lblProductName.TabIndex = 1;
            lblProductName.Text = "ModbusPilot Suite";
            // 
            // lblVersion
            // 
            lblVersion.AutoSize = true;
            lblVersion.Font = new Font("Microsoft YaHei UI", 9F);
            lblVersion.Location = new Point(142, 50);
            lblVersion.Name = "lblVersion";
            lblVersion.Size = new Size(93, 17);
            lblVersion.TabIndex = 2;
            lblVersion.Text = "Version 0.9.0.0";
            // 
            // lblCopyright
            // 
            lblCopyright.AutoSize = true;
            lblCopyright.ForeColor = SystemColors.ControlDarkDark;
            lblCopyright.Location = new Point(142, 72);
            lblCopyright.Name = "lblCopyright";
            lblCopyright.Size = new Size(303, 17);
            lblCopyright.TabIndex = 3;
            lblCopyright.Text = "Copyright © 2025 Zerosys Lab. All rights reserved.";
            // 
            // lblCompany
            // 
            lblCompany.AutoSize = true;
            lblCompany.Font = new Font("Microsoft YaHei UI", 9F, FontStyle.Bold);
            lblCompany.Location = new Point(142, 94);
            lblCompany.Name = "lblCompany";
            lblCompany.Size = new Size(81, 17);
            lblCompany.TabIndex = 4;
            lblCompany.Text = "Zerosys Lab";
            // 
            // txtDescription
            // 
            txtDescription.BackColor = SystemColors.Control;
            txtDescription.BorderStyle = BorderStyle.None;
            txtDescription.Location = new Point(145, 125);
            txtDescription.Multiline = true;
            txtDescription.Name = "txtDescription";
            txtDescription.ReadOnly = true;
            txtDescription.Size = new Size(320, 50);
            txtDescription.TabIndex = 5;
            txtDescription.TabStop = false;
            txtDescription.Text = "A professional Modbus master & SCADA configuration tool designed for industrial automation workflow.";
            // 
            // btnOk
            // 
            btnOk.DialogResult = DialogResult.OK;
            btnOk.Location = new Point(375, 205);
            btnOk.Name = "btnOk";
            btnOk.Size = new Size(90, 30);
            btnOk.TabIndex = 6;
            btnOk.Text = "OK";
            btnOk.UseVisualStyleBackColor = true;
            btnOk.Click += btnOk_Click;
            // 
            // linkWeb
            // 
            linkWeb.AutoSize = true;
            linkWeb.Location = new Point(20, 212);
            linkWeb.Name = "linkWeb";
            linkWeb.Size = new Size(115, 17);
            linkWeb.TabIndex = 7;
            linkWeb.TabStop = true;
            linkWeb.Text = "https://zerosys.lab";
            linkWeb.LinkClicked += linkWeb_LinkClicked;
            // 
            // panelDivider
            // 
            panelDivider.BackColor = Color.LightGray;
            panelDivider.Location = new Point(20, 190);
            panelDivider.Name = "panelDivider";
            panelDivider.Size = new Size(445, 1);
            panelDivider.TabIndex = 8;
            // 
            // F_About
            // 
            AcceptButton = btnOk;
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(484, 251);
            Controls.Add(panelDivider);
            Controls.Add(linkWeb);
            Controls.Add(btnOk);
            Controls.Add(txtDescription);
            Controls.Add(lblCompany);
            Controls.Add(lblCopyright);
            Controls.Add(lblVersion);
            Controls.Add(lblProductName);
            Controls.Add(pictureBoxLogo);
            Font = new Font("Microsoft YaHei UI", 9F);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "F_About";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "About ModbusPilot";
            ((System.ComponentModel.ISupportInitialize)pictureBoxLogo).EndInit();
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBoxLogo;
        private System.Windows.Forms.Label lblProductName;
        private System.Windows.Forms.Label lblVersion;
        private System.Windows.Forms.Label lblCopyright;
        private System.Windows.Forms.Label lblCompany;
        private System.Windows.Forms.TextBox txtDescription;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.LinkLabel linkWeb;
        private System.Windows.Forms.Panel panelDivider;
    }
}