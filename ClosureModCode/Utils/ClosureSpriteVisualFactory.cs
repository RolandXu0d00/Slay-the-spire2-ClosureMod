using Godot;
using MegaCrit.Sts2.Core.Nodes.Combat;

namespace ClosureMod.ClosureModCode.Utils;

/// <summary>
/// 用离线渲染的透明 PNG 动画帧构建游戏内的生物模型，
/// 替代无法被游戏 Spine 4.2 运行时读取的明日方舟 3.8 小人模型。
/// </summary>
public static class ClosureSpriteVisualFactory
{
    /// <summary>
    /// 可露希尔 Die 动画的帧数（frame_0000 起，最后一帧 index = DieFrameCount - 1）。
    /// </summary>
    public const int DieFrameCount = 59;

    private const string CharacterAnimRoot = "res://ClosureMod/images/charanim/closure";

    /// <summary>
    /// 战报等场景展示“蹲下跪地”姿势时，直接取 Die 动画的最后一帧。
    /// </summary>
    public static Texture2D? LoadDieLastFrame()
    {
        return GD.Load<Texture2D>($"{CharacterAnimRoot}/Die/frame_{DieFrameCount - 1:0000}.png");
    }

    public static NCreatureVisuals? BuildCharacter()
    {
        return Build(
            CharacterAnimRoot,
            "Closure",
            scale: 0.4f,
            canvasSize: 640f,
            feetY: 602f,
            specs:
            [
                new("Idle", [new("Idle", 120)], true, 15f),
                new("Attack", [new("Attack_Begin", 4), new("Attack_Loop", 20), new("Attack_End", 5)], false, 20f),
                new("Skill", [new("Skill_3_Begin", 10), new("Skill_3_Loop", 10), new("Skill_3_End", 7)], false, 20f),
                new("Start", [new("Start", 20)], false, 20f),
                new("Die", [new("Die", DieFrameCount)], false, 60f),
            ]);
    }

    public static NCreatureVisuals? BuildTacticalPoint()
    {
        return Build(
            "res://ClosureMod/images/charanim/tactical_point",
            "TacticalPoint",
            scale: 0.165f,
            canvasSize: 1024f,
            feetY: 956f,
            specs:
            [
                new("Idle", [new("Idle", 27)], true, 20f),
                new("Start", [new("Start", 20)], false, 20f),
                new("Die", [new("Die", 20)], false, 20f),
            ]);
    }

    private static NCreatureVisuals? Build(
        string animRootResPath,
        string label,
        float scale,
        float canvasSize,
        float feetY,
        IReadOnlyList<ClosureAnimatorNode.AnimSpec> specs)
    {
        try
        {
            var visuals = new NCreatureVisuals();

            // 帧图里脚底在 feetY（画布坐标），把脚底锚定到生物原点，并按 scale 放大。
            float feetFromCenter = feetY - canvasSize * 0.5f;
            Vector2 visualPos = new(0f, -feetFromCenter * scale);
            Vector2 boundsSize = new Vector2(canvasSize * 0.85f, canvasSize) * scale;
            Vector2 boundsPos = new(-boundsSize.X * 0.5f, -boundsSize.Y);

            var boundsNode = new Control
            {
                Name = "Bounds",
                Position = boundsPos,
                Size = boundsSize,
            };
            visuals.AddChild(boundsNode);
            boundsNode.Owner = visuals;
            boundsNode.UniqueNameInOwner = true;

            var intent = new Marker2D
            {
                Name = "IntentPos",
                Position = new Vector2(0f, boundsPos.Y - 30f),
            };
            visuals.AddChild(intent);
            intent.Owner = visuals;
            intent.UniqueNameInOwner = true;

            var center = new Marker2D
            {
                Name = "CenterPos",
                Position = new Vector2(0f, boundsPos.Y + boundsSize.Y * 0.5f),
            };
            visuals.AddChild(center);
            center.Owner = visuals;
            center.UniqueNameInOwner = true;

            var sprite = new AnimatedSprite2D
            {
                Name = "Visuals",
                Position = visualPos,
                Scale = new Vector2(scale, scale),
            };
            visuals.AddChild(sprite);
            sprite.Owner = visuals;
            sprite.UniqueNameInOwner = true;

            var animator = new ClosureAnimatorNode
            {
                Name = "Animator",
            };
            visuals.AddChild(animator);
            animator.Owner = visuals;
            animator.Setup(sprite, animRootResPath, specs);

            MainFile.Logger.Info($"[SpriteVisual] {label}: 动画模型创建成功（{animRootResPath}）");
            return visuals;
        }
        catch (Exception e)
        {
            MainFile.Logger.Error($"[SpriteVisual] {label}: 创建失败: {e}");
            return null;
        }
    }
}
