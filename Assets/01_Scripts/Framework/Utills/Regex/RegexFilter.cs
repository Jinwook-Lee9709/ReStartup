using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

public static class RegexFilter
{
    private static readonly string[] specialWordFilters =
    {
        @"[^a-zA-Z0-9가-힣]",
        @"[^\w\.@-]"
    };
    private static readonly string[] tempBadWords =
    {
        @"씨발",
        @"개새끼",
    };

    private static readonly List<string> adjustRegexBadWords = new();
    private static readonly string badWordFilterFormat = @"{0}\w*\d*[가-힣]*[ㄱ-ㅎ]*[^a-zA-Z0-9]*";

    public static void Init()
    {
        for (int i = 0; i < tempBadWords.Length; i++)
        {
            char[] strings = tempBadWords[i].ToCharArray();
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

        foreach(var word in adjustRegexBadWords)
        {
            if (StringMatch(check, word))
                return result;
        }

        result = true;
        return result;
    }

    private static string StringNormalize(string check ,string value)
    {
        check = Regex.Replace(check, value, "", RegexOptions.Singleline);
        return check;
    }

    private static bool StringMatch(string check, string pattern)
    {
        return Regex.IsMatch(check, pattern);
    }
}