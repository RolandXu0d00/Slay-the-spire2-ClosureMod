using ClosureMod.ClosureModCode.Monsters;
using ClosureMod.ClosureModCode.Powers;
using ClosureMod.ClosureModCode.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace ClosureMod.ClosureModCode.Cards;

/// <summary>
/// 电弧枪：造成伤害，并让首个战术点协同攻击。
/// </summary>
public sealed class ArcGun : ClosureCard
{
    protected override List<DynamicVar> CanonicalVars => [new DynamicVar("Damage", 7m)];

    public ArcGun() : base(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
    {
    }

    protected override HashSet<CardTag> CanonicalTags => [CardTag.Strike];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await ClosureAttackUtils.Attack(choiceContext, cardPlay.Target!, base.DynamicVars["Damage"].BaseValue, this);
        var point = Owner.PlayerCombatState?.Pets.FirstOrDefault(p => p.Monster is TacticalPoint && p.IsAlive);
        if (point != null)
        {
            decimal attack = point.Powers.OfType<TacticalPointAttackPower>().FirstOrDefault()?.Amount ?? 0m;
            attack += Owner.Creature.Powers.OfType<TacticalDirectivePower>().Sum(p => p.Amount);
            await ClosureAttackUtils.Attack(choiceContext, cardPlay.Target!, attack, this, point);
        }
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars["Damage"].UpgradeValueBy(3m);
    }
}
