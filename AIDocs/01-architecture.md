# 架构总览

## 解决方案与实际代码位置

`ModbusPilot.sln` 包含 5 个项目，但代码分布与项目名不完全对应：

| 项目 | TargetFramework | 实际状态 |
|---|---|---|
| `ModbusPilot.App` | net8.0-windows | **主体业务代码所在地**，见下方目录 |
| `ModbusPilot.Core` | netstandard2.0 | 几乎空壳，仅 `LangProvider.cs`（多语言） |
| `ModbusPilot.UI` | net8.0-windows | 几乎空壳，仅 `F_BaseForm.*`（窗体基类） |
| `PilotKeyGen`（已删除） | - | 原授权密钥生成工具，随开源化决定移除 |
| `RSATool`（已删除） | - | 原 RSA 加解密辅助工具，随开源化决定移除 |

## ModbusPilot.App 内部结构

```
ModbusPilot.App/
├── Program.cs / MainForm.cs / F_About.cs / F_Notice.cs   # 入口与主窗体
├── appsettings.json
├── obfuscar.xml                # 发布混淆配置
├── Core/
│   ├── Driver/                 # Modbus 协议编解码与传输层
│   ├── Models/                 # 数据模型（含 License 相关模型）
│   ├── Services/                # 业务服务（含 License/日志/配置）
│   └── Utils/                  # 工具类
├── UI/
│   ├── Common/                  # 通用窗体与组件
│   └── ModbusConfig/            # 地址配置相关窗体
└── Obfuscated/                  # 混淆后产物（发布用）
```

详细文件清单见 [02-modules.md](02-modules.md)。

## 为什么 Core/UI 类库是空的

历史遗留：项目最初按三层设计（Core/UI/App），但实际开发中业务代码逐渐都写进了 App 项目内部的同名子目录，Core/UI 类库项目未被继续使用。改动代码时**不要**假设它们承载业务逻辑；如无特殊说明，不需要维护这两个空壳项目的同步。

## 技术栈

- .NET 8.0 WinForms（App/UI 项目），.NET Standard 2.0（Core 项目，未实际使用）
- 依赖：Newtonsoft.Json、MiniExcel、System.IO.Ports
- IDE：Visual Studio 2022
- 无 git 版本管理（当前目录非 git repo）
