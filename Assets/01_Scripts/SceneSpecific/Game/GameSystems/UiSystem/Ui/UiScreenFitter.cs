using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiScreenFitter : MonoBehaviour
{
    private void Start()
    {
        StretchToFullScreen(GetComponent<RectTransform>());
    }
    public void StretchToFullScreen(RectTransform rectTransform)
    {
        rectTransform.anchorMin = new Vector2(0, 0);
        rectTransform.anchorMax = new Vector2(1, 1);
        rectTransform.offsetMin = Vector2.zero;
        rectTransform.offsetMax = Vector2.zero;
    }
}
