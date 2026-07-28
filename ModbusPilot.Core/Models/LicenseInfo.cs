using System;
using System.Collections.Generic;
using System.Text;

namespace ModbusPilot.Core.Models
{
    public class LicenseInfo
    {
        public string MachineCode { get; set; }      // 该授权绑定的机器码
        public DateTime ExpiryDate { get; set; }    // 到期时间 (9999-12-31 表示永久)
        public List<string> GrantedFeatures { get; set; } // 授权的功能列表 (如 ["ExcelExport", "TrendPro"])
        public string IssuedTo { get; set; }        // 授权给谁 (客户名)
    }
}
