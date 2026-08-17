using ClosureMod.ClosureModCode.Utils;
using HarmonyLib;
using MegaCrit.Sts2.Core.Nodes.Combat;

namespace ClosureMod.ClosureModCode.Patches;

/// <summary>
/// 游戏内触发攻击/施法/受击/死亡等动作时，把信号转发给可露希尔的逐帧动画组件。
/// </summary>
[HarmonyPatch(typeof(NCreature), nameof(NCreature.SetAnimationTrigger))]
public static class ClosureAnimatorTriggerPatch
{
    static void Postfix(NCreature __instance, string trigger)
    {
        if (__instance.Visuals == null)
        {
            return;
        }
        __instance.Visuals.GetNodeOrNull<ClosureAnimatorNode>("Animator")?.Play(trigger);
    }
}
