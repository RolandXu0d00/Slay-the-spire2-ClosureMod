using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Modding;

namespace ClosureMod.ClosureModCode;

[ModInitializer(nameof(Initialize))]
public partial class MainFile : Node
{
    public const string ModId = "ClosureMod"; //Used for resource filepath
    public const string ResPath = $"res://{ModId}";

    public static MegaCrit.Sts2.Core.Logging.Logger Logger { get; } = new(ModId, MegaCrit.Sts2.Core.Logging.LogType.Generic);

    public static void Initialize()
    {
        //If you want to use scripts defined in your mod for Godot scenes, uncomment the following line.
        //Godot.Bridge.ScriptManagerBridge.LookupScriptsInAssembly(Assembly.GetExecutingAssembly());
        
        Harmony harmony = new(ModId);

        harmony.PatchAll();

        // 旧版游戏（v0.107.x）持久糖果 + 精英战 + 单稀有度能力卡池会导致结算崩溃；
        // 新版游戏已修复，此补丁只会在旧版存在对应方法时生效。
        Patches.LastingCandyCrashFixPatch.TryApply(harmony);
    }
}
