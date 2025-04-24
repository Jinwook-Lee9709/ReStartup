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

        for (int i = 0; i < tempBadWords.Length; i++)
        {
            if (check.Contains(tempBadWords[i]))
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
}