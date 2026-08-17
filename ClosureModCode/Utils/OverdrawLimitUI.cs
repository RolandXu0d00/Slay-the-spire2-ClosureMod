using ClosureMod.ClosureModCode.Powers;
using ClosureMod.ClosureModCode.Relics;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace ClosureMod.ClosureModCode.Utils;

/// <summary>
/// 把当前透支上限同步到玩家身上的"透支额度"能力图标上，
/// 让玩家随时知道最多还能欠几费。
/// </summary>
public static class OverdrawLimitUI
{
    public static async Task Refresh(PlayerChoiceContext choiceContext, Player player)
    {
        if (player?.PlayerCombatState == null || player.Creature == null) return;
        int maxDebt = TacticalCommandTerminal.GetMaxDebt(player);
        var existing = player.Creature.Powers.OfType<OverdrawLimitPower>().FirstOrDefault();
        if (existing == null)
        {
            await PowerCmd.Apply<OverdrawLimitPower>(choiceContext, player.Creature, maxDebt, null, null);
        }
        else
        {
            int delta = maxDebt - existing.Amount;
            if (delta != 0)
            {
                await PowerCmd.ModifyAmount(choiceContext, existing, delta, player.Creature, null, false);
            }
        }
    }
}
