using CsvHelper;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public static class RegexFilter
{
    private static readonly string[] specialWordFilters =
    {
        @"[^a-zA-Z0-9가-힣]",
        @"[^\w\.@-]"
    };
    private static List<string> badWords = new();

    private static readonly List<string> adjustRegexBadWords = new();
    private static readonly string badWordFilterFormat = @"{0}\w*\d*[가-힣]*[ㄱ-ㅎ]*[^a-zA-Z0-9]*";

    private static void InitStringTable()
    {
        badWords = LoadFirstColumn("badwords");
    }
    public static List<string> LoadFirstColumn(string filePath)
    {
        var handle = Addressables.LoadAssetAsync<TextAsset>(filePath);
        handle.WaitForCompletion();
        var result = handle.Result.text;
        List<string> firstColumn = result.Split("\r\n").ToList();

        return firstColumn;
    }
    public static void Init()
    {
        InitStringTable();
        for (int i = 0; i < badWords.Count; i++)
        {
            char[] strings = badWords[i].ToCharArray();
            string adjustPattern = @"\w*\d*[가-힣]*[ㄱ-ㅎ]*[^a-zA-Z0-9]*";
            for (int j = 0; j < strings.Length; j++)
            {
                var currentStr = string.Format(badWordFilterFormat, strings[j]);
                adjustPattern = string.Concat(adjustPattern, currentStr);
                //adjustPattern.Concat(currentStr);
            }
            adjustRegexBadWords.Add(adjustPattern);
        }
    }
    public static bool SpecialStringFilter(string value)
    {
        string check = value;
        for (int i = 0; i < specialWordFilters.Length; i++)
        {
            check = StringNormalize(check, specialWordFilters[i]);
        }
        return check.Equals(value);
    }

    public static bool BadWordFilter(string value)
    {
        bool result = false;
        string check = value;
        for (int i = 0; i < specialWordFilters.Length; i++)
        {
            check = StringNormalize(check, specialWordFilters[i]);
        }

        foreach (var word in adjustRegexBadWords)
        {
            if (StringMatch(check, word))
                return result;
        }

        result = true;
        return result;
    }

    private static string StringNormalize(string check, string value)
    {
        check = Regex.Replace(check, value, "", RegexOptions.Singleline);
        return check;
    }

    private static bool StringMatch(string check, string pattern)
    {
        return Regex.IsMatch(check, pattern);
    }
}