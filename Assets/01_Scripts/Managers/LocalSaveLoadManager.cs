using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using SaveDataVC = SaveDataV1;

public class LocalSaveLoadManager : Singleton<LocalSaveLoadManager>
{
    public  int SaveDataVersion { get; private set; } = 1;

    public  SaveDataVC Data { get; set; }

    private  string LocalSaveFileName => "LocalSave.json";
    private  string LocalSaveDirectory
    {
        get
        {
            return $"{Application.persistentDataPath}/LocalSave";
        }
    }


    private JsonSerializerSettings settings = new JsonSerializerSettings
    {
        Formatting = Formatting.Indented,
        TypeNameHandling = TypeNameHandling.All,
    };


    private LocalSaveLoadManager()
    {
        if (!Load())
        {
            Data = new SaveDataVC();
            Save();
        }
    }

    public void GameSettingInit()
    {
        //TODO : 인게임 세팅 (사운드, 언어 등) 이니셜라이징
    }

    public bool Save()
    {
        if (Data == null)
            return false;

        if (!Directory.Exists(LocalSaveDirectory))
        {
            Directory.CreateDirectory(LocalSaveDirectory);
        }



        var path = Path.Combine(LocalSaveDirectory, LocalSaveFileName);
        var json = JsonConvert.SerializeObject(Data, settings);

        File.WriteAllText(path, json);

        return true;
    }

    public bool Load()
    {
        var path = Path.Combine(LocalSaveDirectory, LocalSaveFileName);
        if (!File.Exists(path))
        {
            return false;
        }

        var data = File.ReadAllText(path);
        var saveData = JsonConvert.DeserializeObject<SaveData>(data, settings);

        while (saveData.Version < SaveDataVersion)
        {
            saveData = saveData.VersionUp();
        }
        Data = saveData as SaveDataVC;
        return true;
    }
}
