# 构建、运行与发布

## 构建

```bash
dotnet build ModbusPilot.sln
```

## 运行

```bash
dotnet run --project ModbusPilot.App
```

## 发布

```bash
dotnet publish ModbusPilot.App -c Release -r win-x64
```

发布产物默认落在 `ModbusPilot.App/bin/Release/publish`（另有 `publish/` 根目录，可能是历史发布产物存放处，改动发布流程前先确认实际输出路径与 `.csproj` 中 PublishProfile 配置一致）。

## 代码混淆

`ModbusPilot.App/obfuscar.xml` 是 Obfuscar 混淆配置，`Obfuscated/` 目录存放混淆后的 dll 与映射表。混淆的主要目的是保护授权(License)逻辑不被轻易逆向，见 [03-licensing.md](03-licensing.md)。

- 涉及混淆配置的改动要谨慎：排除表配置错误可能导致反射/序列化（如 Newtonsoft.Json 用到的模型类）在混淆后运行时出错。
- 修改 `Core/Models` 下参与 JSON 序列化的类名/字段名时，确认 obfuscar 排除规则是否需要同步更新。

## 目标平台

- Windows x64
- 当前版本：V0.9 Beta，MIT 许可
