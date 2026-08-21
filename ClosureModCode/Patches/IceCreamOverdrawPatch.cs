using ClosureMod.ClosureModCode.Character;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models.Relics;

namespace ClosureMod.ClosureModCode.Patches;

/// <summary>
/// 冰淇淋只应保存回合末剩余的正能量，不应把可露希尔的透支债务带到下一回合。
/// 负能量时让游戏执行正常的回合能量重置；零和正能量保持原版冰淇淋行为。
/// </summary>
[HarmonyPatch(typeof(IceCream), nameof(IceCream.ShouldPlayerResetEnergy))]
public static class IceCreamOverdrawPatch
{
    static void Postfix(IceCream __instance, Player player, ref bool __result)
    {
        if (player == __instance.Owner &&
            player.Character is Closure &&
            player.PlayerCombatState?.Energy < 0)
        {
            __result = true;
        }
    }
}
