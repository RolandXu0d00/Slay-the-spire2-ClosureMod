using ClosureMod.ClosureModCode.Character;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Screens.Shops;

namespace ClosureMod.ClosureModCode.Patches;

/// <summary>
/// 商店原本通过 Character.MerchantAnimPath 创建原版 Spine 模型，且不会调用
/// CharacterModel.CreateCustomVisuals。保留原节点供商店内部逻辑调用，但隐藏其
/// 铁甲战士画面，并叠加从基建 Relax 素材抠出的可露希尔循环动画。
/// </summary>
[HarmonyPatch]
public static class ClosureMerchantVisualPatch
{
    private static readonly StringName ClosureVisualMeta = "closure_custom_merchant_visual";

    [HarmonyPatch(typeof(NMerchantRoom), "AfterRoomIsLoaded")]
    [HarmonyPostfix]
    static void ReplaceMerchantVisuals(NMerchantRoom __instance)
    {
        try
        {
            var players = Traverse.Create(__instance).Field("_players").GetValue<List<Player>>();
            var visuals = __instance.PlayerVisuals;
            int count = Math.Min(players.Count, visuals.Count);

            for (int i = 0; i < count; i++)
            {
                if (players[i].Character is not Closure)
                    continue;

                NMerchantCharacter merchantCharacter = visuals[i];
                if (merchantCharacter.HasMeta(ClosureVisualMeta))
                    continue;

                // 清除商店对角色节点施加的灰暗/半透明颜色，避免透明 PNG 进一步变淡。
                merchantCharacter.Modulate = Colors.White;
                merchantCharacter.SelfModulate = Colors.White;

                // 不删除原 Spine 节点：商店之后仍会调用它播放 relaxed_loop 等动画。
                // 仅把画面隐藏，可避免破坏原版商店逻辑。
                foreach (CanvasItem child in merchantCharacter.GetChildren().OfType<CanvasItem>())
                {
                    child.Visible = false;
                }

                var frames = new SpriteFrames();
                frames.RemoveAnimation("default");
                frames.AddAnimation("Relax");
                frames.SetAnimationSpeed("Relax", 15d);
                frames.SetAnimationLoop("Relax", true);
                for (int frame = 1; frame <= 119; frame++)
                {
                    var texture = GD.Load<Texture2D>(
                        $"res://ClosureMod/images/merchant_relax_clean/frame_{frame:0000}.png");
                    if (texture != null)
                        frames.AddFrame("Relax", texture);
                }
                if (frames.GetFrameCount("Relax") == 0)
                    continue;

                // 素材为 492×592，脚底约在 y=547；按商店展示区缩放并锚定脚底。
                const float merchantScale = 0.72f;
                var closureAnimation = new AnimatedSprite2D
                {
                    Name = "ClosureMerchantRelax",
                    SpriteFrames = frames,
                    Position = new Vector2(0f, -(547f - 592f * 0.5f) * merchantScale),
                    Scale = new Vector2(merchantScale, merchantScale),
                    Modulate = Colors.White,
                    SelfModulate = Colors.White,
                };
                merchantCharacter.AddChild(closureAnimation);
                closureAnimation.Play("Relax");
                merchantCharacter.SetMeta(ClosureVisualMeta, true);
                MainFile.Logger.Info("[MerchantVisual] 已将商店角色画面替换为可露希尔 Relax 动画");
            }
        }
        catch (Exception e)
        {
            MainFile.Logger.Error($"[MerchantVisual] 替换商店角色失败: {e}");
        }
    }
}
