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
