using ClosureMod.ClosureModCode.Monsters;
using ClosureMod.ClosureModCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace ClosureMod.ClosureModCode.Cards;

/// <summary>
/// 集火协议：本回合结束时每个战术点额外攻击若干次。
/// </summary>
public sealed class ConcentratedFire : ClosureCard
{
    protected override List<DynamicVar> CanonicalVars => [new DynamicVar("Attacks", 2m)];

    public ConcentratedFire() : base(2, CardType.Skill, CardRarity.Rare, TargetType.None)
    {
    }

    protected override bool IsPlayable => Owner.PlayerCombatState != null &&
        Owner.PlayerCombatState.Pets.Any(p => p.Monster is TacticalPoint && p.IsAlive);

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<ConcentratedFirePower>(choiceContext, Owner.Creature, base.DynamicVars["Attacks"].BaseValue, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars["Attacks"].UpgradeValueBy(1m);
    }
}
