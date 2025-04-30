using System;

public class ReviewData
{
    public int stars;
    public string reviewMessage;
    public string date;
    public string userID;
    public int stringID;
    public int addPoint;
    public void Init(bool isBest, DateTime dateTime)
    {
        var currentTheme = ServiceLocator.Instance.GetSceneService<GameManager>().CurrentTheme;
        stringID = int.Parse(isBest ? string.Format(Strings.positiveReviewFormat, (int)currentTheme, UnityEngine.Random.Range(1, 31)) : string.Format(Strings.negativeReviewFormat, (int)currentTheme, UnityEngine.Random.Range(1, 31)));
        stars = isBest ? 5 : 1;
        reviewMessage = LZString.GetUIString(stringID.ToString());
        date = dateTime.ToString("yyyy:MM:dd HH:mm");
        addPoint = isBest ? 10 : -10;

        userID = CreateRandomID();
    }

    private string CreateRandomID()
    {
        char[] frontString = new char[2];
        char backString;

        for(int i = 0; i < frontString.Length; i++)
        {
            frontString[i] = Strings.alphaNums[UnityEngine.Random.Range(0, Strings.alphaNums.Length)];
        }
        backString = Strings.alphaNums[UnityEngine.Random.Range(0, Strings.alphaNums.Length)];
        return string.Format(Strings.randomReviewIDFormat, new string(frontString), backString.ToString());
    }
    public void Init(bool isBest, DateTime dateTime, int stringID)
    {
        this.stringID = stringID;
        stars = isBest ? 5 : 1;
        reviewMessage = LZString.GetUIString(this.stringID.ToString());
        date = dateTime.ToString("yyyy:MM:dd HH:mm");
        addPoint = isBest ? 10 : -10;
    }


}
