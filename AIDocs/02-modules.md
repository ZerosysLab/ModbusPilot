# 模块清单（代码定位表）

所有路径均相对于 `ModbusPilot.App/`。

## Core/Driver — Modbus 协议驱动

| 文件 | 说明 |
|---|---|
| `IModbusCodec.cs` | 编解码器接口 |
| `ITransport.cs` | 传输层接口 |
| `ModbusMaster.cs` | 主站实现，管理通信循环 |
| `ModbusProtocol.cs` | 报文构建与解析 |
| `ModbusRtuCodec.cs` / `ModbusTcpCodec.cs` | RTU/TCP 编解码器 |
| `RtuTransport.cs` / `TcpTransport.cs` | RTU 串口 / TCP 网络传输实现 |

## Core/Models — 数据模型

| 文件 | 说明 |
|---|---|
| `ChannelConfig.cs` | 通道配置（RTU/TCP 参数） |
| `DeviceConfig.cs` | 设备配置 |
| `ModbusPoint.cs` | 点位定义 |
| `ModbusCommand.cs` | 命令封装 |
| `DashboardWidgetConfig.cs` | 仪表盘组件配置 |
| `LogEntry.cs` | 日志条目 |
| `ProjectProfile.cs` | 项目配置文件 |
| `PointImportModel.cs` | 点位导入模型 |
| `TrendCurve.cs` / `TrendDragData.cs` | 趋势曲线数据与拖拽 |
| `WidgetMode.cs` | 组件模式枚举 |
| `LicenseInfo.cs` / `LicenseType.cs` | 授权信息与类型（见 [03-licensing.md](03-licensing.md)） |

## Core/Services — 服务类

| 文件 | 说明 |
|---|---|
| `CommandPacker.cs` / `WriteCommandPacker.cs` | 读/写命令打包 |
| `DataResolution.cs` | 数据解析（多数据类型） |
| `ValueEncoder.cs` | 值编码 |
| `ProjectManager.cs` | 项目文件管理 |
| `LinkManager.cs` | 远程配置同步 |
| `LocalConfig.cs` / `ServerConfig.cs` | 本地/服务端配置 |
| `LogHub.cs` | 日志中心 |
| `AutoLogPolicy.cs` | 自动日志策略 |
| `LicenseGuard.cs` / `LicenseService.cs` | 授权校验与服务（见 [03-licensing.md](03-licensing.md)） |

## Core/Utils — 工具类

| 文件 | 说明 |
|---|---|
| `ImportHelper.cs` | Excel 点位导入 |
| `CircularBuffer.cs` | 环形缓冲（趋势曲线数据缓存） |
| `HardwareHelper.cs` | 硬件信息获取（授权机器绑定相关） |
| `ProtocolInterpreter.cs` | 协议报文解读辅助 |
| `SystemLogger.cs` | 系统日志 |
| `AppInfoHelper.cs` | 应用信息（版本号等） |

## UI/Common — 通用窗体与组件

| 文件 | 说明 |
|---|---|
| `F_ChannelConfig` | 通道配置窗口 |
| `F_DeviceMonitor` | 设备监控窗口 |
| `F_LogMonitor` | 日志监控窗口 |
| `F_WidgetSelector` | 组件选择器 |
| `F_InputValue` | 数值输入窗口 |
| `F_Registration` | 授权注册窗口（见 [03-licensing.md](03-licensing.md)） |
| `F_Splash` | 启动画面 |
| `F_TrendChart` | 趋势曲线窗口 |
| `UC_WidgetBase` / `UC_WidgetMonitor` / `UC_WidgetControl` / `UC_WidgetSwitch` | 组件基类与只读/可写/开关组件 |
| `UITheme.cs` | 主题定义（极客黑/工业灰/简约白） |
| `InputDialog.cs` | 通用输入对话框 |
| `DragHelper.cs` | 拖拽辅助（组态拖拽生成仪表盘） |

## UI/ModbusConfig — 地址配置

| 文件 | 说明 |
|---|---|
| `F_ModbusAddrManager` | Modbus 地址管理器 |
| `F_ImportGuide` | 导入向导 |
| `F_SmartImport` | 智能导入 |
| `F_ValidationReport` | 验证报告 |

## 顶层文件

| 文件 | 说明 |
|---|---|
| `Program.cs` | 程序入口 |
| `MainForm.cs` | 主窗体 |
| `F_About.cs` | 关于窗口（另有 `F_About1.cs`，疑似重复/废弃，改动前先确认实际引用） |
| `F_Notice.cs` | 通知窗口 |
| `appsettings.json` | 应用配置 |
| `obfuscar.xml` | 发布混淆配置，见 [04-build-and-release.md](04-build-and-release.md) |
