using BaseLib.Patches.Features;
using ClosureMod.ClosureModCode.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace ClosureMod.ClosureModCode.Cards;

public sealed class TacticalRedeploy : ClosureCard
{
    protected override List<DynamicVar> CanonicalVars => [new DynamicVar("HP", 8m), new DynamicVar("Attack", 2m)];
    public TacticalRedeploy() : base(1, CardType.Skill, CardRarity.Uncommon, CustomTargetType.Pet) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Target == null) return;
        int slot = Owner.PlayerCombatState?.Pets.ToList().IndexOf(cardPlay.Target) ?? -1;
        await CreatureCmd.Kill(cardPlay.Target, true);
        await ClosureSummonUtils.SummonTacticalPoint(choiceContext, Owner, (int)DynamicVars["HP"].BaseValue, (int)DynamicVars["Attack"].BaseValue, this, slot);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["HP"].UpgradeValueBy(2m);
        DynamicVars["Attack"].UpgradeValueBy(1m);
    }
}
