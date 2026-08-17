using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace ClosureMod.ClosureModCode.Cards;

/// <summary>
/// 基础“防御”。
/// </summary>
public sealed class DefendClosure : ClosureCard
{
    protected override List<DynamicVar> CanonicalVars => [new BlockVar(6m, ValueProp.Move)];

    public DefendClosure() : base(1, CardType.Skill, CardRarity.Basic, TargetType.None)
    {
    }

    protected override HashSet<CardTag> CanonicalTags => [CardTag.Defend];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.GainBlock(Owner.Creature, base.DynamicVars.Block, cardPlay);
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars.Block.UpgradeValueBy(3m);
    }
}
