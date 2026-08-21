using ClosureMod.ClosureModCode.Monsters;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Rooms;

namespace ClosureMod.ClosureModCode.Patches;

/// <summary>
/// 修正战术点的站位：游戏默认把普通宠物放在角色脚下（会与角色模型重叠），
/// 这里把战术点摆到角色右前方，并且多个战术点之间彼此错开。
/// </summary>
internal static class TacticalPointPositioning
{
    /// <summary>
    /// 战术点相对角色的基础偏移：
    /// X = 角色正前方更远一些（比之前的 160 更远），Y = 与角色脚部同一水平线（不再抬高）。
    /// </summary>
    private static readonly Vector2 BaseOffset = new(240f, 10f);

    /// <summary>多个战术点之间的横向间距。</summary>
    private const float Spacing = 95f;

    public static void Reposition(List<NCreature> nodes)
    {
        if (nodes.Count == 0) return;

        var points = nodes.Where(n => n.Entity is { PetOwner: not null } && n.Entity.Monster is TacticalPoint).ToList();
        foreach (NCreature point in points)
        {
            Player? owner = point.Entity.PetOwner;
            if (owner == null) continue;

            NCreature? ownerNode = nodes.FirstOrDefault(n => n.Entity.Player == owner);
            if (ownerNode == null) continue;

            var petOrder = owner.PlayerCombatState?.Pets.ToList() ?? [];
            var ownerPoints = points.Where(n => n.Entity.PetOwner == owner)
                .OrderBy(n => petOrder.IndexOf(n.Entity))
                .ToList();
            int index = Math.Max(0, ownerPoints.IndexOf(point));

            point.Position = ownerNode.Position + BaseOffset + Vector2.Right * (Spacing * index);
        }
    }

    public static void RepositionCurrentRoom()
    {
        if (NCombatRoom.Instance != null)
        {
            Reposition(NCombatRoom.Instance.CreatureNodes.ToList());
        }
    }
}

/// <summary>战斗开始时按新站位摆放战术点。</summary>
[HarmonyPatch(typeof(NCombatRoom), nameof(NCombatRoom.PositionPlayersAndPets))]
public static class TacticalPointLayoutPatch
{
    static void Postfix(List<NCreature> creatureNodes)
    {
        TacticalPointPositioning.Reposition(creatureNodes);
    }
}

/// <summary>战斗中召唤新的战术点时，重新摆放该玩家所有战术点。</summary>
[HarmonyPatch(typeof(NCombatRoom), nameof(NCombatRoom.AddCreature))]
public static class TacticalPointSummonPatch
{
    static void Postfix(NCombatRoom __instance)
    {
        TacticalPointPositioning.Reposition(__instance.CreatureNodes.ToList());

        // 游戏默认会把“非奥斯提宠物”的交互与血条强制关闭（奥斯提是特例）。
        // 战术点需要像奥斯提一样常显血条（同时可被卡牌选中），这里重新开启。
        foreach (NCreature node in __instance.CreatureNodes)
        {
            if (node.Entity.Monster is TacticalPoint)
            {
                node.ToggleIsInteractable(true);
            }
        }
    }
}
