using ClosureMod.ClosureModCode.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace ClosureMod.ClosureModCode.Cards;

/// <summary>
/// 火力覆盖：每个战术点对目标造成伤害。
/// </summary>
public sealed class FireCoverage : ClosureCard
{
    protected override List<DynamicVar> CanonicalVars => [new DynamicVar("PerPoint", 5m)];

    public FireCoverage() : base(2, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        decimal dmg = ClosureSummonUtils.AliveTacticalPointCount(Owner) * base.DynamicVars["PerPoint"].BaseValue;
        await CreatureCmd.Damage(choiceContext, cardPlay.Target!, dmg, ValueProp.Move, this);
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars["PerPoint"].UpgradeValueBy(2m);
    }
}
