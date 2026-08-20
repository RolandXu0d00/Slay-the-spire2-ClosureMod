using ClosureMod.ClosureModCode.Monsters;
using ClosureMod.ClosureModCode.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace ClosureMod.ClosureModCode.Cards;

/// <summary>
/// 修理光束：造成伤害，并给1个战术点回复生命。
/// </summary>
public sealed class RepairBeam : ClosureCard
{
    protected override List<DynamicVar> CanonicalVars => [new DynamicVar("Damage", 8m), new DynamicVar("Heal", 5m)];

    public RepairBeam() : base(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
    {
    }

    protected override HashSet<CardTag> CanonicalTags => [CardTag.Strike];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await ClosureAttackUtils.Attack(choiceContext, cardPlay.Target!, base.DynamicVars["Damage"].BaseValue, this);
        var point = Owner.PlayerCombatState?.Pets
            .FirstOrDefault(p => p.Monster is TacticalPoint && p.IsAlive);
        if (point != null)
        {
            await CreatureCmd.Heal(point, base.DynamicVars["Heal"].BaseValue, playAnim: true);
        }
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars["Damage"].UpgradeValueBy(3m);
        base.DynamicVars["Heal"].UpgradeValueBy(2m);
    }
}
