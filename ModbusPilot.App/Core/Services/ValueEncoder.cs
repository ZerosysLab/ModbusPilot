using System;
using ModbusPilot.Core.Models;

namespace ModbusPilot.Core.Services
{
    public static class ValueEncoder
    {
        public static byte[] Encode(object value, ModbusPoint point, out byte targetFc)
        {
            targetFc = 0;

            // 1. 预处理输入
            bool boolVal = false;
            double doubleVal = 0;
            bool isBoolType = point.DataType == DataType.Bool;

            try
            {
                if (isBoolType)
                {
                    string s = value.ToString().ToLower();
                    boolVal = (s == "true" || s == "1" || s == "on");
                }
                else
                {
                    doubleVal = Convert.ToDouble(value);
                }
            }
            catch
            {
                throw new ArgumentException($"输入格式错误: '{value}'");
            }

            // 2. 根据存储区处理
            if (point.Zone == StorageZone.CoilStatus_0x)
            {
                targetFc = 0x05;
                return boolVal ? new byte[] { 0xFF, 0x00 } : new byte[] { 0x00, 0x00 };
            }
            else if (point.Zone == StorageZone.HoldingRegister_4x)
            {
                // === A. 逆向线性变换 ===
                // 使用局部变量，绝对不要修改 point 对象本身
                float k = point.Factor;
                float b = point.Offset;

                // 防止除以0
                if (Math.Abs(k) < 1e-6) k = 1;

                // 只有数值类型才需要变换
                if (!isBoolType)
                {
                    // 公式: Raw = (Val - Offset) / Factor
                    doubleVal = (doubleVal - b) / k;
                }

                // === B. 类型转换 (含精度修正) ===
                byte[] rawBytes = null;

                switch (point.DataType)
                {
                    case DataType.Int16:
                        targetFc = 0x06;
                        // 【关键修复】先 Round 再 Cast，防止 499.999 -> 499
                        rawBytes = BitConverter.GetBytes((short)Math.Round(doubleVal));
                        break;
                    case DataType.UInt16:
                        targetFc = 0x06;
                        rawBytes = BitConverter.GetBytes((ushort)Math.Round(doubleVal));
                        break;

                    // 32位/64位无需 Round，直接转保持精度
                    case DataType.Int32:
                        targetFc = 0x10;
                        rawBytes = BitConverter.GetBytes((int)Math.Round(doubleVal));
                        break;
                    case DataType.UInt32:
                        targetFc = 0x10;
                        rawBytes = BitConverter.GetBytes((uint)Math.Round(doubleVal));
                        break;
                    case DataType.Float:
                        targetFc = 0x10;
                        rawBytes = BitConverter.GetBytes((float)doubleVal);
                        break;
                    case DataType.Double:
                        targetFc = 0x10;
                        rawBytes = BitConverter.GetBytes((double)doubleVal);
                        break;

                    case DataType.Bool:
                        throw new NotSupportedException("暂不支持写入寄存器内的位");
                }

                // === C. 字节序处理 ===
                return HandleByteOrder(rawBytes, point.DataFormat);
            }
            else
            {
                throw new InvalidOperationException("只读区 (1x/3x) 不允许写入");
            }
        }

        private static byte[] HandleByteOrder(byte[] input, DataFormat format)
        {
            if (input == null) return null;
            byte[] output = new byte[input.Length];

            if (input.Length == 2)
            {
                if (format == DataFormat.ABCD || format == DataFormat.CDAB)
                {
                    // PC(LE) -> Device(BE)
                    output[0] = input[1]; output[1] = input[0];
                }
                else Array.Copy(input, output, 2);
            }
            else if (input.Length == 4)
            {
                switch (format)
                {
                    case DataFormat.ABCD:
                        Array.Copy(input, output, 4); Array.Reverse(output); break;
                    case DataFormat.CDAB:
                        output[0] = input[1]; output[1] = input[0];
                        output[2] = input[3]; output[3] = input[2]; break;
                    case DataFormat.BADC:
                        output[0] = input[2]; output[1] = input[3];
                        output[2] = input[0]; output[3] = input[1]; break;
                    case DataFormat.DCBA:
                        Array.Copy(input, output, 4); break;
                }
            }
            else if (input.Length == 8 && (format == DataFormat.ABCD || format == DataFormat.CDAB))
            {
                Array.Copy(input, output, 8); Array.Reverse(output);
            }
            else if (input.Length == 8) Array.Copy(input, output, 8);

            return output;
        }
    }
}