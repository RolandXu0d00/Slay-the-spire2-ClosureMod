using ClosureMod.ClosureModCode.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace ClosureMod.ClosureModCode.Cards;

public sealed class Crossfire : ClosureCard
{
    protected override List<DynamicVar> CanonicalVars => [new DynamicVar("Damage", 6m), new DynamicVar("Required", 2m)];
    public Crossfire() : base(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await ClosureAttackUtils.Attack(choiceContext, cardPlay.Target!, DynamicVars["Damage"].BaseValue, this);
        if (ClosureSummonUtils.AliveTacticalPointCount(Owner) >= DynamicVars["Required"].BaseValue)
            await ClosureAttackUtils.Attack(choiceContext, cardPlay.Target!, DynamicVars["Damage"].BaseValue, this);
    }

    protected override void OnUpgrade() => DynamicVars["Damage"].UpgradeValueBy(2m);
}
