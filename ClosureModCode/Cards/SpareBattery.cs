using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace ClosureMod.ClosureModCode.Cards;

public sealed class SpareBattery : ClosureCard
{
    protected override List<DynamicVar> CanonicalVars => [new DynamicVar("Energy", 2m), new DynamicVar("Draw", 1m)];
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
    public SpareBattery() : base(1, CardType.Skill, CardRarity.Common, TargetType.None) { }
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        int energyBeforePlay = (Owner.PlayerCombatState?.Energy ?? 0) + cardPlay.Resources.EnergySpent;
        bool wasNotOverdrawn = energyBeforePlay >= 0;
        await PlayerCmd.GainEnergy(DynamicVars["Energy"].BaseValue, Owner);
        if (wasNotOverdrawn) await CardPileCmd.Draw(choiceContext, DynamicVars["Draw"].BaseValue, Owner, true);
    }

    protected override void OnUpgrade() => DynamicVars["Energy"].UpgradeValueBy(1m);
}
