using ClosureMod.ClosureModCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace ClosureMod.ClosureModCode.Cards;

/// <summary>
/// 能量枢纽：能力。场上有战术点时，透支上限提高固定数值。
/// </summary>
public sealed class EnergyHub : ClosureCard
{
    protected override List<DynamicVar> CanonicalVars => [new DynamicVar("Limit", 1m)];

    public EnergyHub() : base(1, CardType.Power, CardRarity.Rare, TargetType.None)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<EnergyHubPower>(choiceContext, Owner.Creature, base.DynamicVars["Limit"].BaseValue, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars["Limit"].UpgradeValueBy(1m);
    }
}
