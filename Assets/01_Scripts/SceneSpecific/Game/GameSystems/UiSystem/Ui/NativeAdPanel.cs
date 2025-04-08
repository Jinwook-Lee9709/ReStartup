using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NativeAdPanel : MonoBehaviour
{
    private void OnEnable()
    {
        int yPos = Screen.height / 16;
        AdvertisementManager.Instance.RenderNativeAd((int)yPos);
        AdvertisementManager.Instance.ShowNativeAd();
    }

    private void OnDisable()
    {
        if (AdvertisementManager.Instance != null)
            AdvertisementManager.Instance.HideNativeAd();
    }

}
