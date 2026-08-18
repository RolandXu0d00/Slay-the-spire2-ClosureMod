using ClosureMod.ClosureModCode.Monsters;
using ClosureMod.ClosureModCode.Powers;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.ValueProps;

namespace ClosureMod.ClosureModCode.Patches;

/// <summary>
/// 战术点拦截链的补丁：当第一个战术点被溢出伤害打死时，把溢出部分继续交给
/// 下一个存活战术点，直到没有战术点，最后剩下的溢出伤害才由可露希尔承受。
/// </summary>
[HarmonyPatch(typeof(Creature), nameof(Creature.LoseHpInternal))]
public static class TacticalPointLoseHpPatch
{
    private static void Postfix(Creature __instance, ValueProp props, ref DamageResult __result)
    {
        if (TacticalPointInterceptorPower.ChainLead == null || __instance != TacticalPointInterceptorPower.ChainLead)
        {
            return;
        }

        try
        {
            if (__instance.Monster is not TacticalPoint || __result.OverkillDamage <= 0m)
            {
                return;
            }

            var state = __instance.PetOwner?.PlayerCombatState;
            if (state == null)
            {
                return;
            }

            decimal remaining = __result.OverkillDamage;
            var others = state.Pets
                .Where(p => p.Monster is TacticalPoint && p.IsAlive && p != __instance)
                .ToList();

            foreach (Creature point in others)
            {
                if (remaining <= 0m)
                {
                    break;
                }

                DamageResult pointResult = point.LoseHpInternal(remaining, props);

                remaining = pointResult.OverkillDamage;
                if (pointResult.WasTargetKilled)
                {
                    _ = CreatureCmd.Kill(point);
                }
            }

            __result = new DamageResult(__result.Receiver, __result.Props)
            {
                UnblockedDamage = __result.UnblockedDamage,
                WasTargetKilled = __result.WasTargetKilled,
                OverkillDamage = (int)remaining
            };
        }
        finally
        {
            TacticalPointInterceptorPower.ChainLead = null;
        }
    }
}
