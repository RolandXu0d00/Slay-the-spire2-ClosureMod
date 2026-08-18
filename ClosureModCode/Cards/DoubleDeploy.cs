using ClosureMod.ClosureModCode.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace ClosureMod.ClosureModCode.Cards;

/// <summary>
/// 双重部署：召唤2个战术点（受上限限制）。
/// </summary>
public sealed class DoubleDeploy : ClosureCard
{
    protected override List<DynamicVar> CanonicalVars => [new DynamicVar("HP", ClosureSummonUtils.DefaultHp)];

    public DoubleDeploy() : base(1, CardType.Skill, CardRarity.Rare, TargetType.None)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        int hp = (int)base.DynamicVars["HP"].BaseValue;
        await ClosureSummonUtils.SummonTacticalPoint(choiceContext, Owner, hp, ClosureSummonUtils.DefaultAttack, this);
        await ClosureSummonUtils.SummonTacticalPoint(choiceContext, Owner, hp, ClosureSummonUtils.DefaultAttack, this);
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars["HP"].UpgradeValueBy(2m);
    }
}
