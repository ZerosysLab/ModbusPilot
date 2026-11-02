
# 🚀 ModbusPilot(Modbus调试工具、Modbus监控工具-主站)

> [🇨🇳 中文](README.md) | [🇬🇧 English](README.en.md)

> **告别”上古时代”的调试体验。**
>
> 专为工控人打造的 .NET 8 现代化 Modbus 调试与监控利器。
>
> *Designed by Zerosys Lab*

[![OS](https://img.shields.io/badge/OS-Windows_10%2F11-0078D6?style=for-the-badge&logo=windows&logoColor=white)](https://gitee.com/ZerosysLab/ModbusPilot/releases)
[![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/)
[![License](https://img.shields.io/badge/License-Freeware-brightgreen?style=for-the-badge)](../../LICENSE)
[![Download](https://img.shields.io/badge/Download-Gitee-C71D23?style=for-the-badge&logo=gitee&logoColor=white)](https://gitee.com/ZerosysLab/ModbusPilot/releases)

> ⚠️ **使用须知**：ModbusPilot 是面向开发者与自动化工程师的**调试/测试/配置工具**，并未经过认证，不适用于工业安全系统、生命关键系统或生产环境下的关键控制。在此类场景使用需自行承担风险，详见文末《免责声明》。

---

## ⚡ 为什么选择 ModbusPilot？

你是否厌倦了传统 Modbus 调试工具那仿佛来自 Windows 98 的灰暗界面？
**ModbusPilot** 不仅仅是一个调试助手，它更是一个**轻量级的桌面组态终端**。基于高性能 **.NET 8** 架构，我们重新定义了数据交互的方式——让数据不仅“可见”，而且“好看”、“好用”。

---

## ⚡ 界面预览 (Dashboard)

**👇 30秒带你体验现代化调试 **
![Modbus调试工具_可视化组态主界面](docs/images/v1.1/demo_overview.gif)

*(图注：含页/分组选项卡的监控看板、区域框拖动时框内卡片整块跟随、单卡配置弹窗，以及右下角报警恢复提醒)*

---

## ✨ 全能特性 (Full Features)

### 1. 🌐 多通道与多设备拓扑 (Topology)
打破单连接限制，构建完整的现场网络。
*   **混合架构**：支持在同一个项目中同时运行 Modbus TCP、RTU (串口) 通道。
*   **多设备轮询**：单通道下支持挂载多个从站设备 (Slave ID)，软件自动调度轮询队列，互不干扰。
*   **虚拟设备**：支持创建离线虚拟设备用于界面布局测试。

### 2. ⚡ 变量批量导入与智能生成
不再浪费时间手动敲几百个地址。
*   **Excel/CSV 互通**：支持从 Excel 直接导入点表，自动识别地址、数据类型与描述。
*   **变量批量操作**：输入起始地址（如 `40001`）和数量（如 `50`），一键批量生成连续变量，配置效率提升 10 倍。
![Modbus变量导入](docs/cn/demo_smart_import.gif)

### 3. 🎴 沉浸式卡片监控 (Card View)
拒绝枯燥的 Excel 表格模式！
*   **可视化看板**：将枯燥的寄存器地址映射为现代化的**数据卡片**。
*   **拖拽组态**：支持开关、数显、输入控制等多种控件，所见即所得。
*   **状态映射**：自动将 `0/1` 转换为 `运行/停止` 并配以红绿指示灯，一目了然。

![Modbus卡片监控看板_页与分组选项卡](docs/images/v1.1/feature_dashboard_group.png)

### 4. 📈 实时趋势图监控 (Real-time Trends)
捕捉瞬间的数据波动。
*   **示波器级体验**：可拖拽任意数值型变量，右键即可开启**毫秒级趋势曲线**。
*   **多轴对比**：支持将多个变量（如温度、压力、设定值）拖入同一图表进行关联分析，轻松定位 PID 震荡问题。

### 5. 🩺 报文监控与自动写入
既是显微镜，也是手术刀。
*   **全量日志**：内置高性能十六进制日志记录器，自动解析功能码与异常码（Exception Code）。
*   **自动化存储**：支持全量日志或错误日志自动存储，方便回溯错误情况。

### 6. 💾 项目快照与标准导出 (Project Management)
不仅是调试工具，更是资产管理库。
*   **工程文件 (.mpilot)**：一键保存当前所有通道、设备、变量配置及界面布局。下次去现场，3秒恢复工作环境。
*   **标准化导出**：支持将经过验证的变量点表导出为标准 Excel/CSV 格式，方便交付给上位机或 PLC 工程师复用。

### 7. 🔔 报警监控 (Alarm Monitoring)
让异常无处遁形。
*   **多条件规则**：大于 / 小于 / 等于 / 不等于 / 范围内 / 范围外，一个点位可配多条规则，适配下位机报警码寄存器（1=超温、2=超压 各报各的）；三级报警级别着色显示。
*   **工业级防抖**：报警延迟（秒）滤除瞬时毛刺，恢复死区（回差）消除临界值横跳导致的反复报警，普通场景零配置。
*   **右下角弹窗提醒**：按级别配色、内置合成提示音（不依赖系统声音方案），一键确认；报警自动恢复后弹窗变身绿色"已恢复"提醒，显示报警/恢复时间与持续时长，不会错过任何一条发生过的报警。
*   **独立监控窗口**：活动/历史报警分开查看，历史支持通道/设备/级别筛选与 CSV 导出；主界面工具栏实时显示红色报警数量徽标。

![Modbus报警监控_右下角弹窗提醒](docs/images/v1.1/feature_alarm_popup.png)

![Modbus报警规则配置_多条件与死区延迟](docs/images/v1.1/feature_alarm_config.png)

![Modbus报警监控窗口_活动与历史报警](docs/images/v1.1/feature_alarm_monitor.png)

### 8. ⏰ 定时任务 (Scheduled Tasks)
无人值守自动控制。
*   **三类触发**：⏰ 定时（每天固定时刻，可选周几）、🔁 间隔（每 N 秒周期写，如看门狗心跳）、🔄 轮换（值列表按天/按次轮流写入）。
*   **跨设备点位列表**：一个任务可写多台设备的多个点位，每点独立写入值；每个任务可独立启停，也可一键全部启动/停止（全部停止兼作急停）。
*   **状态一眼可辨**：运行中工控绿、未运行灰、运行出错红；列表实时显示下次执行倒计时与上次执行结果（按点位统计成功/失败/挂起）。
*   **可靠性设计**：运行状态不随工程保存（每次打开软件均为停止态，杜绝意外写入）；错过的时刻不补写；通道未启动时自动跳过；断线期间写入当场判失败，恢复后不补发积压指令。
*   **运行日志**：窗口底部内置日志（最近 500 条），可自动存盘（单文件 5MB 滚动、按份数自动清理最旧）。

![Modbus定时任务_定时间隔轮换写值](docs/images/v1.1/feature_schedule.png)

### 9. 🗂️ 仪表盘分组与区域框 (Dashboard Organization)
大型工程的画面管理。
*   **页 + 分组两级结构**：仪表盘可建多页，每页内再分多个分组，均以顶部选项卡切换；一次只看一个分组，全屏监控时画面干净利落。
*   **磁贴自由摆位**：卡片按网格自由摆放，拖到空白处就地落格、拖到有卡片的格子上则整块顺延让位（手机桌面图标式），摆位随工程保存；窗口变窄自动整块换行，恢复宽度即精确还原。
*   **区域框**：用 GroupBox 式的淡色边框划分"一楼泵房""2#机组"等区域，纯背景不占格、卡片可跨框；拖动框体为半透明遮罩精确跟手，框内卡片随框整体移动。
*   **卡片大小与显示**：拖动右下角手柄按整格缩放（宽 1~6 格 × 高 1~4 格）；单卡可配置单位/说明/设备名显隐、数值字号、小数精度。

![Modbus卡片配置_字段显隐字号精度](docs/images/v1.1/feature_card_config.png)

### 10. 🧪 任意指令发送 (Raw Command)
底层调试利器。
*   手搓原始报文直接收发，支持全部功能码（含 01-06/15/16 之外的非常规功能码）。
*   自动补 CRC（RTU）/ 自动修正 MBAP 长度域（TCP），实时预览实际发送的字节序列。
*   发送前帧自洽性校验：帧长、数量范围、数据字节数与功能码不符时拦截并提示，可拦住粘贴截断的残帧。
*   响应逐条回显耗时、状态与异常码中文释义；收发同时进入全局报文监视器。

![Modbus任意指令发送_原始报文收发](docs/images/v1.1/feature_raw_command.png)

### 11. 🔢 数据显示增强 (Display Enhancements)
*   **进制切换**：保持寄存器显示支持十进制/十六进制切换，写入值联动（可直接键入 `1A2F` / `0x1A2F`）。
*   **状态文本映射**：点位值 → 自定义文本翻译（1=运行、2=停机、4=超温），状态字/故障码寄存器直读成文字；监控列表与仪表盘卡片同步生效。
*   **位段提取**：寄存器中连续若干位组成新变量（如 `3~6` 表示第 3~6 位共 4 位），状态字按位拆分读取；同一寄存器可同时配置位段、单个位与整字数值点。
*   **6 位地址表示法**：兼容 ModScan/组态王等 400001~465535 寻址习惯；支持协议地址、PLC 5 位、PLC 6 位、十六进制四种显示档位切换。

![Modbus状态文本映射_数值转文字](docs/images/v1.1/feature_state_map.png)

![Modbus设备监控_说明列与十六进制切换](docs/images/v1.1/feature_device_monitor.png)

---



## 📥 下载与安装

### 🖥️ 运行环境 (System Requirements)
> **✅ 完美支持 Windows 10 / 11 (x64/x86)**
> *   **兼容性**：经测试支持 Windows 10 (1809+), Win11, 以及 **Win10 Enterprise LTSC** 长期支持版。
> *   **硬件要求**：任意双核 CPU，500MB 内存即可流畅运行（极低占用）。


### 🚀 安装步骤
**本软件为绿色免安装版 (Portable)，不写注册表，无残留。永久免费使用，不再区分专业版/免费版，全部功能开放。**

1.  进入 [👉 **发行版 (Releases)**](https://gitee.com/ZerosysLab/ModbusPilot/releases) 页面。
2.  下载最新的独立版压缩包（`ModbusPilot_vX.Y.Z_Win64.zip`）。
3.  解压到任意文件夹（建议非 C 盘）。
4.  双击 `ModbusPilot.exe` 即可起飞！

> **💡 说明**：发行包为独立版（已内置 .NET 8 运行时），电脑无需单独安装 .NET 8 即可直接运行。

---

## 📚 文档与支持
详细的操作指南、接线图示及故障排查，请参阅：
👉 **[ModbusPilot 用户手册 (UserGuide.md)](./UserGuide.md)**

## 🛠️ 开发环境
*   Visual Studio 2022
*   .NET 8.0 SDK (Windows Forms)
*   **反馈**：欢迎提交 [Issues](https://gitee.com/ZerosysLab/ModbusPilot/issues) 反馈 Bug 或建议。

## ⚠️ 免责声明
ModbusPilot 是一款**调试与配置工具**，并非经过认证的工业级 SCADA/控制系统。本软件按"原样"提供，不提供任何形式的明示或暗示保证。

- **禁止用于安全关键或生命关键场景**：请勿将本软件用于可能因故障导致人身伤亡或重大财产/环境损害的设备控制（如医疗设备、核能、航空航天、武器系统等）。
- **未经工业生产级认证**：如用于监控或写入真实工业/商业设备，请务必先充分离线测试，并自行承担相应后果。
- 作者与贡献者不对因使用本软件而产生的任何损害、停机、数据丢失或业务中断承担责任。

使用本软件即表示您同意以上条款。

> **关于授权**：ModbusPilot 自 v1.1 起转为**免费闭源软件（Freeware）**——所有功能永久免费使用，但源代码不再公开。v1.0 及更早版本仍以 MIT 协议发布，已发布版本的授权不可追溯收回。

---
**Copyright © 2026 Zerosys Lab.**

## ⚡ 界面预览 (Interface Preview)

### 1. 🖥️ 现代化 Modbus 调试主界面 (Dashboard)
基于 .NET 8 构建的高性能上位机界面。支持 **Modbus TCP/RTU 多通道并发**，集成了拖拽式组态看板，让调试不再面对枯燥的原始数据，而是可视化的仪表盘。顶部为**页 / 分组**两级选项卡，画面内可用**区域框**按车间或机组划分区块。

![Modbus调试工具_可视化组态主界面](docs/images/v1.1/preview_dashboard.png)

### 2. 📊 多设备实时监控 (Device Monitor)
替代传统的 Modbus Poll。支持**多从站 (Multi-Slave) 并发轮询**，实时监控保持寄存器 (4x)、线圈 (0x) 等全存储区状态。支持 **Float/Double/Long** 等复杂数据类型的自动解析与大小端切换；列表带**说明列**，并可在十进制 / 十六进制间一键切换。

![Modbus_TCP_RTU_多设备实时监控窗口](docs/images/v1.1/feature_device_monitor.png)

### 3. ⚙️ 点位管理与地址映射 (Tag Management)
专业的地址表配置工具。支持设置 **线性变换 (Scale/Offset)**，可直接将 PLC 的原始数值转换为物理量（如温度、压力）。支持设置读写权限，防止误操作。

![PLC点位地址映射_变量管理配置](docs/cn/feature_tag_management.png)

### 4. ⚡ Excel 智能导入 (Smart Import)
工程效率神器！支持 **Excel/CSV 点表一键导入**，不限模板格式，智能识别列名。支持批量生成连续地址变量，拒绝手动重复录入，适合数千点位的大型项目调试。

![Excel点表批量导入_智能识别工具](docs/cn/feature_smart_import.png)

### 5. 🩺 报文分析与故障诊断 (Packet Analyzer)
内置报文级 **串口/网络抓包工具**。实时显示 Tx/Rx 原始十六进制数据 (Hex)，自动解析 **功能码 (Function Code)** 与 **异常码 (Exception Code)**，是排查通讯超时、CRC 校验错误的强力辅助。

![Modbus通讯报文监控_16进制抓包分析](docs/cn/feature_packet_monitor.png)

### 6. 📈 实时趋势示波器 (Real-time Trend)
不仅仅是调试器，更是数据记录仪。支持选中任意数值变量开启 **毫秒级实时曲线**，支持多轴对比分析，帮助工程师精准捕捉 PID 震荡与信号干扰。

![Modbus数据曲线_实时趋势分析图](docs/cn/feature_trend_chart.png)

### 7. 🔔 报警监控 (Alarm Monitoring)
报警规则配置、右下角弹窗提醒与独立监控窗口。支持一个点位多条规则、报警延迟与恢复死区，报警自动恢复后弹窗转为绿色"已恢复"提醒。

![Modbus报警弹窗_严重级别提醒](docs/images/v1.1/feature_alarm_popup.png)

### 8. ⏰ 定时任务 (Scheduled Tasks)
定时 / 间隔 / 轮换三类任务，跨设备点位批量写入。状态列直接区分运行中（绿）、未运行（灰）与运行出错（红），底部为运行日志。

![Modbus定时任务_运行状态与日志](docs/images/v1.1/feature_schedule.png)

### 9. 🧪 任意指令发送 (Raw Command)
手搓原始报文直接收发，自动补 CRC / 修正 MBAP 长度域，发送前校验帧自洽性。

![Modbus原始报文收发_任意指令发送](docs/images/v1.1/feature_raw_command.png)

### 10. 🔀 状态文本映射 (State Mapping)
点位值 → 自定义文本翻译，状态字/故障码寄存器直读成文字，监控列表与仪表盘卡片同步生效。

![Modbus状态文本映射_数值转文字](docs/images/v1.1/feature_state_map.png)


