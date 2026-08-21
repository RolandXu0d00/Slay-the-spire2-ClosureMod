using ClosureMod.ClosureModCode.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace ClosureMod.ClosureModCode.Cards;

public sealed class SuppressiveProtocol : ClosureCard
{
    protected override List<DynamicVar> CanonicalVars => [new DynamicVar("Weak", 1m), new DynamicVar("Block", 4m)];
    public SuppressiveProtocol() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.None) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var enemies = Owner.Creature.CombatState?.Enemies.Where(e => e.IsAlive).ToList() ?? [];
        foreach (var enemy in enemies)
            await PowerCmd.Apply<WeakPower>(choiceContext, enemy, DynamicVars["Weak"].BaseValue, Owner.Creature, this);
        if (ClosureSummonUtils.AliveTacticalPointCount(Owner) > 0)
            await CreatureCmd.GainBlock(Owner.Creature, DynamicVars["Block"].BaseValue, ValueProp.Move, cardPlay, false);
    }

    protected override void OnUpgrade() => DynamicVars["Weak"].UpgradeValueBy(1m);
}
