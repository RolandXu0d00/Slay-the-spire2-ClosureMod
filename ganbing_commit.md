# ClosureMod 更改说明（供作者侧 AI / 维护者阅读）

> 本次提交：`休息点立绘：可露希尔坐姿循环动画（Sit webm 抽帧 119 帧 @15fps，AnimatedSprite2D 自动播放）`
> 提交者：GitHub 协作者（RolandXu0d00 邀请的协作账号）。以下为本提交相对 `e58b72d` 的全部改动说明。

## 一、本次改动内容

为 Closure 角色新增**专属休息点立绘**：可露希尔"基建 Sit"坐姿**循环动画**，替换原来错误显示的默认铁甲战士休息点场景。

## 二、根因（为什么之前休息点显示铁甲战士）

1. 游戏端 `NRestSiteCharacter.Create` 用 `player.Character.RestSiteAnimPath` 加载休息点场景；
2. BaseLib 对 `CharacterModel.RestSiteAnimPath` 有 prefix patch：实例是 `CustomCharacterModel`（Closure 继承 `PlaceholderCharacterModel` → `CustomCharacterModel`）时，直接返回 `CustomRestSiteAnimPath`；
3. Closure 此前**没有 override `CustomRestSiteAnimPath`**，落到 `PlaceholderCharacterModel` 默认实现 = 用 `PlaceholderID`（默认 `"ironclad"`）拼路径 → 永远加载 `ironclad_rest_site.tscn`（铁甲战士）；
4. 场景文件与立绘素材其实一直在 mod 里，但从未被引用。

**修复**：`ClosureModCode/Character/Closure.cs` override：

```csharp
public override string CustomRestSiteAnimPath =>
    SceneHelper.GetScenePath("rest_site/characters/closuremod-closure_rest_site");
```

注意：**不要**用 override `PlaceholderID` 来修（会影响 icon/visual/trail/map marker 等所有 Custom*Path，破坏面大）。

## 三、立绘方案选型（重要决策记录）

- 用户提供素材：`可露希尔-默认-基建-Sit-x1.webm`（Chrome 录制，1000×1000，7.94s，474 帧，黑底透明动画）。
- **Spine 方案已否决**：仓库 `ClosureMod/spine/closure/build/build_char_4228_closur.skel` 含 `Sit`/`Move` 动画（版本 **Spine 3.8.99**），但游戏 Spine 运行时是 **4.x**（sts2.dll 内含 4.0.30 常量；原版场景 SpineSprite 的 `preview_animation="-- Empty --"` 是 4.x 特征）。3.8 二进制 skel 与 4.x 运行时不兼容。`ClosureSpriteVisualFactory.cs` 注释也印证："游戏 Spine 4.2 运行时读取不了明日方舟 3.8 小人模型"。转换需 Spine 编辑器 license，放弃。
- **采用帧序列方案**（与战斗小人 `ClosureAnimatorNode` 同路线）：
  - ffmpeg 抽帧：`fps=15` → **119 帧**（动画本身几乎静止，帧间平均差异极小，15fps 足够）；
  - 抠黑：`colorkey=0x000000:0.03:0`（背景纯黑，0.03 容差保护角色暗部，经 0.03/0.06 对比验证）；
  - 统一裁剪：`crop=401:465:232:502`（7 帧角色包围盒并集 x=[272,592] y=[552,916] + padding，统一框避免逐帧抖动）。

## 四、改动文件清单

| 文件 | 说明 |
|---|---|
| `ClosureModCode/Character/Closure.cs` | +`CustomRestSiteAnimPath` override |
| `scenes/rest_site/characters/closuremod-closure_rest_site.tscn` | 立绘节点 Sprite2D → **AnimatedSprite2D**，挂 `closure_sit.tres` + autoplay 脚本 |
| `ClosureMod/images/restsite/frame_0001..0119.png`(+.import) | 119 帧立绘序列（401×465 RGBA，抠好黑） |
| `ClosureMod/images/restsite/closure_sit.tres` | SpriteFrames 资源：动画 `Sit`，119 帧 @15fps，loop |
| `ClosureMod/scripts/restsite_autoplay.gd`(+.uid) | AnimatedSprite2D 自动播放脚本 |
| `.gitignore` | +`nuget.config`（本地离线 NuGet 源配置，勿提交） |

场景节点关键属性：

```
Sprite: AnimatedSprite2D
  sprite_frames = res://ClosureMod/images/restsite/closure_sit.tres
  animation = "Sit"
  script = res://ClosureMod/scripts/restsite_autoplay.gd
  position = Vector2(-30, -30)   # 最终调校值（经多轮实测）
  scale = Vector2(1.4, 1.4)
```

## 五、踩坑记录（维护必读）

1. **`AnimatedSprite2D.playing` 是只读属性**！场景里写 `playing=true` 无效 → 动画永远不播（静态）。必须脚本 `_ready` 里 `play()`。已封装在 `restsite_autoplay.gd`。
2. **position.y 方向**：Godot 2D y 轴向下，脚底在纹理中心**下方**。让脚底对齐根原点（地面）需 `position.y = -(中心到脚底距离)*scale`（负数）。本场景角色中心在裁剪框 (200,232)，曾误写 `+325` 导致陷地。
3. **游戏脚本兼容性**：`NRestSiteCharacter._Ready` 只对子节点中 `SpineSprite` 类型自动播章节循环动画（`overgrowth_loop`/`hive_loop`/`glory_loop`），对 AnimatedSprite2D **不干扰**，因此本方案无需 patch 游戏端。
4. **多人模式**：`NRestSiteRoom._Ready` 把角色放进游戏槽位容器并重置根 position；`i % 2 == 1`（第 2/4 位）时调用 `FlipX()`，但 FlipX **只翻转 SpineSprite 子节点**，AnimatedSprite2D 不镜像 → 已知小瑕疵：Closure 坐奇数槽位时朝向不与其他角色一致（不崩）。后续可加 Harmony patch 解决。
5. **Godot headless 导出**：报 `.NET Sdk not found` 时，需把 `E:\资料\ClosureMod\_tools\dotnet9` 加到 `$env:PATH` 最前再跑 `godot --headless --export-pack "BasicExport" <out>.pck`。场景引用游戏端脚本（NRestSiteCharacter.cs 等）在 mod 项目导出时必然报 "Cannot open file"，**无害**。
6. **pck 必须用 Godot 导出**，勿用 pckpack 手打（丢 .png.import → 图片全挂）。

## 六、构建与部署

```powershell
$env:DOTNET_ROOT="E:\资料\ClosureMod\_tools\dotnet9"
$env:DOTNET_CLI_HOME="E:\资料\ClosureMod\_tools\dotnet-home"
$env:PATH="E:\资料\ClosureMod\_tools\dotnet9;$env:PATH"
godot --headless --path <repo> --export-pack "BasicExport" <out>.pck
# 输出：mods 三件套 dll + json + pck（dll 未变时只需换 pck）
```

验证：游戏日志出现 `Registered scene 'res://scenes/rest_site/characters/closuremod-closure_rest_site.tscn' for auto-conversion`；进休息点看到可露希尔坐姿循环动画。

## 七、素材工具（如需重做帧序列）

- ffmpeg（`_tools\ffmpeg\ffmpeg-9.0.1-essentials_build\bin\`，Node 走 Clash 代理下载）
- 帧差异分析 `_tools\analyze_frames.js`、包围盒分析 `_tools\analyze_pixels.js`、SpriteFrames 生成 `_tools\gen_spriteframes.js`
- webm 源文件：`E:\资料\ClosureMod\可露希尔-默认-基建-Sit-x1.webm`（未入库，如需入库可放入 `ClosureMod/images/restsite/`）

---

## 八、作者侧项目上下文（2026-08-18 更新）

### 1. 项目现状

- 本项目是《杀戮尖塔 2》可露希尔自定义角色 MOD，核心机制为“战术点”召唤物与能量“透支”。
- 当前已有 25 张可玩卡牌、2 件遗物、11 种能力，并具备中英文本地化。
- 战术点拥有独立生命、回合结束自动攻击、替玩家挡刀、常显血条和最多 3 个单位等功能。
- 多战术点拦截链已修复：玩家格挡先承伤，随后战术点按存活顺序依次承接，最后的剩余伤害才由玩家承担。
- 透支基础上限为 2，升级初始遗物后为 3，卡牌和能力可进一步提高上限。
- 角色及战术点逐帧动画、角色死亡定格和地图标记等功能已有实现。
- 已适配游戏 v0.111.0 的 `CreatureCmd.Damage` 新签名。
- 已加入旧版游戏中“持久糖果 + 单一稀有度卡池”导致结算崩溃的兼容补丁。

### 2. 仓库与本地目录关系

作者侧工作区为：

```text
C:\Users\28393\Documents\ChatGPT\杀戮尖塔mod制作
```

重要目录：

| 用途 | 路径 |
|---|---|
| 日常开发源码 | `C:\Users\28393\Documents\ChatGPT\杀戮尖塔mod制作\ClosureMod` |
| 当前 Git 仓库 | `C:\Users\28393\Documents\ChatGPT\杀戮尖塔mod制作\ClosureMod_上传版_20260816` |
| 工坊上传包 | `C:\Users\28393\Documents\ChatGPT\杀戮尖塔mod制作\ClosureMod_工坊上传_20260816` |
| 完整历史交接 | `C:\Users\28393\Documents\ChatGPT\杀戮尖塔mod制作\项目交接总结.md` |

注意：作者侧工作区根目录和日常开发目录 `ClosureMod` 本身不是 Git 仓库。Git 命令必须在 `ClosureMod_上传版_20260816` 中执行。将仓库更新同步到日常开发目录之前，必须先比较两边差异，以免覆盖尚未进入 Git 的作者侧改动。

### 3. 最近一次 Git 同步

2026-08-18 已执行：

```powershell
git pull origin main --rebase
git push
```

同步结果：

- 当前分支为 `main`。
- `origin` 为 `https://github.com/RolandXu0d00/Slay-the-spire2-ClosureMod.git`。
- 拉取前工作区干净，因此没有创建“本地暂存”提交。
- 从 `e58b72d` 快进到 `b20a071`，没有发生冲突。
- 推送结果为 `Everything up-to-date`，当时本地 `main` 与 `origin/main` 完全同步。
- 仓库没有配置朋友 Fork 的额外 remote；若未来从朋友 Fork 拉取，需要先取得并明确配置其仓库地址。

同步后的三个最新提交为：

```text
b20a071 添加 ganbing_commit.md：休息点立绘改动说明（供作者侧 AI 读取）
185081a 休息点立绘：可露希尔坐姿循环动画（Sit webm 抽帧 119 帧 @15fps，AnimatedSprite2D 自动播放）
e58b72d 平衡卡牌并加强双重部署
```

### 4. 作者侧构建方式

```powershell
dotnet publish "C:\Users\28393\Documents\ChatGPT\杀戮尖塔mod制作\ClosureMod\ClosureMod.csproj" -c Release
```

- 发布会写入 `D:\steam\steamapps\common\Slay the Spire 2\mods\ClosureMod`，通常需要提权，并且应先退出游戏。
- 新增卡牌、能力、遗物或药水时，应先补齐中英文 localization JSON，否则本地化分析器可能导致编译失败。
- 图片或动画变更后，应先让 Godot 重新导入，再发布，确保资源进入 PCK。
- 不要整库扫描 `sts2ncv` 或 `sts2src`，也不要把 `.dll`、`.pck`、`.skel` 当作文本读取。
- Git rebase 若出现冲突，立即停止并用 `git diff --name-only --diff-filter=U` 列出冲突文件，不要自动解决。

### 5. 建议后续验证

1. 比较 Git 仓库与作者侧日常开发目录的差异，再安全同步本次休息点动画改动。
2. 构建并进入游戏验证休息点坐姿动画的资源路径、位置、缩放及循环播放。
3. 多人模式下验证奇数槽位朝向；当前 `FlipX()` 不会自动镜像 `AnimatedSprite2D`，这是已知小瑕疵。
4. 继续处理创意工坊首次上传、药水、能量图标及长局数值平衡测试。
