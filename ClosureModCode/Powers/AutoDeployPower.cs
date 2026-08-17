using ClosureMod.ClosureModCode.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Powers;

namespace ClosureMod.ClosureModCode.Powers;

/// <summary>
/// 自动部署：回合结束时若场上没有战术点，自动召唤1个。
/// </summary>
public sealed class AutoDeployPower : ClosurePower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;

    public override async Task AfterSideTurnEnd(MegaCrit.Sts2.Core.GameActions.Multiplayer.PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<MegaCrit.Sts2.Core.Entities.Creatures.Creature> participants)
    {
        if (side != CombatSide.Player) return;
        var player = Owner.Player;
        if (player == null || player.PlayerCombatState == null) return;
        if (ClosureSummonUtils.AliveTacticalPointCount(player) == 0)
        {
            await ClosureSummonUtils.SummonTacticalPoint(choiceContext, player, ClosureSummonUtils.DefaultHp, ClosureSummonUtils.DefaultAttack, null);
        }
    }
}
