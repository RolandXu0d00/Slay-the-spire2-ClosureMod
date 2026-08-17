using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;

namespace ClosureMod.ClosureModCode.Powers;

/// <summary>
/// “透支惩罚”：下回合开始时能量 -Amount（由回合结束时的缺费状态触发）。
/// </summary>
public sealed class ClosureDebtPower : ClosurePower
{
    public override PowerType Type => PowerType.Debuff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterEnergyReset(Player player)
    {
        if (player == Owner.Player)
        {
            await PlayerCmd.LoseEnergy(Amount, player);
            await PowerCmd.Remove(this);
        }
    }
}
