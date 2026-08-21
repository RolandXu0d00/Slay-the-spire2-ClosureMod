using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace ClosureMod.ClosureModCode.Cards;

public sealed class EmergencyRouting : ClosureCard
{
    protected override List<DynamicVar> CanonicalVars => [new DynamicVar("Draw", 2m), new DynamicVar("Block", 8m)];
    public EmergencyRouting() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.None) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // 出牌流程会先支付费用再进入 OnPlay。“已透支”应以支付本卡费用前为准，
        // 否则 0 能量打出这张 1 费牌会被自身费用误触发。
        int energyBeforePlay = (Owner.PlayerCombatState?.Energy ?? 0) + cardPlay.Resources.EnergySpent;
        bool overdrawn = energyBeforePlay < 0;
        await CardPileCmd.Draw(choiceContext, DynamicVars["Draw"].BaseValue, Owner, true);
        if (overdrawn)
            await CreatureCmd.GainBlock(Owner.Creature, DynamicVars["Block"].BaseValue, ValueProp.Move, cardPlay, false);
    }

    protected override void OnUpgrade() => DynamicVars["Block"].UpgradeValueBy(4m);
}
