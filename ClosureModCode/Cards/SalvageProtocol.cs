using BaseLib.Patches.Features;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace ClosureMod.ClosureModCode.Cards;

public sealed class SalvageProtocol : ClosureCard
{
    protected override List<DynamicVar> CanonicalVars => [new DynamicVar("Energy", 2m), new DynamicVar("Draw", 1m)];
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
    public SalvageProtocol() : base(0, CardType.Skill, CardRarity.Uncommon, CustomTargetType.Pet) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Target == null) return;
        await CreatureCmd.Kill(cardPlay.Target, true);
        await PlayerCmd.GainEnergy(DynamicVars["Energy"].BaseValue, Owner);
        await CardPileCmd.Draw(choiceContext, DynamicVars["Draw"].BaseValue, Owner, true);
    }

    protected override void OnUpgrade() => DynamicVars["Draw"].UpgradeValueBy(1m);
}
