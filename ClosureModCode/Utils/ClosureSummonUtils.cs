using ClosureMod.ClosureModCode.Monsters;
using ClosureMod.ClosureModCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace ClosureMod.ClosureModCode.Utils;

public static class ClosureSummonUtils
{
    public const int MaxTacticalPoints = 3;
    public const int DefaultHp = 8;
    public const int DefaultAttack = 2;

    public static int AliveTacticalPointCount(Player player)
    {
        if (player.PlayerCombatState == null) return 0;
        return player.PlayerCombatState.Pets.Count(p => p.Monster is TacticalPoint && p.IsAlive);
    }

    /// <summary>
    /// 召唤一个战术点；场上已有上限数量时返回 false。
    /// </summary>
    public static async Task<bool> SummonTacticalPoint(PlayerChoiceContext choiceContext, Player player, int hp, int attack, AbstractModel? source)
    {
        if (AliveTacticalPointCount(player) >= MaxTacticalPoints) return false;

        var pet = await PlayerCmd.AddPet<TacticalPoint>(player);
        await CreatureCmd.SetMaxHp(pet, hp);
        await CreatureCmd.Heal(pet, hp, playAnim: false);
        await PowerCmd.Apply<TacticalPointAttackPower>(choiceContext, pet, attack, null, null);
        await PowerCmd.Apply<TacticalPointInterceptorPower>(choiceContext, pet, 1m, null, null);
        await OverdrawLimitUI.Refresh(choiceContext, player);
        MainFile.Logger.Info($"[Summon] 战术点 HP={pet.CurrentHp}/{pet.MaxHp}, 血条可见={pet.Monster?.IsHealthBarVisible}, 存活数={AliveTacticalPointCount(player)}");
        MainFile.Logger.Info($"[Summon] 战术点能力: {string.Join(", ", pet.Powers.Select(p => p.GetType().Name))}");
        return true;
    }
}
