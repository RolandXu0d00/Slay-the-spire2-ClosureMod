using MegaCrit.Sts2.Core.Entities.Powers;

namespace ClosureMod.ClosureModCode.Powers;

/// <summary>
/// 能量枢纽：场上有战术点时，透支上限提高 Amount（由遗物补丁读取生效）。
/// </summary>
public sealed class EnergyHubPower : ClosurePower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;
}
