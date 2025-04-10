using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;
using UnityEngine.ResourceManagement.AsyncOperations;

public static class LZString
{
    public static readonly string ERROR_STRING = "ERROR";
    public static Dictionary<string, StringTable> CachedTables = new();

    private static StringTable GetCachedTable(StringTableIds tableId)
    {
        if (!CachedTables.TryGetValue(tableId.ToString(), out var table))
        {
            table = LocalizationSettings.StringDatabase.GetTable(tableId.ToString());
            if (table != null)
            {
                CachedTables.Add(tableId.ToString(), table);
            }
        }

        return table;
    }

    public static string GetLZStringSync(string key, StringTableIds tableId, params object[] args)
    {
        try
        {
            LocalizedString lzString = new LocalizedString() { TableReference = tableId.ToString(), TableEntryReference = key };
            var stringOperation = args != null ? lzString.GetLocalizedStringAsync(args) : lzString.GetLocalizedStringAsync();
            stringOperation.WaitForCompletion();
            if (stringOperation.Status == AsyncOperationStatus.Succeeded)
            {
                return stringOperation.Result;    
            }
            Debug.Log("GetLZStringSync failed: " + key + "");
            return ERROR_STRING;
        }
        catch (Exception e)
        {
            Debug.Log("GetLZStringSync failed: " + key + "");
            return ERROR_STRING;
        }
    }

    public static void GetLZStringAsync(string key, StringTableIds tableId, Action<string> callback, params object[] args)
    {
        try
        {
            LocalizedString lzString = new LocalizedString() { TableReference = tableId.ToString(), TableEntryReference = key };
            // var stringOperation = args
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
    
    
}
