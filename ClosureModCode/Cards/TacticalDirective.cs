using ClosureMod.ClosureModCode.Monsters;
using ClosureMod.ClosureModCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace ClosureMod.ClosureModCode.Cards;

/// <summary>
/// 基础“打击”：本回合战术点伤害提高。攻击全部由战术点在回合结束时结算。
/// </summary>
public sealed class TacticalDirective : ClosureCard
{
    protected override List<DynamicVar> CanonicalVars => [new DynamicVar("Damage", 4m)];

    protected override HashSet<CardTag> CanonicalTags => [CardTag.Strike];

    public TacticalDirective() : base(1, CardType.Skill, CardRarity.Basic, TargetType.None)
    {
    }

    protected override bool IsPlayable => Owner.PlayerCombatState != null &&
        Owner.PlayerCombatState.Pets.Any(p => p.Monster is TacticalPoint && p.IsAlive);

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<TacticalDirectivePower>(choiceContext, Owner.Creature, base.DynamicVars["Damage"].BaseValue, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars["Damage"].UpgradeValueBy(3m);
    }
}
