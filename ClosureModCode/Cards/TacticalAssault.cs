using ClosureMod.ClosureModCode.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace ClosureMod.ClosureModCode.Cards;

/// <summary>
/// 战术突击：高伤害，场上每个战术点再造成大量伤害。指挥流终端攻击牌。
/// </summary>
public sealed class TacticalAssault : ClosureCard
{
    protected override List<DynamicVar> CanonicalVars => [new DynamicVar("Damage", 12m), new DynamicVar("PerPoint", 4m)];

    public TacticalAssault() : base(2, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
    {
    }

    protected override HashSet<CardTag> CanonicalTags => [CardTag.Strike];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        decimal dmg = base.DynamicVars["Damage"].BaseValue
            + ClosureSummonUtils.AliveTacticalPointCount(Owner) * base.DynamicVars["PerPoint"].BaseValue;
        await CreatureCmd.Damage(choiceContext, cardPlay.Target!, dmg, ValueProp.Move, this);
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars["Damage"].UpgradeValueBy(4m);
    }
}
