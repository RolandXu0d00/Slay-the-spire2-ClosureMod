using ClosureMod.ClosureModCode.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace ClosureMod.ClosureModCode.Cards;

public sealed class SafeDischarge : ClosureCard
{
    protected override List<DynamicVar> CanonicalVars => [new DynamicVar("Damage", 10m), new DynamicVar("PerDebt", 3m)];
    public SafeDischarge() : base(2, CardType.Attack, CardRarity.Uncommon, TargetType.AllEnemies) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        int energy = Owner.PlayerCombatState?.Energy ?? 0;
        decimal damage = DynamicVars["Damage"].BaseValue + Math.Max(0, -energy) * DynamicVars["PerDebt"].BaseValue;
        await ClosureAttackUtils.AttackAll(choiceContext, damage, this);
    }

    protected override void OnUpgrade() => DynamicVars["Damage"].UpgradeValueBy(4m);
}
