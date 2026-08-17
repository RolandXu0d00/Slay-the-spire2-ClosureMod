using ClosureMod.ClosureModCode.Utils;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Screens.GameOverScreen;

namespace ClosureMod.ClosureModCode.Patches;

/// <summary>
/// 结算界面兜底：游戏结算时会把角色模型移到“CreatureContainer”层，
/// 这里确保可露希尔的逐帧模型一定处于死亡姿势（正在播或停在“蹲下跪地”最后一帧），
/// 直到结算界面结束。
/// </summary>
[HarmonyPatch(typeof(NGameOverScreen), nameof(NGameOverScreen.AfterOverlayOpened))]
public static class ClosureGameOverAnimPatch
{
    static void Postfix(NGameOverScreen __instance)
    {
        try
        {
            Control? container = __instance.GetNodeOrNull<Control>("%CreatureContainer");
            if (container == null)
            {
                return;
            }
            foreach (Node child in container.GetChildren())
            {
                if (child is NCreatureVisuals visuals)
                {
                    ClosureAnimatorNode? animator = visuals.GetNodeOrNull<ClosureAnimatorNode>("Animator");
                    if (animator == null)
                    {
                        continue;
                    }
                    animator.EnterDeathPoseIfNeeded();
                    // 若死亡动画仍未生效（例如模型被重建且触发丢失），直接定格在战败动画最后一帧。
                    if (!animator.IsInDeathPose)
                    {
                        animator.PinDeathPoseEnd();
                        MainFile.Logger.Info("[GameOverAnim] 模型未处于死亡状态，已强制定格在战败动画最后一帧");
                    }
                }
            }
        }
        catch (Exception e)
        {
            MainFile.Logger.Warn($"[GameOverAnim] 设置结算死亡姿势失败: {e.Message}");
        }
    }
}
