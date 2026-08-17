using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using ClosureMod.ClosureModCode.Character;
using ClosureMod.ClosureModCode.Extensions;

namespace ClosureMod.ClosureModCode.Relics;

/// <summary>
/// 可露希尔遗物的基类：负责从模组资源加载遗物图标，并标记遗物池。
/// </summary>
[Pool(typeof(ClosureRelicPool))]
public abstract class ClosureRelic : CustomRelicModel
{
    public override string PackedIconPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".RelicImagePath();
    protected override string PackedIconOutlinePath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}_outline.png".RelicImagePath();
    protected override string BigIconPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigRelicImagePath();
}
