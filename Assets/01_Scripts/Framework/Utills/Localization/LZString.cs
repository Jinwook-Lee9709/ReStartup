using System;
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
            if (table != null) CachedTables.Add(tableId.ToString(), table);
        }

        return table;
    }

    public static string GetStringSync(string key, StringTableIds tableId, params string[] args)
    {
        try
        {
            var lzString = new LocalizedString { TableReference = tableId.ToString(), TableEntryReference = key };
            var stringOperation =
                args != null ? lzString.GetLocalizedStringAsync(args) : lzString.GetLocalizedStringAsync();
            stringOperation.WaitForCompletion();    
            if (stringOperation.Status == AsyncOperationStatus.Succeeded) return stringOperation.Result;
            Debug.Log("GetLZStringSync failed: " + key + "");
            return ERROR_STRING;
        }
        catch (Exception e)
        {
            Debug.Log("GetLZStringSync failed: " + key + "");
            return ERROR_STRING;
        }
    }

    public static void GetStringAsync(string key, StringTableIds tableId, Action<string> callback, params string[] args)
    {
        try
        {
            var lzString = new LocalizedString { TableReference = tableId.ToString(), TableEntryReference = key };
            var stringOperation =
                args != null ? lzString.GetLocalizedStringAsync(args) : lzString.GetLocalizedStringAsync();
            stringOperation.Completed += handle =>
            {
                if (stringOperation.Status == AsyncOperationStatus.Succeeded)
                {
                    callback?.Invoke(stringOperation.Result);
                }
                else
                {
                    Debug.Log("GetLZStringAsync failed: " + key + "");
                    callback?.Invoke(ERROR_STRING);
                }
            };
        }
        catch (Exception e)
        {
            Debug.Log("GetLZStringSync failed: " + key + "");
        }
    }

    public static string GetString(string key, StringTableIds tableId, Action<string> callback = null,
        params string[] args)
    {
        try
        {
            var stringTable = GetCachedTable(tableId);

            if (stringTable == null)
            {
                Debug.Log("GetLZString failed, Table not founded: " + key + "");
                return ERROR_STRING;
            }

            var entry = stringTable.GetEntry(key);
            if (entry == null)
            {
                Debug.Log("GetLZString failed, Key not founded: " + key + "");
                return ERROR_STRING;
            }

            if (entry.IsSmart && callback == null) return GetStringSync(key, tableId, args);

            if (entry.IsSmart && callback != null)
            {
                GetStringAsync(key, tableId, callback, args);
                return key;
            }

            if (callback == null) return GetStringSync(key, tableId);

            GetStringAsync(key, tableId, callback);
            return key;
        }
        catch (Exception e)
        {
            Debug.Log("GetLZString failed: " + key + "");
            return ERROR_STRING;
        }
    }

    public static string GetUIString(string key, Action<string> callback = null, params string[] args)
    {
        return GetString(key, StringTableIds.UIString, callback, args);
    }
}