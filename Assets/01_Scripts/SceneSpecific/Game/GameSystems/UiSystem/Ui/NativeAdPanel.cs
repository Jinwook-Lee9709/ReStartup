using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NativeAdPanel : MonoBehaviour
{
    private RectTransform rectTransform;
    private Canvas parentCanvas;

    private void OnEnable()
    {
        if (rectTransform == null)
        {
            rectTransform = GetComponent<RectTransform>();
            parentCanvas = GetComponentInParent<Canvas>();
        }


        var adSize = new Vector2(Screen.width, GetUIHeightInScreenPixels());
        var adPos = Utills.GetLogicalViewPort(GetAdjustPixelPosition());

        AdvertisementManager.Instance.RenderNativeAd(adSize, adPos);
        AdvertisementManager.Instance.ShowNativeAd();
    }

    private void OnDisable()
    {
        if (AdvertisementManager.Instance != null)
        {
            AdvertisementManager.Instance.HideNativeAd();
        }
    }

    public Vector2 GetAdjustPixelPosition()
    {
        Vector3 currentPos = rectTransform.position;
        var adjustPoint = RectTransformUtility.PixelAdjustPoint(currentPos, rectTransform, parentCanvas);
        var result = Screen.height - adjustPoint.y;
        return new Vector2(0,result);
    }

    float GetUIHeightInScreenPixels()
    {
        return rectTransform.sizeDelta.y * parentCanvas.scaleFactor;
    }
}