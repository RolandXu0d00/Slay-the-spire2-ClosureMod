using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using ClosureMod.ClosureModCode.Character;
using ClosureMod.ClosureModCode.Extensions;

namespace ClosureMod.ClosureModCode.Potions;

[Pool(typeof(ClosurePotionPool))]
public abstract class ClosurePotion : CustomPotionModel
{
    public override string? CustomPackedImagePath =>
        $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".PotionImagePath();
    public override string? CustomPackedOutlinePath =>
        $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".PotionOutlineImagePath();
}
