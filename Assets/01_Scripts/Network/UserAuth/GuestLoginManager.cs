using System.IO;
using Newtonsoft.Json;
using UnityEngine;
using Directory = System.IO.Directory;

public static class GuestLoginManager
{
    private static readonly string TokenFileName = "guestID.txt";
    private static readonly string TokenSaveDirectory = $"{Application.persistentDataPath}/Auth";
    public static string UUID;

    public static bool ReadUUID()
    {
        if (!Directory.Exists(TokenSaveDirectory)
            || !File.Exists(Path.Combine(TokenSaveDirectory, TokenFileName)))
            return false;
        UUID = File.ReadAllText(Path.Combine(TokenSaveDirectory, TokenFileName));
        return true;
    }
    
    public static void SaveUUID(string uuid)
    {
        UUID = uuid;
        if (!Directory.Exists(TokenSaveDirectory))
            Directory.CreateDirectory(TokenSaveDirectory);
        File.WriteAllText(Path.Combine(TokenSaveDirectory, TokenFileName), UUID);
        
    }
    
    public static void DeleteUUID()
    {
        if (!Directory.Exists(TokenSaveDirectory))
            return;
        File.Delete(Path.Combine(TokenSaveDirectory, TokenFileName));
        UUID = null;
    }
}