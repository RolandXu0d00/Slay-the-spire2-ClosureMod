using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace ClosureMod.ClosureModCode.Utils;

/// <summary>
/// 使用游戏标准 AttackCommand 结算卡牌攻击，确保胆小等 AfterAttack 机制能识别卡牌来源。
/// </summary>
public static class ClosureAttackUtils
{
    public static async Task Attack(PlayerChoiceContext choiceContext, Creature target, decimal damage, CardModel source, Creature? visualAttacker = null)
    {
        var command = DamageCmd.Attack(damage)
            .FromCard(source)
            .WithAttackerAnim(null, 0f, visualAttacker)
            .Targeting(target);
        await command.Execute(choiceContext);
    }

    public static async Task AttackAll(PlayerChoiceContext choiceContext, decimal damage, CardModel source)
    {
        var combatState = source.Owner.Creature.CombatState;
        if (combatState == null) return;
        await DamageCmd.Attack(damage)
            .FromCard(source)
            .WithAttackerAnim(null, 0f)
            .TargetingAllOpponents(combatState)
            .Execute(choiceContext);
    }
}
