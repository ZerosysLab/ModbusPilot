namespace ModbusPilot.UI.Common
{
    partial class F_ModbusAddrManager
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

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            panelHeader = new Panel();
            btnConfirm = new Button();
            btnExport = new Button();
            btnImport = new Button();
            btnImportExcel = new Button();
            numSlaveId = new NumericUpDown();
            lblSlaveId = new Label();
            txtDeviceName = new TextBox();
            lblDeviceName = new Label();
            grpTags = new GroupBox();
            dgvTags = new DataGridView();
            colTagName = new DataGridViewTextBoxColumn();
            colZone = new DataGridViewComboBoxColumn();
            colAddr = new DataGridViewTextBoxColumn();
            colDataType = new DataGridViewComboBoxColumn();
            colBitIndex = new DataGridViewTextBoxColumn();
            colNote = new DataGridViewTextBoxColumn();
            ctxMenuBatch = new ContextMenuStrip(components);
            menuBatchOffset = new ToolStripMenuItem();
            menuBatchIncrement = new ToolStripMenuItem();
            menuSep1 = new ToolStripSeparator();
            menuBatchZone = new ToolStripMenuItem();
            menuBatchDataType = new ToolStripMenuItem();
            menuBatchFormat = new ToolStripMenuItem();
            menuBatchScale = new ToolStripMenuItem();
            menuSep2 = new ToolStripSeparator();
            menuBatchPrefix = new ToolStripMenuItem();
            menuBatchCopy = new ToolStripMenuItem();
            menuIncreaseTag = new ToolStripMenuItem();
            panelFilter = new Panel();
            btnClearFilter = new Button();
            cmbFilterZone = new ComboBox();
            cmbFilterType = new ComboBox();
            label1 = new Label();
            lblFilterType = new Label();
            txtSearch = new TextBox();
            lblSearch = new Label();
            toolStripTags = new ToolStrip();
            btnAddTag = new ToolStripButton();
            btnInsertTag = new ToolStripButton();
            btnDelTag = new ToolStripButton();
            sep3 = new ToolStripSeparator();
            btnHexDec = new ToolStripButton();
            panelHeader.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numSlaveId).BeginInit();
            grpTags.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvTags).BeginInit();
            ctxMenuBatch.SuspendLayout();
            panelFilter.SuspendLayout();
            toolStripTags.SuspendLayout();
            SuspendLayout();
            // 
            // panelHeader
            // 
            panelHeader.BackColor = Color.WhiteSmoke;
            panelHeader.Controls.Add(btnConfirm);
            panelHeader.Controls.Add(btnExport);
            panelHeader.Controls.Add(btnImport);
            panelHeader.Controls.Add(btnImportExcel);
            panelHeader.Controls.Add(numSlaveId);
            panelHeader.Controls.Add(lblSlaveId);
            panelHeader.Controls.Add(txtDeviceName);
            panelHeader.Controls.Add(lblDeviceName);
            panelHeader.Dock = DockStyle.Top;
            panelHeader.Location = new Point(0, 0);
            panelHeader.Name = "panelHeader";
            panelHeader.Size = new Size(1232, 60);
            panelHeader.TabIndex = 0;
            // 
            // btnConfirm
            // 
            btnConfirm.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnConfirm.BackColor = Color.FromArgb(0, 120, 215);
            btnConfirm.Cursor = Cursors.Hand;
            btnConfirm.FlatAppearance.BorderSize = 0;
            btnConfirm.FlatStyle = FlatStyle.Flat;
            btnConfirm.Font = new Font("Microsoft YaHei UI", 9F, FontStyle.Bold);
            btnConfirm.ForeColor = Color.White;
            btnConfirm.Location = new Point(1080, 12);
            btnConfirm.Name = "btnConfirm";
            btnConfirm.Size = new Size(130, 34);
            btnConfirm.TabIndex = 7;
            btnConfirm.Text = "✅ 确定并关闭";
            btnConfirm.UseVisualStyleBackColor = false;
            // 
            // btnExport
            // 
            btnExport.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnExport.Cursor = Cursors.Hand;
            btnExport.FlatAppearance.BorderColor = Color.Silver;
            btnExport.FlatStyle = FlatStyle.Flat;
            btnExport.Location = new Point(910, 12);
            btnExport.Name = "btnExport";
            btnExport.Size = new Size(110, 34);
            btnExport.TabIndex = 6;
            btnExport.Text = "📤 导入标准表格";
            btnExport.UseVisualStyleBackColor = true;
            // 
            // btnImport
            // 
            btnImport.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnImport.Cursor = Cursors.Hand;
            btnImport.FlatAppearance.BorderColor = Color.Silver;
            btnImport.FlatStyle = FlatStyle.Flat;
            btnImport.Location = new Point(780, 12);
            btnImport.Name = "btnImport";
            btnImport.Size = new Size(110, 34);
            btnImport.TabIndex = 5;
            btnImport.Text = "📥 导出标准表格";
            btnImport.UseVisualStyleBackColor = true;
            // 
            // btnImportExcel
            // 
            btnImportExcel.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnImportExcel.BackColor = Color.FromArgb(33, 115, 70);
            btnImportExcel.Cursor = Cursors.Hand;
            btnImportExcel.FlatAppearance.BorderSize = 0;
            btnImportExcel.FlatStyle = FlatStyle.Flat;
            btnImportExcel.Font = new Font("Microsoft YaHei UI", 9F, FontStyle.Bold);
            btnImportExcel.ForeColor = Color.White;
            btnImportExcel.Location = new Point(620, 12);
            btnImportExcel.Name = "btnImportExcel";
            btnImportExcel.Size = new Size(140, 34);
            btnImportExcel.TabIndex = 4;
            btnImportExcel.Text = "✨ Excel 智能导入";
            btnImportExcel.UseVisualStyleBackColor = false;
            // 
            // numSlaveId
            // 
            numSlaveId.Location = new Point(390, 19);
            numSlaveId.Maximum = new decimal(new int[] { 247, 0, 0, 0 });
            numSlaveId.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            numSlaveId.Name = "numSlaveId";
            numSlaveId.Size = new Size(50, 23);
            numSlaveId.TabIndex = 3;
            numSlaveId.Value = new decimal(new int[] { 1, 0, 0, 0 });
            // 
            // lblSlaveId
            // 
            lblSlaveId.AutoSize = true;
            lblSlaveId.Location = new Point(290, 22);
            lblSlaveId.Name = "lblSlaveId";
            lblSlaveId.Size = new Size(94, 17);
            lblSlaveId.TabIndex = 2;
            lblSlaveId.Text = "站号 (Slave ID):";
            // 
            // txtDeviceName
            // 
            txtDeviceName.Location = new Point(90, 19);
            txtDeviceName.Name = "txtDeviceName";
            txtDeviceName.Size = new Size(180, 23);
            txtDeviceName.TabIndex = 1;
            txtDeviceName.Text = "新设备";
            // 
            // lblDeviceName
            // 
            lblDeviceName.AutoSize = true;
            lblDeviceName.Location = new Point(20, 22);
            lblDeviceName.Name = "lblDeviceName";
            lblDeviceName.Size = new Size(59, 17);
            lblDeviceName.TabIndex = 0;
            lblDeviceName.Text = "设备名称:";
            // 
            // grpTags
            // 
            grpTags.Controls.Add(dgvTags);
            grpTags.Controls.Add(panelFilter);
            grpTags.Controls.Add(toolStripTags);
            grpTags.Dock = DockStyle.Fill;
            grpTags.Location = new Point(0, 60);
            grpTags.Name = "grpTags";
            grpTags.Padding = new Padding(10);
            grpTags.Size = new Size(1232, 601);
            grpTags.TabIndex = 1;
            grpTags.TabStop = false;
            grpTags.Text = "点位列表配置";
            // 
            // dgvTags
            // 
            dgvTags.AllowUserToAddRows = false;
            dgvTags.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvTags.BackgroundColor = Color.White;
            dgvTags.BorderStyle = BorderStyle.None;
            dgvTags.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvTags.Columns.AddRange(new DataGridViewColumn[] { colTagName, colZone, colAddr, colDataType, colBitIndex, colNote });
            dgvTags.ContextMenuStrip = ctxMenuBatch;
            dgvTags.Dock = DockStyle.Fill;
            dgvTags.EditMode = DataGridViewEditMode.EditOnEnter;
            dgvTags.Location = new Point(10, 91);
            dgvTags.Name = "dgvTags";
            dgvTags.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvTags.Size = new Size(1212, 500);
            dgvTags.TabIndex = 2;
            // 
            // colTagName
            // 
            colTagName.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            colTagName.FillWeight = 40F;
            colTagName.HeaderText = "变量名称";
            colTagName.Name = "colTagName";
            // 
            // colZone
            // 
            colZone.HeaderText = "存储区";
            colZone.Name = "colZone";
            // 
            // colAddr
            // 
            colAddr.HeaderText = "地址";
            colAddr.Name = "colAddr";
            // 
            // colDataType
            // 
            colDataType.HeaderText = "类型";
            colDataType.Name = "colDataType";
            // 
            // colBitIndex
            // 
            colBitIndex.HeaderText = "位";
            colBitIndex.Name = "colBitIndex";
            // 
            // colNote
            // 
            colNote.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            colNote.FillWeight = 30F;
            colNote.HeaderText = "备注说明";
            colNote.Name = "colNote";
            // 
            // ctxMenuBatch
            // 
            ctxMenuBatch.Items.AddRange(new ToolStripItem[] { menuBatchOffset, menuBatchIncrement, menuSep1, menuBatchZone, menuBatchDataType, menuBatchFormat, menuBatchScale, menuSep2, menuBatchPrefix, menuBatchCopy, menuIncreaseTag });
            ctxMenuBatch.Name = "ctxMenuBatch";
            ctxMenuBatch.Size = new Size(276, 214);
            // 
            // menuBatchOffset
            // 
            menuBatchOffset.Name = "menuBatchOffset";
            menuBatchOffset.Size = new Size(275, 22);
            menuBatchOffset.Text = "🔢 批量地址平移 (Offset)...";
            // 
            // menuBatchIncrement
            // 
            menuBatchIncrement.Name = "menuBatchIncrement";
            menuBatchIncrement.Size = new Size(275, 22);
            menuBatchIncrement.Text = "📈 地址递增填充 (Auto-Fill)...";
            // 
            // menuSep1
            // 
            menuSep1.Name = "menuSep1";
            menuSep1.Size = new Size(272, 6);
            // 
            // menuBatchZone
            // 
            menuBatchZone.Name = "menuBatchZone";
            menuBatchZone.Size = new Size(275, 22);
            menuBatchZone.Text = "🔄 批量修改存储区...";
            // 
            // menuBatchDataType
            // 
            menuBatchDataType.Name = "menuBatchDataType";
            menuBatchDataType.Size = new Size(275, 22);
            menuBatchDataType.Text = "📝 批量修改数据类型...";
            // 
            // menuBatchFormat
            // 
            menuBatchFormat.Name = "menuBatchFormat";
            menuBatchFormat.Size = new Size(275, 22);
            menuBatchFormat.Text = "🔀 批量修改字节序...";
            // 
            // menuBatchScale
            // 
            menuBatchScale.Name = "menuBatchScale";
            menuBatchScale.Size = new Size(275, 22);
            menuBatchScale.Text = "⚖️ 批量设置系数/偏移...";
            // 
            // menuSep2
            // 
            menuSep2.Name = "menuSep2";
            menuSep2.Size = new Size(272, 6);
            // 
            // menuBatchPrefix
            // 
            menuBatchPrefix.Name = "menuBatchPrefix";
            menuBatchPrefix.Size = new Size(275, 22);
            menuBatchPrefix.Text = "🏷️ 批量添加前缀/后缀...";
            // 
            // menuBatchCopy
            // 
            menuBatchCopy.Name = "menuBatchCopy";
            menuBatchCopy.Size = new Size(275, 22);
            menuBatchCopy.Text = "📋 批量复制属性(向下填充)...";
            // 
            // menuIncreaseTag
            // 
            menuIncreaseTag.Name = "menuIncreaseTag";
            menuIncreaseTag.Size = new Size(275, 22);
            menuIncreaseTag.Text = "🔢 变量名递增(Temp1 -> Temp2)...";
            // 
            // panelFilter
            // 
            panelFilter.BackColor = Color.WhiteSmoke;
            panelFilter.Controls.Add(btnClearFilter);
            panelFilter.Controls.Add(cmbFilterZone);
            panelFilter.Controls.Add(cmbFilterType);
            panelFilter.Controls.Add(label1);
            panelFilter.Controls.Add(lblFilterType);
            panelFilter.Controls.Add(txtSearch);
            panelFilter.Controls.Add(lblSearch);
            panelFilter.Dock = DockStyle.Top;
            panelFilter.Location = new Point(10, 51);
            panelFilter.Name = "panelFilter";
            panelFilter.Padding = new Padding(5);
            panelFilter.Size = new Size(1212, 40);
            panelFilter.TabIndex = 3;
            // 
            // btnClearFilter
            // 
            btnClearFilter.Cursor = Cursors.Hand;
            btnClearFilter.FlatAppearance.BorderSize = 0;
            btnClearFilter.FlatStyle = FlatStyle.Flat;
            btnClearFilter.Location = new Point(589, 8);
            btnClearFilter.Name = "btnClearFilter";
            btnClearFilter.Size = new Size(60, 25);
            btnClearFilter.TabIndex = 0;
            btnClearFilter.Text = "❌ 重置";
            btnClearFilter.UseVisualStyleBackColor = true;
            // 
            // cmbFilterZone
            // 
            cmbFilterZone.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbFilterZone.Location = new Point(277, 9);
            cmbFilterZone.Name = "cmbFilterZone";
            cmbFilterZone.Size = new Size(130, 25);
            cmbFilterZone.TabIndex = 1;
            // 
            // cmbFilterType
            // 
            cmbFilterType.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbFilterType.Location = new Point(467, 9);
            cmbFilterType.Name = "cmbFilterType";
            cmbFilterType.Size = new Size(100, 25);
            cmbFilterType.TabIndex = 1;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(224, 12);
            label1.Name = "label1";
            label1.Size = new Size(47, 17);
            label1.TabIndex = 2;
            label1.Text = "存储区:";
            // 
            // lblFilterType
            // 
            lblFilterType.AutoSize = true;
            lblFilterType.Location = new Point(428, 13);
            lblFilterType.Name = "lblFilterType";
            lblFilterType.Size = new Size(35, 17);
            lblFilterType.TabIndex = 2;
            lblFilterType.Text = "类型:";
            // 
            // txtSearch
            // 
            txtSearch.Location = new Point(65, 9);
            txtSearch.Name = "txtSearch";
            txtSearch.PlaceholderText = "名称/地址/备注";
            txtSearch.Size = new Size(150, 23);
            txtSearch.TabIndex = 3;
            // 
            // lblSearch
            // 
            lblSearch.AutoSize = true;
            lblSearch.Location = new Point(10, 12);
            lblSearch.Name = "lblSearch";
            lblSearch.Size = new Size(55, 17);
            lblSearch.TabIndex = 4;
            lblSearch.Text = "🔍 搜索:";
            // 
            // toolStripTags
            // 
            toolStripTags.Items.AddRange(new ToolStripItem[] { btnAddTag, btnInsertTag, btnDelTag, sep3, btnHexDec });
            toolStripTags.Location = new Point(10, 26);
            toolStripTags.Name = "toolStripTags";
            toolStripTags.Size = new Size(1212, 25);
            toolStripTags.TabIndex = 0;
            toolStripTags.Text = "toolStrip1";
            // 
            // btnAddTag
            // 
            btnAddTag.ImageTransparentColor = Color.Magenta;
            btnAddTag.Name = "btnAddTag";
            btnAddTag.Size = new Size(80, 22);
            btnAddTag.Text = "➕ 新增变量";
            // 
            // btnInsertTag
            // 
            btnInsertTag.ImageTransparentColor = Color.Magenta;
            btnInsertTag.Name = "btnInsertTag";
            btnInsertTag.Size = new Size(80, 22);
            btnInsertTag.Text = "📥 插入变量";
            // 
            // btnDelTag
            // 
            btnDelTag.ImageTransparentColor = Color.Magenta;
            btnDelTag.Name = "btnDelTag";
            btnDelTag.Size = new Size(80, 22);
            btnDelTag.Text = "➖ 删除选中";
            // 
            // sep3
            // 
            sep3.Name = "sep3";
            sep3.Size = new Size(6, 25);
            // 
            // btnHexDec
            // 
            btnHexDec.Alignment = ToolStripItemAlignment.Right;
            btnHexDec.ImageTransparentColor = Color.Magenta;
            btnHexDec.Name = "btnHexDec";
            btnHexDec.Size = new Size(81, 22);
            btnHexDec.Text = "🔢 Hex/Dec";
            btnHexDec.ToolTipText = "切换地址显示格式";
            // 
            // F_ModbusAddrManager
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1232, 661);
            Controls.Add(grpTags);
            Controls.Add(panelHeader);
            Name = "F_ModbusAddrManager";
            StartPosition = FormStartPosition.CenterParent;
            Text = "设备地址表编辑器";
            panelHeader.ResumeLayout(false);
            panelHeader.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numSlaveId).EndInit();
            grpTags.ResumeLayout(false);
            grpTags.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dgvTags).EndInit();
            ctxMenuBatch.ResumeLayout(false);
            panelFilter.ResumeLayout(false);
            panelFilter.PerformLayout();
            toolStripTags.ResumeLayout(false);
            toolStripTags.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        // 成员变量
        private System.Windows.Forms.Panel panelHeader;

        // Header Controls
        private System.Windows.Forms.Label lblDeviceName;
        private System.Windows.Forms.TextBox txtDeviceName;
        private System.Windows.Forms.Label lblSlaveId;
        private System.Windows.Forms.NumericUpDown numSlaveId;

        // Header Buttons (Type changed to Button)
        private System.Windows.Forms.Button btnConfirm;
        private System.Windows.Forms.Button btnExport;
        private System.Windows.Forms.Button btnImport;
        private System.Windows.Forms.Button btnImportExcel;

        private System.Windows.Forms.GroupBox grpTags;
        private System.Windows.Forms.DataGridView dgvTags;

        // ToolStrip
        private System.Windows.Forms.ToolStrip toolStripTags;
        private System.Windows.Forms.ToolStripButton btnAddTag;
        private System.Windows.Forms.ToolStripButton btnInsertTag;
        private System.Windows.Forms.ToolStripButton btnDelTag;
        private System.Windows.Forms.ToolStripSeparator sep3;
        private System.Windows.Forms.ToolStripButton btnHexDec;

        // Columns
        private System.Windows.Forms.DataGridViewTextBoxColumn colTagName;
        private System.Windows.Forms.DataGridViewComboBoxColumn colZone;
        private System.Windows.Forms.DataGridViewTextBoxColumn colAddr;
        private System.Windows.Forms.DataGridViewComboBoxColumn colDataType;
        private System.Windows.Forms.DataGridViewTextBoxColumn colBitIndex;
        private System.Windows.Forms.DataGridViewTextBoxColumn colNote;
        private DataGridViewTextBoxColumn colUnit;
        private DataGridViewComboBoxColumn colDataFormat;
        private DataGridViewTextBoxColumn colFactor;
        private DataGridViewTextBoxColumn colOffset;

        // Filter
        private System.Windows.Forms.Panel panelFilter;
        private System.Windows.Forms.Label lblSearch;
        private System.Windows.Forms.TextBox txtSearch;
        private System.Windows.Forms.Label lblFilterType;
        private System.Windows.Forms.ComboBox cmbFilterType;
        private System.Windows.Forms.Button btnClearFilter;

        // Context Menu
        private System.Windows.Forms.ContextMenuStrip ctxMenuBatch;
        private System.Windows.Forms.ToolStripMenuItem menuBatchOffset;
        private System.Windows.Forms.ToolStripMenuItem menuBatchIncrement;
        private System.Windows.Forms.ToolStripSeparator menuSep1;
        private System.Windows.Forms.ToolStripMenuItem menuBatchZone;
        private System.Windows.Forms.ToolStripMenuItem menuBatchDataType;
        private System.Windows.Forms.ToolStripMenuItem menuBatchFormat;
        private System.Windows.Forms.ToolStripMenuItem menuBatchScale;
        private System.Windows.Forms.ToolStripSeparator menuSep2;
        private System.Windows.Forms.ToolStripMenuItem menuBatchPrefix;
        private System.Windows.Forms.ToolStripMenuItem menuBatchCopy;
        private System.Windows.Forms.ToolStripMenuItem menuIncreaseTag;
        private ComboBox cmbFilterZone;
        private Label label1;
    }
}