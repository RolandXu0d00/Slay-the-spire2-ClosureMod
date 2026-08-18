using ClosureMod.ClosureModCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace ClosureMod.ClosureModCode.Cards;

/// <summary>
/// 预支：抽2张卡，下回合开始时能量-1（透支1点）。
/// </summary>
public sealed class AdvanceDraw : ClosureCard
{
    protected override List<DynamicVar> CanonicalVars => [new DynamicVar("Draw", 2m), new DynamicVar("Debt", 1m)];

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    public AdvanceDraw() : base(0, CardType.Skill, CardRarity.Common, TargetType.None)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CardPileCmd.Draw(choiceContext, base.DynamicVars["Draw"].BaseValue, Owner, true);
        await PowerCmd.Apply<ClosureDebtPower>(choiceContext, Owner.Creature, base.DynamicVars["Debt"].BaseValue, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars["Draw"].UpgradeValueBy(1m);
    }
}
