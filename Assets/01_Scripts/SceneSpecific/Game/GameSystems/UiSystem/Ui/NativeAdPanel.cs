using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NativeAdPanel : MonoBehaviour
{
    private RectTransform rectTransform;
    private Canvas parentCanvas;

    public Button testButton;
    private float testYPos = 0;
    private void Start()
    {
        testButton.onClick.AddListener(onTestButtonClick);
    }

    private void onTestButtonClick()
    {
        var adSize = new Vector2(Screen.width, GetUIHeightInScreenPixels());
        var adPos = new Vector2(0, testYPos);

        testYPos += adSize.y;

        AdvertisementManager.Instance.RenderNativeAd(adSize, adPos);
        AdvertisementManager.Instance.ShowNativeAd();
    }

    private void OnEnable()
    {
        if (rectTransform == null)
        {
            rectTransform = GetComponent<RectTransform>();
            parentCanvas = GetComponentInParent<Canvas>();
        }



        //var screenSize = new Vector2(Screen.width, GetUIHeightInScreenPixels());
        //var screenPos = GetAdjustPixelPosition();


        ////Debug.Log($"Screen.Height : {Screen.height}");
        ////Debug.Log(GetScreenPointToRect());
        ////Debug.Log(GetAdjustPixelPosition());
        ////Debug.Log(GetTopLeftLocalPositionInCanvas());
        ////Debug.Log(GetTopLeftScreenPosition());
        ////Debug.Log(RectTransformUtility.WorldToScreenPoint(null, rectTransform.position));

        //AdvertisementManager.Instance.RenderNativeAd(screenSize, screenPos);
        //AdvertisementManager.Instance.ShowNativeAd();
    }

    private void OnDisable()
    {
        if (AdvertisementManager.Instance != null)
        {
            AdvertisementManager.Instance.HideNativeAd();
        }
    }

    public Vector2 GetScreenPointToRect()
    {
        Vector3 currentPos = rectTransform.position;
        if(!RectTransformUtility.ScreenPointToLocalPointInRectangle(parentCanvas.GetComponent<RectTransform>(), currentPos, Camera.main, out Vector2 resultVec2))
        {
            Debug.LogError($"올바른 좌표가 아님! : {resultVec2}");
            return Vector2.zero;
        }
        var result = (Screen.height * 0.5f) + resultVec2.y;
        return new Vector2(0, result) ;
    }

    public Vector2 GetAdjustPixelPosition()
    {
        Vector3 currentPos = rectTransform.position;
        var adjustPoint = RectTransformUtility.PixelAdjustPoint(currentPos, rectTransform, parentCanvas);
        var result = Screen.height - adjustPoint.y;
        return new Vector2(0,result);
    }

    public Vector3 GetTopLeftWorldPosition()
    {
        Vector3[] corners = new Vector3[4];
        rectTransform.GetWorldCorners(corners);

        return corners[1];
    }

    public Vector2 GetTopLeftLocalPositionInCanvas()
    {
        Vector3 worldTopLeft = GetTopLeftWorldPosition();
        RectTransform canvasRect = parentCanvas.GetComponent<RectTransform>();
        Vector2 localPos = canvasRect.InverseTransformPoint(worldTopLeft);
        var canvasHalfHeight = canvasRect.sizeDelta.y / 2;
        return new Vector2(localPos.x, canvasHalfHeight - localPos.y);
    }

    public Vector2 GetTopLeftScreenPosition()
    {
        Vector3 worldTopLeft = GetTopLeftWorldPosition();
        Vector2 worldPoint = RectTransformUtility.WorldToScreenPoint(null, worldTopLeft);
        var adjustYPos = Screen.height - worldTopLeft.y;
        return new Vector2(worldPoint.x, adjustYPos);
    }

    float GetUIHeightInScreenPixels()
    {
        return rectTransform.sizeDelta.y * parentCanvas.scaleFactor;
    }
}