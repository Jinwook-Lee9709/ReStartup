using UnityEngine;

public class ScrollRectWidthLimiter : MonoBehaviour
{
    public RectTransform scrollRectTransform; // Scroll Rect의 RectTransform
    public float maxWidth = 800f; // Scroll Rect의 최대 너비 (픽셀 단위)
    public float adjustAmount = 388f;
    
    void Update()
    {
        // 현재 화면의 너비 가져오기
        float screenWidth = Screen.width;

        // Scroll Rect의 현재 크기 가져오기
        Vector2 size = scrollRectTransform.sizeDelta;

        // 화면 크기가 최대 너비보다 작으면 크기를 줄이기
        if (screenWidth - adjustAmount < maxWidth)
        {
            size.x = screenWidth - adjustAmount; // 화면 크기로 설정
        }
        else
        {
            size.x = maxWidth; // 최대 크기로 설정
        }

        // 크기 적용
        scrollRectTransform.sizeDelta = size;
    }
}