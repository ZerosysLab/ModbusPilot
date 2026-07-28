# ModbusPilot — AI 协作入口

轻量级 Modbus 上位机/组态工具，.NET 8 WinForms。给 AI 的完整分层文档见 `AIDocs/`，**先读这里，按需再展开**。

## 30 秒读懂项目结构（关键：实际代码位置和解决方案名不一致）

- `ModbusPilot.App/` —— **真正的业务代码全部在这里**，包括 `Core/`（协议驱动、模型、服务）和 `UI/`（窗体、组件）子目录。
- `ModbusPilot.Core/`、`ModbusPilot.UI/` —— 两个类库项目基本是空壳（历史遗留），**不要**去这两个目录找业务代码。
- 项目已决定放弃商业授权路线，转为纯免费开源软件。原 `PilotKeyGen/`、`RSATool/` 密钥工具已删除，License 相关代码逐步断开调用链（保留文件，见 [`AIDocs/03-licensing.md`](AIDocs/03-licensing.md)）。
- `MyDocument/` —— 人类使用的杂项资料目录（图标、测试点表、日志样本、宣传资料等），**不要修改**。
- `AIDocs/` —— 本次新建的 AI 专用文档体系（渐进式披露），持续维护。

## 文档导航

详细架构、模块清单、授权体系、构建发布流程、开发规范，见 [`AIDocs/00-index.md`](AIDocs/00-index.md)。

## 硬规则

- 根目录 `project.md` 中的"解决方案结构/模块详解"章节已过时（仍写着代码在 Core/UI 类库里），修改代码前以实际目录结构为准，不要据此文档定位文件。
- `MyDocument/` 只读，不要新增/修改其内容；新文档写入 `AIDocs/`。
- 涉及授权/License 相关代码改动前，先读 [`AIDocs/03-licensing.md`](AIDocs/03-licensing.md)。
