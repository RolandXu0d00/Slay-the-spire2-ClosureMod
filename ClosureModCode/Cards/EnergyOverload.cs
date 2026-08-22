using ClosureMod.ClosureModCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace ClosureMod.ClosureModCode.Cards;

/// <summary>
/// 能源超载：能力。透支上限+1；回合结束时若处于透支状态，每个战术点攻击力+1。
/// </summary>
public sealed class EnergyOverload : ClosureCard
{
    protected override List<DynamicVar> CanonicalVars => [new DynamicVar("Limit", 1m)];

    public EnergyOverload() : base(2, CardType.Power, CardRarity.Rare, TargetType.None)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<EnergyOverloadPower>(choiceContext, Owner.Creature, base.DynamicVars["Limit"].BaseValue, Owner.Creature, this);
        await Utils.OverdrawLimitUI.Refresh(choiceContext, Owner);
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars["Limit"].UpgradeValueBy(1m);
    }
}
