using ClosureMod.ClosureModCode.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace ClosureMod.ClosureModCode.Cards;

/// <summary>
/// 双重部署：召唤2个战术点（受上限限制）。
/// </summary>
public sealed class DoubleDeploy : ClosureCard
{
    public DoubleDeploy() : base(2, CardType.Skill, CardRarity.Rare, TargetType.None)
    {
    }

    protected override int CanonicalEnergyCost => IsUpgraded ? 1 : 2;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await ClosureSummonUtils.SummonTacticalPoint(choiceContext, Owner, ClosureSummonUtils.DefaultHp, ClosureSummonUtils.DefaultAttack, this);
        await ClosureSummonUtils.SummonTacticalPoint(choiceContext, Owner, ClosureSummonUtils.DefaultHp, ClosureSummonUtils.DefaultAttack, this);
    }
}
