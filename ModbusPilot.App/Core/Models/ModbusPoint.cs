using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Newtonsoft.Json;

namespace ModbusPilot.Core.Models
{
    /// <summary>
    /// 存储区类型 (决定了功能码)
    /// </summary>
    public enum StorageZone
    {
        // 对应功能码 01, 05, 15
        CoilStatus_0x,          // 线圈 (可读写 bool)

        // 对应功能码 02
        InputStatus_1x,         // 离散输入 (只读 bool)

        // 对应功能码 04
        InputRegister_3x,       // 输入寄存器 (只读 word)

        // 对应功能码 03, 06, 16
        HoldingRegister_4x      // 保持寄存器 (可读写 word)
    }

    /// <summary>
    /// 数据类型 (决定了长度和解析方式)
    /// </summary>
    public enum DataType
    {
        Bool,       // 1位 (可以是线圈，也可以是寄存器里的某一位)
        Int16,      // 16位 有符号
        UInt16,     // 16位 无符号
        Int32,      // 32位 有符号 (需要拼2个寄存器)
        UInt32,     // 32位 无符号
        Float,      // 32位 浮点数 (需要拼2个寄存器)
        Double,     // 64位 双精度 (需要拼4个寄存器)
        //String,     // 字符串
    }

    /// <summary>
    /// 字节序 (解决大小端问题 ABCD, CDAB...)
    /// </summary>
    public enum DataFormat
    {
        ABCD, // Big-Endian (标准 Modbus)
        CDAB, // Byte Swap (常见于某些 PLC 中间件)
        BADC, // Word Swap (小端)
        DCBA  // Little-Endian (完全反转)
    }
    /// <summary>
    /// 核心点位定义
    /// 实现了 INotifyPropertyChanged 接口，是为了让 UI 能自动感知数据变化
    /// </summary>
    public class ModbusPoint : INotifyPropertyChanged
    {
        // ==========================
        // 场景一：定义配置属性
        // ==========================

        public string Name { get; set; } = "New_Point";

        /// <summary>
        /// 存储区：决定了功能码。
        /// 如果是 CoilStatus_0x，驱动层会自动用 FC01/FC05。
        /// 如果是 HoldingRegister_4x，驱动层会自动用 FC03/FC06/FC16。
        /// </summary>
        public StorageZone Zone { get; set; } = StorageZone.HoldingRegister_4x;

        /// <summary>
        /// 物理地址 (比如 1, 40001)
        /// </summary>
        public int Address { get; set; } = 1;

        /// <summary>
        /// 数据类型
        /// </summary>
        public DataType DataType { get; set; } = DataType.Int16;

        /// <summary>
        /// 字节序：仅当数据跨越多个寄存器(Float/Int32)时有效
        /// </summary>
        public DataFormat DataFormat { get; set; } = DataFormat.ABCD;

        /// <summary>
        /// 位索引 (0-15)：
        /// 专用于解决 "寄存器里的某一位" 这种情况。
        /// 如果 Zone 是 0x/1x，此字段无效（或忽略）。
        /// 如果 Zone 是 4x 且 DataType 是 Bool，此字段必填。
        /// </summary>
        public int? BitIndex { get; set; } = null;

        // 辅助显示属性
        public string Unit { get; set; } = ""; // 单位
        public string Note { get; set; } = ""; // 备注
        public float Factor { get; set; } = 1.0f;
        public float Offset { get; set; } = 0.0f;

        // ==========================
        // 场景二：运行时数据存储
        // ==========================

        private object _currentValue;

        /// <summary>
        /// 实时值。
        /// [JsonIgnore] 表示保存配置文件时，不保存这个值。
        /// </summary>
        [JsonIgnore]
        public object CurrentValue
        {
            get => _currentValue;
            set
            {
                if (_currentValue != value)
                {
                    _currentValue = value;
                    OnPropertyChanged(nameof(CurrentValue)); // 通知 UI 刷新
                }
            }
        }

        // 记录最后一次更新时间，判断通讯是否超时
        [JsonIgnore]
        public DateTime LastUpdateTime { get; set; }

        // ==========================
        // MVVM 事件通知实现
        // ==========================
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public override bool Equals(object obj)
        {
            if (obj is ModbusPoint other)
            {
                // 只有 存储区、协议地址、位索引 均相同时，才认为是同一个物理点位
                return this.Zone == other.Zone &&
                       this.Address == other.Address &&
                       this.BitIndex == other.BitIndex;
            }
            return false;
        }

        public override int GetHashCode()
        {
            // 组合哈希值
            return (Zone, Address, BitIndex).GetHashCode();
        }
    }
}
