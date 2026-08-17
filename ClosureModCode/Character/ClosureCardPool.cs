using BaseLib.Abstracts;
using ClosureMod.ClosureModCode.Extensions;
using Godot;

namespace ClosureMod.ClosureModCode.Character;

public class ClosureCardPool : CustomCardPoolModel
{
    public override string Title => Closure.CharacterId; //This is not a display name.

    public override string BigEnergyIconPath => "charui/big_energy.png".ImagePath();
    public override string TextEnergyIconPath => "charui/text_energy.png".ImagePath();

    //淡蓝色卡背
    public override float H => 0.56f;
    public override float S => 0.85f;
    public override float V => 0.95f;

    //Color of small card icons
    public override Color DeckEntryCardColor => Closure.Color;

    public override bool IsColorless => false;
}
