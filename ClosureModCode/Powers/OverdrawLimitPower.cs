using MegaCrit.Sts2.Core.Entities.Powers;

namespace ClosureMod.ClosureModCode.Powers;

/// <summary>
/// 透支额度：显示当前最多能透支几点能量（Amount = 透支上限）。
/// 挂在玩家身上，数字实时反映当前透支上限。
/// </summary>
public sealed class OverdrawLimitPower : ClosurePower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
}
