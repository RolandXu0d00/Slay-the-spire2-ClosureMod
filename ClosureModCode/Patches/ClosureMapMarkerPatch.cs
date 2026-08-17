using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Nodes.Screens.Map;

namespace ClosureMod.ClosureModCode.Patches;

/// <summary>
/// 官方地图把角色头像放在当前房间的正上方；改为贴在房间的右下角。
/// </summary>
[HarmonyPatch(typeof(NMapMarker), nameof(NMapMarker.SetMapPoint))]
public static class ClosureMapMarkerPatch
{
    static void Postfix(NMapMarker __instance, NMapPoint node)
    {
        try
        {
            // 只调整可露希尔的头像；未启用的标记（非单人模式）也不处理。
            Texture2D? texture = __instance.Texture;
            if (!__instance.Visible || texture == null
                || !texture.ResourcePath.Contains("map_marker_closure", System.StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            // 终止官方默认的“顶部居中”弹出动画，改放到房间右下角。
            Tween? tween = Traverse.Create(__instance).Field("_tween").GetValue<Tween>();
            tween?.Kill();

            // 以房间节点右下角为锚点，头像中心对齐该角，
            // 再往右下方多挪 18px，避免和房间图标贴得太近。
            Vector2 corner = node.Position + new Vector2(node.Size.X, node.Size.Y);
            __instance.Position = corner + new Vector2(18f, 18f) - __instance.Size * 0.5f;
            __instance.Scale = Vector2.One;
        }
        catch (Exception e)
        {
            MainFile.Logger.Warn($"[MapMarker] 调整头像位置失败: {e.Message}");
        }
    }
}
