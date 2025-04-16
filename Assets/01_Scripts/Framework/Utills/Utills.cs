using UnityEngine;

public static class Utills
{
    public static Vector2 GetLogicalViewPort(Vector2 screenPos)
    {
        return screenPos / (Screen.dpi / 160);
    }
    private static float GetWhiteness(Color color)
    {
        return Vector3.Distance(new Vector3(color.r, color.g, color.b), new Vector3(1, 1, 1));
    }
    public static Color LerpTowardWhiter(Color a, Color b, float t)
    {
        float whitenessA = GetWhiteness(a);
        float whitenessB = GetWhiteness(b);

        // b가 더 흰색에 가깝다면 → a → b로 빠르게
        if (whitenessB < whitenessA)
        {
            // 빠르게 b 쪽으로 이동
            t = Mathf.Pow(t, 0.5f); // Easing: b 방향 강조
            return Color.Lerp(a, b, t);
        }
        else
        {
            // 빠르게 a 쪽으로 이동
            t = Mathf.Pow(t, 2f); // 반대로 약하게 b로 이동
            return Color.Lerp(a, b, t);
        }
    }
}