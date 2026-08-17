using System.Reflection;
using ClosureMod.ClosureModCode.Character;
using ClosureMod.ClosureModCode.Powers;
using ClosureMod.ClosureModCode.Utils;
using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace ClosureMod.ClosureModCode.Relics;

/// <summary>
/// 可露希尔的起始遗物：战斗开始时部署 1 个战术点；
/// 回合结束时如果能量为负（缺费），施加“透支惩罚”。
/// 同时，整个“缺费（透支）”机制也归属于这件遗物：
/// - 能量允许扣到负数（下限 -MaxDebt）；
/// - 能量不够时依然可以出牌（差额 <= MaxDebt）。
/// </summary>
public class TacticalCommandTerminal : ClosureRelic
{
    /// <summary>可透支的最大能量数（能量下限为 -5）。</summary>
    public virtual int MaxDebt => 2;

    /// <summary>初始战术点生命。</summary>
    public virtual int SummonHp => ClosureSummonUtils.DefaultHp;

    public override RelicRarity Rarity => RelicRarity.Starter;

    public override bool SpawnsPets => true;

    public override async Task BeforeCombatStart()
    {
        await ClosureSummonUtils.SummonTacticalPoint(new ThrowingPlayerChoiceContext(), Owner, SummonHp, ClosureSummonUtils.DefaultAttack, this);
        await Utils.OverdrawLimitUI.Refresh(new ThrowingPlayerChoiceContext(), Owner);
    }

    public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (side != CombatSide.Player || !participants.Any(c => c.Player == Owner)) return;
        await Utils.OverdrawLimitUI.Refresh(choiceContext, Owner);

        if (Owner.PlayerCombatState!.Energy < 0 &&
            !Owner.Creature.Powers.OfType<ClosureDebtPower>().Any())
        {
            await PowerCmd.Apply<ClosureDebtPower>(choiceContext, Owner.Creature, 1m, null, null);
        }
    }

    private static readonly FieldInfo PlayerField =
        AccessTools.Field(typeof(PlayerCombatState), "_player");

    private static bool IsClosureOwner(PlayerCombatState state)
    {
        return PlayerField?.GetValue(state) is Player player && player.Character is Closure;
    }

    /// <summary>
    /// 计算当前透支上限：基础值（终端遗物）+ 能量枢纽（每个战术点+1）+ 能源超载。
    /// </summary>
    public static int GetMaxDebt(Player player)
    {
        int maxDebt = player.Relics.OfType<TacticalCommandTerminal>().FirstOrDefault()?.MaxDebt ?? 2;
        int hubPerPoint = (int)player.Creature.Powers.OfType<Powers.EnergyHubPower>().Sum(p => p.Amount);
        maxDebt += hubPerPoint * ClosureSummonUtils.AliveTacticalPointCount(player);
        maxDebt += (int)player.Creature.Powers.OfType<Powers.EnergyOverloadPower>().Sum(p => p.Amount);
        return maxDebt;
    }

    /// <summary>
    /// 缺费（透支）第 1 步：能量不够时允许出牌（最多透支 MaxDebt 点）。
    /// 只放宽“能量不够”这一条；其他原因（诅咒牌等）仍然阻止出牌。
    /// 使用 Harmony Postfix + 保留原因位：避免覆盖其他 mod 对 reason 的修改。
    /// </summary>
    [HarmonyPatch(typeof(PlayerCombatState), nameof(PlayerCombatState.HasEnoughResourcesFor))]
    public class OverdrawAffordabilityPatch
    {
        static void Postfix(PlayerCombatState __instance, CardModel card, ref bool __result, ref UnplayableReason reason)
        {
            if (__result || !IsClosureOwner(__instance)) return;
            var player = PlayerField?.GetValue(__instance) as Player;
            if (player == null) return;
            int maxDebt = GetMaxDebt(player);

            // 只在"能量不够"这一条原因上放行；如果原 reason 中根本没有能量不足原因，则不干预。
            if ((reason & UnplayableReason.EnergyCostTooHigh) == 0)
            {
                return;
            }

            // 如果还有除能量不足以外的原因（诅咒、禁用等），保持原样不干预，
            // 让其他 mod / 游戏逻辑继续阻止出牌，避免覆盖其他原因。
            if ((reason & ~UnplayableReason.EnergyCostTooHigh) != 0)
            {
                return;
            }

            // 透支规则：打出后能量不能低于 -maxDebt（0费卡在透支满时仍可打出）
            decimal cost = Math.Max(0m, card.EnergyCost.GetWithModifiers(CostModifiers.All));
            if (cost - __instance.Energy <= maxDebt)
            {
                __result = true;
                reason = UnplayableReason.None;
            }
            else
            {
                reason |= UnplayableReason.EnergyCostTooHigh;
            }
            MainFile.Logger.Info($"[Overdraw] card={card.Id.Entry} cost={cost} energy={__instance.Energy} maxDebt={maxDebt} allow={__result}");
        }
    }

    /// <summary>
    /// 缺费（透支）第 2 步：扣能量时允许扣成负数（下限 -MaxDebt），而不是停在 0。
    /// 直接拦截并自行扣费（Prefix 返回 false，跳过原始方法）：
    /// 调用方/其他 mod 对本方法的修改不会产生冲突，因为本方法本身就是完整的替代实现。
    /// </summary>
    [HarmonyPatch(typeof(PlayerCombatState), nameof(PlayerCombatState.LoseEnergy))]
    public class OverdrawLoseEnergyPatch
    {
        static bool Prefix(PlayerCombatState __instance, decimal amount)
        {
            if (!IsClosureOwner(__instance)) return true;
            var player = PlayerField?.GetValue(__instance) as Player;
            int maxDebt = player == null ? 2 : GetMaxDebt(player);

            __instance.Energy = (int)Math.Clamp((decimal)__instance.Energy - amount, -maxDebt, 999999999m);
            return false;
        }
    }
}
