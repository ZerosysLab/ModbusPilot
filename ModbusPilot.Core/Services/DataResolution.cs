using System;
using System.Linq;
using ModbusPilot.Core.Models; // 确保引用了你的 Model

namespace ModbusPilot.Core.Services
{
    public static class DataResolution
    {
        /// <summary>
        /// 核心解析方法：将报文原始字节转换为最终的工程数值
        /// </summary>
        /// <param name="message">完整的 Modbus 响应报文的数据段 (去掉了头部)</param>
        /// <param name="cmd">发送的指令 (包含起始地址等上下文)</param>
        /// <param name="point">要解析的点位配置</param>
        /// <returns>解析后的最终值 (object 类型，可能是 bool, float, double 等)</returns>
        public static object Parse(byte[] message, ModbusCommand cmd, ModbusPoint point)
        {
            try
            {
                // 1. 根据存储区类型分流
                if (point.Zone == StorageZone.CoilStatus_0x || point.Zone == StorageZone.InputStatus_1x)
                {
                    return ParseBit(message, cmd, point);
                }
                else
                {
                    return ParseRegister(message, cmd, point);
                }
            }
            catch (Exception)
            {
                // 解析失败返回 null 或 特定错误值，由 UI 处理显示
                return null;
            }
        }

        // --- 1. 线圈/离散输入解析 (Bool) ---
        private static bool ParseBit(byte[] data, ModbusCommand cmd, ModbusPoint point)
        {
            // 计算该点位相对于指令起始地址的偏移量
            int offset = point.Address - cmd.StartAddress;
            if (offset < 0) return false;

            // 定位到具体的字节索引 (byteIndex) 和 位索引 (bitIndex)
            int byteIndex = offset / 8;
            int bitInByte = offset % 8;

            if (byteIndex >= data.Length) return false;

            // 提取位：(字节 >> 位移) & 1
            return ((data[byteIndex] >> bitInByte) & 1) == 1;
        }

        // --- 2. 寄存器解析 (数值) ---
        private static object ParseRegister(byte[] data, ModbusCommand cmd, ModbusPoint point)
        {
            // 寄存器偏移 (每个寄存器占2个字节)
            int offsetWords = point.Address - cmd.StartAddress;
            int startByteIndex = offsetWords * 2;

            // 根据数据类型决定读取字节长度
            int byteLength = GetByteLength(point.DataType);

            // 越界检查
            if (startByteIndex + byteLength > data.Length) return 0;

            // A. 【提取】 复制出相关的字节
            byte[] buffer = new byte[byteLength];
            Array.Copy(data, startByteIndex, buffer, 0, byteLength);

            // B. 【重排】 根据字节序调整 (DataFormat)
            // 注意：BitConverter 依赖系统架构，Windows 通常是 Little-Endian
            // 我们需要把 buffer 调整为系统能识别的顺序
            buffer = HandleByteOrder(buffer, point.DataFormat);

            // C. 【转换】 原始字节 -> 原始数值
            double rawValue = 0;
            object finalValue = null;

            switch (point.DataType)
            {
                case DataType.Int16:
                    rawValue = BitConverter.ToInt16(buffer, 0);
                    finalValue = ApplyScaling(rawValue, point);
                    break;
                case DataType.UInt16:
                    rawValue = BitConverter.ToUInt16(buffer, 0);
                    finalValue = ApplyScaling(rawValue, point);
                    break;
                case DataType.Int32:
                    rawValue = BitConverter.ToInt32(buffer, 0);
                    finalValue = ApplyScaling(rawValue, point);
                    break;
                case DataType.UInt32:
                    rawValue = BitConverter.ToUInt32(buffer, 0);
                    finalValue = ApplyScaling(rawValue, point);
                    break;
                case DataType.Float:
                    rawValue = BitConverter.ToSingle(buffer, 0);
                    finalValue = ApplyScaling(rawValue, point);
                    break;
                case DataType.Double:
                    rawValue = BitConverter.ToDouble(buffer, 0);
                    finalValue = ApplyScaling(rawValue, point);
                    break;              
                case DataType.Bool:
                    // 寄存器里的 Bool，需要看 BitIndex
                    if (point.BitIndex.HasValue)
                    {
                        // 寄存器通常是 2字节。Modbus大端传输，Buffer[0]是高位，Buffer[1]是低位
                        // 这里我们使用原始字节(未Swap)来解位比较安全，或者统一用 UInt16 解
                        // 简单处理：取出一个 UInt16，然后移位
                        // 重新取2字节不做Swap处理，按 Modbus 标准 BigEndian 逻辑
                        byte bHigh = data[startByteIndex];
                        byte bLow = data[startByteIndex + 1];
                        ushort wordVal = (ushort)((bHigh << 8) | bLow);

                        return ((wordVal >> point.BitIndex.Value) & 1) == 1;
                    }
                    return false;
            }

            return finalValue;
        }

        // --- 线性变换 ( y = kx + b ) ---
        private static object ApplyScaling(double raw, ModbusPoint p)
        {
            // 如果系数是 1 且 偏移是 0，直接返回原始值 (保持类型纯净)
            // 但为了统一，UI 显示时通常都转为 double
            double val = (raw * p.Factor) + p.Offset;

            // 这里我们返回 double，UI 层再决定保留几位小数 (ToString("F2"))
            return val;
        }

        // --- 辅助：获取类型占用的字节数 ---
        public static int GetByteLength(DataType type)
        {
            switch (type)
            {
                case DataType.Bool: return 2; // 寄存器里的Bool占1个字
                case DataType.Int16:
                case DataType.UInt16: return 2;
                case DataType.Int32:
                case DataType.UInt32:
                case DataType.Float: return 4;
                case DataType.Double: return 8;
                default: return 2;
            }
        }

        // --- 核心：字节序处理 ---
        /// <summary>
        /// 将不同设备格式的字节数组，转换为当前系统(Windows/Intel)可识别的 Little-Endian 数组
        /// </summary>
        private static byte[] HandleByteOrder(byte[] input, DataFormat format)
        {
            // 如果 input 长度不对(比如解析 string 失败)，直接返回
            if (input == null || input.Length == 0) return input;

            byte[] output = new byte[input.Length];

            // 假设当前系统是 Little Endian (绝大多数 PC 都是)
            // 我们需要把 input 里的字节搬运到 output 的正确位置

            if (input.Length == 2) // 16位 (Int16, UInt16)
            {
                // Modbus 标准是 Big-Endian (AB)
                // ABCD (0) = Big Endian -> 需要转为 BA (Little Endian)
                // DCBA (3) = Little Endian -> 保持 AB

                if (format == DataFormat.ABCD || format == DataFormat.CDAB)
                {
                    // 输入: [High, Low] -> 输出(系统LE): [Low, High]
                    output[0] = input[1];
                    output[1] = input[0];
                }
                else // DCBA or BADC (假设设备发来就是 Little Endian)
                {
                    output[0] = input[0];
                    output[1] = input[1];
                }
            }
            else if (input.Length == 4) // 32位 (Int32, Float)
            {
                // 输入索引: 0 1 2 3
                switch (format)
                {
                    case DataFormat.ABCD: // Big Endian (Standard Modbus)
                        // 设备: A B C D
                        // 系统(LE)需: D C B A
                        output[0] = input[3];
                        output[1] = input[2];
                        output[2] = input[1];
                        output[3] = input[0];
                        break;

                    case DataFormat.CDAB: // Word Swap (Common in Modbus)
                        // 设备: C D A B
                        // 系统(LE)需: D C B A -> 也就是 input[1] input[0] input[3] input[2]
                        output[0] = input[1];
                        output[1] = input[0];
                        output[2] = input[3];
                        output[3] = input[2];
                        break;

                    case DataFormat.BADC: // Byte Swap
                        // 设备: B A D C
                        // 系统(LE)需: D C B A -> 也就是 input[3] input[2] input[1] input[0] (Reverse of Reverse?)
                        // 让我们推导一下：
                        // Value 0x01020304.
                        // BigEndian: 01 02 03 04
                        // BADC:      02 01 04 03
                        // Target(LE): 04 03 02 01
                        // 映射关系: Out[0]=In[2], Out[1]=In[3], Out[2]=In[0], Out[3]=In[1]
                        output[0] = input[2];
                        output[1] = input[3];
                        output[2] = input[0];
                        output[3] = input[1];
                        break;

                    case DataFormat.DCBA: // Little Endian
                        // 设备: D C B A
                        // 系统(LE)需: D C B A (No Change)
                        Array.Copy(input, output, 4);
                        break;
                }
            }
            else if (input.Length == 8) // 64位 (Double)
            {
                // Double 的字节序更复杂，这里只处理最常见的 ABCD (Big) 和 DCBA (Little)
                if (format == DataFormat.ABCD || format == DataFormat.CDAB)
                {
                    // 简单粗暴：反转整个数组变为 Little Endian
                    Array.Copy(input, output, 8);
                    Array.Reverse(output);
                }
                else
                {
                    Array.Copy(input, output, 8);
                }
                // 注：64位的 CDAB/BADC 变种较少见，暂按全反转处理，实际遇到特殊设备再加 Case
            }

            return output;
        }
    }
}