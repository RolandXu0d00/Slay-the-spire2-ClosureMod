using ClosureMod.ClosureModCode.Monsters;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace ClosureMod.ClosureModCode.Cards;

public sealed class RecoveryParts : ClosureCard
{
    protected override List<DynamicVar> CanonicalVars => [new DynamicVar("Block", 5m), new DynamicVar("Heal", 4m)];
    public RecoveryParts() : base(1, CardType.Skill, CardRarity.Common, TargetType.None) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars["Block"].BaseValue, ValueProp.Move, cardPlay, false);
        var point = Owner.PlayerCombatState?.Pets.FirstOrDefault(p => p.Monster is TacticalPoint && p.IsAlive);
        if (point != null) await CreatureCmd.Heal(point, DynamicVars["Heal"].BaseValue, true);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["Block"].UpgradeValueBy(3m);
        DynamicVars["Heal"].UpgradeValueBy(2m);
    }
}
