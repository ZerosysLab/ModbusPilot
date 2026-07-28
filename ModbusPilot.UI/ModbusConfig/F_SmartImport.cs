using ExcelDataReader;
using MiniExcelLibs;
using ModbusPilot.Core;
using ModbusPilot.Core.Models;
using ModbusPilot.Core.Utils; // 引用刚才写的 Helper
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace ModbusPilot.UI.Common
{
    public partial class F_SmartImport : Form
    {
        private class ValidationResult
        {
            public int CriticalCount = 0;
            public int WarningCount = 0;
            public int ValidCount = 0;
            public string SampleMsg = "";
            public HashSet<int> InvalidRowIndices = new HashSet<int>(); // 记录要跳过的行号(相对于 _allRows)
            public bool HasErrors => CriticalCount > 0 || WarningCount > 0;
        }

        // 解析后的结果，供外部读取
        public List<ModbusPoint> ResultPoints { get; private set; } = new List<ModbusPoint>();

        private string _currentFilePath;
        private List<dynamic> _allRows; // 原始数据缓存
                                        // 用于保持引用，防止点一次弹出一个新窗口
        private F_ImportGuide _guideForm = null;
        // 记录每一列映射到什么属性 (列索引 -> 目标属性)
        private Dictionary<int, ImportHelper.TargetColumn> _columnMappings = new Dictionary<int, ImportHelper.TargetColumn>();
        // 用于记录哪些列有冲突，以便 CellPainting 标红
        private HashSet<int> _conflictColumns = new HashSet<int>();
        // 记录数据格式疑似不匹配的列索引 (例如：地址列里全是中文)
        private HashSet<int> _suspectColumns = new HashSet<int>();

        public F_SmartImport()
        {
            InitializeComponent();

            dgvPreview.AllowUserToOrderColumns = false; // 禁止拖拽交换列位置，防止映射索引错乱
                                                        // 1. 禁止自动调整高度 (否则你设了高度也会被系统改回去)
            dgvPreview.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;

            // 2. 强制设为 50 像素 (足够容纳两行文字：原始名 + 映射名)
            dgvPreview.ColumnHeadersHeight = 50;

            // 【关键】注册编码支持，否则读 .xls 会报错 "No data is available for encoding 1252"
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            StyleHelpButton_Flat();

            // 构造函数里：
            lblGuide.Visible = true;

            // UI 初始化
            btnOpenFile.Click += (s, e) => OpenFile();
            numHeaderRow.ValueChanged += (s, e) => LoadPreview(); // 调整行号重读
            btnImport.Click += BtnImport_Click;

            // 表头点击事件：左键循环切换，右键弹出菜单
            dgvPreview.ColumnHeaderMouseClick += DgvPreview_ColumnHeaderMouseClick;

            // 自动触发表头绘制 (显示映射状态)
            dgvPreview.CellPainting += DgvPreview_CellPainting;


            btnHelp.Text = LangProvider.Get("Btn_Help"); // 这里用 Emoji 当图标，后面跟文字
                                                         // 如果只想显示灯泡，就写: btnHelp.Text = "💡"; 
                                                         // --- 视觉微调 ---
                                                         // 使用稍大一点的字体，或者专门支持 Emoji 的字体，让灯泡看起来更清晰
                                                         // Segoe UI Emoji 是 Win10/11 自带的 Emoji 字体，如果没有会自动回退
            btnHelp.Font = new Font("Microsoft YaHei UI", 9F, FontStyle.Regular);
            // 修改后的代码：
            btnHelp.Click += (s, e) =>
            {
                // 1. 如果窗口不存在，或者已经被关闭了，就创建一个新的
                if (_guideForm == null || _guideForm.IsDisposed)
                {
                    _guideForm = new F_ImportGuide();

                    // 【关键】使用 Show() 而不是 ShowDialog()
                    // 传入 'this' 作为所有者，这样当 F_SmartImport 关闭时，指引也会自动关闭
                    // 且指引窗口会始终浮在 F_SmartImport 上面，不会被盖住
                    _guideForm.Show(this);
                }
                else
                {
                    // 2. 如果窗口已经打开了，就把它激活（提到最前），防止用户找不到
                    _guideForm.Activate();

                    // 如果最小化了，还原它
                    if (_guideForm.WindowState == FormWindowState.Minimized)
                        _guideForm.WindowState = FormWindowState.Normal;
                }

            };
        }

        private void StyleHelpButton_Flat()
        {
            // 1. 去掉厚重的背景和边框
            btnHelp.FlatStyle = FlatStyle.Flat;
            btnHelp.FlatAppearance.BorderSize = 0;
            btnHelp.BackColor = Color.Transparent; // 或者 SystemColors.Control

            // 2. 调整文字颜色 (使用主题色或深灰色)
            btnHelp.ForeColor = Color.FromArgb(0, 102, 204); // 链接蓝
                                                             // 或者用橙色配合灯泡图标： Color.DarkOrange

            // 3. 字体微调
            btnHelp.Font = new Font("Microsoft YaHei UI", 9F, FontStyle.Underline); // 加下划线更有指引感
            btnHelp.Cursor = Cursors.Hand;

            // 4. 对齐
            btnHelp.TextAlign = ContentAlignment.MiddleRight;
            // 调整大小，不要那么宽
            btnHelp.Size = new Size(80, 25);
            // 靠右停靠
            btnHelp.Location = new Point(this.Width - 100, 12);
            btnHelp.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        }
        private void OpenFile()
        {
            using (var ofd = new OpenFileDialog { Filter = "Excel Files|*.xlsx;*.xls;*.csv" })
            {
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    _currentFilePath = ofd.FileName;
                    lblFile.Text = System.IO.Path.GetFileName(_currentFilePath);

                    string ext = System.IO.Path.GetExtension(_currentFilePath).ToLower();

                    try
                    {
                        // 分流处理
                        if (ext == ".xls")
                        {
                            // 针对老格式：使用 ExcelDataReader
                            _allRows = ReadOldExcel(_currentFilePath);
                        }
                        else
                        {
                            // 针对新格式/CSV：使用 MiniExcel (速度更快)
                            _allRows = MiniExcel.Query(_currentFilePath, useHeaderRow: false).ToList();
                        }

                        // 加载预览
                        LoadPreview();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("读取文件失败: " + ex.Message);
                    }
                }
            }
        }

        /// <summary>
        /// 专门读取 .xls 格式，并转换为 MiniExcel 兼容的格式
        /// </summary>
        private List<dynamic> ReadOldExcel(string path)
        {
            var result = new List<dynamic>();

            using (var stream = File.Open(path, FileMode.Open, FileAccess.Read))
            {
                using (var reader = ExcelReaderFactory.CreateBinaryReader(stream))
                {
                    // 转换为 DataSet (默认第一张表)
                    var resultDs = reader.AsDataSet();
                    if (resultDs.Tables.Count > 0)
                    {
                        var table = resultDs.Tables[0];

                        // 遍历 DataTable 行
                        foreach (DataRow row in table.Rows)
                        {
                            // 转换为 Dictionary<string, object> 以保持和 MiniExcel 结构一致
                            // MiniExcel 的列名默认是 "A", "B", "C"...
                            var dict = new Dictionary<string, object>();

                            for (int i = 0; i < table.Columns.Count; i++)
                            {
                                // 模拟 MiniExcel 的列名生成逻辑 (A, B, C...)
                                // 简单起见，我们其实不需要字母列名，LoadPreview 里是按 keys 遍历的
                                // 我们直接用 "Column" + i 或者直接用 MiniExcelHelper 里的转换
                                // 为了简单，我们这里直接用索引数字转字母有点麻烦，
                                // 其实 LoadPreview 里的逻辑是 foreach (var key in dataRow.Keys)
                                // 所以只要 Key 是唯一的就行。

                                string colName = GetExcelColumnName(i + 1); // A, B, C
                                dict[colName] = row[i];
                            }

                            result.Add(dict);
                        }
                    }
                }
            }
            return result;
        }

        // 辅助：数字转 Excel 列名 (1->A, 2->B, 27->AA)
        private string GetExcelColumnName(int columnNumber)
        {
            string columnName = "";
            while (columnNumber > 0)
            {
                int modulo = (columnNumber - 1) % 26;
                columnName = Convert.ToChar('A' + modulo) + columnName;
                columnNumber = (columnNumber - modulo) / 26;
            }
            return columnName;
        }

        private void LoadPreview()
        {
           
            if (_allRows == null || _allRows.Count == 0) return;

            lblGuide.Visible = false; // 隐藏提示，显示表格

            int headerIndex = (int)numHeaderRow.Value - 1; // 0-based
            if (headerIndex < 0) headerIndex = 0;
            if (headerIndex >= _allRows.Count) return;

            // 1. 获取表头行数据，作为列名
            var headerRow = _allRows[headerIndex] as IDictionary<string, object>;
            if (headerRow == null) return; // 容错

            // 2. 配置 DataGridView
            dgvPreview.Columns.Clear();
            _columnMappings.Clear();

            // 用于记录已经分配过的唯一类型 (Name, Address, Zone, DataType, Unit)
            // Note 可以重复，所以不算在内
            HashSet<ImportHelper.TargetColumn> assignedTypes = new HashSet<ImportHelper.TargetColumn>();

            int colIndex = 0;
            // MiniExcel 的 dynamic 其实是 Dictionary<string, object>，key 是 "A", "B", "C"...
            foreach (var key in headerRow.Keys)
            {
                var val = headerRow[key]?.ToString() ?? "";

                var col = new DataGridViewTextBoxColumn();
                col.HeaderText = val; // 暂时显示Excel内容，后面绘制会覆盖
                col.Tag = val;        // 存原始表头文字，用于猜测
                col.SortMode = DataGridViewColumnSortMode.NotSortable;
                dgvPreview.Columns.Add(col);

                // **智能猜测**
                var guess = ImportHelper.GuessColumnType(val);

                // 如果猜出来的类型不是 Ignore/Note，且已经被分配过了，则降级为 Ignore
                if (guess != ImportHelper.TargetColumn.Ignore &&
                    guess != ImportHelper.TargetColumn.Note &&
                    assignedTypes.Contains(guess))
                {
                    guess = ImportHelper.TargetColumn.Ignore;
                }

                if (guess != ImportHelper.TargetColumn.Ignore)
                {
                    assignedTypes.Add(guess);
                }

                _columnMappings[colIndex] = guess;

                colIndex++;
            }

            // 3. 填充预览数据 (取表头后面的 20 行)
            dgvPreview.Rows.Clear();
            for (int i = headerIndex + 1; i < Math.Min(headerIndex + 21, _allRows.Count); i++)
            {
                var dataRow = _allRows[i] as IDictionary<string, object>;
                int rIdx = dgvPreview.Rows.Add();
                int cIdx = 0;
                foreach (var key in dataRow.Keys)
                {
                    if (cIdx < dgvPreview.Columns.Count)
                    {
                        dgvPreview.Rows[rIdx].Cells[cIdx].Value = dataRow[key];
                    }
                    cIdx++;
                }
            }

            UpdateHeadersStyle();

            CheckStatus();
        }

        private void DgvPreview_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            int colIdx = e.ColumnIndex;

            if (e.Button == MouseButtons.Left)
            {
                // 1. 计算打算切换到的新类型
                var currentMap = _columnMappings.ContainsKey(colIdx) ? _columnMappings[colIdx] : ImportHelper.TargetColumn.Ignore;

                int next = (int)currentMap + 1;
                if (next > (int)ImportHelper.TargetColumn.BitIndex) next = 0; // 循环
                var newMapType = (ImportHelper.TargetColumn)next;

                // 2. 调用带校验的赋值方法
                TrySetColumnMapping(colIdx, newMapType);
            }
            else if (e.Button == MouseButtons.Right)
            {
                ShowHeaderMenu(colIdx);
            }
        }

        /// <summary>
        /// 尝试设置列映射（包含数据抽样检查逻辑）
        /// </summary>
        private void TrySetColumnMapping(int colIndex, ImportHelper.TargetColumn newType)
        {
            // 1. 【无条件执行】先应用用户的选择 (用户最大)
            _columnMappings[colIndex] = newType;

            // 2. 清除该列之前的“嫌疑”状态
            _suspectColumns.Remove(colIndex);

            // 3. 执行静默检查
            if (newType != ImportHelper.TargetColumn.Ignore &&
                newType != ImportHelper.TargetColumn.Note)
            {
                // 调用之前的 CheckColumnValidity (返回错误信息字符串，如果为null说明通过)
                string warning = CheckColumnValidity(colIndex, newType);

                if (!string.IsNullOrEmpty(warning))
                {
                    // 发现问题，加入“嫌疑名单”，但不弹窗
                    _suspectColumns.Add(colIndex);
                }
            }

            // 4. 刷新全局状态和界面 (绘制逻辑会根据 _suspectColumns 变色)
            CheckStatus();
            UpdateHeadersStyle();
        }
        private void ShowHeaderMenu(int colIdx)
        {
            ContextMenuStrip ctx = new ContextMenuStrip();
            foreach (ImportHelper.TargetColumn type in Enum.GetValues(typeof(ImportHelper.TargetColumn)))
            {
                var item = ctx.Items.Add(ImportHelper.GetColumnText(type));
                item.Click += (s, e) =>
                {
                    TrySetColumnMapping(colIdx, type);
                };
                // 勾选当前状态
                if (_columnMappings.ContainsKey(colIdx) && _columnMappings[colIdx] == type)
                {
                    ((ToolStripMenuItem)item).Checked = true;
                }
            }
            ctx.Show(Cursor.Position);
        }

        // 绘制表头：显示映射结果
        private void DgvPreview_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex == -1 && e.ColumnIndex >= 0)
            {
                var map = _columnMappings.ContainsKey(e.ColumnIndex) ? _columnMappings[e.ColumnIndex] : ImportHelper.TargetColumn.Ignore;
                string originalText = dgvPreview.Columns[e.ColumnIndex].Tag?.ToString();

                // --- 1. 决定背景色 ---
                Brush backBrush;
                Color iconColor;
                string icon;

                if (_conflictColumns.Contains(e.ColumnIndex))
                {
                    // 优先级最高：冲突 (红)
                    backBrush = new SolidBrush(Color.MistyRose);
                    iconColor = Color.Red;
                    icon = "⛔";
                }
                else if (_suspectColumns.Contains(e.ColumnIndex))
                {
                    // 优先级第二：格式疑似不对 (橙/黄)
                    backBrush = new SolidBrush(Color.FromArgb(255, 245, 230)); // 淡橙色
                    iconColor = Color.DarkOrange;
                    icon = "⚠️";
                }
                else if (map != ImportHelper.TargetColumn.Ignore)
                {
                    // 优先级第三：正常 (绿)
                    backBrush = new SolidBrush(Color.FromArgb(220, 255, 220));
                    iconColor = Color.Green;
                    icon = "✅";
                }
                else
                {
                    // 优先级最低：忽略 (灰)
                    backBrush = new SolidBrush(Color.WhiteSmoke);
                    iconColor = Color.Gray;
                    icon = "▼";
                }

                e.Graphics.FillRectangle(backBrush, e.CellBounds);
                e.Paint(e.CellBounds, DataGridViewPaintParts.Border);

                // --- 2. 绘制文字 ---
                using (var brushText = new SolidBrush(Color.Black))
                using (var brushIcon = new SolidBrush(iconColor))
                using (var fontBold = new Font(e.CellStyle.Font, FontStyle.Bold))
                {
                    // 原始表头
                    e.Graphics.DrawString(originalText, e.CellStyle.Font, brushText, e.CellBounds.X + 4, e.CellBounds.Y + 4);

                    // 映射结果
                    if (map != ImportHelper.TargetColumn.Ignore)
                    {
                        string mapText = $"{icon} {ImportHelper.GetColumnText(map)}";
                        e.Graphics.DrawString(mapText, fontBold, brushIcon, e.CellBounds.X + 4, e.CellBounds.Y + 24); // Y坐标根据高度微调
                    }
                    else
                    {
                        e.Graphics.DrawString("▼ [忽略]", new Font(e.CellStyle.Font, FontStyle.Italic), Brushes.Gray, e.CellBounds.X + 4, e.CellBounds.Y + 24);
                    }
                }

                e.Handled = true;
                backBrush.Dispose();
            }
        }

        private void UpdateHeadersStyle()
        {
            // 简单触发重绘
            dgvPreview.Invalidate();
        }

       

       
        private void CheckStatus()
        {
            _conflictColumns.Clear();

            // 1. 检查重复列 (Conflict)
            var typeCounts = _columnMappings.Values
                .Where(t => t != ImportHelper.TargetColumn.Ignore && t != ImportHelper.TargetColumn.Note)
                .GroupBy(t => t)
                .ToDictionary(g => g.Key, g => g.Count());

            foreach (var kvp in typeCounts)
            {
                if (kvp.Value > 1)
                {
                    var indices = _columnMappings.Where(x => x.Value == kvp.Key).Select(x => x.Key);
                    foreach (var idx in indices) _conflictColumns.Add(idx);
                }
            }

            // 2. 检查关键列
            bool hasName = _columnMappings.ContainsValue(ImportHelper.TargetColumn.Name);
            bool hasAddr = _columnMappings.ContainsValue(ImportHelper.TargetColumn.Address);

            // 3. 生成提示信息 (优先级控制)
            if (_conflictColumns.Count > 0)
            {
                lblStatus.Text = "⛔ 错误：存在重复映射的列，请修正红色高亮列！";
                lblStatus.ForeColor = Color.Red;
                btnImport.Enabled = false; // 冲突必须修
            }
            else if (_suspectColumns.Count > 0)
            {
                // 格式不对只是警告，允许强行导入 (Enabled = true)
                lblStatus.Text = $"⚠️ 警告：第 {string.Join(",", _suspectColumns.Select(i => i + 1))} 列的数据格式似乎不正确，请检查橙色高亮列。";
                lblStatus.ForeColor = Color.DarkOrange;
                btnImport.Enabled = true;
            }
            else if (!hasName || !hasAddr)
            {
                List<string> missing = new List<string>();
                if (!hasName) missing.Add("[变量名称]");
                if (!hasAddr) missing.Add("[Modbus地址]");
                lblStatus.Text = $"ℹ️ 等待配置：缺少关键列 {string.Join(", ", missing)}";
                lblStatus.ForeColor = Color.Blue; // 或者深灰
                btnImport.Enabled = false;
            }
            else
            {
                lblStatus.Text = "✅ 映射配置正确，准备就绪。";
                lblStatus.ForeColor = Color.Green;
                btnImport.Enabled = true;
            }

            // 触发重绘
            dgvPreview.Invalidate();
        }

        // ================================================================
        // 4. 执行导入 (The Execution)
        // ================================================================
        private void BtnImport_Click(object sender, EventArgs e)
        {
            if (_allRows == null) return;

            int headerIndex = (int)numHeaderRow.Value - 1;

            // 1. 基础配置检查
            if (!_columnMappings.ContainsValue(ImportHelper.TargetColumn.Name) ||
                !_columnMappings.ContainsValue(ImportHelper.TargetColumn.Address))
            {
                MessageBox.Show("错误：必须映射 [变量名称] 和 [Modbus地址] 列！", "缺少关键列", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.DialogResult = DialogResult.None;
                return;
            }

            // 2. 【核心】数据质量预检 (Validation)
            var report = ValidateData((int)numHeaderRow.Value - 1);

            // 如果有问题，弹出报告窗口
            if (report.HasErrors)
            {
                using (var frmReport = new F_ValidationReport(
                    report.ValidCount, report.WarningCount, report.CriticalCount, report.SampleMsg))
                {
                    if (frmReport.ShowDialog() != DialogResult.OK)
                    {
                        // 用户点击了“返回修改”
                        this.DialogResult = DialogResult.None;
                        return;
                    }
                }
            }

            // 3. 正式执行导入 (逻辑复用之前的，只是要把错误行跳过)
            ExecuteImport(headerIndex, report.InvalidRowIndices);
        }
        private void ExecuteImport(int headerIndex, HashSet<int> skipIndices)
        {
            ResultPoints.Clear();

            // 遍历所有数据行 (从表头下一行开始)
            for (int i = headerIndex + 1; i < _allRows.Count; i++)
            {
                // 1. 跳过预检中标记为错误的行
                if (skipIndices.Contains(i)) continue;

                var dataRow = _allRows[i] as IDictionary<string, object>;
                if (dataRow == null) continue;

                var keys = dataRow.Keys.ToList(); // 获取列索引键 (A, B, C...)

                var p = new ModbusPoint();
                p.DataFormat = DataFormat.ABCD; // 默认字节序
                p.Factor = 1.0f;                // 默认系数
                p.Offset = 0.0f;                // 默认偏移

                // --- 关键变量：暂存区域判定结果 ---
                // 策略：优先信赖用户显式映射的 [存储区] 列
                // 如果没映射或识别失败，再回退使用 [地址] 列推断出的结果
                StorageZone? zoneFromCol = null;   // 显式解析结果
                StorageZone zoneFromAddr = StorageZone.HoldingRegister_4x; // 推断结果 (默认4x)

                // 2. 遍历该行的每一列
                for (int c = 0; c < keys.Count; c++)
                {
                    // 如果这一列没有被映射，跳过
                    if (!_columnMappings.ContainsKey(c)) continue;

                    var mapType = _columnMappings[c];
                    if (mapType == ImportHelper.TargetColumn.Ignore) continue;

                    // 安全获取单元格内容
                    string val = dataRow[keys[c]]?.ToString();

                    switch (mapType)
                    {
                        case ImportHelper.TargetColumn.Name:
                            p.Name = val;
                            break;

                        case ImportHelper.TargetColumn.Unit:
                            p.Unit = val;
                            break;

                        case ImportHelper.TargetColumn.Note:
                            p.Note = val;
                            break;

                        case ImportHelper.TargetColumn.DataType:
                            // 智能识别类型 (REAL -> Float, BOOL -> Bool)
                            // 注意：这里不再进行 TryParse 校验，直接尽力解析，解析不了回退 Int16
                            p.DataType = ImportHelper.ParseDataType(val);
                            break;

                        case ImportHelper.TargetColumn.Address:
                            // 智能解析地址 (如 "40001" -> Addr=0, Zone=4x)
                            var addrInfo = ImportHelper.ParseAddress(val);
                            p.Address = addrInfo.Addr;

                            // 暂存推断出的区域 (作为备胎)
                            zoneFromAddr = addrInfo.Zone;
                            break;

                        case ImportHelper.TargetColumn.Zone:
                            // 尝试解析显式指定的存储区文本 (如 "RW", "0x", "Coil")
                            var parsedZone = ImportHelper.ParseStorageZone(val);
                            if (parsedZone.HasValue)
                            {
                                zoneFromCol = parsedZone.Value; // 暂存显式结果
                            }
                            break;

                        case ImportHelper.TargetColumn.BitIndex:
                            if (int.TryParse(val, out int bitIdx))
                                p.BitIndex = bitIdx;
                            break;

                        case ImportHelper.TargetColumn.Factor:
                            if (float.TryParse(val, out float factor))
                                p.Factor = factor;
                            break;

                        case ImportHelper.TargetColumn.Offset:
                            if (float.TryParse(val, out float offset))
                                p.Offset = offset;
                            break;
                    }
                }

                if (p.DataType == DataType.Int32 || p.DataType == DataType.Float) p.DataFormat = DataFormat.CDAB;

                // 3. 最终决策：确定存储区
                if (zoneFromCol.HasValue)
                {
                    p.Zone = zoneFromCol.Value; // 显式指定优先 (User Override)
                }
                else
                {
                    p.Zone = zoneFromAddr; // 否则使用地址推断结果 (Auto Infer)
                }

                // 4. 有效性检查：只有名称不为空才添加 (防止Excel末尾有空行)
                if (!string.IsNullOrWhiteSpace(p.Name))
                {
                    ResultPoints.Add(p);
                }
            }

            this.DialogResult = DialogResult.OK;
            this.Close();
        }
        private ValidationResult ValidateData(int headerIndex)
        {
            var res = new ValidationResult();
            int sampleLimit = 5;

            // 遍历数据行
            for (int i = headerIndex + 1; i < _allRows.Count; i++)
            {
                var dataRow = _allRows[i] as IDictionary<string, object>;
                var keys = dataRow.Keys.ToList();

                bool isRowInvalid = false;
                string rowError = "";

                // --- 检查地址 ---
                // 找到映射为 Address 的列
                int addrColIdx = _columnMappings.FirstOrDefault(x => x.Value == ImportHelper.TargetColumn.Address).Key;
                if (addrColIdx < keys.Count)
                {
                    string valAddr = dataRow[keys[addrColIdx]]?.ToString();
                    // 检查地址是否为空
                    if (string.IsNullOrWhiteSpace(valAddr))
                    {
                        isRowInvalid = true;
                        rowError = "地址为空";
                    }
                    else
                    {
                        // 尝试解析地址
                        var addrInfo = ImportHelper.ParseAddress(valAddr);
                        // 这里的校验逻辑：ParseAddress 如果完全解析失败会返回 4x, 0。
                        // 严格来说很难判定解析失败（因为 0 也是合法地址），除非原来的字符串是乱码
                        // 我们可以简单判断：如果输入不为空，但解析出来是0，且原字符串不像0，则报警
                        if (addrInfo.Addr == 0 && valAddr.Trim() != "0" && !valAddr.Contains("40001"))
                        {
                            // 这是一个弱校验，视情况开启
                        }
                    }
                }

                // --- 检查数据类型 ---
                int typeColIdx = _columnMappings.FirstOrDefault(x => x.Value == ImportHelper.TargetColumn.DataType).Key;
                if (typeColIdx > 0 && typeColIdx < keys.Count) // 有这一列才检查
                {
                    string valType = dataRow[keys[typeColIdx]]?.ToString();
                    if (!string.IsNullOrWhiteSpace(valType))
                    {
                        if (!ImportHelper.TryParseDataType(valType, out _))
                        {
                            // 类型解析失败，这不算严重错误，默认 Int16，但要警告
                            res.WarningCount++;
                            if (res.WarningCount <= sampleLimit) res.SampleMsg += $"行 {i + 1}: 未知类型 '{valType}' (将默认为 Int16)\n";
                        }
                    }
                }

                // --- 检查存储区 (Zone) ---
                int zoneColIdx = _columnMappings.FirstOrDefault(x => x.Value == ImportHelper.TargetColumn.Zone).Key;
                // 如果存在 Zone 列映射
                if (zoneColIdx > 0 && zoneColIdx < keys.Count && _columnMappings.ContainsValue(ImportHelper.TargetColumn.Zone))
                {
                    string valZone = dataRow[keys[zoneColIdx]]?.ToString();
                    if (!string.IsNullOrWhiteSpace(valZone))
                    {
                        // 如果解析结果为 null，说明无法识别
                        if (ImportHelper.ParseStorageZone(valZone) == null)
                        {
                            res.WarningCount++;
                            if (res.WarningCount <= sampleLimit)
                                res.SampleMsg += $"行 {i + 1}: 无法识别存储区 '{valZone}' (将使用地址推断)\n";
                        }
                    }
                }

                // --- 统计结果 ---
                if (isRowInvalid)
                {
                    res.CriticalCount++;
                    res.InvalidRowIndices.Add(i);
                    if (res.CriticalCount <= sampleLimit) res.SampleMsg += $"行 {i + 1}: {rowError}\n";
                }
                else
                {
                    res.ValidCount++;
                }
            }

            return res;
        }

        /// <summary>
        /// 抽样检查某一列的数据是否符合目标类型的特征
        /// </summary>
        /// <param name="colIndex">列索引</param>
        /// <param name="targetType">目标映射类型</param>
        /// <returns>如果不符合，返回警告信息；符合则返回 null</returns>
        private string CheckColumnValidity(int colIndex, ImportHelper.TargetColumn targetType)
        {
            if (_allRows == null || _allRows.Count == 0) return null;

            int headerIndex = (int)numHeaderRow.Value - 1;
            int checkCount = 0;
            int validCount = 0;
            int maxSample = 20; // 只检查前 20 行有效数据，保证速度

            for (int i = headerIndex + 1; i < _allRows.Count; i++)
            {
                if (checkCount >= maxSample) break;

                var dataRow = _allRows[i] as IDictionary<string, object>;
                var keys = dataRow.Keys.ToList();

                // 获取单元格内容
                string val = (colIndex < keys.Count) ? dataRow[keys[colIndex]]?.ToString() : "";
                if (string.IsNullOrWhiteSpace(val)) continue; // 跳过空单元格

                checkCount++;
                bool isValid = false;

                // 根据类型进行针对性检查
                switch (targetType)
                {
                    case ImportHelper.TargetColumn.Zone:
                        // 检查是否包含 0x, 4x, RW, Coil 等关键字
                        isValid = ImportHelper.ParseStorageZone(val).HasValue;
                        break;

                    case ImportHelper.TargetColumn.DataType:
                        // 严格检查是否包含 INT, BOOL, FLOAT 等关键字
                        isValid = ImportHelper.TryParseDataType(val, out _);
                        break;

                    case ImportHelper.TargetColumn.Address:
                        // 检查是否包含数字
                        // 简单的数字判断，或者解析结果 > 0
                        var addrInfo = ImportHelper.ParseAddress(val);
                        // 只要不是解析失败的默认值(4x, 0) 或者 原始值就是0，就算通过
                        // 这里简单判：只要字符串里有数字就行
                        isValid = System.Text.RegularExpressions.Regex.IsMatch(val, @"\d+");
                        break;
                    case ImportHelper.TargetColumn.BitIndex:
                        // 必须是整数，且通常在 0-15 之间 (Modbus 16位寄存器)
                        if (int.TryParse(val, out int bitIdx))
                        {
                            isValid = (bitIdx >= 0 && bitIdx <= 15);
                        }
                        else
                        {
                            isValid = false;
                        }
                        break;
                    default:
                        isValid = true; // 其他列（如名称、备注）不做检查
                        break;
                }

                if (isValid) validCount++;
            }

            // 判定标准：如果有效样本中，成功率低于 50%，则报警
            if (checkCount > 0 && (double)validCount / checkCount < 0.5)
            {
                string typeName = ImportHelper.GetColumnText(targetType);
                return $"该列数据看起来不像 [{typeName}]。\n" +
                       $"在前 {checkCount} 行数据中，仅有 {validCount} 行符合格式。\n\n" +
                       "是否确定要应用此映射？";
            }

            return null; // 检查通过
        }
    }
}