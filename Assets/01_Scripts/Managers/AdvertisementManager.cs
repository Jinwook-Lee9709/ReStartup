using GoogleMobileAds.Api;
using System;
using UnityEngine;

public class AdvertisementManager : Singleton<AdvertisementManager>
{
    public RewardedAd rewarded;
    public NativeOverlayAd nativeAd;
    public BannerView bannerView;
    private bool isInitialized = false;

#if UNITY_ANDROID
    private string rewardTestADID = "ca-app-pub-3940256099942544/5224354917";
    private string nativeTestADID = "ca-app-pub-3940256099942544/2247696110";
    private string bannerTestADID = "ca-app-pub-3940256099942544/6300978111";
#elif UNITY_IPHONE
    private string rewardTestADID = "ca-app-pub-3940256099942544/1712485313";
    private string nativeTestADID = "ca-app-pub-3940256099942544/3986624511";
    private string bannerTestADID = "ca-app-pub-3940256099942544/2934735716";
#else
    private string rewardTestADID = "unused";
    private string nativeTestADID = "unused";
    private string bannerTestADID = "unused";
#endif

    public void Init()
    {
        if (isInitialized)
        {
            return;
        }
        MobileAds.Initialize((InitializationStatus initStatus) =>
        {
            isInitialized = true;
            LoadRewardedAd();
            LoadNativeAd();
        });

    }

    public void LoadNativeAd()
    {
        if (nativeAd != null)
        {
            nativeAd.Destroy();
            nativeAd = null;
        }
        Debug.Log("Loading native overlay ad.");

        var adRequest = new AdRequest();
        var options = new NativeAdOptions();

        NativeOverlayAd.Load(nativeTestADID, adRequest, options,
            (ad, error) =>
            {
                if (error != null)
                {
                    Debug.LogError("Native Overlay ad failed to load an ad " +
                                   " with error: " + error);
                    return;
                }

                // The ad should always be non-null if the error is null, but
                // double-check to avoid a crash.
                if (ad == null)
                {
                    Debug.LogError("Unexpected error: Native Overlay ad load event " +
                                   " fired with null ad and null error.");
                    return;
                }

                // The operation completed successfully.
                Debug.Log("Native Overlay ad loaded with response : " +
                           ad.GetResponseInfo());

                nativeAd = ad;
                Debug.Log(nativeAd == null);

                // Register to ad events to extend functionality.
                RegisterEventHandlers(ad);
            });
    }

    public void RenderNativeAd(int yPos)
    {
        Debug.Log("Render!!!!");
        Debug.Log($"Func : {System.Reflection.MethodBase.GetCurrentMethod().Name}\n nativeAd == null : {nativeAd == null}");
        Debug.Log(yPos);
        if (nativeAd != null)
        {
            Debug.Log("Rendering Native Overlay ad.");

            // Define a native template style with a custom style.
            var style = new NativeTemplateStyle
            {
                TemplateId = NativeTemplateId.Small,
                MainBackgroundColor = Color.white,
                CallToActionText = new NativeTemplateTextStyle
                {
                    BackgroundColor = Color.grey,
                    TextColor = Color.black,
                    FontSize = 9,
                    Style = NativeTemplateFontStyle.Bold
                }
            };

            // Renders a native overlay ad at the default size
            // and anchored to the bottom of the screne.
            nativeAd.RenderTemplate(style, 0, yPos);
        }
    }

    public void ShowNativeAd()
    {
        Debug.Log("Show!!!!");
        Debug.Log($"Func : {System.Reflection.MethodBase.GetCurrentMethod().Name}\n nativeAd == null : {nativeAd == null}");
        if (nativeAd != null)
        {
            nativeAd.Show();
        }
    }

    public void HideNativeAd()
    {
        if (nativeAd != null)
        {
            nativeAd.Hide();
        }
    }

    public void DestroyNativeAd()
    {
        if (nativeAd != null)
        {
            nativeAd.Destroy();
            nativeAd = null;
        }
    }

    //public void CreateBannerView()
    //{
    //    Debug.Log("Creating banner view");

    //    // If we already have a banner, destroy the old one.
    //    if (bannerView != null)
    //    {
    //        bannerView.Destroy();
    //        bannerView = null;
    //    }

    //    // Create a 320x50 banner at top of the screen
    //    //AdSize.GetCurrentOrientationAnchoredAdaptiveBannerAdSizeWithWidth(AdSize.FullWidth)
    //    bannerView = new BannerView(bannerTestADID, AdSize.GetCurrentOrientationAnchoredAdaptiveBannerAdSizeWithWidth(AdSize.FullWidth), AdPosition.BottomLeft);

    //    var adRequest = new AdRequest();
    //    bannerView.LoadAd(adRequest);
    //}


    public void LoadRewardedAd()
    {
        if (rewarded != null)
        {
            rewarded.Destroy();
            rewarded = null;
        }

        var adRequest = new AdRequest();

        RewardedAd.Load(rewardTestADID, adRequest,
            (RewardedAd ad, LoadAdError error) =>
            {
                if (error != null || ad == null)
                {
                    Debug.LogError("Rewarded ad failed to load an ad " +
                                   "with error : " + error);
                    return;
                }

                Debug.Log("Rewarded ad loaded with response : "
                          + ad.GetResponseInfo());
                RegisterEventHandlers(ad);
                rewarded = ad;
            });
    }

    public void ShowRewardedAd(Action adCallBack)
    {
        if (rewarded != null && rewarded.CanShowAd())
        {
            rewarded.Show((Reward reward) =>
            {
                adCallBack?.Invoke();
                LoadRewardedAd();
            });
        }
        else
        {
            Debug.LogWarning("광고가 준비되지 않음.");
        }
    }


    private void RegisterEventHandlers(RewardedAd ad)
    {
        ad.OnAdFullScreenContentClosed += () =>
        {
            LoadRewardedAd();
        };
        ad.OnAdFullScreenContentFailed += (err) =>
        {
            LoadRewardedAd();
        };
    }
    private void RegisterEventHandlers(NativeOverlayAd ad)
    {

    }
}
