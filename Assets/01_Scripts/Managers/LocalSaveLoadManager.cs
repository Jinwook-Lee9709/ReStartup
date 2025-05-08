using System.IO;
using Newtonsoft.Json;
using UnityEngine;
using SaveDataVC = SaveDataV1;

public static class LocalSaveLoadManager
{
    private static readonly JsonSerializerSettings settings = new()
    {
        Formatting = Formatting.Indented,
        TypeNameHandling = TypeNameHandling.All
    };

    public static int SaveDataVersion { get; } = 1;

    public static SaveDataVC Data { get; set; }

    private static string LocalSaveFileName => "LocalSave.json";
    private static string LocalSaveDirectory => $"{Application.persistentDataPath}/LocalSave";

    public static void GameSettingInit()
    {
        if (!Load())
            Data = new SaveDataVC();
    }

    public static void UpdateSaveData()
    {
    }

    public static bool Save()
    {
        if (Data == null)
            return false;

        if (!Directory.Exists(LocalSaveDirectory)) Directory.CreateDirectory(LocalSaveDirectory);

        UpdateSaveData();

        var path = Path.Combine(LocalSaveDirectory, LocalSaveFileName);
        var json = JsonConvert.SerializeObject(Data, settings);

        File.WriteAllText(path, json);

        return true;
    }

    public static bool Load()
    {
        var path = Path.Combine(LocalSaveDirectory, LocalSaveFileName);
        if (!File.Exists(path)) return false;

        var data = File.ReadAllText(path);
        var saveData = JsonConvert.DeserializeObject<SaveData>(data, settings);

        while (saveData.Version < SaveDataVersion) saveData = saveData.VersionUp();
        Data = saveData as SaveDataVC;
        return true;
    }
}