namespace ModbusPilot.UI.Common
{
    partial class F_DeviceMonitor
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
            DataGridViewCellStyle dataGridViewCellStyle2 = new DataGridViewCellStyle();
            pnlFilter = new Panel();
            txtSearch = new TextBox();
            cmbFilterType = new ComboBox();
            btnClearFilter = new Button();
            btnAddrFormat = new Button();
            dgv = new DataGridView();
            colName = new DataGridViewTextBoxColumn();
            colAddr = new DataGridViewTextBoxColumn();
            colUnit = new DataGridViewTextBoxColumn();
            colValue = new DataGridViewTextBoxColumn();
            colInput = new DataGridViewTextBoxColumn();
            colBtn = new DataGridViewButtonColumn();
            cmbFilterZone = new ComboBox();
            label1 = new Label();
            lblFilterType = new Label();
            pnlFilter.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgv).BeginInit();
            SuspendLayout();
            // 
            // pnlFilter
            // 
            pnlFilter.Controls.Add(cmbFilterZone);
            pnlFilter.Controls.Add(label1);
            pnlFilter.Controls.Add(lblFilterType);
            pnlFilter.Controls.Add(txtSearch);
            pnlFilter.Controls.Add(cmbFilterType);
            pnlFilter.Controls.Add(btnClearFilter);
            pnlFilter.Controls.Add(btnAddrFormat);
            pnlFilter.Dock = DockStyle.Top;
            pnlFilter.Location = new Point(0, 0);
            pnlFilter.Name = "pnlFilter";
            pnlFilter.Padding = new Padding(10, 8, 10, 8);
            pnlFilter.Size = new Size(750, 50);
            pnlFilter.TabIndex = 1;
            // 
            // txtSearch
            // 
            txtSearch.Location = new Point(10, 12);
            txtSearch.Name = "txtSearch";
            txtSearch.PlaceholderText = "🔍 搜索变量名称/地址...";
            txtSearch.Size = new Size(157, 23);
            txtSearch.TabIndex = 0;
            // 
            // cmbFilterType
            // 
            cmbFilterType.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbFilterType.Location = new Point(421, 11);
            cmbFilterType.Name = "cmbFilterType";
            cmbFilterType.Size = new Size(120, 25);
            cmbFilterType.TabIndex = 1;
            // 
            // btnClearFilter
            // 
            btnClearFilter.Location = new Point(555, 10);
            btnClearFilter.Name = "btnClearFilter";
            btnClearFilter.Size = new Size(70, 28);
            btnClearFilter.TabIndex = 2;
            btnClearFilter.Text = "❌ 重置";
            btnClearFilter.UseVisualStyleBackColor = true;
            // 
            // btnAddrFormat
            // 
            btnAddrFormat.Location = new Point(650, 11);
            btnAddrFormat.Name = "btnAddrFormat";
            btnAddrFormat.Size = new Size(90, 28);
            btnAddrFormat.TabIndex = 3;
            btnAddrFormat.Text = "🏭 PLC";
            btnAddrFormat.UseVisualStyleBackColor = true;
            // 
            // dgv
            // 
            dgv.AllowUserToAddRows = false;
            dgv.AllowUserToDeleteRows = false;
            dgv.AllowUserToResizeRows = false;
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgv.BackgroundColor = Color.White;
            dgv.BorderStyle = BorderStyle.None;
            dgv.ColumnHeadersHeight = 30;
            dgv.Columns.AddRange(new DataGridViewColumn[] { colName, colAddr, colUnit, colValue, colInput, colBtn });
            dgv.Dock = DockStyle.Fill;
            dgv.Location = new Point(0, 50);
            dgv.Name = "dgv";
            dgv.RowHeadersVisible = false;
            dgv.RowTemplate.Height = 28;
            dgv.Size = new Size(750, 480);
            dgv.TabIndex = 0;
            // 
            // colName
            // 
            colName.FillWeight = 35F;
            colName.HeaderText = "变量名称";
            colName.Name = "colName";
            colName.ReadOnly = true;
            // 
            // colAddr
            // 
            colAddr.FillWeight = 20F;
            colAddr.HeaderText = "地址";
            colAddr.Name = "colAddr";
            colAddr.ReadOnly = true;
            // 
            // colUnit
            // 
            colUnit.FillWeight = 12F;
            colUnit.HeaderText = "单位";
            colUnit.Name = "colUnit";
            colUnit.ReadOnly = true;
            // 
            // colValue
            // 
            dataGridViewCellStyle2.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.Font = new Font("Consolas", 10F, FontStyle.Bold);
            dataGridViewCellStyle2.ForeColor = Color.Blue;
            colValue.DefaultCellStyle = dataGridViewCellStyle2;
            colValue.FillWeight = 18F;
            colValue.HeaderText = "当前值";
            colValue.Name = "colValue";
            colValue.ReadOnly = true;
            // 
            // colInput
            // 
            colInput.FillWeight = 18F;
            colInput.HeaderText = "写入数值";
            colInput.Name = "colInput";
            // 
            // colBtn
            // 
            colBtn.FillWeight = 12F;
            colBtn.HeaderText = "操作";
            colBtn.Name = "colBtn";
            colBtn.Text = "写入";
            colBtn.UseColumnTextForButtonValue = true;
            // 
            // cmbFilterZone
            // 
            cmbFilterZone.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbFilterZone.Location = new Point(234, 11);
            cmbFilterZone.Name = "cmbFilterZone";
            cmbFilterZone.Size = new Size(130, 25);
            cmbFilterZone.TabIndex = 4;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(178, 15);
            label1.Name = "label1";
            label1.Size = new Size(47, 17);
            label1.TabIndex = 5;
            label1.Text = "存储区:";
            // 
            // lblFilterType
            // 
            lblFilterType.AutoSize = true;
            lblFilterType.Location = new Point(380, 15);
            lblFilterType.Name = "lblFilterType";
            lblFilterType.Size = new Size(35, 17);
            lblFilterType.TabIndex = 6;
            lblFilterType.Text = "类型:";
            // 
            // F_DeviceMonitor
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(750, 530);
            Controls.Add(dgv);
            Controls.Add(pnlFilter);
            Name = "F_DeviceMonitor";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "设备数据监控";
            pnlFilter.ResumeLayout(false);
            pnlFilter.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dgv).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Panel pnlFilter;
        private System.Windows.Forms.TextBox txtSearch;
        private System.Windows.Forms.ComboBox cmbFilterType;
        private System.Windows.Forms.Button btnClearFilter;
        private System.Windows.Forms.Button btnAddrFormat;
        private System.Windows.Forms.DataGridView dgv;
        private DataGridViewTextBoxColumn colName;
        private DataGridViewTextBoxColumn colAddr;
        private DataGridViewTextBoxColumn colUnit;
        private DataGridViewTextBoxColumn colValue;
        private DataGridViewTextBoxColumn colInput;
        private DataGridViewButtonColumn colBtn;
        private ComboBox cmbFilterZone;
        private Label label1;
        private Label lblFilterType;
    }
}
