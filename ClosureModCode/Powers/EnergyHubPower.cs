using MegaCrit.Sts2.Core.Entities.Powers;

namespace ClosureMod.ClosureModCode.Powers;

/// <summary>
/// 能量枢纽：场上每有1个战术点，透支上限+1（由遗物补丁读取生效）。
/// </summary>
public sealed class EnergyHubPower : ClosurePower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;
}
