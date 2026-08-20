using BaseLib.Abstracts;
using ClosureMod.ClosureModCode.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace ClosureMod.ClosureModCode.Cards;

/// <summary>
/// 源石脉冲：远古稀有卡，Darv 的尘土之书会给可露希尔这张卡（升级版）。
/// 造成大量伤害；本回合每透支1点能量额外造成伤害。
/// </summary>
public sealed class OriginiumPulse : ClosureCard, ITomeCard
{
    protected override List<DynamicVar> CanonicalVars => [new DynamicVar("Damage", 24m), new DynamicVar("PerDebt", 5m)];

    public OriginiumPulse() : base(2, CardType.Attack, CardRarity.Ancient, TargetType.AnyEnemy)
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
        await ClosureAttackUtils.Attack(choiceContext, cardPlay.Target!, dmg, this);
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars["Damage"].UpgradeValueBy(8m);
        base.DynamicVars["PerDebt"].UpgradeValueBy(1m);
    }
}
