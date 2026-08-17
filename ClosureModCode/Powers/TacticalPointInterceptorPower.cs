using ClosureMod.ClosureModCode.Monsters;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace ClosureMod.ClosureModCode.Powers;

/// <summary>
/// 战术点的“拦截”能力：代替可露希尔承受攻击伤害（机制同亡灵契约师的奥斯提）。
/// 场上多个战术点时，只有“第一个存活的战术点”承担拦截；它倒下后由下一个接替。
/// </summary>
public sealed class TacticalPointInterceptorPower : ClosurePower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;
    public override bool ShouldPlayVfx => false;

    public override Creature ModifyUnblockedDamageTarget(Creature target, decimal _, ValueProp props, Creature? __)
    {
        MainFile.Logger.Info($"[拦截] 调用: 目标={target.Name}, 持有者={Owner.Name}, 攻击判定={props.IsPoweredAttack()}");
        if (target != Owner.PetOwner?.Creature)
        {
            MainFile.Logger.Info($"[拦截] 目标不是主人，不拦截");
            return target;
        }
        if (Owner.IsDead)
        {
            MainFile.Logger.Info($"[拦截] 战术点已死亡，不拦截");
            return target;
        }
        if (!props.IsPoweredAttack())
        {
            MainFile.Logger.Info($"[拦截] 非攻击伤害，不拦截");
            return target;
        }
        if (Owner.PetOwner?.PlayerCombatState == null)
        {
            MainFile.Logger.Info($"[拦截] 主人战斗状态为空，不拦截");
            return target;
        }

        // 只让第一个存活的战术点拦截，避免多个战术点同时转移同一份伤害
        Creature? lead = Owner.PetOwner.PlayerCombatState.Pets
            .FirstOrDefault(p => p.Monster is TacticalPoint && p.IsAlive);
        if (lead != Owner)
        {
            MainFile.Logger.Info($"[拦截] 非首个存活战术点，不拦截");
            return target;
        }
        MainFile.Logger.Info($"[拦截] 转移伤害到战术点: {Owner.Name}");
        return Owner;
    }
}
