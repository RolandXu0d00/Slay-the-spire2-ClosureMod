using ClosureMod.ClosureModCode.Monsters;
using ClosureMod.ClosureModCode.Powers;
using ClosureMod.ClosureModCode.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace ClosureMod.ClosureModCode.Cards;

public sealed class CoordinatedVolley : ClosureCard
{
    protected override List<DynamicVar> CanonicalVars => [new DynamicVar("Damage", 8m), new DynamicVar("Bonus", 2m)];
    public CoordinatedVolley() : base(2, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await ClosureAttackUtils.Attack(choiceContext, cardPlay.Target!, DynamicVars["Damage"].BaseValue, this);
        var points = Owner.PlayerCombatState?.Pets.Where(p => p.Monster is TacticalPoint && p.IsAlive).ToList() ?? [];
        foreach (var point in points)
        {
            decimal attack = point.Powers.OfType<TacticalPointAttackPower>().FirstOrDefault()?.Amount ?? 0m;
            attack += Owner.Creature.Powers.OfType<TacticalDirectivePower>().Sum(p => p.Amount);
            await ClosureAttackUtils.Attack(choiceContext, cardPlay.Target!, attack + DynamicVars["Bonus"].BaseValue, this, point);
        }
    }

    protected override void OnUpgrade() => DynamicVars["Bonus"].UpgradeValueBy(2m);
}
