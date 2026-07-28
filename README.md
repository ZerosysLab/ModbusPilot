Here is the English version of your README file.

***

# 🚀 ModbusPilot

> **A Lightweight, Modern Modbus Master & Configuration Tool**
>
> *Designed by Zerosys Lab*

![Platform](https://img.shields.io/badge/Platform-Windows_x64-blue) ![Runtime](https://img.shields.io/badge/.NET-8.0-purple) ![License](https://img.shields.io/badge/License-MIT-green)

> ⚠️ **Intended Use**: ModbusPilot is a **debugging / testing / configuration tool** for developers and automation engineers working with Modbus devices during development. It is **not** designed, tested, or certified for industrial safety systems, life-critical systems, or production/commercial mission-critical control. Use in such environments is at your own risk — see the Disclaimer section near the bottom of this page.

## 📖 Introduction
ModbusPilot is a modern debugging tool designed for automation engineers. Built with **.NET 8 (WinForms)**, it moves away from the outdated interfaces of traditional tools, focusing on **"Lightweight Configuration"** and **"Automated Workflows"**.

No need to install massive SCADA systems; you can quickly build device monitoring dashboards with just a few drag-and-drop operations.

## 📚 Documentation
For detailed operation guides and configuration instructions, please refer to:
👉 **[ModbusPilot User Guide (https://github.com/ZerosysLab/ModbusPilot/blob/main/UserGuide.md)](https://github.com/ZerosysLab/ModbusPilot/blob/main/UserGuide.md)**

## ✨ Core Features (V0.9 Beta)
*   **Multi-Protocol**: Full support for Modbus RTU / TCP with auto-reconnection.
*   **Drag & Drop**: Directly drag points from the resource tree to generate visual dashboards.
*   **Theming**: Built-in **Cyberpunk**, Industrial Grey, and Simple White themes.
*   **Bilingual**: Native support for real-time switching between Chinese and English.
*   **Cloud Config**: Supports remote update detection and configuration synchronization.

## 📥 Download & Install
This software is portable (no installation required):
1.  Go to the [Releases](https://github.com/ZerosysLab/ModbusPilot/releases) page.
2.  Download the latest `ModbusPilot_v0.9.x_Win64.zip`.
3.  Unzip and double-click `ModbusPilot.exe` to run.

> **Note**: If you are unsure whether .NET 8 is installed on your computer, please download the **Self-Contained** version (larger file size but works out of the box).

## 🛠️ Development Environment
*   Visual Studio 2022
*   .NET 8.0 SDK
*   Windows Forms

## 🤝 Contribution
Feel free to submit [Issues](https://github.com/ZerosysLab/ModbusPilot/issues) for bug reports or feature requests.

## ⚠️ Disclaimer
ModbusPilot is a **debugging and configuration tool**, not an industrial-grade SCADA/control system. It is provided "AS IS" without warranty of any kind.

- **Not for safety-critical or life-critical use.** Do not use this software to control equipment where a malfunction could cause injury, death, or significant property/environmental damage (e.g. medical devices, nuclear, aerospace, weapons systems).
- **Not certified for industrial production control.** If you use it to monitor or write to real industrial/commercial equipment, thoroughly test offline first and take full responsibility for the outcome.
- The authors and contributors are not liable for any damages, downtime, data loss, or business interruption arising from the use of this software.

By using this software you agree to the above. See [LICENSE](LICENSE) (MIT) for the full legal terms.

---
**Copyright © 2025 Zerosys Lab.**