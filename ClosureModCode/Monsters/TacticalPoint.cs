using BaseLib.Abstracts;
using ClosureMod.ClosureModCode.Utils;
using Godot;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Nodes.Combat;

namespace ClosureMod.ClosureModCode.Monsters;

/// <summary>
/// 战术点：可露希尔的召唤物（宠物）。有生命值，由卡片/遗物召唤；
/// 每回合结束时在 TacticalPointAttackPower 的驱动下自动攻击。
/// </summary>
public sealed class TacticalPoint : CustomPetModel
{
    public TacticalPoint() : base(visibleHp: true)
    {
    }

    public override int MinInitialHp => 5;
    public override int MaxInitialHp => 5;

    public override bool HasDeathSfx => false;

    /// <summary>
    /// 缩短战术点血条。战术点彼此间距较小，默认血条宽度会互相遮挡；
    /// 这里把血条宽度压到比召唤物间距更窄，保留血量数字的显示空间。
    /// </summary>
    public override float HpBarSizeReduction => 73f;

    // 占位场景：仅供资源预加载使用；真正进入战斗时会用 CreateCustomVisuals 生成的指挥中心 Spine 模型。
    public override string? CustomVisualPath => SceneHelper.GetScenePath("creature_visuals/tactical_point_placeholder");

    /// <summary>
    /// 使用指挥中心小人动画帧替换亡灵契约师之手的占位模型。
    /// </summary>
    public override NCreatureVisuals? CreateCustomVisuals()
    {
        return ClosureSpriteVisualFactory.BuildTacticalPoint();
    }
}
