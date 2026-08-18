using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Powers;

namespace ClosureMod.ClosureModCode.Powers;

/// <summary>
/// 能源回收：回合结束时若透支，抽固定数量的牌；不再随透支深度成倍增长。
/// </summary>
public sealed class EnergyRecoveryPower : ClosurePower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;

    public override async Task AfterSideTurnEnd(MegaCrit.Sts2.Core.GameActions.Multiplayer.PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<MegaCrit.Sts2.Core.Entities.Creatures.Creature> participants)
    {
        if (side != CombatSide.Player) return;
        var player = Owner.Player;
        if (player?.PlayerCombatState == null) return;
        int energy = player.PlayerCombatState.Energy;
        if (energy < 0)
        {
            await CardPileCmd.Draw(choiceContext, Amount, player, true);
        }
    }
}
