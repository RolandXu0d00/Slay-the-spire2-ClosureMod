using ClosureMod.ClosureModCode.Character;
using ClosureMod.ClosureModCode.Utils;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Nodes.Screens.RunHistoryScreen;
using MegaCrit.Sts2.Core.Runs;

namespace ClosureMod.ClosureModCode.Patches;

/// <summary>
/// 战报（RunHistory）里可露希尔的头像改为 Die 动画最后一帧（蹲下跪地姿势），
/// 与结算界面的死亡定格姿势保持一致。
/// </summary>
[HarmonyPatch(typeof(NRunHistoryPlayerIcon), nameof(NRunHistoryPlayerIcon.LoadRun))]
public static class ClosureRunHistoryDiePosePatch
{
    static void Postfix(NRunHistoryPlayerIcon __instance, RunHistoryPlayer player)
    {
        try
        {
            if (!string.Equals(player.Character.Entry, Closure.CharacterId, System.StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            Texture2D? lastDieFrame = ClosureSpriteVisualFactory.LoadDieLastFrame();
            if (lastDieFrame == null)
            {
                MainFile.Logger.Warn("[RunHistory] 未找到 Die 动画最后一帧资源，头像保持原样");
                return;
            }

            TextureRect? icon = Traverse.Create(__instance).Field("_icon").GetValue<TextureRect>();
            if (icon == null)
            {
                return;
            }
            icon.Texture = lastDieFrame;

            // 原图是 640×640 的全身帧，直接填满 64×64 的图标区域会显得偏大；
            // 缩到 50% 并保持居中，观感更接近官方头像。
            Vector2 originalSize = icon.Size;
            Vector2 smallerSize = originalSize * 0.5f;
            icon.Size = smallerSize;
            icon.Position += (originalSize - smallerSize) * 0.5f;

            MainFile.Logger.Info("[RunHistory] 可露希尔头像已替换为 Die 最后一帧");
        }
        catch (Exception e)
        {
            MainFile.Logger.Warn($"[RunHistory] 替换头像失败: {e.Message}");
        }
    }
}
