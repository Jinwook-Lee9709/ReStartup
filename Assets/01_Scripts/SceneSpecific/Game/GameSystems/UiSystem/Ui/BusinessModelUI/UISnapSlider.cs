using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;

public class ScrollSnapUIWithIndicator : MonoBehaviour, IBeginDragHandler, IEndDragHandler
{
    public ScrollRect scrollRect;
    public int totalPages = 3;
    public float moveSpeed = 5f;
    public float swipeThreshold = 100f;

    public Image[] pageIndicators;           // ← 인디케이터 이미지들
    public Color activeColor = Color.white;  // 현재 페이지 색
    public Color inactiveColor = Color.gray; // 나머지 색

    private int currentPage = 0;
    private float targetPosition = 0f;
    private bool isLerping = false;
    private Vector2 dragStartPos;
    void Start()
    {
        SnapToPage(0);
    }
    void Update()
    {
        if (isLerping)
        {
            scrollRect.horizontalNormalizedPosition = Mathf.Lerp(
                scrollRect.horizontalNormalizedPosition,
                targetPosition,
                Time.deltaTime * moveSpeed
            );

            if (Mathf.Abs(scrollRect.horizontalNormalizedPosition - targetPosition) < 0.001f)
            {
                scrollRect.horizontalNormalizedPosition = targetPosition;
                isLerping = false;
            }
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        dragStartPos = eventData.pressPosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        float dragDelta = eventData.position.x - dragStartPos.x;

        if (Mathf.Abs(dragDelta) > swipeThreshold)
        {
            if (dragDelta < 0 && currentPage < totalPages - 1)
                currentPage++;
            else if (dragDelta > 0 && currentPage > 0)
                currentPage--;
        }

        SnapToPage(currentPage);
    }

    void SnapToPage(int pageIndex)
    {
        currentPage = pageIndex;
        targetPosition = (float)pageIndex / (totalPages - 1);
        isLerping = true;
        UpdateIndicators();
    }

    void UpdateIndicators()
    {
        for (int i = 0; i < pageIndicators.Length; i++)
        {
            pageIndicators[i].color = (i == currentPage) ? activeColor : inactiveColor;
        }
    }
}