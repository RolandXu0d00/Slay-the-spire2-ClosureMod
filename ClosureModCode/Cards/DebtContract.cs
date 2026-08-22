using ClosureMod.ClosureModCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace ClosureMod.ClosureModCode.Cards;

/// <summary>
/// 透支契约：能力。回合结束时若处于透支状态，下回合开始时额外获得2点能量。
/// </summary>
public sealed class DebtContract : ClosureCard
{
    protected override List<DynamicVar> CanonicalVars => [new DynamicVar("Energy", 1m)];

    public DebtContract() : base(1, CardType.Power, CardRarity.Rare, TargetType.None)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<DebtContractPower>(choiceContext, Owner.Creature, base.DynamicVars["Energy"].BaseValue, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars["Energy"].UpgradeValueBy(1m);
    }
}
