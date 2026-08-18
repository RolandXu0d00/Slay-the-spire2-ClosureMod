using ClosureMod.ClosureModCode.Monsters;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace ClosureMod.ClosureModCode.Cards;

/// <summary>
/// 战术自爆：引爆所有战术点，每个造成15点伤害（战术点随之销毁）。指挥流终端金卡。
/// </summary>
public sealed class TacticalDetonation : ClosureCard
{
    protected override List<DynamicVar> CanonicalVars => [new DynamicVar("Damage", 18m)];

    public TacticalDetonation() : base(2, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
    {
    }

    protected override bool IsPlayable => Owner.PlayerCombatState != null &&
        Owner.PlayerCombatState.Pets.Any(p => p.Monster is TacticalPoint && p.IsAlive);

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var points = Owner.PlayerCombatState!.Pets
            .Where(p => p.Monster is TacticalPoint && p.IsAlive)
            .ToList();
        foreach (var point in points)
        {
            await CreatureCmd.Damage(choiceContext, cardPlay.Target!, base.DynamicVars["Damage"].BaseValue, ValueProp.Move, this);
            await CreatureCmd.Kill(point, true);
        }
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars["Damage"].UpgradeValueBy(5m);
    }
}
