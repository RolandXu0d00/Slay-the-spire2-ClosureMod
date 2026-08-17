using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Powers;

namespace ClosureMod.ClosureModCode.Powers;

/// <summary>
/// 透支契约：回合结束时若处于透支状态，下回合开始时额外获得2点能量。
/// </summary>
public sealed class DebtContractPower : ClosurePower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;

    private bool _lastTurnOverdrawn;

    public override async Task AfterSideTurnEnd(MegaCrit.Sts2.Core.GameActions.Multiplayer.PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<MegaCrit.Sts2.Core.Entities.Creatures.Creature> participants)
    {
        if (side != CombatSide.Player) return;
        _lastTurnOverdrawn = Owner.Player?.PlayerCombatState?.Energy < 0;
        await Task.CompletedTask;
    }

    public override async Task AfterEnergyReset(MegaCrit.Sts2.Core.Entities.Players.Player player)
    {
        if (player == Owner.Player && _lastTurnOverdrawn)
        {
            await PlayerCmd.GainEnergy(Amount, player);
            _lastTurnOverdrawn = false;
        }
    }
}
