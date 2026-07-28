# ModbusPilot 项目文档

> **轻量级、现代化的 Modbus 上位机与组态工具**
> 
> *Designed by Zerosys Lab*

## 项目概述

ModbusPilot 是一款专为自动化工程师设计的现代化 Modbus 调试工具。采用 **.NET 8 (WinForms)** 构建，主打 **轻量级组态** 与 **自动化流程**，无需安装庞大的 SCADA 系统即可快速搭建设备监控看板。

## 技术栈

| 技术 | 版本/说明 |
|------|----------|
| 开发框架 | .NET 8.0 (Windows Forms) |
| 核心库目标框架 | .NET Standard 2.0 |
| IDE | Visual Studio 2022 |
| 主要依赖 | Newtonsoft.Json, MiniExcel, System.IO.Ports |

## 解决方案结构

```
ModbusPilot.sln
├── ModbusPilot.Core      # 核心业务逻辑层
├── ModbusPilot.UI        # 用户界面组件层
└── ModbusPilot.App       # 应用程序入口
```

## 模块详解

### 1. ModbusPilot.Core (核心层)

核心层包含 Modbus 协议驱动、数据模型和服务类，采用 .NET Standard 2.0 以保证兼容性。

#### Driver (驱动模块)
| 文件 | 说明 |
|------|------|
| `IModbusCodec.cs` | Modbus 编解码器接口定义 |
| `ITransport.cs` | 传输层接口定义 |
| `ModbusMaster.cs` | Modbus 主站实现，管理通信循环 |
| `ModbusProtocol.cs` | Modbus 协议报文构建与解析 |
| `ModbusRtuCodec.cs` | RTU 模式编解码器 |
| `ModbusTcpCodec.cs` | TCP 模式编解码器 |
| `RtuTransport.cs` | RTU 串口传输实现 |
| `TcpTransport.cs` | TCP 网络传输实现 |

#### Models (数据模型)
| 文件 | 说明 |
|------|------|
| `ChannelConfig.cs` | 通道配置 (RTU/TCP参数) |
| `DeviceConfig.cs` | 设备配置 |
| `ModbusPoint.cs` | Modbus 点位定义 |
| `ModbusCommand.cs` | Modbus 命令封装 |
| `DashboardWidgetConfig.cs` | 仪表盘组件配置 |
| `LogEntry.cs` | 日志条目 |
| `ProjectProfile.cs` | 项目配置文件 |
| `PointImportModel.cs` | 点位导入模型 |

#### Services (服务类)
| 文件 | 说明 |
|------|------|
| `CommandPacker.cs` | 读取命令打包器 |
| `WriteCommandPacker.cs` | 写入命令打包器 |
| `DataResolution.cs` | 数据解析 (支持多种数据类型) |
| `ValueEncoder.cs` | 值编码器 |
| `ProjectManager.cs` | 项目文件管理 |
| `LinkManager.cs` | 远程配置同步 |
| `LocalConfig.cs` | 本地配置 |
| `LogHub.cs` | 日志中心 |

#### Utils (工具类)
| 文件 | 说明 |
|------|------|
| `ImportHelper.cs` | Excel 点位导入工具 |

#### 多语言支持
| 文件 | 说明 |
|------|------|
| `LangProvider.cs` | 中/英文双语支持 |

---

### 2. ModbusPilot.UI (界面层)

界面层提供可复用的 WinForms 组件和窗体。

#### Common (通用组件)
| 文件 | 说明 |
|------|------|
| `F_BaseForm.cs` | 窗体基类 |
| `F_ChannelConfig.cs` | 通道配置窗口 |
| `F_DeviceMonitor.cs` | 设备监控窗口 |
| `F_LogMonitor.cs` | 日志监控窗口 |
| `F_WidgetSelector.cs` | 组件选择器 |
| `UC_WidgetBase.cs` | 组件基类 |
| `UC_WidgetMonitor.cs` | 监控组件 (只读显示) |
| `UC_WidgetControl.cs` | 控制组件 (可写入) |
| `UC_WidgetSwitch.cs` | 开关组件 |
| `UITheme.cs` | 主题定义类 |
| `InputDialog.cs` | 通用输入对话框 |

#### ModbusConfig (配置窗口)
| 文件 | 说明 |
|------|------|
| `F_ModbusAddrManager.cs` | Modbus 地址管理器 |
| `F_ImportGuide.cs` | 导入向导 |
| `F_SmartImport.cs` | 智能导入 |
| `F_ValidationReport.cs` | 验证报告 |

---

### 3. ModbusPilot.App (应用层)

应用程序入口，包含主窗体和启动逻辑。

| 文件 | 说明 |
|------|------|
| `Program.cs` | 程序入口 |
| `MainForm.cs` | 主窗体 |
| `F_About.cs` | 关于窗口 |
| `appsettings.json` | 应用配置 |

---

## 核心功能

- **多协议支持**：Modbus RTU / TCP，支持断线重连
- **拖拽组态**：从资源树拖拽点位生成可视化仪表盘
- **多主题切换**：极客黑 (Cyberpunk)、工业灰、简约白等
- **双语支持**：中文/English 实时切换
- **Excel 导入**：支持从 Excel 批量导入点位配置
- **项目管理**：支持保存/加载项目配置
- **日志监控**：实时查看通信报文

---

## 开发说明

### 构建项目
```bash
dotnet build ModbusPilot.sln
```

### 运行项目
```bash
dotnet run --project ModbusPilot.App
```

### 发布项目
```bash
dotnet publish ModbusPilot.App -c Release -r win-x64
```

---

## 版本信息

- **当前版本**：V0.9 Beta
- **目标平台**：Windows x64
- **许可证**：MIT

---

**Copyright © 2025 Zerosys Lab. All Rights Reserved.**
