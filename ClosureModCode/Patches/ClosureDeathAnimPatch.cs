using ClosureMod.ClosureModCode.Utils;
using HarmonyLib;
using MegaCrit.Sts2.Core.Nodes.Combat;

namespace ClosureMod.ClosureModCode.Patches;

/// <summary>
/// 游戏原生只对 Spine 模型播放死亡触发；可露希尔使用逐帧精灵动画，
/// 这里在角色死亡开始时把 Dead 信号转给逐帧动画器，播放蹲下倒地动画。
/// </summary>
[HarmonyPatch(typeof(NCreature), nameof(NCreature.StartDeathAnim))]
public static class ClosureDeathAnimPatch
{
    static void Postfix(NCreature __instance)
    {
        if (__instance.Visuals == null)
        {
            return;
        }
        __instance.Visuals.GetNodeOrNull<ClosureAnimatorNode>("Animator")?.Play("Dead");
    }
}
