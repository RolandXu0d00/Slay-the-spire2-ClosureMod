using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Runs;

namespace ClosureMod.ClosureModCode.Patches;

/// <summary>
/// 修复旧版游戏（v0.107.x）中“持久糖果”遗物在精英战触发时的结算崩溃：
/// 持久糖果会把能力牌过滤成一个只剩单一稀有度的卡池，再拿精英战稀有度概率去创建卡牌奖励选项，
/// 而旧版 CardCreationOptions 规定“单稀有度卡池必须用 Uniform 概率”，否则直接抛异常导致无法结算。
/// 新版游戏（v0.111+）已移除该限制，本补丁按“方法是否存在”动态应用，新旧版本都安全。
/// </summary>
public static class LastingCandyCrashFixPatch
{
    public static void TryApply(Harmony harmony)
    {
        var cardCreationOptionsType = AccessTools.TypeByName("MegaCrit.Sts2.Core.Runs.CardCreationOptions");
        if (cardCreationOptionsType == null)
        {
            return;
        }

        // 旧版构造函数：CardCreationOptions(IEnumerable<CardModel> customCardPool, CardCreationSource source, CardRarityOddsType rarityOdds)
        // 不直接引用 CardCreationSource 类型，避免旧版命名空间差异导致加载失败，改为按参数结构匹配。
        var oldCtor = FindSingleRarityPoolConstructor(cardCreationOptionsType);
        if (oldCtor != null)
        {
            var prefix = AccessTools.Method(typeof(LastingCandyCrashFixPatch), nameof(ForceUniformOddsForSingleRarityPoolPrefix));
            harmony.Patch(oldCtor, prefix: new HarmonyMethod(prefix));
            MainFile.Logger.Info("已应用旧版卡牌奖励单一稀有度崩溃修复（单稀有度卡池自动改用 Uniform 概率）");
        }

        // 兜底：直接跳过旧版断言方法，防止其他代码路径触发同类崩溃。
        var assertMethod = AccessTools.Method(cardCreationOptionsType, "AssertUniformOddsIfSingleRarityPool");
        if (assertMethod != null)
        {
            var skipPrefix = AccessTools.Method(typeof(LastingCandyCrashFixPatch), nameof(SkipSingleRarityAssertPrefix));
            harmony.Patch(assertMethod, prefix: new HarmonyMethod(skipPrefix));
            MainFile.Logger.Info("已应用旧版卡牌奖励单一稀有度崩溃修复（断言兜底）");
        }
    }

    private static System.Reflection.ConstructorInfo? FindSingleRarityPoolConstructor(System.Type type)
    {
        foreach (var ctor in type.GetConstructors(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance))
        {
            var parameters = ctor.GetParameters();
            if (parameters.Length == 3
                && parameters[0].ParameterType == typeof(IEnumerable<CardModel>)
                && parameters[2].ParameterType == typeof(CardRarityOddsType))
            {
                return ctor;
            }
        }
        return null;
    }

    /// <summary>
    /// 当传入的卡池只有一种稀有度时，把稀有度概率改成 Uniform，满足旧版“单稀有度必须用 Uniform”的约束。
    /// </summary>
    private static void ForceUniformOddsForSingleRarityPoolPrefix(IEnumerable<CardModel> customCardPool, ref CardRarityOddsType rarityOdds)
    {
        if (customCardPool == null || rarityOdds == CardRarityOddsType.Uniform)
        {
            return;
        }

        CardRarity? firstRarity = null;
        foreach (var card in customCardPool)
        {
            if (card == null)
            {
                continue;
            }
            if (firstRarity == null)
            {
                firstRarity = card.Rarity;
            }
            else if (card.Rarity != firstRarity.Value)
            {
                return; // 卡池包含多种稀有度，原逻辑可以正常处理。
            }
        }

        if (firstRarity != null)
        {
            rarityOdds = CardRarityOddsType.Uniform;
            MainFile.Logger.Info($"持久糖果奖励卡池只剩单一稀有度（{firstRarity}），已改用 Uniform 稀有度概率，避免结算崩溃");
        }
    }

    /// <summary>
    /// 跳过旧版断言。新版游戏已移除该断言，且生成逻辑本身支持单稀有度卡池。
    /// </summary>
    private static bool SkipSingleRarityAssertPrefix()
    {
        return false;
    }
}
