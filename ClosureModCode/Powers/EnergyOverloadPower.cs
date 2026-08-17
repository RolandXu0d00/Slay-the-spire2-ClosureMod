using ClosureMod.ClosureModCode.Monsters;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Powers;

namespace ClosureMod.ClosureModCode.Powers;

/// <summary>
/// 能源超载：透支上限+1；回合结束时若处于透支状态，每个战术点攻击力+1。
/// </summary>
public sealed class EnergyOverloadPower : ClosurePower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterSideTurnEnd(MegaCrit.Sts2.Core.GameActions.Multiplayer.PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<MegaCrit.Sts2.Core.Entities.Creatures.Creature> participants)
    {
        if (side != CombatSide.Player) return;
        var player = Owner.Player;
        if (player?.PlayerCombatState == null || player.PlayerCombatState.Energy >= 0) return;
        var points = player.PlayerCombatState.Pets
            .Where(p => p.Monster is TacticalPoint && p.IsAlive)
            .ToList();
        foreach (var point in points)
        {
            await PowerCmd.Apply<TacticalPointAttackPower>(choiceContext, point, 1m, null, null);
        }
    }
}
