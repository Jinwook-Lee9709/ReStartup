using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AppsUIFontAutoSize : MonoBehaviour
{
    private float autoSizeVal = 28.6f;
    void Update()
    {
        var newTextSize = ServiceLocator.Instance.GetSceneService<GameManager>().uiManager.canvas.GetComponent<RectTransform>().sizeDelta.x / autoSizeVal;
        GetComponent<TextMeshProUGUI>().fontSize = newTextSize;
    }
}
