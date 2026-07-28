using System;
using System.Collections.Generic;
using System.Linq;
using ModbusPilot.Core.Models;

namespace ModbusPilot.Core.Services
{
    /// <summary>
    /// 辅助类：用于传递待写入的点位和值
    /// </summary>
    public class WriteItem
    {
        public ModbusPoint Point { get; set; }
        public object Value { get; set; }
    }

    public static class WriteCommandPacker
    {
        /// <summary>
        /// 将多个散乱的写入请求打包成最少数量的 Modbus 写指令 (FC15/FC16)
        /// </summary>
        public static List<ModbusCommand> Pack(byte slaveId, List<WriteItem> items)
        {
            var result = new List<ModbusCommand>();
            if (items == null || items.Count == 0) return result;

            // 1. 按存储区分组 (只处理 0x 和 4x，只读区忽略)
            var coilItems = items.Where(x => x.Point.Zone == StorageZone.CoilStatus_0x)
                                 .OrderBy(x => x.Point.Address).ToList();

            var regItems = items.Where(x => x.Point.Zone == StorageZone.HoldingRegister_4x)
                                .OrderBy(x => x.Point.Address).ToList();

            // 2. 执行打包
            if (coilItems.Any()) result.AddRange(PackCoils(slaveId, coilItems));
            if (regItems.Any()) result.AddRange(PackRegisters(slaveId, regItems));

            return result;
        }

        // --- 打包寄存器 (FC16) ---
        private static List<ModbusCommand> PackRegisters(byte slaveId, List<WriteItem> items)
        {
            var cmds = new List<ModbusCommand>();
            if (items.Count == 0) return cmds;

            // 初始化第一个包
            var currentBuffer = new List<byte>();
            int startAddr = items[0].Point.Address;
            int currentAddrPtr = startAddr; // 当前期望的下一个地址指针

            foreach (var item in items)
            {
                // A. 编码当前值 (利用之前写的 ValueEncoder，但只要字节不要FC)
                byte[] itemBytes = ValueEncoder.Encode(item.Value, item.Point, out _);

                // 计算该数据占几个寄存器 (字节数 / 2)
                int itemRegCount = itemBytes.Length / 2;

                // B. 检查连续性 (当前地址 vs 期望地址)
                // 如果地址不连续，或者这是一个"写入位"的操作(不支持打包)，则断开
                if (item.Point.Address != currentAddrPtr)
                {
                    // 封包上一个
                    cmds.Add(CreateFC16(slaveId, startAddr, currentBuffer.ToArray()));

                    //以此开始新包
                    currentBuffer.Clear();
                    startAddr = item.Point.Address;
                    currentAddrPtr = startAddr;
                }

                // C. 追加数据
                currentBuffer.AddRange(itemBytes);
                currentAddrPtr += itemRegCount; // 指针后移
            }

            // 封包最后一个
            if (currentBuffer.Count > 0)
            {
                cmds.Add(CreateFC16(slaveId, startAddr, currentBuffer.ToArray()));
            }

            return cmds;
        }

        // --- 打包线圈 (FC15) ---
        private static List<ModbusCommand> PackCoils(byte slaveId, List<WriteItem> items)
        {
            var cmds = new List<ModbusCommand>();
            if (items.Count == 0) return cmds;

            // 线圈打包稍微复杂，需要把 bool 塞进 byte 的位里
            // 策略：先收集连续的 bool 列表，最后统一转 byte[]

            var currentBools = new List<bool>();
            int startAddr = items[0].Point.Address;
            int nextAddr = startAddr;

            foreach (var item in items)
            {
                // 转换值为 bool
                bool val = Convert.ToBoolean(item.Value); // 简单转换，实际建议加 TryCatch

                // 检查连续性
                if (item.Point.Address != nextAddr)
                {
                    // 封包
                    cmds.Add(CreateFC15(slaveId, startAddr, currentBools));

                    // 新包
                    currentBools.Clear();
                    startAddr = item.Point.Address;
                    nextAddr = startAddr;
                }

                currentBools.Add(val);
                nextAddr++;
            }

            // 封包最后一个
            if (currentBools.Count > 0)
            {
                cmds.Add(CreateFC15(slaveId, startAddr, currentBools));
            }

            return cmds;
        }

        // --- 辅助构建 FC16 指令 ---
        private static ModbusCommand CreateFC16(byte slaveId, int startAddr, byte[] payload)
        {
            return new ModbusCommand
            {
                SlaveId = slaveId,
                FunctionCode = 0x10, // FC16 Write Multiple Registers
                StartAddress = startAddr,
                Count = payload.Length / 2, // 寄存器数量
                WritePayload = payload,
                ResultStatus = CommStatus.Pending
            };
        }

        // --- 辅助构建 FC15 指令 (Bool List -> Byte Array) ---
        private static ModbusCommand CreateFC15(byte slaveId, int startAddr, List<bool> bools)
        {
            // 计算需要的字节数 (N / 8 向上取整)
            int byteCount = (bools.Count + 7) / 8;
            byte[] payload = new byte[byteCount];

            for (int i = 0; i < bools.Count; i++)
            {
                if (bools[i])
                {
                    int byteIdx = i / 8;
                    int bitIdx = i % 8;
                    payload[byteIdx] |= (byte)(1 << bitIdx);
                }
            }

            return new ModbusCommand
            {
                SlaveId = slaveId,
                FunctionCode = 0x0F, // FC15 Write Multiple Coils
                StartAddress = startAddr,
                Count = bools.Count, // 线圈数量
                WritePayload = payload,
                ResultStatus = CommStatus.Pending
            };
        }
    }
}