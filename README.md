# ClosureMod 上传版

这是“可露希尔”角色的《杀戮尖塔 2》自定义角色 MOD 源码包。

## 角色机制

- 可露希尔拥有初始遗物“战术指挥终端”，战斗开始时部署一个“战术点”。
- 战术点是可露希尔的召唤物，有生命值和攻击力，回合结束时自动攻击。
- 可露希尔可以透支能量：能量不足时仍可打出不超过透支上限的牌，下回合开始时受到缺费惩罚。

## 目录结构

- `ClosureModCode/`：C# MOD 代码。
- `ClosureMod/`：角色图片、卡面、动画帧、本地化文本、选人背景等资源。
- `scenes/`：生物占位场景。
- `ClosureMod.csproj`：构建入口。
- `ClosureMod.json`：MOD 清单。

## 构建

1. 安装 .NET 9 SDK 和 Godot 4.5.1 mono 版。
2. 将 `local.props.example` 复制为 `local.props`，并把其中的 `GodotPath` 改成你自己的 Godot 编辑器路径。
3. 在命令行中进入本目录，执行：

```powershell
dotnet publish ClosureMod.csproj -c Release
```

`dotnet publish` 会自动编译 DLL，并把 DLL、清单和 Godot 导出的 PCK 复制到检测到的《杀戮尖塔 2》`mods` 目录。

如果只检查 C# 代码是否编译通过，可执行：

```powershell
dotnet build ClosureMod.csproj -c Release
```

## 说明

本上传包已移除本地日志、开发笔记、图片处理脚本和 `.godot` 生成目录，不包含 API Key、个人路径或对话记录。
