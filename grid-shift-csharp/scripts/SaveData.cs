using Godot;
using Godot.Collections;

public static class SaveData
{
    private const string SavePath = "user://save_data.json";

    public static int LoadHighestUnlockedLevelIndex()
    {
        if (!FileAccess.FileExists(SavePath))
            return 0;

        using var file = FileAccess.Open(SavePath, FileAccess.ModeFlags.Read);
        if (file == null)
            return 0;

        var json = new Json();
        if (json.Parse(file.GetAsText()) != Error.Ok)
            return 0;

        if (json.Data.VariantType != Variant.Type.Dictionary)
            return 0;

        var dict = (Dictionary)json.Data;
        if (!dict.TryGetValue("highest_unlocked_level_index", out var value))
            return 0;

        return Mathf.Max(0, (int)value);
    }

    public static void SaveHighestUnlockedLevelIndex(int highestUnlockedLevelIndex)
    {
        var data = new Dictionary
        {
            { "highest_unlocked_level_index", Mathf.Max(0, highestUnlockedLevelIndex) }
        };

        using var file = FileAccess.Open(SavePath, FileAccess.ModeFlags.Write);
        if (file == null)
            return;

        file.StoreString(Json.Stringify(data));
    }
}
