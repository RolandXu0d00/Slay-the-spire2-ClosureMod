using BaseLib.Abstracts;
using BaseLib.Extensions;
using ClosureMod.ClosureModCode.Extensions;

namespace ClosureMod.ClosureModCode.Powers;

/// <summary>
/// 可露希尔能力的基类：负责从模组资源加载能力图标。
/// </summary>
public abstract class ClosurePower : CustomPowerModel
{
    public override string CustomPackedIconPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".PowerImagePath();
    public override string CustomBigIconPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigPowerImagePath();
}
