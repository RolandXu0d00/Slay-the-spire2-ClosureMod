using ClosureMod.ClosureModCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace ClosureMod.ClosureModCode.Cards;

/// <summary>
/// 自动部署：能力。回合结束时若场上没有战术点，自动召唤1个。
/// </summary>
public sealed class AutoDeploy : ClosureCard
{
    public AutoDeploy() : base(2, CardType.Power, CardRarity.Rare, TargetType.None)
    {
    }

    protected override int CanonicalEnergyCost => IsUpgraded ? 1 : 2;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<AutoDeployPower>(choiceContext, Owner.Creature, 1m, Owner.Creature, this);
    }
}
