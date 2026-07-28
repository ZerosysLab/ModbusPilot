namespace ModbusPilot.UI.Common
{
    partial class F_LogMonitor
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(F_LogMonitor));
            DataGridViewCellStyle dataGridViewCellStyle4 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle3 = new DataGridViewCellStyle();
            toolStrip1 = new ToolStrip();
            lblFilter = new ToolStripLabel();
            cmbChannels = new ToolStripComboBox();
            sep1 = new ToolStripSeparator();
            btnPause = new ToolStripButton();
            btnClear = new ToolStripButton();
            sep2 = new ToolStripSeparator();
            btnExport = new ToolStripButton();
            btnAutoLog = new ToolStripSplitButton();
            itemPolicyError = new ToolStripMenuItem();
            itemPolicyAll = new ToolStripMenuItem();
            chkShowTx = new CheckBox();
            chkShowRx = new CheckBox();
            chkShowErr = new CheckBox();
            chkShowInfo = new CheckBox();
            panelTop = new Panel();
            dgvLog = new DataGridView();
            colTime = new DataGridViewTextBoxColumn();
            colCh = new DataGridViewTextBoxColumn();
            colDir = new DataGridViewTextBoxColumn();
            colHex = new DataGridViewTextBoxColumn();
            colMsg = new DataGridViewTextBoxColumn();
            timerFlush = new System.Windows.Forms.Timer(components);
            toolStrip1.SuspendLayout();
            panelTop.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvLog).BeginInit();
            SuspendLayout();
            // 
            // toolStrip1
            // 
            toolStrip1.Items.AddRange(new ToolStripItem[] { lblFilter, cmbChannels, sep1, btnPause, btnClear, sep2, btnExport, btnAutoLog });
            toolStrip1.Location = new Point(0, 0);
            toolStrip1.Name = "toolStrip1";
            toolStrip1.Size = new Size(800, 25);
            toolStrip1.TabIndex = 0;
            // 
            // lblFilter
            // 
            lblFilter.Name = "lblFilter";
            lblFilter.Size = new Size(59, 22);
            lblFilter.Text = "通道筛选:";
            // 
            // cmbChannels
            // 
            cmbChannels.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbChannels.Name = "cmbChannels";
            cmbChannels.Size = new Size(120, 25);
            // 
            // sep1
            // 
            sep1.Name = "sep1";
            sep1.Size = new Size(6, 25);
            // 
            // btnPause
            // 
            btnPause.CheckOnClick = true;
            btnPause.Name = "btnPause";
            btnPause.Size = new Size(80, 22);
            btnPause.Text = "⏸ 暂停滚屏";
            // 
            // btnClear
            // 
            btnClear.Name = "btnClear";
            btnClear.Size = new Size(56, 22);
            btnClear.Text = "🗑 清空";
            // 
            // sep2
            // 
            sep2.Name = "sep2";
            sep2.Size = new Size(6, 25);
            // 
            // btnExport
            // 
            btnExport.Name = "btnExport";
            btnExport.Size = new Size(56, 22);
            btnExport.Text = "💾 导出";
            // 
            // btnAutoLog
            // 
            btnAutoLog.DisplayStyle = ToolStripItemDisplayStyle.Text;
            btnAutoLog.DropDownItems.AddRange(new ToolStripItem[] { itemPolicyError, itemPolicyAll });
            btnAutoLog.Image = (Image)resources.GetObject("btnAutoLog.Image");
            btnAutoLog.ImageTransparentColor = Color.Magenta;
            btnAutoLog.Name = "btnAutoLog";
            btnAutoLog.Size = new Size(245, 22);
            btnAutoLog.Text = "⚪ 自动存盘 (未启用) / \U0001f7e2 存盘中 (启用)";
            // 
            // itemPolicyError
            // 
            itemPolicyError.Name = "itemPolicyError";
            itemPolicyError.Size = new Size(295, 22);
            itemPolicyError.Text = "[√] 仅记录异常 (推荐) (Tag: ErrorsOnly)";
            // 
            // itemPolicyAll
            // 
            itemPolicyAll.Name = "itemPolicyAll";
            itemPolicyAll.Size = new Size(295, 22);
            itemPolicyAll.Text = "[ ] 记录所有通讯 (Tag: All)";
            // 
            // chkShowTx
            // 
            chkShowTx.AutoSize = true;
            chkShowTx.Checked = true;
            chkShowTx.CheckState = CheckState.Checked;
            chkShowTx.ForeColor = Color.Green;
            chkShowTx.Location = new Point(12, 5);
            chkShowTx.Name = "chkShowTx";
            chkShowTx.Size = new Size(78, 21);
            chkShowTx.TabIndex = 3;
            chkShowTx.Text = "发送 (TX)";
            // 
            // chkShowRx
            // 
            chkShowRx.AutoSize = true;
            chkShowRx.Checked = true;
            chkShowRx.CheckState = CheckState.Checked;
            chkShowRx.ForeColor = Color.Black;
            chkShowRx.Location = new Point(100, 5);
            chkShowRx.Name = "chkShowRx";
            chkShowRx.Size = new Size(79, 21);
            chkShowRx.TabIndex = 2;
            chkShowRx.Text = "接收 (RX)";
            // 
            // chkShowErr
            // 
            chkShowErr.AutoSize = true;
            chkShowErr.Checked = true;
            chkShowErr.CheckState = CheckState.Checked;
            chkShowErr.ForeColor = Color.Red;
            chkShowErr.Location = new Point(190, 5);
            chkShowErr.Name = "chkShowErr";
            chkShowErr.Size = new Size(80, 21);
            chkShowErr.TabIndex = 1;
            chkShowErr.Text = "错误 (Err)";
            // 
            // chkShowInfo
            // 
            chkShowInfo.AutoSize = true;
            chkShowInfo.Checked = true;
            chkShowInfo.CheckState = CheckState.Checked;
            chkShowInfo.ForeColor = Color.Blue;
            chkShowInfo.Location = new Point(280, 5);
            chkShowInfo.Name = "chkShowInfo";
            chkShowInfo.Size = new Size(86, 21);
            chkShowInfo.TabIndex = 0;
            chkShowInfo.Text = "信息 (Info)";
            // 
            // panelTop
            // 
            panelTop.Controls.Add(chkShowInfo);
            panelTop.Controls.Add(chkShowErr);
            panelTop.Controls.Add(chkShowRx);
            panelTop.Controls.Add(chkShowTx);
            panelTop.Dock = DockStyle.Top;
            panelTop.Location = new Point(0, 25);
            panelTop.Name = "panelTop";
            panelTop.Padding = new Padding(10, 5, 0, 0);
            panelTop.Size = new Size(800, 30);
            panelTop.TabIndex = 1;
            // 
            // dgvLog
            // 
            dgvLog.AllowUserToAddRows = false;
            dgvLog.AllowUserToDeleteRows = false;
            dgvLog.AllowUserToResizeRows = false;
            dgvLog.BackgroundColor = Color.White;
            dgvLog.BorderStyle = BorderStyle.None;
            dgvLog.ColumnHeadersHeight = 28;
            dgvLog.Columns.AddRange(new DataGridViewColumn[] { colTime, colCh, colDir, colHex, colMsg });
            dataGridViewCellStyle4.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.BackColor = SystemColors.Window;
            dataGridViewCellStyle4.Font = new Font("Consolas", 9F);
            dataGridViewCellStyle4.ForeColor = SystemColors.ControlText;
            dataGridViewCellStyle4.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle4.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle4.WrapMode = DataGridViewTriState.False;
            dgvLog.DefaultCellStyle = dataGridViewCellStyle4;
            dgvLog.Dock = DockStyle.Fill;
            dgvLog.Location = new Point(0, 55);
            dgvLog.Name = "dgvLog";
            dgvLog.ReadOnly = true;
            dgvLog.RowHeadersVisible = false;
            dgvLog.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvLog.Size = new Size(800, 445);
            dgvLog.TabIndex = 0;
            // 
            // colTime
            // 
            colTime.HeaderText = "时间";
            colTime.Name = "colTime";
            colTime.ReadOnly = true;
            colTime.Width = 90;
            // 
            // colCh
            // 
            colCh.HeaderText = "通道";
            colCh.Name = "colCh";
            colCh.ReadOnly = true;
            colCh.Width = 80;
            // 
            // colDir
            // 
            dataGridViewCellStyle3.Alignment = DataGridViewContentAlignment.MiddleCenter;
            colDir.DefaultCellStyle = dataGridViewCellStyle3;
            colDir.HeaderText = "类型";
            colDir.Name = "colDir";
            colDir.ReadOnly = true;
            colDir.Width = 60;
            // 
            // colHex
            // 
            colHex.HeaderText = "报文 (Hex)";
            colHex.Name = "colHex";
            colHex.ReadOnly = true;
            colHex.Width = 300;
            // 
            // colMsg
            // 
            colMsg.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            colMsg.HeaderText = "信息/错误";
            colMsg.Name = "colMsg";
            colMsg.ReadOnly = true;
            // 
            // timerFlush
            // 
            timerFlush.Interval = 200;
            timerFlush.Tick += TimerFlush_Tick;
            // 
            // F_LogMonitor
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 500);
            Controls.Add(dgvLog);
            Controls.Add(panelTop);
            Controls.Add(toolStrip1);
            Name = "F_LogMonitor";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "通讯报文监视器 (Log Monitor)";
            toolStrip1.ResumeLayout(false);
            toolStrip1.PerformLayout();
            panelTop.ResumeLayout(false);
            panelTop.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dgvLog).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripLabel lblFilter;
        private System.Windows.Forms.ToolStripComboBox cmbChannels;
        private System.Windows.Forms.ToolStripSeparator sep1;
        private System.Windows.Forms.ToolStripButton btnPause;
        private System.Windows.Forms.ToolStripButton btnClear;
        private System.Windows.Forms.ToolStripSeparator sep2;
        private System.Windows.Forms.ToolStripButton btnExport;
        private System.Windows.Forms.Panel panelTop;
        private System.Windows.Forms.CheckBox chkShowTx;
        private System.Windows.Forms.CheckBox chkShowRx;
        private System.Windows.Forms.CheckBox chkShowErr;
        private System.Windows.Forms.CheckBox chkShowInfo;
        private System.Windows.Forms.DataGridView dgvLog;
        private System.Windows.Forms.Timer timerFlush;
        private System.Windows.Forms.DataGridViewTextBoxColumn colTime;
        private System.Windows.Forms.DataGridViewTextBoxColumn colCh;
        private System.Windows.Forms.DataGridViewTextBoxColumn colDir;
        private System.Windows.Forms.DataGridViewTextBoxColumn colHex;
        private System.Windows.Forms.DataGridViewTextBoxColumn colMsg;
        private ToolStripSplitButton btnAutoLog;
        private ToolStripMenuItem itemPolicyError;
        private ToolStripMenuItem itemPolicyAll;
    }
}