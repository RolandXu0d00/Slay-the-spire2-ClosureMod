using ClosureMod.ClosureModCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace ClosureMod.ClosureModCode.Cards;

/// <summary>
/// 能源回收：能力。回合结束时若透支，抽固定数量的牌。
/// </summary>
public sealed class EnergyRecovery : ClosureCard
{
    protected override List<DynamicVar> CanonicalVars => [new DynamicVar("Draw", 1m)];

    public EnergyRecovery() : base(1, CardType.Power, CardRarity.Uncommon, TargetType.None)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<EnergyRecoveryPower>(choiceContext, Owner.Creature, base.DynamicVars["Draw"].BaseValue, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars["Draw"].UpgradeValueBy(1m);
    }
}
