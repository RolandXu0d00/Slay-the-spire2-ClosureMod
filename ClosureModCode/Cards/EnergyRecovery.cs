using ClosureMod.ClosureModCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace ClosureMod.ClosureModCode.Cards;

/// <summary>
/// 能源回收：能力。回合结束时每透支1点能量，抽1张卡。
/// </summary>
public sealed class EnergyRecovery : ClosureCard
{
    protected override List<DynamicVar> CanonicalVars => [new DynamicVar("DrawMult", 1m)];

    public EnergyRecovery() : base(1, CardType.Power, CardRarity.Uncommon, TargetType.None)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<EnergyRecoveryPower>(choiceContext, Owner.Creature, base.DynamicVars["DrawMult"].BaseValue, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars["DrawMult"].UpgradeValueBy(1m);
    }
}
