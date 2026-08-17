using BaseLib.Patches.Features;
using ClosureMod.ClosureModCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace ClosureMod.ClosureModCode.Cards;

/// <summary>
/// 强化模块：为目标战术点永久提高攻击力。
/// </summary>
public sealed class UpgradeModule : ClosureCard
{
    protected override List<DynamicVar> CanonicalVars => [new DynamicVar("Attack", 1m)];

    public UpgradeModule() : base(1, CardType.Skill, CardRarity.Uncommon, CustomTargetType.Pet)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Target != null)
        {
            await PowerCmd.Apply<TacticalPointAttackPower>(choiceContext, cardPlay.Target, base.DynamicVars["Attack"].BaseValue, Owner.Creature, this);
        }
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars["Attack"].UpgradeValueBy(1m);
    }
}
