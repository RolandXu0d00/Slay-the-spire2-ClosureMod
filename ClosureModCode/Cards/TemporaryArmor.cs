using ClosureMod.ClosureModCode.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace ClosureMod.ClosureModCode.Cards;

public sealed class TemporaryArmor : ClosureCard
{
    protected override List<DynamicVar> CanonicalVars => [new DynamicVar("Block", 7m), new DynamicVar("Bonus", 3m)];

    public TemporaryArmor() : base(1, CardType.Skill, CardRarity.Common, TargetType.None) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        decimal block = DynamicVars["Block"].BaseValue;
        if (ClosureSummonUtils.AliveTacticalPointCount(Owner) > 0) block += DynamicVars["Bonus"].BaseValue;
        await CreatureCmd.GainBlock(Owner.Creature, block, ValueProp.Move, cardPlay, false);
    }

    protected override void OnUpgrade() => DynamicVars["Block"].UpgradeValueBy(3m);
}
