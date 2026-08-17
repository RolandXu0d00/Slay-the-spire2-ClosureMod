using ClosureMod.ClosureModCode.Relics;
using HarmonyLib;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Relics;

namespace ClosureMod.ClosureModCode.Patches;

/// <summary>
/// 欧洛巴斯之触升级初始遗物时，可露希尔的战术指挥终端应升级为"升级版终端"，
/// 而不是退化成普通遗物。
/// </summary>
[HarmonyPatch(typeof(TouchOfOrobas), nameof(TouchOfOrobas.GetUpgradedStarterRelic))]
public static class TouchOfOrobasPatch
{
    static void Postfix(RelicModel starterRelic, ref RelicModel __result)
    {
        if (starterRelic is TacticalCommandTerminal)
        {
            __result = ModelDb.Relic<TacticalCommandTerminalPlus>().ToMutable();
        }
    }
}
