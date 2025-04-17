using System;

public class ReviewData
{
    public int stars;
    public string reviewMessage;
    public string date;
    public int addPoint;
    public void Init(bool isBest, DateTime dateTime)
    {
        stars = isBest ? 5 : 1;
        reviewMessage = isBest ? "임시 긍정 리뷰" : "임시 부정 리뷰";
        date = dateTime.ToString("yyyy:MM:dd HH:mm");
        addPoint = isBest ? 10 : -10;
    }
}
