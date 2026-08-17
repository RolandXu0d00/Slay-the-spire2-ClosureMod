using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;

namespace ClosureMod.ClosureModCode.Powers;

/// <summary>
/// “集火协议”：本回合结束时每个战术点额外攻击 +Amount 次。
/// 在下一个回合开始时自动清除。
/// </summary>
public sealed class ConcentratedFirePower : ClosurePower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterEnergyReset(Player player)
    {
        if (player == Owner.Player)
        {
            await PowerCmd.Remove(this);
        }
    }
}
