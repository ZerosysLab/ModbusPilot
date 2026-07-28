namespace ModbusPilot.UI.Common
{
    partial class F_TrendChart
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
            DataGridViewCellStyle dataGridViewCellStyle1 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle3 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle2 = new DataGridViewCellStyle();
            pnlBottom = new Panel();
            lblRangeValue = new Label();
            trackRange = new TrackBar();
            lblRangeTitle = new Label();
            numDuration = new NumericUpDown();
            numInterval = new NumericUpDown();
            label1 = new Label();
            lblInterval = new Label();
            lblStatus = new Label();
            btnExport = new Button();
            btnClear = new Button();
            btnPause = new Button();
            btnStart = new Button();
            splitContainerMain = new SplitContainer();
            dgvCurves = new DataGridView();
            colVisible = new DataGridViewCheckBoxColumn();
            colColor = new DataGridViewTextBoxColumn();
            colName = new DataGridViewTextBoxColumn();
            colAxis = new DataGridViewComboBoxColumn();
            colValue = new DataGridViewTextBoxColumn();
            colDelete = new DataGridViewButtonColumn();
            pnlLeftTop = new Panel();
            lblCurveCount = new Label();
            pnlChart = new Panel();
            lblGuide = new Label();
            lblDragTip = new Label();
            pnlStatistics = new Panel();
            lblStatsTitle = new Label();
            pnlBottom.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)trackRange).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numDuration).BeginInit();
            ((System.ComponentModel.ISupportInitialize)numInterval).BeginInit();
            ((System.ComponentModel.ISupportInitialize)splitContainerMain).BeginInit();
            splitContainerMain.Panel1.SuspendLayout();
            splitContainerMain.Panel2.SuspendLayout();
            splitContainerMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvCurves).BeginInit();
            pnlLeftTop.SuspendLayout();
            pnlChart.SuspendLayout();
            pnlStatistics.SuspendLayout();
            SuspendLayout();
            // 
            // pnlBottom
            // 
            pnlBottom.BackColor = Color.FromArgb(45, 45, 48);
            pnlBottom.Controls.Add(lblRangeValue);
            pnlBottom.Controls.Add(trackRange);
            pnlBottom.Controls.Add(lblRangeTitle);
            pnlBottom.Controls.Add(numDuration);
            pnlBottom.Controls.Add(numInterval);
            pnlBottom.Controls.Add(label1);
            pnlBottom.Controls.Add(lblInterval);
            pnlBottom.Controls.Add(lblStatus);
            pnlBottom.Controls.Add(btnExport);
            pnlBottom.Controls.Add(btnClear);
            pnlBottom.Controls.Add(btnPause);
            pnlBottom.Controls.Add(btnStart);
            pnlBottom.Dock = DockStyle.Bottom;
            pnlBottom.Location = new Point(0, 468);
            pnlBottom.Name = "pnlBottom";
            pnlBottom.Size = new Size(1021, 70);
            pnlBottom.TabIndex = 3;
            // 
            // lblRangeValue
            // 
            lblRangeValue.AutoSize = true;
            lblRangeValue.ForeColor = Color.WhiteSmoke;
            lblRangeValue.Location = new Point(568, 44);
            lblRangeValue.Name = "lblRangeValue";
            lblRangeValue.Size = new Size(150, 17);
            lblRangeValue.TabIndex = 9;
            lblRangeValue.Text = "视野: 1000 点(最近10.0m)";
            // 
            // trackRange
            // 
            trackRange.AutoSize = false;
            trackRange.Location = new Point(623, 12);
            trackRange.Maximum = 5000;
            trackRange.Minimum = 100;
            trackRange.Name = "trackRange";
            trackRange.Size = new Size(150, 30);
            trackRange.TabIndex = 8;
            trackRange.TickFrequency = 500;
            trackRange.TickStyle = TickStyle.None;
            trackRange.Value = 1000;
            // 
            // lblRangeTitle
            // 
            lblRangeTitle.AutoSize = true;
            lblRangeTitle.ForeColor = Color.Silver;
            lblRangeTitle.Location = new Point(568, 14);
            lblRangeTitle.Name = "lblRangeTitle";
            lblRangeTitle.Size = new Size(48, 17);
            lblRangeTitle.TabIndex = 7;
            lblRangeTitle.Text = "Range:";
            // 
            // numDuration
            // 
            numDuration.BackColor = Color.FromArgb(60, 60, 60);
            numDuration.ForeColor = Color.White;
            numDuration.Location = new Point(483, 42);
            numDuration.Maximum = new decimal(new int[] { 2000, 0, 0, 0 });
            numDuration.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            numDuration.Name = "numDuration";
            numDuration.Size = new Size(70, 23);
            numDuration.TabIndex = 6;
            numDuration.TextAlign = HorizontalAlignment.Center;
            numDuration.Value = new decimal(new int[] { 30, 0, 0, 0 });
            // 
            // numInterval
            // 
            numInterval.BackColor = Color.FromArgb(60, 60, 60);
            numInterval.ForeColor = Color.White;
            numInterval.Increment = new decimal(new int[] { 100, 0, 0, 0 });
            numInterval.Location = new Point(483, 12);
            numInterval.Maximum = new decimal(new int[] { 60000, 0, 0, 0 });
            numInterval.Minimum = new decimal(new int[] { 100, 0, 0, 0 });
            numInterval.Name = "numInterval";
            numInterval.Size = new Size(70, 23);
            numInterval.TabIndex = 6;
            numInterval.TextAlign = HorizontalAlignment.Center;
            numInterval.Value = new decimal(new int[] { 1000, 0, 0, 0 });
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.ForeColor = Color.Silver;
            label1.Location = new Point(380, 44);
            label1.Name = "label1";
            label1.Size = new Size(88, 17);
            label1.TabIndex = 5;
            label1.Text = "采样时长(min):";
            // 
            // lblInterval
            // 
            lblInterval.AutoSize = true;
            lblInterval.ForeColor = Color.Silver;
            lblInterval.Location = new Point(380, 14);
            lblInterval.Name = "lblInterval";
            lblInterval.Size = new Size(84, 17);
            lblInterval.TabIndex = 5;
            lblInterval.Text = "采样周期(ms):";
            // 
            // lblStatus
            // 
            lblStatus.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            lblStatus.AutoSize = true;
            lblStatus.ForeColor = Color.WhiteSmoke;
            lblStatus.Location = new Point(813, 27);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(205, 17);
            lblStatus.TabIndex = 4;
            lblStatus.Text = "已采: 000000 pts | 总时长: 00:00:00";
            lblStatus.TextAlign = ContentAlignment.MiddleRight;
            // 
            // btnExport
            // 
            btnExport.BackColor = Color.FromArgb(60, 60, 60);
            btnExport.FlatAppearance.BorderColor = Color.Gray;
            btnExport.FlatStyle = FlatStyle.Flat;
            btnExport.ForeColor = Color.White;
            btnExport.Location = new Point(280, 22);
            btnExport.Name = "btnExport";
            btnExport.Size = new Size(80, 30);
            btnExport.TabIndex = 3;
            btnExport.Text = "💾 导出";
            btnExport.UseVisualStyleBackColor = false;
            btnExport.Click += BtnExport_Click;
            // 
            // btnClear
            // 
            btnClear.BackColor = Color.FromArgb(60, 60, 60);
            btnClear.FlatAppearance.BorderColor = Color.Gray;
            btnClear.FlatStyle = FlatStyle.Flat;
            btnClear.ForeColor = Color.White;
            btnClear.Location = new Point(190, 22);
            btnClear.Name = "btnClear";
            btnClear.Size = new Size(80, 30);
            btnClear.TabIndex = 2;
            btnClear.Text = "⏹️ 停止";
            btnClear.UseVisualStyleBackColor = false;
            btnClear.Click += BtnClear_Click;
            // 
            // btnPause
            // 
            btnPause.BackColor = Color.FromArgb(60, 60, 60);
            btnPause.Enabled = false;
            btnPause.FlatAppearance.BorderColor = Color.Gray;
            btnPause.FlatStyle = FlatStyle.Flat;
            btnPause.ForeColor = Color.White;
            btnPause.Location = new Point(100, 22);
            btnPause.Name = "btnPause";
            btnPause.Size = new Size(80, 30);
            btnPause.TabIndex = 1;
            btnPause.Text = "⏸️ 暂停";
            btnPause.UseVisualStyleBackColor = false;
            btnPause.Click += BtnPause_Click;
            // 
            // btnStart
            // 
            btnStart.BackColor = Color.FromArgb(30, 160, 80);
            btnStart.FlatAppearance.BorderSize = 0;
            btnStart.FlatStyle = FlatStyle.Flat;
            btnStart.ForeColor = Color.White;
            btnStart.Location = new Point(10, 22);
            btnStart.Name = "btnStart";
            btnStart.Size = new Size(80, 30);
            btnStart.TabIndex = 0;
            btnStart.Text = "▶️ 开始";
            btnStart.UseVisualStyleBackColor = false;
            btnStart.Click += BtnStart_Click;
            // 
            // splitContainerMain
            // 
            splitContainerMain.BackColor = Color.FromArgb(60, 60, 60);
            splitContainerMain.Dock = DockStyle.Fill;
            splitContainerMain.FixedPanel = FixedPanel.Panel1;
            splitContainerMain.Location = new Point(0, 0);
            splitContainerMain.Name = "splitContainerMain";
            // 
            // splitContainerMain.Panel1
            // 
            splitContainerMain.Panel1.BackColor = Color.FromArgb(45, 45, 48);
            splitContainerMain.Panel1.Controls.Add(dgvCurves);
            splitContainerMain.Panel1.Controls.Add(pnlLeftTop);
            splitContainerMain.Panel1MinSize = 300;
            // 
            // splitContainerMain.Panel2
            // 
            splitContainerMain.Panel2.Controls.Add(pnlChart);
            splitContainerMain.Panel2.Controls.Add(pnlStatistics);
            splitContainerMain.Size = new Size(1021, 468);
            splitContainerMain.SplitterDistance = 320;
            splitContainerMain.TabIndex = 4;
            // 
            // dgvCurves
            // 
            dgvCurves.AllowUserToAddRows = false;
            dgvCurves.AllowUserToResizeRows = false;
            dgvCurves.BackgroundColor = Color.FromArgb(45, 45, 48);
            dgvCurves.BorderStyle = BorderStyle.None;
            dgvCurves.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle1.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = Color.FromArgb(30, 30, 30);
            dataGridViewCellStyle1.Font = new Font("Microsoft YaHei UI", 9F);
            dataGridViewCellStyle1.ForeColor = Color.WhiteSmoke;
            dataGridViewCellStyle1.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = DataGridViewTriState.True;
            dgvCurves.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            dgvCurves.ColumnHeadersHeight = 32;
            dgvCurves.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dgvCurves.Columns.AddRange(new DataGridViewColumn[] { colVisible, colColor, colName, colAxis, colValue, colDelete });
            dataGridViewCellStyle3.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = Color.FromArgb(45, 45, 48);
            dataGridViewCellStyle3.Font = new Font("Microsoft YaHei UI", 9F);
            dataGridViewCellStyle3.ForeColor = Color.WhiteSmoke;
            dataGridViewCellStyle3.SelectionBackColor = Color.FromArgb(60, 60, 60);
            dataGridViewCellStyle3.SelectionForeColor = Color.WhiteSmoke;
            dataGridViewCellStyle3.WrapMode = DataGridViewTriState.False;
            dgvCurves.DefaultCellStyle = dataGridViewCellStyle3;
            dgvCurves.Dock = DockStyle.Fill;
            dgvCurves.EnableHeadersVisualStyles = false;
            dgvCurves.GridColor = Color.FromArgb(64, 64, 64);
            dgvCurves.Location = new Point(0, 35);
            dgvCurves.Name = "dgvCurves";
            dgvCurves.RowHeadersVisible = false;
            dgvCurves.RowTemplate.Height = 28;
            dgvCurves.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvCurves.Size = new Size(320, 433);
            dgvCurves.TabIndex = 1;
            dgvCurves.CellContentClick += DgvCurves_CellContentClick;
            dgvCurves.CellValueChanged += DgvCurves_CellValueChanged;
            // 
            // colVisible
            // 
            colVisible.HeaderText = "";
            colVisible.Name = "colVisible";
            colVisible.Resizable = DataGridViewTriState.False;
            colVisible.Width = 30;
            // 
            // colColor
            // 
            colColor.HeaderText = "";
            colColor.Name = "colColor";
            colColor.ReadOnly = true;
            colColor.Resizable = DataGridViewTriState.False;
            colColor.Width = 10;
            // 
            // colName
            // 
            colName.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            colName.HeaderText = "变量名";
            colName.Name = "colName";
            colName.ReadOnly = true;
            // 
            // colAxis
            // 
            dataGridViewCellStyle2.BackColor = Color.FromArgb(45, 45, 48);
            dataGridViewCellStyle2.ForeColor = Color.White;
            colAxis.DefaultCellStyle = dataGridViewCellStyle2;
            colAxis.DisplayStyle = DataGridViewComboBoxDisplayStyle.ComboBox;
            colAxis.HeaderText = "Y轴";
            colAxis.Items.AddRange(new object[] { "左轴", "右轴" });
            colAxis.Name = "colAxis";
            colAxis.Width = 60;
            // 
            // colValue
            // 
            colValue.HeaderText = "当前值";
            colValue.Name = "colValue";
            colValue.ReadOnly = true;
            colValue.Width = 60;
            // 
            // colDelete
            // 
            colDelete.HeaderText = "";
            colDelete.Name = "colDelete";
            colDelete.Text = "×";
            colDelete.UseColumnTextForButtonValue = true;
            colDelete.Width = 30;
            // 
            // pnlLeftTop
            // 
            pnlLeftTop.BackColor = Color.FromArgb(35, 35, 37);
            pnlLeftTop.Controls.Add(lblCurveCount);
            pnlLeftTop.Dock = DockStyle.Top;
            pnlLeftTop.Location = new Point(0, 0);
            pnlLeftTop.Name = "pnlLeftTop";
            pnlLeftTop.Size = new Size(320, 35);
            pnlLeftTop.TabIndex = 2;
            // 
            // lblCurveCount
            // 
            lblCurveCount.AutoSize = true;
            lblCurveCount.ForeColor = Color.Silver;
            lblCurveCount.Location = new Point(10, 9);
            lblCurveCount.Name = "lblCurveCount";
            lblCurveCount.Size = new Size(87, 17);
            lblCurveCount.TabIndex = 0;
            lblCurveCount.Text = "曲线列表 (0/4)";
            // 
            // pnlChart
            // 
            pnlChart.BackColor = Color.FromArgb(30, 30, 30);
            pnlChart.Controls.Add(lblGuide);
            pnlChart.Controls.Add(lblDragTip);
            pnlChart.Dock = DockStyle.Fill;
            pnlChart.Location = new Point(0, 0);
            pnlChart.Name = "pnlChart";
            pnlChart.Size = new Size(697, 468);
            pnlChart.TabIndex = 1;
            // 
            // lblGuide
            // 
            lblGuide.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            lblGuide.AutoSize = true;
            lblGuide.BackColor = Color.Transparent;
            lblGuide.ForeColor = Color.Gray;
            lblGuide.Location = new Point(456, 448);
            lblGuide.Name = "lblGuide";
            lblGuide.Size = new Size(238, 17);
            lblGuide.TabIndex = 1;
            lblGuide.Text = "🖱️ 左键拖动 | ⚙️ 滚轮缩放 | 🖱️🖱️ 双击归位";
            // 
            // lblDragTip
            // 
            lblDragTip.Dock = DockStyle.Fill;
            lblDragTip.Font = new Font("Microsoft YaHei UI", 14F, FontStyle.Regular, GraphicsUnit.Point, 134);
            lblDragTip.ForeColor = Color.Gray;
            lblDragTip.Location = new Point(0, 0);
            lblDragTip.Name = "lblDragTip";
            lblDragTip.Size = new Size(697, 468);
            lblDragTip.TabIndex = 0;
            lblDragTip.Text = "📤 请从设备监控窗口拖拽变量到此处\r\n(Drag Tags Here)";
            lblDragTip.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // pnlStatistics
            // 
            pnlStatistics.BackColor = Color.FromArgb(30, 30, 30);
            pnlStatistics.Controls.Add(lblStatsTitle);
            pnlStatistics.Dock = DockStyle.Right;
            pnlStatistics.Location = new Point(697, 0);
            pnlStatistics.Name = "pnlStatistics";
            pnlStatistics.Size = new Size(0, 468);
            pnlStatistics.TabIndex = 2;
            pnlStatistics.Visible = false;
            // 
            // lblStatsTitle
            // 
            lblStatsTitle.AutoSize = true;
            lblStatsTitle.Font = new Font("Microsoft YaHei UI", 9F, FontStyle.Bold);
            lblStatsTitle.ForeColor = Color.WhiteSmoke;
            lblStatsTitle.Location = new Point(10, 10);
            lblStatsTitle.Name = "lblStatsTitle";
            lblStatsTitle.Size = new Size(77, 17);
            lblStatsTitle.TabIndex = 0;
            lblStatsTitle.Text = "📊 数据统计";
            // 
            // F_TrendChart
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(30, 30, 30);
            ClientSize = new Size(1021, 538);
            Controls.Add(splitContainerMain);
            Controls.Add(pnlBottom);
            Name = "F_TrendChart";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "实时趋势 (Real-time Trend)";
            pnlBottom.ResumeLayout(false);
            pnlBottom.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)trackRange).EndInit();
            ((System.ComponentModel.ISupportInitialize)numDuration).EndInit();
            ((System.ComponentModel.ISupportInitialize)numInterval).EndInit();
            splitContainerMain.Panel1.ResumeLayout(false);
            splitContainerMain.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainerMain).EndInit();
            splitContainerMain.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgvCurves).EndInit();
            pnlLeftTop.ResumeLayout(false);
            pnlLeftTop.PerformLayout();
            pnlChart.ResumeLayout(false);
            pnlChart.PerformLayout();
            pnlStatistics.ResumeLayout(false);
            pnlStatistics.PerformLayout();
            ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlBottom;
        private System.Windows.Forms.Button btnExport;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.Button btnPause;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.SplitContainer splitContainerMain;
        private System.Windows.Forms.Panel pnlLeftTop;
        private System.Windows.Forms.Label lblCurveCount;
        private System.Windows.Forms.DataGridView dgvCurves;
        private System.Windows.Forms.Panel pnlChart;
        private System.Windows.Forms.Label lblDragTip;
        private System.Windows.Forms.Panel pnlStatistics;
        private System.Windows.Forms.Label lblStatsTitle;
        private System.Windows.Forms.NumericUpDown numInterval;
        private System.Windows.Forms.Label lblInterval;

        // 新增控件
        private System.Windows.Forms.TrackBar trackRange;
        private System.Windows.Forms.Label lblRangeTitle;
        private System.Windows.Forms.Label lblRangeValue;

        // 列定义
        private System.Windows.Forms.DataGridViewCheckBoxColumn colVisible;
        private System.Windows.Forms.DataGridViewTextBoxColumn colColor;
        private System.Windows.Forms.DataGridViewTextBoxColumn colName;
        private System.Windows.Forms.DataGridViewComboBoxColumn colAxis;
        private System.Windows.Forms.DataGridViewTextBoxColumn colValue;
        private System.Windows.Forms.DataGridViewButtonColumn colDelete;
        private Label lblGuide;
        private NumericUpDown numDuration;
        private Label label1;
    }
}