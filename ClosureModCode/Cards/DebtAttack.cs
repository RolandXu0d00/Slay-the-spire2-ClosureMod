using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace ClosureMod.ClosureModCode.Cards;

/// <summary>
/// 负债攻击：造成伤害；处于透支状态时造成更多伤害。
/// </summary>
public sealed class DebtAttack : ClosureCard
{
    protected override List<DynamicVar> CanonicalVars => [new DynamicVar("Damage", 8m), new DynamicVar("DebtBonus", 4m)];

    public DebtAttack() : base(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
    {
    }

    protected override HashSet<CardTag> CanonicalTags => [CardTag.Strike];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        decimal dmg = base.DynamicVars["Damage"].BaseValue;
        if (Owner.PlayerCombatState?.Energy < 0)
        {
            dmg += base.DynamicVars["DebtBonus"].BaseValue;
        }
        await CreatureCmd.Damage(choiceContext, cardPlay.Target!, dmg, ValueProp.Move, this);
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars["Damage"].UpgradeValueBy(3m);
    }
}
