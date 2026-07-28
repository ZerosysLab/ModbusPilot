using MiniExcelLibs.Attributes; // 引用 MiniExcel

namespace ModbusPilot.Core.Models
{
    public class PointImportModel
    {
        [ExcelColumnName("变量名称")]
        public string Name { get; set; }

        [ExcelColumnName("存储区")]
        public string Zone { get; set; } // 0x, 1x, 3x, 4x

        [ExcelColumnName("地址")]
        public string Address { get; set; } // 建议导出 PLC 格式 (40001)，用户易读

        [ExcelColumnName("数据类型")]
        public string DataType { get; set; }

        [ExcelColumnName("位索引")]
        public string BitIndex { get; set; } // 可空

        [ExcelColumnName("单位")]
        public string Unit { get; set; }

        [ExcelColumnName("系数")]
        public double Factor { get; set; }

        [ExcelColumnName("偏移")]
        public double Offset { get; set; }

        [ExcelColumnName("字节序")]
        public string DataFormat { get; set; }

        [ExcelColumnName("备注")]
        public string Note { get; set; }
    }
}