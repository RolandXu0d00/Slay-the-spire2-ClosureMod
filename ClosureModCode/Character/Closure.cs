using BaseLib.Abstracts;
using BaseLib.Utils.NodeFactories;
using ClosureMod.ClosureModCode.Utils;
using ClosureMod.ClosureModCode.Extensions;
using Godot;
using MegaCrit.Sts2.Core.Entities.Characters;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.Nodes.Combat;

namespace ClosureMod.ClosureModCode.Character;

public class Closure : PlaceholderCharacterModel
{
    public const string CharacterId = "Closure";

    public static readonly Color Color = new("9fd8ff");

    public override Color NameColor => Color;
    public override CharacterGender Gender => CharacterGender.Feminine;
    public override int StartingHp => 70;

    public override IEnumerable<CardModel> StartingDeck =>
    [
        ModelDb.Card<Cards.TacticalDirective>(),
        ModelDb.Card<Cards.TacticalDirective>(),
        ModelDb.Card<Cards.TacticalDirective>(),
        ModelDb.Card<Cards.TacticalDirective>(),
        ModelDb.Card<Cards.DefendClosure>(),
        ModelDb.Card<Cards.DefendClosure>(),
        ModelDb.Card<Cards.DefendClosure>(),
        ModelDb.Card<Cards.DefendClosure>(),
        ModelDb.Card<Cards.DefendClosure>(),
        ModelDb.Card<Cards.DeployTacticalPoint>()
    ];

    public override IReadOnlyList<RelicModel> StartingRelics =>
    [
        ModelDb.Relic<Relics.TacticalCommandTerminal>()
    ];

    public override CardPoolModel CardPool => ModelDb.CardPool<ClosureCardPool>();
    public override RelicPoolModel RelicPool => ModelDb.RelicPool<ClosureRelicPool>();
    public override PotionPoolModel PotionPool => ModelDb.PotionPool<ClosurePotionPool>();

    /*  PlaceholderCharacterModel will utilize placeholder basegame assets for most of your character assets until you
        override all the other methods that define those assets.
        These are just some of the simplest assets, given some placeholders to differentiate your character with. */
    public override Control CustomIcon
    {
        get
        {
            var icon = NodeFactory<Control>.CreateFromResource(CustomIconTexturePath);
            icon.SetAnchorsAndOffsetsPreset(Control.LayoutPreset.FullRect);
            return icon;
        }
    }

    public override string CustomIconTexturePath => "character_icon_closure.png".CharacterUiPath();
    public override string CustomCharacterSelectIconPath => "char_select_char_name.png".CharacterUiPath();
    public override string CustomCharacterSelectLockedIconPath => "char_select_char_name_locked.png".CharacterUiPath();
    public override string CustomMapMarkerPath => "map_marker_closure.png".CharacterUiPath();

    public override string CustomCharacterSelectBg => "res://ClosureMod/scenes/screens/char_select/char_select_bg_closure.tscn";

    // 占位场景：仅供资源预加载使用；真正进入战斗时会用 CreateCustomVisuals 生成的小人 Spine 模型。
    public override string CustomVisualPath => SceneHelper.GetScenePath("creature_visuals/closure_placeholder");

    // 休息点动画：默认 PlaceholderCharacterModel 会用 PlaceholderID("ironclad") 拼路径，
    // 导致休息点显示铁甲战士，这里显式指向 Closure 专属休息点场景。
    public override string CustomRestSiteAnimPath => SceneHelper.GetScenePath("rest_site/characters/closuremod-closure_rest_site");

    /// <summary>
    /// 使用可露希尔小人动画帧替换铁甲战士占位模型。
    /// </summary>
    public override NCreatureVisuals? CreateCustomVisuals()
    {
        return ClosureSpriteVisualFactory.BuildCharacter();
    }
}
