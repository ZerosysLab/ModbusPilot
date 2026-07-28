# AIDocs 索引

面向 AI 协作的项目文档，渐进式披露：先看本页，再按任务类型跳转到对应文档，不需要一次性通读全部。

| 文档 | 什么时候看 |
|---|---|
| [01-architecture.md](01-architecture.md) | 需要了解项目整体架构、解决方案结构、技术栈时 |
| [02-modules.md](02-modules.md) | 需要定位某个具体功能模块（协议驱动/数据模型/服务/界面组件）的代码位置时 |
| [03-licensing.md](03-licensing.md) | 涉及授权(License)相关遗留代码改动、了解商业化历史与开源化决定时 |
| [04-build-and-release.md](04-build-and-release.md) | 需要构建、运行、发布、代码混淆相关操作时 |
| [05-conventions.md](05-conventions.md) | 编写/修改代码前，了解项目代码风格与约定时 |
| [06-product-roadmap.md](06-product-roadmap.md) | 需要了解产品功能规划、免费/付费(Pro)边界划分时 |

## 项目一句话简介

ModbusPilot 是面向自动化工程师的 Modbus RTU/TCP 调试与轻量组态工具，.NET 8 WinForms 桌面应用，采用免费核心功能 + Pro 付费增值(曲线导出/批量写入)的商业模式。

## 关键事实（避免踩坑）

1. 业务代码主体在 `ModbusPilot.App/Core` 和 `ModbusPilot.App/UI`，不在 `ModbusPilot.Core`/`ModbusPilot.UI` 项目里（那两个几乎是空壳）。
2. 根目录 `project.md` 的模块结构描述已过时，请以本目录下文档和实际代码为准。
3. `MyDocument/` 是人类资料目录，只读不要动；本 AI 文档体系独立维护在 `AIDocs/`。
4. 项目未使用 git 版本管理（当前目录不是 git repository）。
