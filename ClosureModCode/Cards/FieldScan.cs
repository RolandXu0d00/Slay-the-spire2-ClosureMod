using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace ClosureMod.ClosureModCode.Cards;

public sealed class FieldScan : ClosureCard
{
    protected override List<DynamicVar> CanonicalVars => [new DynamicVar("Block", 5m), new DynamicVar("Draw", 1m)];
    public FieldScan() : base(1, CardType.Skill, CardRarity.Common, TargetType.None) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars["Block"].BaseValue, ValueProp.Move, cardPlay, false);
        await CardPileCmd.Draw(choiceContext, DynamicVars["Draw"].BaseValue, Owner, true);
    }

    protected override void OnUpgrade() => DynamicVars["Block"].UpgradeValueBy(3m);
}
