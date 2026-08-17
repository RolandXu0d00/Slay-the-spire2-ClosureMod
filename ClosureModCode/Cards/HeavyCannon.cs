using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace ClosureMod.ClosureModCode.Cards;

/// <summary>
/// 高能炮：造成巨额伤害。
/// </summary>
public sealed class HeavyCannon : ClosureCard
{
    protected override List<DynamicVar> CanonicalVars => [new DynamicVar("Damage", 24m)];

    public HeavyCannon() : base(3, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.Damage(choiceContext, cardPlay.Target!, base.DynamicVars["Damage"].BaseValue, ValueProp.Move, this);
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars["Damage"].UpgradeValueBy(8m);
    }
}
