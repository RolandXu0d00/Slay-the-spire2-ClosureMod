using ClosureMod.ClosureModCode.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace ClosureMod.ClosureModCode.Cards;

/// <summary>
/// 弹幕：造成伤害，场上每个战术点再造成少量伤害。
/// </summary>
public sealed class Barrage : ClosureCard
{
    protected override List<DynamicVar> CanonicalVars => [new DynamicVar("Damage", 5m), new DynamicVar("PerPoint", 1m)];

    public Barrage() : base(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
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
        base.DynamicVars["Damage"].UpgradeValueBy(2m);
    }
}
