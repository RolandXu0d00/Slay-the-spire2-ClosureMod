using BaseLib.Patches.Features;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace ClosureMod.ClosureModCode.Cards;

/// <summary>
/// 紧急维修：为目标战术点回复生命。
/// </summary>
public sealed class EmergencyRepair : ClosureCard
{
    protected override List<DynamicVar> CanonicalVars => [new DynamicVar("Heal", 8m)];

    public EmergencyRepair() : base(1, CardType.Skill, CardRarity.Common, CustomTargetType.Pet)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Target != null)
        {
            await CreatureCmd.Heal(cardPlay.Target, base.DynamicVars["Heal"].BaseValue, playAnim: true);
        }
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars["Heal"].UpgradeValueBy(3m);
    }
}
