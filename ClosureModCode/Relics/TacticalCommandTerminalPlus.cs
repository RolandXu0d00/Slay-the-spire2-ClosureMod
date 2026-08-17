using MegaCrit.Sts2.Core.Entities.Relics;

namespace ClosureMod.ClosureModCode.Relics;

/// <summary>
/// 升级版战术指挥终端（欧洛巴斯之触替换所得）：透支上限3、战术点生命8。
/// </summary>
public sealed class TacticalCommandTerminalPlus : TacticalCommandTerminal
{
    public override int MaxDebt => 3;

    public override int SummonHp => 8;

    public override RelicRarity Rarity => RelicRarity.Ancient;
}
