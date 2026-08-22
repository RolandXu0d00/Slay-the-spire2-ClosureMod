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
    protected override List<DynamicVar> CanonicalVars =>
        [new DynamicVar("HP", 6m), new DynamicVar("Attack", 1m)];

    public DoubleDeploy() : base(1, CardType.Skill, CardRarity.Rare, TargetType.None)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        int hp = (int)base.DynamicVars["HP"].BaseValue;
        int attack = (int)base.DynamicVars["Attack"].BaseValue;
        await ClosureSummonUtils.SummonTacticalPoint(choiceContext, Owner, hp, attack, this);
        await ClosureSummonUtils.SummonTacticalPoint(choiceContext, Owner, hp, attack, this);
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars["HP"].UpgradeValueBy(2m);
        base.DynamicVars["Attack"].UpgradeValueBy(1m);
    }
}
