using UnityEngine;

public static class Utills
{
    public static Vector2 GetLogicalViewPort(Vector2 screenPos)
    {
        return screenPos / (Screen.dpi / 160);
    }
}