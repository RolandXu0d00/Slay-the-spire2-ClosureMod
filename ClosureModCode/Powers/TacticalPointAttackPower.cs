using ClosureMod.ClosureModCode.Monsters;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.ValueProps;

namespace ClosureMod.ClosureModCode.Powers;

/// <summary>
/// 挂在战术点身上：既是战术点的攻击力数值，也负责回合结束时自动攻击。
/// </summary>
public sealed class TacticalPointAttackPower : ClosurePower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task BeforeSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        //只在玩家回合结束时结算（怪物还没行动）
        if (side != CombatSide.Player) return;
        if (Owner == null || !Owner.IsAlive || Owner.PetOwner == null) return;

        Player? player = Owner.PetOwner;
        if (player?.PlayerCombatState == null) return;

        var enemies = Owner.CombatState?.Enemies.Where(e => e.IsAlive).ToList();
        if (enemies == null || enemies.Count == 0) return;

        //基础攻击力 + 本回合的“战术指令/集火协议”加成
        decimal damage = Amount;
        Creature playerCreature = player.Creature;
        damage += playerCreature.Powers.OfType<TacticalDirectivePower>().Sum(p => p.Amount);

        int extraAttacks = (int)playerCreature.Powers.OfType<ConcentratedFirePower>().Sum(p => p.Amount);
        int hits = 1 + extraAttacks;

        for (int i = 0; i < hits; i++)
        {
            Creature? target = player.RunState.Rng.CombatTargets.NextItem(enemies);
            if (target == null) continue;
            await CreatureCmd.Damage(choiceContext, target, damage, ValueProp.Move, playerCreature);
        }
    }
}
