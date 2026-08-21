using ClosureMod.ClosureModCode.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;

namespace ClosureMod.ClosureModCode.Cards;

public sealed class TargetLock : ClosureCard
{
    protected override List<DynamicVar> CanonicalVars => [new DynamicVar("Vulnerable", 2m), new DynamicVar("Draw", 1m)];
    public TargetLock() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.AnyEnemy) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<VulnerablePower>(choiceContext, cardPlay.Target!, DynamicVars["Vulnerable"].BaseValue, Owner.Creature, this);
        if (ClosureSummonUtils.AliveTacticalPointCount(Owner) > 0)
            await CardPileCmd.Draw(choiceContext, DynamicVars["Draw"].BaseValue, Owner, true);
    }

    protected override void OnUpgrade() => DynamicVars["Vulnerable"].UpgradeValueBy(1m);
}
