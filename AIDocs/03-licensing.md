# 授权(License)体系（已弃用，转开源免费）

> **状态更新**：项目已决定转为纯免费开源软件，不再维护商业授权路线。`PilotKeyGen/`、`RSATool` 两个密钥工具项目已从磁盘和 `.sln` 中彻底删除。本文档保留用于说明历史上遗留在 `ModbusPilot.App` 主程序里的授权判断代码（`LicenseGuard`/`LicenseService` 等），这些代码文件本身**保留不删**，后续会断开调用链（让判断始终放行、隐藏对应UI入口），具体方案见对话历史/后续 PR。

project.md 未记录此模块，但代码中曾实际存在，改动前请先读本文档。

## 商业模式

免费核心 + Pro 付费增值，详细功能边界见 [06-product-roadmap.md](06-product-roadmap.md)。已知的付费拦截点：

- 曲线窗口“导出数据(CSV)”按钮 → `if (!IsPro) 弹窗`
- 右键菜单“批量写入数值” → `if (!IsPro) 弹窗`
- 右键菜单“批量修改属性（地址等）” → **不拦截**，免费

## 相关代码

| 文件 | 位置 | 说明 |
|---|---|---|
| `LicenseInfo.cs` / `LicenseType.cs` | `Core/Models/` | 授权信息与类型定义 |
| `LicenseGuard.cs` | `Core/Services/` | 授权校验/拦截逻辑 |
| `LicenseService.cs` | `Core/Services/` | 授权服务（读取/验证密钥等） |
| `HardwareHelper.cs` | `Core/Utils/` | 硬件信息获取，用于机器绑定授权 |
| `F_Registration.cs` | `UI/Common/` | 用户输入授权码的注册窗口 |

## 配套工具（已删除）

- `PilotKeyGen/`、`RSATool/` — 原本是生成/验证 RSA 授权密钥的独立小工具，随开源化决定已从仓库和 `.sln` 中删除。

## 改动授权逻辑时的注意事项

- 授权校验逻辑改动的目标是"始终放行"而不是删除代码，方便未来需要时可逆恢复。
- 发布时曾走代码混淆（`Obfuscated/` 目录 + `obfuscar.xml`）保护授权逻辑，开源后混淆已无意义，见 [04-build-and-release.md](04-build-and-release.md) 中关于关闭混淆的说明。
- **重要安全提醒**：`ModbusPilot.App/Core/Services/LicenseService.cs` 里硬编码的是 RSA **公钥**（仅 Modulus+Exponent），公开没问题；但 `MyDocument/密钥.txt` 里是对应的 RSA **私钥**（含 D/P/Q/InverseQ 等字段），已在 `.gitignore` 中排除，绝不能提交到仓库。之前 `PilotKeyGen/Form1.cs` 曾把这把私钥硬编码在源码里作为默认值，已在删除该项目前修正过，仅供记录，不要在其他地方重新引入明文私钥。
