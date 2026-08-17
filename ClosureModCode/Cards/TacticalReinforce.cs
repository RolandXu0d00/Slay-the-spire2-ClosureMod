using BaseLib.Patches.Features;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace ClosureMod.ClosureModCode.Cards;

/// <summary>
/// 战术加固：目标战术点最大生命+3并回复3点生命。
/// </summary>
public sealed class TacticalReinforce : ClosureCard
{
    protected override List<DynamicVar> CanonicalVars => [new DynamicVar("HP", 3m)];

    public TacticalReinforce() : base(1, CardType.Skill, CardRarity.Common, CustomTargetType.Pet)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Target != null)
        {
            await CreatureCmd.GainMaxHp(cardPlay.Target, base.DynamicVars["HP"].BaseValue);
            await CreatureCmd.Heal(cardPlay.Target, base.DynamicVars["HP"].BaseValue, playAnim: true);
        }
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars["HP"].UpgradeValueBy(2m);
    }
}
