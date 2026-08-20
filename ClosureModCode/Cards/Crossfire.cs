using ClosureMod.ClosureModCode.Monsters;
using ClosureMod.ClosureModCode.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace ClosureMod.ClosureModCode.Cards;

public sealed class Crossfire : ClosureCard
{
    protected override List<DynamicVar> CanonicalVars => [new DynamicVar("Damage", 7m), new DynamicVar("PerPoint", 1m)];
    public Crossfire() : base(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await ClosureAttackUtils.Attack(choiceContext, cardPlay.Target!, DynamicVars["Damage"].BaseValue, this);
        var points = Owner.PlayerCombatState?.Pets.Where(p => p.Monster is TacticalPoint && p.IsAlive).ToList() ?? [];
        foreach (var _ in points)
            await ClosureAttackUtils.Attack(choiceContext, cardPlay.Target!, DynamicVars["PerPoint"].BaseValue, this);
    }

    protected override void OnUpgrade() => DynamicVars["Damage"].UpgradeValueBy(3m);
}
