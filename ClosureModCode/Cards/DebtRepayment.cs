using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace ClosureMod.ClosureModCode.Cards;

public sealed class DebtRepayment : ClosureCard
{
    protected override List<DynamicVar> CanonicalVars => [new DynamicVar("BaseBlock", 3m), new DynamicVar("PerDebt", 3m)];
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
    public DebtRepayment() : base(0, CardType.Skill, CardRarity.Common, TargetType.None) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        int energy = Owner.PlayerCombatState?.Energy ?? 0;
        decimal block = DynamicVars["BaseBlock"].BaseValue;
        if (energy < 0)
        {
            int debt = -energy;
            block = debt * DynamicVars["PerDebt"].BaseValue;
            await PlayerCmd.SetEnergy(0m, Owner);
        }
        await CreatureCmd.GainBlock(Owner.Creature, block, ValueProp.Move, cardPlay, false);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["BaseBlock"].UpgradeValueBy(2m);
        DynamicVars["PerDebt"].UpgradeValueBy(2m);
    }
}
