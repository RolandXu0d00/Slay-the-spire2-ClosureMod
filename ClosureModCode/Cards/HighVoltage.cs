using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace ClosureMod.ClosureModCode.Cards;

/// <summary>
/// 高能放电：造成伤害；本回合每透支1点能量额外造成伤害。
/// </summary>
public sealed class HighVoltage : ClosureCard
{
    protected override List<DynamicVar> CanonicalVars => [new DynamicVar("Damage", 12m), new DynamicVar("PerDebt", 3m)];

    public HighVoltage() : base(2, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        decimal dmg = base.DynamicVars["Damage"].BaseValue;
        int energy = Owner.PlayerCombatState?.Energy ?? 0;
        if (energy < 0)
        {
            dmg += -energy * base.DynamicVars["PerDebt"].BaseValue;
        }
        await CreatureCmd.Damage(choiceContext, cardPlay.Target!, dmg, ValueProp.Move, this);
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars["Damage"].UpgradeValueBy(4m);
    }
}
