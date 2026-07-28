using MiniExcelLibs;
using ModbusPilot.Core;
using ModbusPilot.Core.Driver;   // 引用 Driver 以使用 Protocol 帮助类
using ModbusPilot.Core.Models;
using ModbusPilot.Core.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace ModbusPilot.UI.Common
{
    public partial class F_ModbusAddrManager : F_BaseForm
    {
        public DeviceConfig CurrentDevice { get; private set; }

        // 是否以 16 进制显示地址
        private bool _showHexAddress = false;
        
        // 是否显示PLC地址格式（默认开启）
        private bool _showPlcAddress = true;

        // 在类成员定义处添加
        public List<byte> ForbiddenSlaveIds { get; set; } = new List<byte>();


        public F_ModbusAddrManager(DeviceConfig editDevice = null)
        {
            InitializeComponent();

            this.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
            InitializeAdvancedColumns();


            CurrentDevice = editDevice ?? new DeviceConfig();

            InitComboBoxDataSources();

            BindEvents();

            StyleImportButton();

            ApplyUIText();

            LoadFromObjectToUI(CurrentDevice);

            // 遍历所有列，将排序模式设为 NotSortable
            foreach (DataGridViewColumn col in dgvTags.Columns)
            {
                col.SortMode = DataGridViewColumnSortMode.NotSortable;
            }
        }
        // ==========================================
        // 【核心】实现界面文字替换
        // ==========================================
        private void ApplyUIText()
        {
            // 1. 窗口与头部
            this.Text = LangProvider.Get("Addr_Title");
            lblDeviceName.Text = LangProvider.Get("Addr_Lbl_DevName");
            lblSlaveId.Text = LangProvider.Get("Addr_Lbl_SlaveId");

            // 2. 工具栏
            btnConfirm.Text = LangProvider.Get("Addr_Btn_Confirm");
            btnExport.Text = LangProvider.Get("Addr_Btn_Export");
            btnImport.Text = LangProvider.Get("Addr_Btn_Import");

            // 按钮文本根据当前状态设置
            if (_showPlcAddress)
                btnHexDec.Text = "🏭 PLC";
            else if (_showHexAddress)
                btnHexDec.Text = "🔢 Hex";
            else
                btnHexDec.Text = "🔢 Dec";

            btnAddTag.Text = LangProvider.Get("Addr_Btn_Add");
            btnInsertTag.Text = LangProvider.Get("Addr_Btn_Insert");
            btnDelTag.Text = LangProvider.Get("Addr_Btn_Del");

            // 3. 分组框
            grpTags.Text = LangProvider.Get("Addr_Grp_Tags");

            // 4. 左侧表格列名
            colTagName.HeaderText = LangProvider.Get("Addr_Col_Name");
            colUnit.HeaderText = LangProvider.Get("Addr_Col_Unit");
            colZone.HeaderText = LangProvider.Get("Addr_Col_Zone");
            colAddr.HeaderText = LangProvider.Get("Addr_Col_Addr");
            colDataType.HeaderText = LangProvider.Get("Addr_Col_Type");
            colBitIndex.HeaderText = LangProvider.Get("Addr_Col_Bit");
            colDataFormat.HeaderText = LangProvider.Get("Addr_Col_Format");
            colFactor.HeaderText = LangProvider.Get("Addr_Col_Factor");
            colOffset.HeaderText = LangProvider.Get("Addr_Col_Offset");
            colNote.HeaderText = LangProvider.Get("Addr_Col_Note");


            // 7. 【关键】重新绑定下拉框数据源 (以更新翻译)
            InitComboBoxDataSources();
        }
        private void InitComboBoxDataSources()
        {
            // 1. 存储区下拉框
            var zoneSource = new List<EnumItem>
            {
                new EnumItem { Text = LangProvider.Get("Zone_0x"), Value = StorageZone.CoilStatus_0x },
                new EnumItem { Text = LangProvider.Get("Zone_1x"), Value = StorageZone.InputStatus_1x },
                new EnumItem { Text = LangProvider.Get("Zone_3x"), Value = StorageZone.InputRegister_3x },
                new EnumItem { Text = LangProvider.Get("Zone_4x"), Value = StorageZone.HoldingRegister_4x }
            };
            colZone.DataSource = zoneSource;
            colZone.DisplayMember = "Text";
            colZone.ValueMember = "Value";

            colZone.Width = 180; // 之前是 130，可能有点挤

            // 2. 数据类型下拉框 (【修改这里】改为 List<EnumItem> 绑定)
            // 这样能消除 DataError 导致的自动回滚问题
            var typeSource = new List<EnumItem>();
            foreach (DataType t in Enum.GetValues(typeof(DataType)))
            {
                typeSource.Add(new EnumItem { Text = t.ToString(), Value = t });
            }
            colDataType.DataSource = typeSource;
            colDataType.DisplayMember = "Text";
            colDataType.ValueMember = "Value";

            // 3. 字节序下拉框绑定
            var formatSource = new List<EnumItem>
            {
                new EnumItem { Text = LangProvider.Get("Fmt_ABCD"), Value = DataFormat.ABCD },
                new EnumItem { Text = LangProvider.Get("Fmt_CDAB"), Value = DataFormat.CDAB },
                new EnumItem { Text = LangProvider.Get("Fmt_BADC"), Value = DataFormat.BADC },
                new EnumItem { Text = LangProvider.Get("Fmt_DCBA"), Value = DataFormat.DCBA }
            };
            colDataFormat.DataSource = formatSource;
            colDataFormat.DisplayMember = "Text";
            colDataFormat.ValueMember = "Value";

            // 初始化筛选器的存储区下拉框
            var filterZoneSource = new List<EnumItem> { new EnumItem { Text = "全部 (All)", Value = -1 } };
            filterZoneSource.Add(new EnumItem { Text = LangProvider.Get("Zone_0x"), Value = StorageZone.CoilStatus_0x });
            filterZoneSource.Add(new EnumItem { Text = LangProvider.Get("Zone_1x"), Value = StorageZone.InputStatus_1x });
            filterZoneSource.Add(new EnumItem { Text = LangProvider.Get("Zone_3x"), Value = StorageZone.InputRegister_3x });
            filterZoneSource.Add(new EnumItem { Text = LangProvider.Get("Zone_4x"), Value = StorageZone.HoldingRegister_4x });
            cmbFilterZone.DisplayMember = "Text";
            cmbFilterZone.ValueMember = "Value";
            cmbFilterZone.DataSource = filterZoneSource;

            // 初始化筛选器的类型下拉框
            var filterTypeSource = new List<EnumItem> { new EnumItem { Text = "全部 (All)", Value = -1 } };
            foreach (DataType t in Enum.GetValues(typeof(DataType)))
            {
                filterTypeSource.Add(new EnumItem { Text = t.ToString(), Value = t });
            }
            cmbFilterType.DisplayMember = "Text";
            cmbFilterType.ValueMember = "Value";
            cmbFilterType.DataSource = filterTypeSource;
           
        }

        private void InitializeAdvancedColumns()
        {
            // =========================================================================
            // 1. 定义新列 (单位、字节序、系数、偏移)
            // =========================================================================

            // [单位]：放在第二列，宽度调窄，固定不伸缩
            colUnit = new DataGridViewTextBoxColumn
            {
                Name = "colUnit",
                HeaderText = "单位",
                Width = 45, // 足够放下 "MPa", "kg" 即可
                AutoSizeMode = DataGridViewAutoSizeColumnMode.None // 关键：固定宽度
            };

            // [字节序]：默认显示，宽度较宽以显示文本
            colDataFormat = new DataGridViewComboBoxColumn
            {
                Name = "colDataFormat",
                HeaderText = "字节序",
                Width = 110, // 足够放下 "ABCD (BigEndian)"
                Visible = true,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.None // 关键：固定宽度
            };

            // [系数]：默认显示，数字列窄一点
            colFactor = new DataGridViewTextBoxColumn
            {
                Name = "colFactor",
                HeaderText = "系数",
                Width = 55,
                Visible = true,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.None // 关键：固定宽度
            };

            // [偏移]：默认显示
            colOffset = new DataGridViewTextBoxColumn
            {
                Name = "colOffset",
                HeaderText = "偏移",
                Width = 55,
                Visible = true,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.None // 关键：固定宽度
            };

            // =========================================================================
            // 2. 重新编排所有列的顺序 (Name -> Unit -> Zone -> Addr...)
            // =========================================================================

            // 为了保证顺序绝对正确，先清空所有列，再按顺序加回来
            dgvTags.Columns.Clear();

            dgvTags.Columns.AddRange(new DataGridViewColumn[] {
        colTagName,     // 1. 变量名称
        colUnit,        // 2. 单位 (新位置)
        colZone,        // 3. 存储区
        colAddr,        // 4. 地址
        colDataType,    // 5. 类型
        colBitIndex,    // 6. 位
        colDataFormat,  // 7. 字节序 (隐藏)
        colFactor,      // 8. 系数 (隐藏)
        colOffset,      // 9. 偏移 (隐藏)
        colNote         // 10. 备注
    });

            // =========================================================================
            // 3. 调整原有标准列的宽度模式 (防止挤压)
            // =========================================================================

            // 自动填充列 (随窗口变宽)
            colTagName.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            colTagName.FillWeight = 20;

            colZone.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            colZone.FillWeight = 35; // 存储区文字长，权重给高点

            colNote.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            colNote.FillWeight = 25;

            // 固定宽度列 (保持不变)
            colAddr.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            colAddr.Width = 65;

            colDataType.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            colDataType.Width = 85;

            colBitIndex.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            colBitIndex.Width = 40;

           
        }

        private void StyleImportButton()
        {
            // 鼠标悬停变色效果需要事件绑定，建议保留这部分
            btnImportExcel.MouseEnter += (s, e) => btnImportExcel.BackColor = Color.FromArgb(43, 135, 80);
            btnImportExcel.MouseLeave += (s, e) => btnImportExcel.BackColor = Color.FromArgb(33, 115, 70);
            btnImportExcel.Click += btnImportExcel_Click;

            // btnConfirm 也可以加点特效
            btnConfirm.MouseEnter += (s, e) => btnConfirm.BackColor = Color.FromArgb(20, 140, 235);
            btnConfirm.MouseLeave += (s, e) => btnConfirm.BackColor = Color.FromArgb(0, 120, 215);
        }
        private void BindEvents()
        {
            // 工具栏事件
            btnConfirm.Click += (s, e) => ConfirmAndClose();
            btnExport.Click += (s, e) => ExportToFile();
            btnImport.Click += (s, e) => ImportFromFile();

            // 地址格式切换
            btnHexDec.Click += (s, e) => ToggleAddressFormat();

            // 增删改
            btnAddTag.Click += (s, e) => AddNewTagRow();
            btnInsertTag.Click += (s, e) => InsertNewTagRow();
            btnDelTag.Click += (s, e) => DeleteTagRows(); // 批量删除

            // 表格交互
            dgvTags.CurrentCellDirtyStateChanged += (s, e) => { if (dgvTags.IsCurrentCellDirty) dgvTags.CommitEdit(DataGridViewDataErrorContexts.Commit); };
       

            // 点击展开下拉框
            dgvTags.CellClick += (s, e) =>
            {
                if (e.RowIndex >= 0 && e.ColumnIndex >= 0 && dgvTags.Columns[e.ColumnIndex] is DataGridViewComboBoxColumn)
                {
                    dgvTags.BeginEdit(true);
                    if (dgvTags.EditingControl is ComboBox combo) combo.DroppedDown = true;
                }
            };

            dgvTags.CellValueChanged += DgvTags_CellValueChanged;
            dgvTags.CellEnter += DgvTags_CellEnter; // 用于处理只读单元格的视觉反馈

            // 绘制行号
            dgvTags.RowPostPaint += DgvTags_RowPostPaint;

            // 【必须加上】屏蔽 DataError，防止类型转换失败导致的自动回滚
            dgvTags.DataError += (s, e) =>
            {
                // 调试时可以把 e.Exception 打印出来看，但在生产中直接 Cancel 掉
                // e.Cancel = true 告诉 Grid "我已经处理了这个错误，不要回滚，也不要弹窗"
                // 但为了防止回滚，我们不仅仅要 Cancel，通常保持默认值即可
                e.ThrowException = false;
                e.Cancel = false;
            };

            // --- 筛选事件 ---
            txtSearch.TextChanged += (s, e) => ExecuteFilter();
            cmbFilterType.SelectedIndexChanged += (s, e) => ExecuteFilter();
            cmbFilterZone.SelectedIndexChanged += (s, e) => ExecuteFilter();
            btnClearFilter.Click += (s, e) =>
            {
                txtSearch.Text = "";
                cmbFilterType.SelectedIndex = 0;
                cmbFilterZone.SelectedIndex = 0;
                ExecuteFilter(); // 恢复全部显示
            };

            // --- 批量操作事件 ---
            menuBatchOffset.Click += MenuBatchOffset_Click;
            menuBatchIncrement.Click += MenuBatchIncrement_Click;
            menuBatchZone.Click += MenuBatchZone_Click;
            menuBatchDataType.Click += MenuBatchDataType_Click;
            menuBatchFormat.Click += MenuBatchFormat_Click;
            menuBatchScale.Click += MenuBatchScale_Click;
            menuBatchPrefix.Click += MenuBatchPrefix_Click;
            menuBatchCopy.Click += MenuBatchCopy_Click;
            menuIncreaseTag.Click += MenuBatchNameSeries_Click;

            // 拦截左上角“全选”按钮的点击
            dgvTags.CellMouseDown += (s, e) =>
            {
                // ColumnIndex == -1 且 RowIndex == -1 表示点击的是左上角那个全选格子
                if (e.ColumnIndex == -1 && e.RowIndex == -1)
                {
                    // 1. 取消默认的选择行为（非常重要，防止系统自动执行全选）
                    dgvTags.ClearSelection();

                    // 2. 挂起界面刷新，防止大量行选中时闪烁
                    dgvTags.SuspendLayout();

                    // 3. 循环遍历，只选中可见的行
                    foreach (DataGridViewRow row in dgvTags.Rows)
                    {
                        if (row.IsNewRow) continue;

                        // 核心逻辑：只让 Visible 为 true 的行变为选中状态
                        if (row.Visible)
                        {
                            row.Selected = true;
                        }
                    }

                    dgvTags.ResumeLayout();

                    // 4. 强制返回，不再触发系统的默认 SelectAll
                    // 注意：在 CellMouseDown 中我们无法直接 Return 来阻止系统行为
                    // 所以我们通常结合 CellClick 或使用一个标志位。
                    // 但最简单稳妥的办法是：将 MultiSelect 模式下的默认全选逻辑重写。
                }
            };
        }

        private void DgvTags_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            // 【新增】PLC模式下，地址改变时自动更新存储区
            if (_showPlcAddress && e.ColumnIndex == colAddr.Index)
            {
                dgvTags.CellValueChanged -= DgvTags_CellValueChanged;
                try
                {
                    var row = dgvTags.Rows[e.RowIndex];
                    string addrStr = row.Cells[colAddr.Index].Value?.ToString();
                    if (!string.IsNullOrEmpty(addrStr) && int.TryParse(addrStr, out int plcAddr))
                    {
                        // 根据PLC地址自动判断存储区
                        StorageZone newZone = StorageZone.HoldingRegister_4x;
                        if (plcAddr >= 40001) newZone = StorageZone.HoldingRegister_4x;
                        else if (plcAddr >= 30001) newZone = StorageZone.InputRegister_3x;
                        else if (plcAddr >= 10001) newZone = StorageZone.InputStatus_1x;
                        else if (plcAddr >= 1) newZone = StorageZone.CoilStatus_0x;
                        
                        row.Cells[colZone.Index].Value = newZone;
                        UpdateRowState(row);
                    }
                }
                finally
                {
                    dgvTags.CellValueChanged += DgvTags_CellValueChanged;
                }
            }

            if (e.ColumnIndex == colZone.Index || e.ColumnIndex == colDataType.Index)
            {
                // 【关键】在这里解绑，防止 UpdateRowState 修改 Value 时死循环
                dgvTags.CellValueChanged -= DgvTags_CellValueChanged;

                try
                {
                    UpdateRowState(dgvTags.Rows[e.RowIndex]);
                }
                finally
                {
                    // 确保一定会绑回来
                    dgvTags.CellValueChanged += DgvTags_CellValueChanged;
                }
            }

            // 假设第 2 列是 DataType，第 5 列是 Endian
            if (e.ColumnIndex == colDataType.Index && e.RowIndex >= 0)
            {
                var row = dgvTags.Rows[e.RowIndex];
                var newType = row.Cells[colDataType.Index].Value.ToString();

                if (newType == "Float" || newType == "Int32")
                {
                    // 自动补全默认字节序
                    row.Cells[colDataFormat.Index].Value = DataFormat.CDAB;
                    row.Cells[colDataFormat.Index].ReadOnly = false;
                }
            }
        }

        /// <summary>
        /// 纯净版：只负责检查数据和修改样式，不负责事件管理
        /// </summary>
        private void UpdateRowState(DataGridViewRow row)
        {
            if (row == null) return;

            // --- 1. 获取 Zone 和 Type ---
            StorageZone zone = StorageZone.HoldingRegister_4x;
            if (row.Cells[colZone.Index].Value is StorageZone z) zone = z;
            else if (row.Cells[colZone.Index].Value is string s && Enum.TryParse(s, out StorageZone zp)) zone = zp;

            DataType type = DataType.Int16;
            if (row.Cells[colDataType.Index].Value is DataType t) type = t;
            else if (row.Cells[colDataType.Index].Value is string ts && Enum.TryParse(ts, out DataType tp)) type = tp;

            // --- 2. 逻辑判断与样式应用 ---
            var cellType = row.Cells[colDataType.Index];
            var cellBit = row.Cells[colBitIndex.Index];

            // === 场景 A: 线圈 (0x/1x) ===
            if (zone == StorageZone.CoilStatus_0x || zone == StorageZone.InputStatus_1x)
            {
                // 强制修正数据 (注意：在批量加载时，row还没加入Grid，修改Value不会触发Grid事件，安全)
                if (type != DataType.Bool)
                {
                    cellType.Value = DataType.Bool;
                    type = DataType.Bool;
                }

                // 样式锁定
                cellType.ReadOnly = true;
                cellType.Style.BackColor = Color.WhiteSmoke;
                cellType.Style.ForeColor = Color.Gray;

                cellBit.Value = "";
                cellBit.ReadOnly = true;
                cellBit.Style.BackColor = Color.WhiteSmoke;
            }
            // === 场景 B: 寄存器 (3x/4x) ===
            else
            {
                cellType.ReadOnly = false;
                cellType.Style.BackColor = Color.White;
                cellType.Style.ForeColor = Color.Black;

                if (type == DataType.Bool)
                {
                    cellBit.ReadOnly = false;
                    cellBit.Style.BackColor = Color.LightYellow;
                    cellBit.Style.ForeColor = Color.Black;
                    if (string.IsNullOrEmpty(cellBit.Value?.ToString())) cellBit.Value = "0";
                }
                else
                {
                    cellBit.Value = "";
                    cellBit.ReadOnly = true;
                    cellBit.Style.BackColor = Color.WhiteSmoke;
                }
            }
        }

        // 辅助：处理只读单元格的焦点问题（可选，防止用户点进去以为能写）
        private void DgvTags_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvTags.CurrentCell.ReadOnly)
            {
                // 可以在这里把焦点移走，或者不做处理只靠 BackColor 提示
            }
        }

        private void ConfirmAndClose()
        {
            // 1. ID 查重 (硬错误)
            byte newId = (byte)numSlaveId.Value;
            if (newId != CurrentDevice.SlaveId && ForbiddenSlaveIds.Contains(newId))
            {
                MessageBox.Show($"从站地址 (ID:{newId}) 已被使用，请更换。", "ID 冲突", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // 2. 执行检查 (获取错误和警告)
            // ValidateData 返回 false 代表有【硬错误】，必须阻断
            if (!ValidateData(out string errorReport, out string warningReport))
            {
                MessageBox.Show(errorReport, "配置错误 (无法保存)", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // 3. 处理警告 (软错误)
            // 如果有警告信息，弹窗询问用户是否忽略
            if (!string.IsNullOrEmpty(warningReport))
            {
                string msg = "检测到以下非关键性问题：\r\n\r\n" +
                             warningReport +
                             "\r\n\r\n是否忽略这些警告并强制保存？";

                DialogResult dr = MessageBox.Show(msg, "存在警告", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2);

                if (dr == DialogResult.No)
                {
                    return; // 用户选择回去修改
                }
                // 用户选择 Yes -> 继续向下执行保存
            }

            // 4. 保存
            SaveFromUIToObject();
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        #region 行号显示

        private void DgvTags_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            var grid = sender as DataGridView;
            var rowIdx = (e.RowIndex + 1).ToString();

            var centerFormat = new StringFormat()
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            };

            var headerBounds = new Rectangle(e.RowBounds.Left, e.RowBounds.Top, grid.RowHeadersWidth, e.RowBounds.Height);
            e.Graphics.DrawString(rowIdx, this.Font, SystemBrushes.ControlText, headerBounds, centerFormat);
        }

        #endregion

        #region 地址格式切换 (Hex/Dec/PLC)

        private void ToggleAddressFormat()
        {
            // 1. 【关键】先记录当前的模式状态（切换前）
            bool wasPlc = _showPlcAddress;
            bool wasHex = _showHexAddress;

            // 2. 遍历所有行，将当前看到的文字还原为“真值” (协议地址 0-based)
            // 这一步必须用切换前的 wasPlc/wasHex 参数来解析
            var currentValues = new Dictionary<int, int>(); // RowIndex -> ProtocolAddr

            foreach (DataGridViewRow row in dgvTags.Rows)
            {
                if (row.IsNewRow) continue;

                string text = row.Cells[colAddr.Index].Value?.ToString();
                StorageZone zone = (StorageZone)row.Cells[colZone.Index].Value;

                // 使用【旧模式】进行解析
                int protoAddr = ParseAddressStrict(text, zone, wasPlc, wasHex);
                currentValues[row.Index] = protoAddr;
            }

            // 3. 【状态切换】计算下一状态
            // 顺序：Dec -> Hex -> PLC -> Dec
            if (!wasHex && !wasPlc) // Dec
            {
                _showHexAddress = true;
                _showPlcAddress = false;
                btnHexDec.Text = "🔢 Hex";
            }
            else if (wasHex && !wasPlc) // Hex
            {
                _showHexAddress = false;
                _showPlcAddress = true;
                btnHexDec.Text = "🏭 PLC";
            }
            else // PLC
            {
                _showHexAddress = false;
                _showPlcAddress = false;
                btnHexDec.Text = "🔢 Dec";
            }

            // 4. 使用【新模式】将“真值”格式化回界面
            dgvTags.SuspendLayout();
            foreach (var kvp in currentValues)
            {
                var row = dgvTags.Rows[kvp.Key];
                StorageZone zone = (StorageZone)row.Cells[colZone.Index].Value;

                // 格式化
                row.Cells[colAddr.Index].Value = FormatAddress(kvp.Value, zone);

                // 更新只读/变灰样式
                if (colZone.Index >= 0)
                {
                    row.Cells[colZone.Index].ReadOnly = _showPlcAddress;
                    row.Cells[colZone.Index].Style.BackColor = _showPlcAddress ? Color.WhiteSmoke : Color.White;
                }
            }
            dgvTags.ResumeLayout();
        }
        /// <summary>
        /// 严格解析器：明确告知当前是什么模式，绝不瞎猜
        /// </summary>
        private int ParseAddressStrict(string input, StorageZone zone, bool isPlcMode, bool isHexMode)
        {
            if (string.IsNullOrWhiteSpace(input)) return 0;
            input = input.Trim().ToUpper();

            // 1. 如果当前是 Hex 模式
            if (isHexMode)
            {
                try
                {
                    if (input.StartsWith("0X")) return Convert.ToInt32(input.Substring(2), 16);
                    if (input.EndsWith("H")) return Convert.ToInt32(input.TrimEnd('H'), 16);
                    return Convert.ToInt32(input, 16); // 纯 Hex 字符串
                }
                catch { return 0; }
            }

            // 2. 如果当前是 PLC 模式 (需要减去偏移量)
            if (isPlcMode)
            {
                if (int.TryParse(input, out int plcVal))
                {
                    // PLC地址是 1-based，协议地址是 0-based
                    // 且 PLC 地址通常包含前缀 (40001)，我们需要根据 Zone 去掉前缀

                    switch (zone)
                    {
                        case StorageZone.CoilStatus_0x:
                            // 显示的是 00001 或 1 -> 协议 0
                            if (plcVal >= 1) return plcVal - 1;
                            break;
                        case StorageZone.InputStatus_1x:
                            // 显示的是 10001 -> 协议 0
                            if (plcVal >= 10001) return plcVal - 10001;
                            if (plcVal >= 1) return plcVal - 1; // 兼容没输前缀的情况
                            break;
                        case StorageZone.InputRegister_3x:
                            // 显示的是 30001 -> 协议 0
                            if (plcVal >= 30001) return plcVal - 30001;
                            break;
                        case StorageZone.HoldingRegister_4x:
                            // 显示的是 40001 -> 协议 0
                            if (plcVal >= 40001) return plcVal - 40001;
                            break;
                    }
                    // 如果没命中上面的范围，说明数据有问题，但为了防炸，返回 raw - 1
                    return Math.Max(0, plcVal - 1);
                }
                return 0;
            }

            // 3. 如果当前是 Dec (协议原值) 模式
            if (int.TryParse(input, out int rawVal))
            {
                return rawVal;
            }

            return 0;
        }
        private void RefreshAddressDisplay()
        {
            dgvTags.SuspendLayout();
            
            foreach (DataGridViewRow row in dgvTags.Rows)
            {
                if (row.IsNewRow) continue;
                
                // 获取存储区
                StorageZone zone = StorageZone.HoldingRegister_4x;
                if (row.Cells[colZone.Index].Value is StorageZone z) zone = z;
                
                // 获取协议地址
                string currentVal = row.Cells[colAddr.Index].Value?.ToString();
                int protocolAddr = ParseAddressToProtocol(currentVal, zone);
                
                // 格式化显示
                row.Cells[colAddr.Index].Value = FormatAddress(protocolAddr, zone);
                
                // PLC模式下锁定存储区
                row.Cells[colZone.Index].ReadOnly = _showPlcAddress;
                row.Cells[colZone.Index].Style.BackColor = _showPlcAddress ? Color.WhiteSmoke : Color.White;
            }
            
            dgvTags.ResumeLayout();
        }

        // 解析地址为协议地址
        private int ParseAddressToProtocol(string input, StorageZone zone)
        {
            if (string.IsNullOrWhiteSpace(input)) return 0;
            input = input.Trim().ToUpper();

            try
            {
                int addr = 0;
                
                // 解析数值
                if (input.StartsWith("0X"))
                    addr = Convert.ToInt32(input.Substring(2), 16);
                else if (input.EndsWith("H"))
                    addr = Convert.ToInt32(input.TrimEnd('H'), 16);
                else
                    addr = int.Parse(input);
                
                // 如果是PLC地址格式，转换为协议地址
                if (addr >= 40001) return addr - 40001;  // 4x区
                if (addr >= 30001) return addr - 30001;  // 3x区
                if (addr >= 10001) return addr - 10001;  // 1x区
                if (addr >= 1) return addr - 1;          // 0x区
                
                return addr;
            }
            catch
            {
                return 0;
            }
        }

        // 格式化地址显示
        private string FormatAddress(int protocolAddr, StorageZone zone)
        {
            if (_showHexAddress)
            {
                return $"0x{protocolAddr:X4}";
            }
            else if (_showPlcAddress)
            {
                // PLC 模式：协议地址(0) + 基数(1/10001/...)
                switch (zone)
                {
                    case StorageZone.CoilStatus_0x: return (protocolAddr + 1).ToString("D5"); // 输出 00001 样式更专业，或者 D1 也行
                    case StorageZone.InputStatus_1x: return (protocolAddr + 10001).ToString();
                    case StorageZone.InputRegister_3x: return (protocolAddr + 30001).ToString();
                    case StorageZone.HoldingRegister_4x: return (protocolAddr + 40001).ToString();
                    default: return protocolAddr.ToString();
                }
            }
            else
            {
                // Dec 模式
                return protocolAddr.ToString();
            }
        }

        // 解析地址（兼容旧代码调用）
        private int ParseAddress(string input)
        {
            return ParseAddressToProtocol(input, StorageZone.HoldingRegister_4x);
        }

        #endregion

        #region 数据加载与保存

        private void LoadFromObjectToUI(DeviceConfig dev)
        {
            // 1. 暂时挂起 UI 逻辑，防止闪烁和重绘
            dgvTags.SuspendLayout();

            txtDeviceName.Text = dev.DeviceName;
            numSlaveId.Value = dev.SlaveId;
           
            dgvTags.Rows.Clear();

            // 准备一个列表来暂存 Row 对象
            var rowList = new List<DataGridViewRow>();
            // 获取行模板 (克隆模板比每次 new 快)
            var rowTemplate = dgvTags.RowTemplate;

            foreach (var p in dev.Points)
            {
                // 创建新行 (在内存中，不涉及 UI 渲染)
                DataGridViewRow row = (DataGridViewRow)rowTemplate.Clone();
                row.CreateCells(dgvTags); // 根据列结构创建单元格

                // 赋值 (注意：使用索引比使用列名稍微快一点点，但列名可读性好，这里保持列名)
                row.Cells[colTagName.Index].Value = p.Name;
                row.Cells[colZone.Index].Value = p.Zone;
                row.Cells[colAddr.Index].Value = FormatAddress(p.Address, p.Zone);
                row.Cells[colDataType.Index].Value = p.DataType;

                if (p.BitIndex != null) row.Cells[colBitIndex.Index].Value = p.BitIndex;

                // 新增列
                row.Cells[colUnit.Index].Value = p.Unit;
                row.Cells[colDataFormat.Index].Value = p.DataFormat;
                row.Cells[colFactor.Index].Value = p.Factor;
                row.Cells[colOffset.Index].Value = p.Offset;
                row.Cells[colNote.Index].Value = p.Note;

                // 【关键】UI状态处理 (只读/变灰)
                // 这一步逻辑提取出来，直接操作内存中的 row，依然很快
                UpdateRowState(row);

                rowList.Add(row);
            }

            // 3. 一次性添加到表格 (批量操作，只触发一次布局计算)
            if (rowList.Count > 0)
            {
                dgvTags.Rows.AddRange(rowList.ToArray());
            }

            // 4. 恢复 UI
            dgvTags.ResumeLayout();
        }

        private void SaveFromUIToObject()
        {
            dgvTags.EndEdit();

            CurrentDevice.DeviceName = txtDeviceName.Text;
            CurrentDevice.SlaveId = (byte)numSlaveId.Value;
            CurrentDevice.Points.Clear();

            foreach (DataGridViewRow row in dgvTags.Rows)
            {
                // 地址为空跳过
                if (row.Cells[colAddr.Index].Value == null) continue;

                var p = new ModbusPoint();
                p.Name = row.Cells[colTagName.Index].Value?.ToString();

                // 统一解析地址
                p.Address = ParseAddress(row.Cells[colAddr.Index].Value.ToString());

                if (row.Cells[colZone.Index].Value is StorageZone zone) p.Zone = zone;

                // ==============================================================
                // 【核心修复】根据当前显示模式来解析地址
                // ==============================================================
                string addrStr = row.Cells[colAddr.Index].Value.ToString();

                if (_showPlcAddress)
                {
                    // 如果是 PLC 模式 (40001)，需要解析并减去基数
                    p.Address = ParseAddressToProtocol(addrStr, p.Zone);
                }
                else
                {
                    // 如果是 Hex/Dec 模式，用户填的就是协议地址 (0, 1, 2...)
                    // 直接解析数值，不要减 1
                    p.Address = ParseRawValue(addrStr);
                }
                // ==============================================================



                if (row.Cells[colDataType.Index].Value is DataType dtype) p.DataType = dtype;

                var bitVal = row.Cells[colBitIndex.Index].Value?.ToString();
                if (int.TryParse(bitVal, out int bitIdx)) p.BitIndex = bitIdx;

                p.Note = row.Cells[colNote.Index].Value?.ToString();

                // === 新增列解析 ===
                p.Unit = row.Cells[colUnit.Index].Value?.ToString() ?? "";

                // 解析字节序
                if (row.Cells[colDataFormat.Index].Value is DataFormat df) p.DataFormat = df;
                else p.DataFormat = DataFormat.ABCD; // 默认

                // 解析系数 (默认1)
                if (float.TryParse(row.Cells[colFactor.Index].Value?.ToString(), out float f)) p.Factor = f;
                else p.Factor = 1.0f;

                // 解析偏移 (默认0)
                if (float.TryParse(row.Cells[colOffset.Index].Value?.ToString(), out float o)) p.Offset = o;
                else p.Offset = 0.0f;

                CurrentDevice.Points.Add(p);
            }
        }
        // 仅解析数值 (支持 0x, H 后缀)，不做任何业务偏移计算
        private int ParseRawValue(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return 0;
            input = input.Trim().ToUpper();

            try
            {
                if (input.StartsWith("0X"))
                    return Convert.ToInt32(input.Substring(2), 16);
                if (input.EndsWith("H"))
                    return Convert.ToInt32(input.TrimEnd('H'), 16);

                return int.Parse(input);
            }
            catch
            {
                return 0;
            }
        }
        private void ExportToFile()
        {
            // 1. 确保数据最新
            SaveFromUIToObject();

            // 2. 决定地址列的表头名称
            string addrHeader = "地址(协议)"; // 默认
            if (_showPlcAddress) addrHeader = "地址(PLC)";
            else if (_showHexAddress) addrHeader = "地址(Hex)";

            using (var sfd = new SaveFileDialog { Filter = "Excel 文件 (*.xlsx)|*.xlsx", FileName = $"{CurrentDevice.DeviceName}_点表.xlsx" })
            {
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        // 3. 构建动态数据列表
                        var exportData = new List<Dictionary<string, object>>();

                        foreach (var p in CurrentDevice.Points)
                        {
                            var row = new Dictionary<string, object>();
                            // 按顺序添加列
                            row["变量名称"] = p.Name;
                            row["单位"] = p.Unit;
                            row["存储区"] = p.Zone.ToString();

                            // 【关键】使用动态表头，并根据当前模式格式化地址
                            // 这里利用 FormatAddress 直接获取当前 UI 上看到的那个字符串
                            row[addrHeader] = FormatAddress(p.Address, p.Zone);

                            row["数据类型"] = p.DataType.ToString();
                            row["位索引"] = p.BitIndex?.ToString() ?? "";
                            row["系数"] = p.Factor;
                            row["偏移"] = p.Offset;
                            row["字节序"] = p.DataFormat.ToString();
                            row["备注"] = p.Note;

                            exportData.Add(row);
                        }

                        // 4. 导出
                        MiniExcel.SaveAs(sfd.FileName, exportData, overwriteFile: true);

                        if (MessageBox.Show("导出成功！是否打开文件？", "提示", MessageBoxButtons.YesNo) == DialogResult.Yes)
                        {
                            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(sfd.FileName) { UseShellExecute = true });
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("导出失败: " + ex.Message);
                    }
                }
            }
        }

        private void ImportFromFile()
        {
            using (var ofd = new OpenFileDialog { Filter = "Excel 文件 (*.xlsx)|*.xlsx" })
            {
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        // 1. 【核心修改】强制 useHeaderRow: false
                        // 这样所有列都会变成 A, B, C, D... 第一行就是表头文字
                        var rows = MiniExcel.Query(ofd.FileName, useHeaderRow: false)
                                            .Cast<IDictionary<string, object>>()
                                            .ToList();

                        if (rows.Count < 2) // 至少要有1行表头 + 1行数据
                        {
                            MessageBox.Show("文件中没有数据（或只有表头）。");
                            return;
                        }

                        // 2. 【建立列映射】扫描第一行，找到中文名对应的 A, B, C 列号
                        var headerRow = rows[0]; // 第一行是表头
                        var colMap = new Dictionary<string, string>(); // 键:中文表头(变量名称), 值:列Key(A)

                        foreach (var key in headerRow.Keys)
                        {
                            if (headerRow[key] != null)
                            {
                                string headerText = headerRow[key].ToString().Trim();
                                if (!string.IsNullOrEmpty(headerText))
                                {
                                    colMap[headerText] = key;
                                }
                            }
                        }

                        // 3. 【侦测地址模式】检查 colMap 里包含哪种地址头
                        bool isPlcMode = false;
                        bool isHexMode = false;
                        string targetAddrHeader = "地址(协议)"; // 我们想找的那个表头文字

                        if (colMap.ContainsKey("地址(PLC)"))
                        {
                            targetAddrHeader = "地址(PLC)";
                            isPlcMode = true;
                        }
                        else if (colMap.ContainsKey("地址(Hex)"))
                        {
                            targetAddrHeader = "地址(Hex)";
                            isHexMode = true;
                        }
                        else if (colMap.ContainsKey("地址(协议)"))
                        {
                            targetAddrHeader = "地址(协议)";
                        }
                        else if (colMap.ContainsKey("地址"))
                        {
                            targetAddrHeader = "地址";
                        }
                        else
                        {
                            MessageBox.Show($"未找到有效的地址列！\r\n请检查 Excel 表头是否包含：地址(协议)、地址(PLC) 或 地址(Hex)。", "格式错误");
                            return;
                        }

                        // 检查必要的"变量名称"列是否存在
                        if (!colMap.ContainsKey("变量名称"))
                        {
                            MessageBox.Show("未找到 [变量名称] 列，请检查模板。", "格式错误");
                            return;
                        }

                        // 4. 询问覆盖
                        if (dgvTags.Rows.Count > 1)
                        {
                            var dr = MessageBox.Show("是否清空当前列表？\n[是] 清空并导入\n[否] 追加到末尾\n[取消] 放弃", "导入", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                            if (dr == DialogResult.Cancel) return;
                            if (dr == DialogResult.Yes) CurrentDevice.Points.Clear();
                        }

                        // 5. 【解析数据】从第二行开始遍历 (Skip(1))
                        int successCount = 0;
                        foreach (var row in rows.Skip(1))
                        {
                            // === 内部取值助手 ===
                            // 根据中文表头 -> 查 colMap 得到 Key(A/B/C) -> 去 row 里取值
                            string GetVal(string headerName)
                            {
                                if (colMap.ContainsKey(headerName)) // 列存在
                                {
                                    string colKey = colMap[headerName]; // 拿到 A, B, C...
                                    if (row.ContainsKey(colKey) && row[colKey] != null)
                                    {
                                        return row[colKey].ToString().Trim();
                                    }
                                }
                                return "";
                            }
                            // ===================

                            var p = new ModbusPoint();

                            p.Name = GetVal("变量名称");
                            // 名字和地址都为空，视为无效行
                            if (string.IsNullOrEmpty(p.Name) && string.IsNullOrEmpty(GetVal(targetAddrHeader))) continue;

                            p.Unit = GetVal("单位");

                            // 存储区
                            string zoneStr = GetVal("存储区");
                            if (Enum.TryParse(zoneStr, true, out StorageZone z)) p.Zone = z;
                            else p.Zone = StorageZone.HoldingRegister_4x;

                            // 地址 (核心)
                            string addrVal = GetVal(targetAddrHeader);
                            p.Address = ParseAddressStrict(addrVal, p.Zone, isPlcMode, isHexMode);

                            // 数据类型
                            string typeStr = GetVal("数据类型");
                            if (Enum.TryParse(typeStr, true, out DataType t)) p.DataType = t;
                            else p.DataType = DataType.Int16;

                            // 位索引
                            string bitStr = GetVal("位索引");
                            if (int.TryParse(bitStr, out int bit)) p.BitIndex = bit;
                            else p.BitIndex = null;

                            // 系数/偏移
                            string factorStr = GetVal("系数");
                            if (float.TryParse(factorStr, out float f)) p.Factor = f == 0 ? 1 : f; else p.Factor = 1;

                            string offsetStr = GetVal("偏移");
                            if (float.TryParse(offsetStr, out float o)) p.Offset = o; else p.Offset = 0;

                            // 字节序
                            string fmtStr = GetVal("字节序");
                            if (Enum.TryParse(fmtStr, true, out DataFormat df)) p.DataFormat = df; else p.DataFormat = DataFormat.ABCD;

                            p.Note = GetVal("备注");

                            CurrentDevice.Points.Add(p);
                            successCount++;
                        }

                        // 6. 刷新
                        LoadFromObjectToUI(CurrentDevice);
                        MessageBox.Show($"成功导入 {successCount} 条数据！\r\n识别模式: {targetAddrHeader}", "导入成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"导入异常: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
        private void btnImportExcel_Click(object sender, EventArgs e)
        {
            // 直接打开新向导
            using (var frm = new F_SmartImport())
            {
                // 这里的 OpenFile 逻辑其实可以在 Form_Load 里自动触发，或者让用户点按钮
                // 为了体验好，我们可以在 F_SmartImport 的构造函数里不弹窗，
                // 而是 ShowDialog 后，如果用户还没选文件，让他选。
                // 不过最简单的是：F_SmartImport 一出来是空的，用户点“选择文件”。

                if (frm.ShowDialog() == DialogResult.OK)
                {
                    var newPoints = frm.ResultPoints;
                    if (newPoints.Count > 0)
                    {
                        if (MessageBox.Show($"解析成功！共 {newPoints.Count} 个点位。\r\n是否清空现有列表？", "导入确认", MessageBoxButtons.YesNo) == DialogResult.Yes)
                        {
                            CurrentDevice.Points.Clear();
                        }

                        CurrentDevice.Points.AddRange(newPoints);
                        LoadFromObjectToUI(CurrentDevice);
                        MessageBox.Show("导入完成！");
                    }
                }
            }
        }
        #endregion

        #region 表格操作 (增删改)

        private void AddNewTagRow()
        {
            // 1. 先加空行
            int idx = dgvTags.Rows.Add();
            var row = dgvTags.Rows[idx];

            // 2. 按列名填充 (不受列顺序影响)
            row.Cells[colTagName.Name].Value = LangProvider.Get("Def_NewTag");
            row.Cells[colUnit.Name].Value = "";       // 单位默认空
            row.Cells[colZone.Name].Value = StorageZone.HoldingRegister_4x;
            row.Cells[colAddr.Name].Value = FormatAddress(0, StorageZone.HoldingRegister_4x);
            row.Cells[colDataType.Name].Value = DataType.Int16;
            row.Cells[colBitIndex.Name].Value = "";

            // 隐藏列也要赋初始值
            row.Cells[colDataFormat.Name].Value = DataFormat.ABCD;
            row.Cells[colFactor.Name].Value = "1";
            row.Cells[colOffset.Name].Value = "0";

            row.Cells[colNote.Name].Value = "";

            // 3. 刷新状态
            UpdateRowState(row);
            dgvTags.CurrentCell = row.Cells[0];
        }

        private void InsertNewTagRow()
        {
            // 1. 提交当前编辑，防止干扰
            dgvTags.EndEdit();

            // 2. 确定插入位置 (在当前行上方插入)
            int insertIndex = dgvTags.CurrentRow != null ? dgvTags.CurrentRow.Index : dgvTags.Rows.Count;
            if (insertIndex < 0) insertIndex = 0;

            // 3. 插入空行
            dgvTags.Rows.Insert(insertIndex, 1);
            var row = dgvTags.Rows[insertIndex];

            // 4. 【核心修复】按列名精准赋值，不再依赖参数顺序
            row.Cells[colTagName.Name].Value = LangProvider.Get("Def_InsertTag"); // "新变量_插"
            row.Cells[colUnit.Name].Value = "";
            row.Cells[colZone.Name].Value = StorageZone.HoldingRegister_4x;

            // 计算默认地址
            row.Cells[colAddr.Name].Value = FormatAddress(0, StorageZone.HoldingRegister_4x);

            row.Cells[colDataType.Name].Value = DataType.Int16;
            row.Cells[colBitIndex.Name].Value = "";

            // 隐藏列赋默认值 (防止空指针报错)
            row.Cells[colDataFormat.Name].Value = DataFormat.ABCD;
            row.Cells[colFactor.Name].Value = "1";
            row.Cells[colOffset.Name].Value = "0";
            row.Cells[colNote.Name].Value = "";

            // 5. 刷新该行的样式 (只读/变灰等)
            UpdateRowState(row);

            // 6. 选中新行
            dgvTags.ClearSelection();
            row.Selected = true;
            dgvTags.CurrentCell = row.Cells[0];
            dgvTags.CurrentCell = row.Cells[0];
        }

        private void DeleteTagRows()
        {
            if (dgvTags.SelectedRows.Count > 0)
            {
                // 倒序删除，防止索引错乱
                foreach (DataGridViewRow row in dgvTags.SelectedRows)
                {
                    if (!row.IsNewRow) dgvTags.Rows.Remove(row);
                }

            }
            else if (dgvTags.CurrentRow != null && !dgvTags.CurrentRow.IsNewRow)
            {
                dgvTags.Rows.Remove(dgvTags.CurrentRow);
            }
        }

      
        #endregion

        #region 变量筛选逻辑
        //private void ExecuteFilter()
        //{
        //    string keyword = txtSearch.Text.Trim().ToUpper();

        //    // 获取选中的类型 (如果 Value 是 -1 则表示全部)
        //    object selectedTypeObj = cmbFilterType.SelectedValue;
        //    DataType? targetType = null;
        //    if (selectedTypeObj is DataType dt) targetType = dt;

        //    // 挂起布局，防止刷新闪烁
        //    dgvTags.SuspendLayout();

        //    // 使用 CurrencyManager 挂起绑定 (对于大量行非常重要)
        //    CurrencyManager cm = (CurrencyManager)BindingContext[dgvTags.DataSource ?? dgvTags.Rows];
        //    cm.SuspendBinding();

        //    bool anyVisible = false;

        //    foreach (DataGridViewRow row in dgvTags.Rows)
        //    {
        //        if (row.IsNewRow) continue;

        //        bool matchText = true;
        //        bool matchType = true;

        //        // 1. 文本匹配 (名称、地址、备注)
        //        if (!string.IsNullOrEmpty(keyword))
        //        {
        //            string name = row.Cells[colTagName.Index].Value?.ToString().ToUpper() ?? "";
        //            string addr = row.Cells[colAddr.Index].Value?.ToString().ToUpper() ?? "";
        //            string note = row.Cells[colNote.Index].Value?.ToString().ToUpper() ?? "";

        //            // 地址支持前缀匹配 (比如输入 400 会匹配 40001) 或完全包含
        //            matchText = name.Contains(keyword) || addr.Contains(keyword) || note.Contains(keyword);
        //        }

        //        // 2. 类型匹配
        //        if (targetType.HasValue)
        //        {
        //            if (row.Cells[colDataType.Index].Value is DataType t)
        //            {
        //                matchType = (t == targetType.Value);
        //            }
        //        }

        //        // 设置可见性
        //        bool isVisible = matchText && matchType;
        //        row.Visible = isVisible;
        //        if (isVisible) anyVisible = true;
        //    }

        //    cm.ResumeBinding();
        //    dgvTags.ResumeLayout();
        //}

        private void ExecuteFilter()
        {
            // 1. 获取所有筛选参数
            string keyword = txtSearch.Text.Trim(); // 先不要 ToUpper，内部使用 StringComparison

            // 获取选中的存储区 (假设 index 0 是 "全部")
            StorageZone? targetZone = null;
            if (cmbFilterZone.SelectedIndex > 0 && cmbFilterZone.SelectedValue is StorageZone sz)
                targetZone = sz;

            // 获取选中的数据类型 (假设 Value 为 -1 或 index 0 是 "全部")
            DataType? targetType = null;
            object selectedTypeObj = cmbFilterType.SelectedValue;
            if (selectedTypeObj is DataType dt) targetType = dt;

            // 2. 界面准备
            dgvTags.CurrentCell = null; // 挂起当前单元格，防止设置 Visible 时冲突
            dgvTags.SuspendLayout();

            // 挂起绑定环境
            CurrencyManager cm = (CurrencyManager)BindingContext[dgvTags.DataSource ?? dgvTags.Rows];
            cm.SuspendBinding();

            try
            {
                foreach (DataGridViewRow row in dgvTags.Rows)
                {
                    if (row.IsNewRow) continue;

                    // --- A. 存储区匹配 (全等) ---
                    bool matchZone = true;
                    if (targetZone.HasValue)
                    {
                        if (row.Cells[colZone.Index].Value is StorageZone rowZone)
                            matchZone = (rowZone == targetZone.Value);
                    }

                    // --- B. 数据类型匹配 (全等) ---
                    bool matchType = true;
                    if (targetType.HasValue)
                    {
                        if (row.Cells[colDataType.Index].Value is DataType rowType)
                            matchType = (rowType == targetType.Value);
                    }

                    // --- C. 文本关键字匹配 (核心改动) ---
                    bool matchText = true;
                    if (!string.IsNullOrEmpty(keyword))
                    {
                        string name = row.Cells[colTagName.Index].Value?.ToString() ?? "";
                        string addr = row.Cells[colAddr.Index].Value?.ToString() ?? "";
                        string note = row.Cells[colNote.Index].Value?.ToString() ?? "";

                        // 【核心逻辑】：
                        // 1. 地址列：采用 StartsWith (起始匹配)，解决输入 10 搜出 210 的烦恼
                        bool addrMatch = addr.StartsWith(keyword, StringComparison.OrdinalIgnoreCase);

                        // 2. 名称和备注列：采用 Contains (包含匹配)
                        bool nameMatch = name.Contains(keyword, StringComparison.OrdinalIgnoreCase);
                        bool noteMatch = note.Contains(keyword, StringComparison.OrdinalIgnoreCase);

                        matchText = addrMatch || nameMatch || noteMatch;
                    }

                    // --- D. 综合判定 ---
                    row.Visible = matchZone && matchType && matchText;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("筛选执行异常: " + ex.Message);
            }
            finally
            {
                cm.ResumeBinding();
                dgvTags.ResumeLayout();
            }
        }
        #endregion

        #region 批量修改逻辑
        // ==========================================
        // 批量操作：地址平移 (Offset)
        // ==========================================
        private void MenuBatchOffset_Click(object sender, EventArgs e)
        { 
            if (dgvTags.IsCurrentCellInEditMode) dgvTags.EndEdit();
            this.ActiveControl = lblDeviceName;

            // 1. 获取选中的行 (转为 List 防止遍历时集合变动)
            var selectedRows = dgvTags.SelectedRows.Cast<DataGridViewRow>().ToList();

            if (selectedRows.Count == 0) return;

            string input = InputDialog.Show("请输入地址偏移量 (+/-):", "批量平移", "1");
            if (!int.TryParse(input, out int offset)) return;

            foreach (DataGridViewRow row in selectedRows)
            {
                if (row.IsNewRow) continue;

                // 获取当前显示的字符串
                string currentAddrStr = row.Cells[colAddr.Index].Value?.ToString();
                // 获取当前存储区
                StorageZone zone = (StorageZone)row.Cells[colZone.Index].Value;

                // A. 转为协议绝对地址 (0, 1, 2...) 防止格式干扰
                int currentProtoAddr = ParseAddressToProtocol(currentAddrStr, zone);

                // B. 计算偏移
                int newProtoAddr = currentProtoAddr + offset;
                if (newProtoAddr < 0) newProtoAddr = 0;

                // C. 格式化回写 (根据当前显示模式 Hex/PLC/Dec 自动格式化)
                row.Cells[colAddr.Index].Value = FormatAddress(newProtoAddr, zone);
            }

            // 强制刷新界面
            dgvTags.Invalidate();
        }

        // ==========================================
        // 批量操作：地址递增填充 (Auto-Increment)
        // ==========================================
        private void MenuBatchIncrement_Click(object sender, EventArgs e)
        {
            if (dgvTags.IsCurrentCellInEditMode) dgvTags.EndEdit();
            this.ActiveControl = lblDeviceName;
            if (dgvTags.SelectedRows.Count < 2)
            {
                MessageBox.Show("请至少选择两行。", "提示");
                return;
            }

            // 1. 【核心修复】必须按视觉顺序排序 (从小到大)
            var sortedRows = dgvTags.SelectedRows.Cast<DataGridViewRow>()
                                                 .OrderBy(r => r.Index)
                                                 .Where(r => !r.IsNewRow)
                                                 .ToList();

            // 2. 获取基准行 (视觉上的第一行)
            var startRow = sortedRows[0];

            // 获取基准地址 (协议值)
            StorageZone zone = (StorageZone)startRow.Cells[colZone.Index].Value;
            string addrStr = startRow.Cells[colAddr.Index].Value?.ToString();
            //int startAddr = ParseAddressToProtocol(startRow.Cells[colAddr.Index].Value?.ToString(), zone);
            int startAddr = ParseAddressStrict(addrStr, zone, _showPlcAddress, _showHexAddress);

            // 4. 推断步长
            DataType startType = (DataType)startRow.Cells[colDataType.Index].Value;
            int defaultStep = 1;
            if (startType == DataType.Float || startType == DataType.Int32 || startType == DataType.UInt32) defaultStep = 2;
            if (startType == DataType.Double) defaultStep = 4;

            string input = InputDialog.Show($"起始协议地址: {startAddr} (当前显示: {addrStr})\r\n请输入递增步长:", "地址递增", defaultStep.ToString());

            if (int.TryParse(input, out int step))
            {
                // 4. 循环填充 (从第 1 个索引开始，即第二行，修改为基准值 + 步长)
                // i=0 是基准行，保持不变；i=1 是第二行
                for (int i = 1; i < sortedRows.Count; i++)
                {
                    var row = sortedRows[i];
                    // 确保后续行的存储区与第一行一致 (防止跨区导致地址混乱)
                    row.Cells[colZone.Index].Value = zone;                  
                    int newAddr = startAddr + (i * step);
                    row.Cells[colAddr.Index].Value = FormatAddress(newAddr, zone);
                    UpdateRowState(row); // 刷新该行的只读状态
                }
            }
        }
        // ==========================================
        // 批量操作：修改存储区
        // ==========================================
        private void MenuBatchZone_Click(object sender, EventArgs e)
        {
            if (dgvTags.IsCurrentCellInEditMode) dgvTags.EndEdit();
            this.ActiveControl = lblDeviceName;
            var selectedRows = dgvTags.SelectedRows;
            if (selectedRows.Count == 0) return;

            using (var dialog = new Form())
            {
                dialog.Text = "批量修改存储区";
                dialog.Size = new Size(300, 150);
                dialog.StartPosition = FormStartPosition.CenterParent;
                
                var label = new Label { Text = "选择存储区:", Location = new Point(20, 20), AutoSize = true };
                var combo = new ComboBox { Location = new Point(20, 45), Width = 240, DropDownStyle = ComboBoxStyle.DropDownList };
                combo.Items.AddRange(new object[] { "0x - 线圈", "1x - 离散输入", "3x - 输入寄存器", "4x - 保持寄存器" });
                combo.SelectedIndex = 3;
                
                var btnOk = new Button { Text = "确定", Location = new Point(100, 80), DialogResult = DialogResult.OK };
                var btnCancel = new Button { Text = "取消", Location = new Point(180, 80), DialogResult = DialogResult.Cancel };
                
                dialog.Controls.AddRange(new Control[] { label, combo, btnOk, btnCancel });
                dialog.AcceptButton = btnOk;
                dialog.CancelButton = btnCancel;
                
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    StorageZone zone = (StorageZone)combo.SelectedIndex;
                    foreach (DataGridViewRow row in selectedRows)
                    {
                        if (row.IsNewRow) continue;
                        row.Cells[colZone.Index].Value = zone;
                        UpdateRowState(row);
                    }
                }
            }
        }

        // ==========================================
        // 批量操作：修改数据类型
        // ==========================================
        private void MenuBatchDataType_Click(object sender, EventArgs e)
        {
            if (dgvTags.IsCurrentCellInEditMode) dgvTags.EndEdit();
            this.ActiveControl = lblDeviceName;
            var selectedRows = dgvTags.SelectedRows;
            if (selectedRows.Count == 0) return;

            using (var dialog = new Form())
            {
                dialog.Text = "批量修改数据类型";
                dialog.Size = new Size(300, 150);
                dialog.StartPosition = FormStartPosition.CenterParent;
                
                var label = new Label { Text = "选择数据类型:", Location = new Point(20, 20), AutoSize = true };
                var combo = new ComboBox { Location = new Point(20, 45), Width = 240, DropDownStyle = ComboBoxStyle.DropDownList };
                foreach (DataType t in Enum.GetValues(typeof(DataType)))
                    combo.Items.Add(t.ToString());
                combo.SelectedIndex = 0;
                
                var btnOk = new Button { Text = "确定", Location = new Point(100, 80), DialogResult = DialogResult.OK };
                var btnCancel = new Button { Text = "取消", Location = new Point(180, 80), DialogResult = DialogResult.Cancel };
                
                dialog.Controls.AddRange(new Control[] { label, combo, btnOk, btnCancel });
                dialog.AcceptButton = btnOk;
                dialog.CancelButton = btnCancel;
                
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    DataType dataType = (DataType)Enum.Parse(typeof(DataType), combo.SelectedItem.ToString());
                    foreach (DataGridViewRow row in selectedRows)
                    {
                        if (row.IsNewRow) continue;
                        row.Cells[colDataType.Index].Value = dataType;
                        UpdateRowState(row);
                    }
                }
            }
        }

        // ==========================================
        // 批量操作：修改字节序
        // ==========================================
        private void MenuBatchFormat_Click(object sender, EventArgs e)
        {
            if (dgvTags.IsCurrentCellInEditMode) dgvTags.EndEdit();
            this.ActiveControl = lblDeviceName;
            var selectedRows = dgvTags.SelectedRows;
            if (selectedRows.Count == 0) return;

            using (var dialog = new Form())
            {
                dialog.Text = "批量修改字节序";
                dialog.Size = new Size(300, 150);
                dialog.StartPosition = FormStartPosition.CenterParent;
                
                var label = new Label { Text = "选择字节序:", Location = new Point(20, 20), AutoSize = true };
                var combo = new ComboBox { Location = new Point(20, 45), Width = 240, DropDownStyle = ComboBoxStyle.DropDownList };
                combo.Items.AddRange(new object[] { "ABCD (Big Endian)", "CDAB (Little Endian)", "BADC", "DCBA" });
                combo.SelectedIndex = 0;
                
                var btnOk = new Button { Text = "确定", Location = new Point(100, 80), DialogResult = DialogResult.OK };
                var btnCancel = new Button { Text = "取消", Location = new Point(180, 80), DialogResult = DialogResult.Cancel };
                
                dialog.Controls.AddRange(new Control[] { label, combo, btnOk, btnCancel });
                dialog.AcceptButton = btnOk;
                dialog.CancelButton = btnCancel;
                
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    DataFormat format = (DataFormat)combo.SelectedIndex;
                    foreach (DataGridViewRow row in selectedRows)
                    {
                        if (row.IsNewRow) continue;
                        row.Cells[colDataFormat.Index].Value = format;
                    }
                }
            }
        }

        // ==========================================
        // 批量操作：设置系数/偏移
        // ==========================================
        private void MenuBatchScale_Click(object sender, EventArgs e)
        {
            if (dgvTags.IsCurrentCellInEditMode) dgvTags.EndEdit();
            this.ActiveControl = lblDeviceName;
            var selectedRows = dgvTags.SelectedRows;
            if (selectedRows.Count == 0) return;

            using (var dialog = new Form())
            {
                dialog.Text = "批量设置系数/偏移";
                dialog.Size = new Size(300, 180);
                dialog.StartPosition = FormStartPosition.CenterParent;
                
                var lblFactor = new Label { Text = "系数:", Location = new Point(20, 20), AutoSize = true };
                var txtFactor = new TextBox { Location = new Point(80, 17), Width = 180, Text = "1" };
                var lblOffset = new Label { Text = "偏移:", Location = new Point(20, 55), AutoSize = true };
                var txtOffset = new TextBox { Location = new Point(80, 52), Width = 180, Text = "0" };
                
                var btnOk = new Button { Text = "确定", Location = new Point(100, 110), DialogResult = DialogResult.OK };
                var btnCancel = new Button { Text = "取消", Location = new Point(180, 110), DialogResult = DialogResult.Cancel };
                
                dialog.Controls.AddRange(new Control[] { lblFactor, txtFactor, lblOffset, txtOffset, btnOk, btnCancel });
                dialog.AcceptButton = btnOk;
                dialog.CancelButton = btnCancel;
                
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    if (float.TryParse(txtFactor.Text, out float factor) && float.TryParse(txtOffset.Text, out float offset))
                    {
                        foreach (DataGridViewRow row in selectedRows)
                        {
                            if (row.IsNewRow) continue;
                            row.Cells[colFactor.Index].Value = factor.ToString();
                            row.Cells[colOffset.Index].Value = offset.ToString();
                        }
                    }
                }
            }
        }

        // ==========================================
        // 批量操作：添加前缀/后缀
        // ==========================================
        private void MenuBatchPrefix_Click(object sender, EventArgs e)
        {
            if (dgvTags.IsCurrentCellInEditMode) dgvTags.EndEdit();
            this.ActiveControl = lblDeviceName;
            var selectedRows = dgvTags.SelectedRows;
            if (selectedRows.Count == 0) return;

            using (var dialog = new Form())
            {
                dialog.Text = "批量添加前缀/后缀";
                dialog.Size = new Size(300, 180);
                dialog.StartPosition = FormStartPosition.CenterParent;
                
                var lblPrefix = new Label { Text = "前缀:", Location = new Point(20, 20), AutoSize = true };
                var txtPrefix = new TextBox { Location = new Point(80, 17), Width = 180 };
                var lblSuffix = new Label { Text = "后缀:", Location = new Point(20, 55), AutoSize = true };
                var txtSuffix = new TextBox { Location = new Point(80, 52), Width = 180 };
                
                var btnOk = new Button { Text = "确定", Location = new Point(100, 110), DialogResult = DialogResult.OK };
                var btnCancel = new Button { Text = "取消", Location = new Point(180, 110), DialogResult = DialogResult.Cancel };
                
                dialog.Controls.AddRange(new Control[] { lblPrefix, txtPrefix, lblSuffix, txtSuffix, btnOk, btnCancel });
                dialog.AcceptButton = btnOk;
                dialog.CancelButton = btnCancel;
                
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    string prefix = txtPrefix.Text;
                    string suffix = txtSuffix.Text;
                    foreach (DataGridViewRow row in selectedRows)
                    {
                        if (row.IsNewRow) continue;
                        string currentName = row.Cells[colTagName.Index].Value?.ToString() ?? "";
                        row.Cells[colTagName.Index].Value = prefix + currentName + suffix;
                    }
                }
            }
        }

        // ==========================================
        // 批量操作：复制属性
        // ==========================================
        private void MenuBatchCopy_Click(object sender, EventArgs e)
        {
            if (dgvTags.IsCurrentCellInEditMode) dgvTags.EndEdit();
            this.ActiveControl = lblDeviceName;
            if (dgvTags.SelectedRows.Count < 2)
            {
                MessageBox.Show("请至少选择两行（首行作为源，其余为目标）", "提示");
                return;
            }

            // 1. 排序，确保取到的是视觉上的第一行
            var sortedRows = dgvTags.SelectedRows.Cast<DataGridViewRow>()
                                                 .OrderBy(r => r.Index)
                                                 .Where(r => !r.IsNewRow)
                                                 .ToList();

            var sourceRow = sortedRows[0]; // 源模板

            // 提取源属性
            var z = sourceRow.Cells[colZone.Index].Value;
            var t = sourceRow.Cells[colDataType.Index].Value;
            var u = sourceRow.Cells[colUnit.Index].Value;
            var fmt = sourceRow.Cells[colDataFormat.Index].Value;
            var fac = sourceRow.Cells[colFactor.Index].Value;
            var off = sourceRow.Cells[colOffset.Index].Value;
            var note = sourceRow.Cells[colNote.Index].Value;

            // 2. 覆盖后续行
            for (int i = 1; i < sortedRows.Count; i++)
            {
                var row = sortedRows[i];

                row.Cells[colZone.Index].Value = z;
                row.Cells[colDataType.Index].Value = t;
                row.Cells[colUnit.Index].Value = u;
                row.Cells[colDataFormat.Index].Value = fmt;
                row.Cells[colFactor.Index].Value = fac;
                row.Cells[colOffset.Index].Value = off;
                row.Cells[colNote.Index].Value = note;

                // 刷新状态 (比如 Int16 变 Bool，BitIndex 列要变色)
                UpdateRowState(row);
            }
        }
        // 批量操作：递增变量
        private void MenuBatchNameSeries_Click(object sender, EventArgs e)
        {
            if (dgvTags.IsCurrentCellInEditMode) dgvTags.EndEdit();
            this.ActiveControl = lblDeviceName;

            if (dgvTags.SelectedRows.Count < 1) return;

            // 1. 排序
            var sortedRows = dgvTags.SelectedRows.Cast<DataGridViewRow>()
                                                 .OrderBy(r => r.Index)
                                                 .Where(r => !r.IsNewRow)
                                                 .ToList();

            // 2. 分析第一行的名称
            string baseName = sortedRows[0].Cells[colTagName.Index].Value?.ToString() ?? "Tag";

            string prefix = baseName;
            string suffix = "";
            int startNum = 1;
            bool hasNumber = false;

            // 正则策略：优先匹配字符串中的数字
            // 逻辑：尝试把字符串拆分为 "前缀" + "数字" + "后缀"

            // 尝试1: 结尾有数字 (Temp1) -> 匹配 Group1=Temp, Group2=1
            var matchEnd = Regex.Match(baseName, @"^(.*?)(\d+)$");

            // 尝试2: 开头有数字 (1_Temp) -> 匹配 Group1=1, Group2=_Temp
            var matchStart = Regex.Match(baseName, @"^(\d+)(.*)$");

            // 尝试3: 中间有数字 (P_1_A) -> 匹配 Group1=P_, Group2=1, Group3=_A
            var matchMid = Regex.Match(baseName, @"^(.*?)(\d+)(.*)$");

            if (matchEnd.Success)
            {
                prefix = matchEnd.Groups[1].Value;
                startNum = int.Parse(matchEnd.Groups[2].Value);
                suffix = "";
                hasNumber = true;
            }
            else if (matchStart.Success)
            {
                prefix = ""; // 数字在最前，前缀为空
                startNum = int.Parse(matchStart.Groups[1].Value);
                suffix = matchStart.Groups[2].Value;
                hasNumber = true;
            }
            else if (matchMid.Success)
            {
                prefix = matchMid.Groups[1].Value;
                startNum = int.Parse(matchMid.Groups[2].Value);
                suffix = matchMid.Groups[3].Value;
                hasNumber = true;
            }
            else
            {
                // 没有数字，直接在末尾追加
                prefix = baseName;
                startNum = 1;
                suffix = "";
                hasNumber = false;
            }

            // 3. 执行填充
            // 如果原名是 "Temp1"，选中3行 -> Temp1, Temp2, Temp3 (Excel逻辑，第一行也参与重命名过程以保持序列)
            // 但为了防止修改第一行，我们通常保持第一行不变，从第二行开始递增。

            // 逻辑修正：
            // 如果是 "Temp"，则变成 "Temp", "Temp1", "Temp2" (第一行不变，后面追加)
            // 如果是 "Temp1"，则变成 "Temp1", "Temp2", "Temp3"

            for (int i = 1; i < sortedRows.Count; i++) // 从第二行开始改
            {
                var row = sortedRows[i];
                int currentNum = startNum + i; // 这里的 i 就是递增量

                if (!hasNumber)
                {
                    // 原名没数字：Temp -> Temp1, Temp2
                    row.Cells[colTagName.Index].Value = $"{prefix}{i}"; // 直接加序号
                }
                else
                {
                    // 原名有数字：Temp1 -> Temp2
                    row.Cells[colTagName.Index].Value = $"{prefix}{currentNum}{suffix}";
                }
            }
        }

        #endregion

        #region 输入校验
        /// <summary>
        /// 执行所有数据检查，返回是否通过
        /// </summary>
        /// <param name="report">错误报告内容</param>
        /// <summary>
        /// 验证数据
        /// </summary>
        /// <param name="errorMsg">返回的阻断性错误信息</param>
        /// <param name="warningMsg">返回的非阻断性警告信息</param>
        /// <returns>true=无硬错误(可继续), false=有硬错误(必须终止)</returns>
        private bool ValidateData(out string errorMsg, out string warningMsg)
        {
            StringBuilder sbErr = new StringBuilder();     // 硬错误
            StringBuilder sbWarn = new StringBuilder();    // 软警告

            bool hasCriticalError = false;
            int errorCount = 0;
            int warnCount = 0;

            // 记录名字第一次出现的行号：Name -> RowIndex
            var nameHistory = new Dictionary<string, int>();

            // 占用地图 (用于查地址冲突)
            //var occupancyMap = new Dictionary<StorageZone, Dictionary<int, int>>();
            //foreach (StorageZone z in Enum.GetValues(typeof(StorageZone)))
            //    occupancyMap[z] = new Dictionary<int, int>();

            var occupancyMap = new Dictionary<StorageZone, Dictionary<int, List<int>>>();
            foreach (StorageZone z in Enum.GetValues(typeof(StorageZone)))
                occupancyMap[z] = new Dictionary<int, List<int>>();


            // 1. 重置所有行背景色 (清除之前的红色/黄色)
            foreach (DataGridViewRow row in dgvTags.Rows)
            {
                row.DefaultCellStyle.BackColor = Color.Empty; // 恢复默认
            }

            // 2. 开始遍历检查
            foreach (DataGridViewRow row in dgvTags.Rows)
            {
                if (row.IsNewRow) continue;
                int rowNum = row.Index + 1;

                // --- A. 基础检查 (硬错误) ---
                string name = row.Cells[colTagName.Index].Value?.ToString()?.Trim();
                if (string.IsNullOrEmpty(name))
                {
                    sbErr.AppendLine($"[行 {rowNum}] 变量名不能为空。");
                    row.DefaultCellStyle.BackColor = Color.MistyRose; // 红
                    hasCriticalError = true;
                    continue;
                }

                // --- B. 重名检查 (软警告) ---
                // 这里的逻辑：如果名字已存在，标记当前行和之前的行
                if (nameHistory.ContainsKey(name))
                {
                    int prevRowIdx = nameHistory[name];
                   if(warnCount <= 20)
                    {
                        sbWarn.AppendLine($"[行 {rowNum}] 变量名 \"{name}\" 与第 {prevRowIdx + 1} 行重复。");

                        // 标记颜色 (浅黄色警告)
                        // 注意：如果之前已经是红色(硬错误)，不要覆盖成黄色
                        if (row.DefaultCellStyle.BackColor != Color.MistyRose)
                            row.DefaultCellStyle.BackColor = Color.LightYellow;

                        // 把之前的那个重复行也标黄
                        var prevRow = dgvTags.Rows[prevRowIdx];
                        if (prevRow.DefaultCellStyle.BackColor != Color.MistyRose)
                            prevRow.DefaultCellStyle.BackColor = Color.LightYellow;

                        warnCount++;
                    }
                    
                }
                else
                {
                    nameHistory[name] = row.Index;
                }


                // --- C. 地址解析与越界 (硬错误) ---
                if (!(row.Cells[colZone.Index].Value is StorageZone zone)) zone = StorageZone.HoldingRegister_4x;
                if (!(row.Cells[colDataType.Index].Value is DataType dtype)) dtype = DataType.Int16;

                string addrStr = row.Cells[colAddr.Index].Value?.ToString();
                int protocolAddr = ParseAddressStrict(addrStr, zone, _showPlcAddress, _showHexAddress);

                if (protocolAddr < 0 || protocolAddr > 65535)
                {
                    sbErr.AppendLine($"[行 {rowNum}] 地址无效或越界。");
                    row.DefaultCellStyle.BackColor = Color.MistyRose;
                    hasCriticalError = true;
                    continue;
                }

                // --- D. 位索引检查 (硬错误) ---
                int currentBitIdx = -1;
                bool isCurrentRegBool = (dtype == DataType.Bool) &&
                                        (zone == StorageZone.HoldingRegister_4x || zone == StorageZone.InputRegister_3x);

                if (isCurrentRegBool)
                {
                    string bitStr = row.Cells[colBitIndex.Index].Value?.ToString();
                    if (!int.TryParse(bitStr, out currentBitIdx) || currentBitIdx < 0 || currentBitIdx > 15)
                    {
                        sbErr.AppendLine($"[行 {rowNum}] 寄存器位索引无效 (需0-15)。");
                        row.DefaultCellStyle.BackColor = Color.MistyRose;
                        hasCriticalError = true;
                        continue;
                    }
                }

                // --- E. 地址重叠检查 (修正版：支持位/字影寄存器逻辑) ---
                int length = GetDataTypeLength(dtype);
                if (zone == StorageZone.CoilStatus_0x || zone == StorageZone.InputStatus_1x) length = 1;

                // 当前是否是 16位短整数 (用于影寄存器判断)
                bool isCurrentShort = (dtype == DataType.Int16 || dtype == DataType.UInt16) &&
                                      (zone == StorageZone.HoldingRegister_4x || zone == StorageZone.InputRegister_3x);

                for (int i = 0; i < length; i++)
                {
                    int checkAddr = protocolAddr + i;

                    // 如果该地址已经被占用过了
                    if (occupancyMap[zone].ContainsKey(checkAddr))
                    {
                        // 遍历该地址之前所有的占用者，逐一判断是否属于“合法重叠”
                        foreach (int existingRowIdx in occupancyMap[zone][checkAddr])
                        {
                            var existingRow = dgvTags.Rows[existingRowIdx];
                            DataType existingType = (DataType)existingRow.Cells[colDataType.Index].Value;

                            bool isExistingRegBool = (existingType == DataType.Bool) &&
                                                     (zone == StorageZone.HoldingRegister_4x || zone == StorageZone.InputRegister_3x);

                            bool isExistingShort = (existingType == DataType.Int16 || existingType == DataType.UInt16) &&
                                                   (zone == StorageZone.HoldingRegister_4x || zone == StorageZone.InputRegister_3x);

                            // --- 影寄存器豁免逻辑判断 ---
                            bool isExempt = false;

                            // 场景 1: Bool位 与 Bool位 (位索引不同则豁免)
                            if (isCurrentRegBool && isExistingRegBool)
                            {
                                string exBitStr = existingRow.Cells[colBitIndex.Index].Value?.ToString();
                                int.TryParse(exBitStr, out int exBitIdx);
                                if (currentBitIdx != exBitIdx) isExempt = true;
                            }
                            // 场景 2: Bool位 与 Int16校准字 (地址相同时豁免)
                            else if ((isCurrentRegBool && isExistingShort) || (isCurrentShort && isExistingRegBool))
                            {
                                // 注意：只有地址完全相同才叫“影寄存器”，如果是 Int32(2字) 重叠了 Bool，依然算冲突
                                isExempt = true;
                            }

                            // 如果不属于豁免情况，则是真正的冲突
                            if (!isExempt)
                            {
                                string conflictName = existingRow.Cells[colTagName.Index].Value?.ToString();
                                sbErr.AppendLine($"[行 {rowNum}] 地址冲突: 地址 {FormatAddress(checkAddr, zone)} 已被第 {existingRowIdx + 1} 行占用。");

                                row.DefaultCellStyle.BackColor = Color.MistyRose;
                                existingRow.DefaultCellStyle.BackColor = Color.MistyRose;

                                hasCriticalError = true;
                                errorCount++;
                                break; // 只要有一个冲突，该行就报错，跳出占用者循环
                            }
                        }
                    }

                    // --- 登记占用 ---
                    if (!occupancyMap[zone].ContainsKey(checkAddr))
                        occupancyMap[zone][checkAddr] = new List<int>();

                    // 把当前行加入到这个地址的占用者名单中
                    occupancyMap[zone][checkAddr].Add(row.Index);
                }


                // 限制错误数量
                if (errorCount > 10) { sbErr.AppendLine("... (错误过多中断检查)"); break; }
                if (warnCount > 20) { /* 警告太多就不记录了，但不中断循环 */ }
            }

            errorMsg = sbErr.ToString();
            warningMsg = sbWarn.ToString();

            // 只有当没有硬错误时，才返回 true
            return !hasCriticalError;
        }
        /// <summary>
        /// 判定两个点位是否存在物理冲突 (核心逻辑)
        /// </summary>
        private bool CheckIsOverlap(ModbusPoint p1, ModbusPoint p2)
        {
            // 1. 存储区不同，互不干扰
            if (p1.Zone != p2.Zone) return false;

            // 2. 获取各自占用的寄存器长度
            int len1 = GetDataTypeLength(p1.DataType);
            int len2 = GetDataTypeLength(p2.DataType);

            // 3. 基本的范围判定
            bool isRegOverlap = Math.Max(p1.Address, p2.Address) <= Math.Min(p1.Address + len1 - 1, p2.Address + len2 - 1);

            // 如果地址范围完全没有交集，直接不冲突
            if (!isRegOverlap) return false;

            // 4. 【核心逻辑】如果地址有交集，处理 3x/4x 区的特殊重叠规则
            if (p1.Zone == StorageZone.HoldingRegister_4x || p1.Zone == StorageZone.InputRegister_3x)
            {
                // 情况 A: 两个都是 Bool 位变量 -> 只有地址和位索引都相同时才算冲突
                if (p1.DataType == DataType.Bool && p2.DataType == DataType.Bool)
                {
                    if (p1.Address == p2.Address)
                    {
                        return p1.BitIndex == p2.BitIndex; // 位索引相同才冲突
                    }
                }

                // 情况 B: 一个是位变量(Bool)，一个是字变量(Int16/UInt16)
                bool p1IsShort = (p1.DataType == DataType.Int16 || p1.DataType == DataType.UInt16);
                bool p2IsShort = (p2.DataType == DataType.Int16 || p2.DataType == DataType.UInt16);

                if ((p1.DataType == DataType.Bool && p2IsShort) || (p2.DataType == DataType.Bool && p1IsShort))
                {
                    if (p1.Address == p2.Address)
                    {
                        return false; // 地址相同，允许这种“位/字”共存逻辑
                    }
                }
            }

            // 5. 其他任何形式的地址交集（如 Float 和 Int 重叠）均视为冲突
            return true;
        }
        /// <summary>
        /// 获取数据类型占用的寄存器数量 (16位字)
        /// </summary>
        private int GetDataTypeLength(DataType type)
        {
            switch (type)
            {
                case DataType.Bool:
                case DataType.Int16:
                case DataType.UInt16:
                    return 1;

                case DataType.Int32:
                case DataType.UInt32:
                case DataType.Float:
                    return 2;

                case DataType.Double:
                    return 4; // Modbus 协议中 Double 通常占 4 个寄存器

                default:
                    return 1;
            }
        }
        #endregion
       
        
    }

    // 辅助类
    public class EnumItem { public string Text { get; set; } public object Value { get; set; } }
}
