using BaseLib.Abstracts;
using ClosureMod.ClosureModCode.Extensions;
using Godot;

namespace ClosureMod.ClosureModCode.Character;

public class ClosureRelicPool : CustomRelicPoolModel
{
    public override Color LabOutlineColor => Closure.Color;

    public override string BigEnergyIconPath => "charui/big_energy.png".ImagePath();
    public override string TextEnergyIconPath => "charui/text_energy.png".ImagePath();
}
