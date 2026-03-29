using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Api;
using System;

public class AdMobManager : MonoBehaviour
{
    public static AdMobManager Instance;

    private RewardedAd rewardedAd;

    private string adUnitId = "ca-app-pub-3940256099942544/5224354917";

    public bool IsRewardAdReady => rewardedAd != null && rewardedAd.CanShowAd();

    void Awake()
    {
        if (AdMobManager.Instance != null)
            Destroy(gameObject);
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    void Start()
    {
        MobileAds.Initialize(initStatus => { });

        LoadRewardedAd();
    }

    public void LoadRewardedAd()
    {
        if (rewardedAd != null)
        {
            rewardedAd.Destroy();
            rewardedAd = null;
        }

        Debug.Log("Loading the rewarded ad.");

        var adRequest = new AdRequest();

        RewardedAd.Load(adUnitId, adRequest,
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

                rewardedAd = ad;

                RegisterEventHandlers(rewardedAd);
            });
    }

    public void ShowRewardedAd()
    {
        const string rewardMsg =
            "Rewarded ad rewarded the user. Type: {0}, amount: {1}.";

        Debug.Log($"ShowRewardedAd() called. rewardedAd is null: {rewardedAd == null}");
        
        if (rewardedAd != null)
        {
            bool canShow = rewardedAd.CanShowAd();
            Debug.Log($"rewardedAd.CanShowAd() = {canShow}");
            
            if (canShow)
            {
                Debug.Log("Showing rewarded ad now...");
                rewardedAd.Show((Reward reward) =>
                {
                    Debug.Log(String.Format(rewardMsg, reward.Type, reward.Amount));
                    GlobalValue.SavedCoins += 10; 
                    LoadRewardedAd();
                });
            }
            else
            {
                Debug.LogWarning("Rewarded ad loaded but CanShowAd() = false. Trying to reload...");
                LoadRewardedAd();
            }
        }
        else
        {
            Debug.LogWarning("Rewarded ad is null. Loading ad now...");
            LoadRewardedAd();
        }
    }

    private void RegisterEventHandlers(RewardedAd ad)
    {
        ad.OnAdPaid += (AdValue adValue) =>
        {
            Debug.Log(String.Format("Rewarded ad paid {0} {1}.",
                adValue.Value,
                adValue.CurrencyCode));
        };
        ad.OnAdImpressionRecorded += () =>
        {
            Debug.Log("Rewarded ad recorded an impression.");
        };
        ad.OnAdClicked += () =>
        {
            Debug.Log("Rewarded ad was clicked.");
        };
        ad.OnAdFullScreenContentOpened += () =>
        {
            Debug.Log("Rewarded ad full screen content opened.");
        };
        ad.OnAdFullScreenContentClosed += () =>
        {
            Debug.Log("Rewarded ad full screen content closed.");
        };
        ad.OnAdFullScreenContentFailed += (AdError error) =>
        {
            Debug.LogError("Rewarded ad failed to open full screen content " +
                           "with error : " + error);
        };
    }
}