using ModbusPilot.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ModbusPilot.Core.Services
{
    public class CommandPacker
    {
        // 最大允许的地址空洞（为了效率，中间断开几个地址我们可以连起来一起读）
        private const int MAX_GAP = 10;
        // Modbus 协议单包最大通常限制在 120-125 个寄存器
        private const int MAX_READ_COUNT = 100;

        /// <summary>
        /// 核心方法：将散乱的点位列表，打包成最优的指令列表
        /// </summary>
        public static List<ModbusCommand> Pack(List<ModbusPoint> points)
        {
            var commands = new List<ModbusCommand>();

            // 1. 先按 站号(SlaveId) 分组
            // (目前我们Device类还没完全整合，假设Point里后续会包含SlaveId信息，
            // 或者这只是针对单设备的打包。这里暂且假设所有点位属于同一个 SlaveId = 1)
            byte currentSlaveId = 1;

            // 2. 再按 存储区(Zone) 分组 (0x, 1x, 3x, 4x 不能混着读)
            var zoneGroups = points.GroupBy(p => p.Zone);

            foreach (var group in zoneGroups)
            {
                // 3. 组内按 地址 排序
                var sortedPoints = group.OrderBy(p => p.Address).ToList();

                if (sortedPoints.Count == 0) continue;

                // 4. 开始打包算法
                var currentCmd = new ModbusCommand
                {
                    SlaveId = currentSlaveId,
                    FunctionCode = GetReadFunctionCode(group.Key),
                    StartAddress = sortedPoints[0].Address,
                    Count = 0,
                    RelatedPoints = new List<ModbusPoint>()
                };

                // 记录当前包覆盖到的最大地址
                int currentEndAddr = sortedPoints[0].Address;

                foreach (var point in sortedPoints)
                {
                    // 计算这个点位的长度 (Bool占1位，Float占2个寄存器)
                    int pointLen = GetPointLength(point, group.Key);
                    int pointEndAddr = point.Address + pointLen;

                    // 判断是否需要“断开”新开一个包
                    // 条件A: 当前点位地址 超过了 (上一个结束地址 + 允许空洞)
                    // 条件B: 当前包总长度 超过了 协议限制
                    bool isGapTooLarge = point.Address > (currentEndAddr + MAX_GAP);
                    bool isPacketFull = (pointEndAddr - currentCmd.StartAddress) > MAX_READ_COUNT;

                    if (isGapTooLarge || isPacketFull)
                    {
                        // 结算上一包
                        currentCmd.Count = currentEndAddr - currentCmd.StartAddress;
                        commands.Add(currentCmd);

                        // 开启新包
                        currentCmd = new ModbusCommand
                        {
                            SlaveId = currentSlaveId,
                            FunctionCode = GetReadFunctionCode(group.Key),
                            StartAddress = point.Address,
                            RelatedPoints = new List<ModbusPoint>()
                        };
                        currentEndAddr = point.Address;
                    }

                    // 加入当前包
                    currentCmd.RelatedPoints.Add(point);

                    // 更新包的结束边界 (注意：要取 max，因为可能存在地址重叠的情况)
                    if (pointEndAddr > currentEndAddr)
                    {
                        currentEndAddr = pointEndAddr;
                    }
                }

                // 循环结束，把最后一包加上
                if (currentCmd.RelatedPoints.Count > 0)
                {
                    currentCmd.Count = currentEndAddr - currentCmd.StartAddress;
                    commands.Add(currentCmd);
                }
            }

            return commands;
        }

        // 辅助：根据存储区获取默认的读取功能码
        private static byte GetReadFunctionCode(StorageZone zone)
        {
            switch (zone)
            {
                case StorageZone.CoilStatus_0x: return 1;
                case StorageZone.InputStatus_1x: return 2;
                case StorageZone.InputRegister_3x: return 4;
                case StorageZone.HoldingRegister_4x: return 3;
                default: return 3;
            }
        }

        // 辅助：计算占位长度
        private static int GetPointLength(ModbusPoint point, StorageZone zone)
        {
            // 如果是线圈/离散输入，Modbus协议里 Count 代表“位的个数”
            if (zone == StorageZone.CoilStatus_0x || zone == StorageZone.InputStatus_1x)
            {
                return 1; // 这里的1代表1位
            }

            // 如果是寄存器，Count 代表“字的个数”
            switch (point.DataType)
            {
                case DataType.Int32:
                case DataType.UInt32:
                case DataType.Float:
                    return 2;
                case DataType.Double:
                    return 4;
                default:
                    return 1;
            }
        }
    }
}
