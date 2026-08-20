using BaseLib.Patches.Features;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace ClosureMod.ClosureModCode.Cards;

public sealed class EmergencyRecall : ClosureCard
{
    protected override List<DynamicVar> CanonicalVars => [new DynamicVar("Bonus", 2m)];
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
    public EmergencyRecall() : base(0, CardType.Skill, CardRarity.Common, CustomTargetType.Pet) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Target == null) return;
        decimal block = Math.Ceiling(cardPlay.Target.CurrentHp / 2m) + DynamicVars["Bonus"].BaseValue;
        await CreatureCmd.GainBlock(Owner.Creature, block, ValueProp.Move, cardPlay, false);
        await CreatureCmd.Kill(cardPlay.Target, true);
    }

    protected override void OnUpgrade() => DynamicVars["Bonus"].UpgradeValueBy(2m);
}
