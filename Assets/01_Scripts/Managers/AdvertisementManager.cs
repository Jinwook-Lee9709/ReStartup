using GoogleMobileAds.Api;
using System;
using UnityEngine;

public class AdvertisementManager : Singleton<AdvertisementManager>
{
    public RewardedAd rewarded;
    public NativeOverlayAd nativeAd;
    private bool isInitialized = false;

#if UNITY_ANDROID
    private string rewardTestADID = "ca-app-pub-3940256099942544/5224354917";
#elif UNITY_IPHONE
    private string rewardTestADID = "ca-app-pub-3940256099942544/1712485313";
#else
    private string rewardTestADID = "unused";
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
        });

    }


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
}