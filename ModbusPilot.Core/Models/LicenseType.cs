using System;
using System.Collections.Generic;
using System.Text;

namespace ModbusPilot.Core.Models
{
    public enum LicenseType { Free, Professional }

    public class LicenseStatus
    {
        public LicenseType Type { get; set; } = LicenseType.Free;
        public DateTime ExpiryDate { get; set; } = DateTime.MinValue;
        public string Message { get; set; } = "未激活";
        public bool IsValid => Type == LicenseType.Professional && ExpiryDate > DateTime.Now;
    }
}
