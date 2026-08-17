using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using ClosureMod.ClosureModCode.Character;
using ClosureMod.ClosureModCode.Extensions;
using MegaCrit.Sts2.Core.Entities.Cards;

namespace ClosureMod.ClosureModCode.Cards;

/// <summary>
/// 可露希尔卡牌的基类：负责从模组资源加载卡面图，并标记卡池。
/// </summary>
[Pool(typeof(ClosureCardPool))]
public abstract class ClosureCard(int cost, CardType type, CardRarity rarity, TargetType target) :
    CustomCardModel(cost, type, rarity, target)
{
    public override string CustomPortraitPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigCardImagePath();

    public override string PortraitPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".CardImagePath();
    public override string BetaPortraitPath => $"beta/{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".CardImagePath();
}
