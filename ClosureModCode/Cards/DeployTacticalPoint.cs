using ClosureMod.ClosureModCode.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace ClosureMod.ClosureModCode.Cards;

/// <summary>
/// 召唤一个战术点（默认 8 生命 / 2 攻击），场上最多 3 个。
/// </summary>
public sealed class DeployTacticalPoint : ClosureCard
{
    protected override List<DynamicVar> CanonicalVars => [new DynamicVar("HP", ClosureSummonUtils.DefaultHp)];

    public DeployTacticalPoint() : base(1, CardType.Skill, CardRarity.Common, TargetType.None)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await ClosureSummonUtils.SummonTacticalPoint(choiceContext, Owner, (int)base.DynamicVars["HP"].BaseValue, ClosureSummonUtils.DefaultAttack, this);
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars["HP"].UpgradeValueBy(2m);
    }
}
