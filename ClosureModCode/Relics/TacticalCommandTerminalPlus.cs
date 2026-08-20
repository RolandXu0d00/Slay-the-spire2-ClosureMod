using ClosureMod.ClosureModCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace ClosureMod.ClosureModCode.Relics;

/// <summary>
/// 升级版战术指挥终端（欧洛巴斯之触替换所得）：透支上限3；
/// 每当支付卡牌费用新增1点透支，本回合战术点攻击力+2。
/// </summary>
public sealed class TacticalCommandTerminalPlus : TacticalCommandTerminal
{
    public override int MaxDebt => 3;

    public override RelicRarity Rarity => RelicRarity.Ancient;

    public override async Task BeforeCardPlayed(CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner != Owner || !cardPlay.IsFirstInSeries || Owner.PlayerCombatState == null) return;

        int energyAfterPayment = Owner.PlayerCombatState.Energy;
        int energyBeforePayment = energyAfterPayment + cardPlay.Resources.EnergySpent;
        int newOverdraw = Math.Max(0, -energyAfterPayment) - Math.Max(0, -energyBeforePayment);
        if (newOverdraw <= 0) return;

        await PowerCmd.Apply<TacticalDirectivePower>(
            new ThrowingPlayerChoiceContext(), Owner.Creature, newOverdraw * 2m, Owner.Creature, null);
    }
}
