using Godot;

namespace ClosureMod.ClosureModCode.Extensions;

//Mostly utilities to get asset paths.
public static class StringExtensions
{
    /// <summary>
    /// 用正斜杠拼接 Godot 资源路径（不使用 Path.Join，避免 Windows 下生成反斜杠导致 ResourceLoader 找不到资源）。
    /// </summary>
    private static string JoinResPath(params string[] parts)
    {
        return string.Join("/", parts);
    }

    public static string ImagePath(this string path)
    {
        return JoinResPath(MainFile.ResPath, "images", path);
    }

    public static string CardImagePath(this string path)
    {
        path = JoinResPath(MainFile.ResPath, "images", "card_portraits", path);
        if (ResourceLoader.Exists(path)) return path;
        
        MainFile.Logger.Info("Could not find card image path: " + path);
        return JoinResPath(MainFile.ResPath, "images", "card_portraits", "card.png");
    }

    public static string BigCardImagePath(this string path)
    {
        path = JoinResPath(MainFile.ResPath, "images", "card_portraits", "big", path);
        if (ResourceLoader.Exists(path)) return path;
        
        MainFile.Logger.Info("Could not find big card image path: " + path);
        return JoinResPath(MainFile.ResPath, "images", "card_portraits", "big", "card.png");
    }

    public static string PowerImagePath(this string path)
    {
        path = JoinResPath(MainFile.ResPath, "images", "powers", path);
        if (ResourceLoader.Exists(path)) return path;
        
        MainFile.Logger.Info("Could not find power image path: " + path);
        return JoinResPath(MainFile.ResPath, "images", "powers", "power.png");
    }

    public static string BigPowerImagePath(this string path)
    {
        path = JoinResPath(MainFile.ResPath, "images", "powers", "big", path);
        if (ResourceLoader.Exists(path)) return path;
        
        MainFile.Logger.Info("Could not find big power image path: " + path);
        return JoinResPath(MainFile.ResPath, "images", "powers", "big", "power.png");
    }

    public static string RelicImagePath(this string path)
    {
        path = JoinResPath(MainFile.ResPath, "images", "relics", path);
        if (ResourceLoader.Exists(path)) return path;
        
        MainFile.Logger.Info("Could not find relic image path: " + path);
        return JoinResPath(MainFile.ResPath, "images", "relics", "relic.png");
    }

    public static string BigRelicImagePath(this string path)
    {
        path = JoinResPath(MainFile.ResPath, "images", "relics", "big", path);
        if (ResourceLoader.Exists(path)) return path;
        
        MainFile.Logger.Info("Could not find big relic image path: " + path);
        return JoinResPath(MainFile.ResPath, "images", "relics", "big", "relic.png");
    }

    public static string PotionImagePath(this string path)
    {
        path = JoinResPath(MainFile.ResPath, "images", "potions", path);
        if (ResourceLoader.Exists(path)) return path;

        MainFile.Logger.Info("Could not find potion image path: " + path);
        return JoinResPath(MainFile.ResPath, "images", "potions", "potion.png");
    }

    public static string PotionOutlineImagePath(this string path)
    {
        path = JoinResPath(MainFile.ResPath, "images", "potions", "outline", path);
        if (ResourceLoader.Exists(path)) return path;

        MainFile.Logger.Info("Could not find potion image path: " + path);
        return JoinResPath(MainFile.ResPath, "images", "potions", "outline", "potion.png");
    }

    public static string CharacterUiPath(this string path)
    {
        return JoinResPath(MainFile.ResPath, "images", "charui", path);
    }
}
