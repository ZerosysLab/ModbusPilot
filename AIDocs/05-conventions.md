# 代码约定

以下约定基于对现有代码结构的观察归纳，改动代码时尽量保持一致，如发现与实际代码不符请更新本文档。

## 命名

- 窗体类：`F_XxxName`（如 `F_ChannelConfig`、`F_DeviceMonitor`），配套 `.resx` 同名。
- 用户控件（组件）：`UC_XxxName`（如 `UC_WidgetMonitor`）。
- 每个窗体/控件的 `.Designer.cs` 由 WinForms 设计器生成，一般不手工编辑，改 UI 布局优先通过设计器或谨慎手改 Designer 文件并保持结构一致。

## 目录归类原则

- 协议/通信相关代码放 `Core/Driver/`。
- 纯数据结构（无业务逻辑）放 `Core/Models/`。
- 有状态或跨模块协作的业务逻辑放 `Core/Services/`。
- 无状态辅助函数放 `Core/Utils/`。
- 窗体与可复用 UI 组件放 `UI/Common/`，与 Modbus 地址配置强相关的窗体放 `UI/ModbusConfig/`。

新增文件时按此归类，不要新建额外的顶层分类目录。

## 依赖

- JSON 序列化统一用 Newtonsoft.Json（不要引入 System.Text.Json 混用）。
- Excel 读写用 MiniExcel。
- 串口通信用 System.IO.Ports。

## 多语言

- 文案通过 `LangProvider.cs`（`ModbusPilot.Core` 项目里，是该项目唯一还在用的文件）提供中英双语，新增用户可见文案时需同步补充双语条目，不要硬编码中文/英文字符串到窗体代码里。

## 待确认事项（后续使用中补充）

- 具体的日志格式规范、异常处理规范、单元测试框架选型等，目前代码中未发现明确证据，如需要请先调研现状再补充本文档，不要凭空假设。
