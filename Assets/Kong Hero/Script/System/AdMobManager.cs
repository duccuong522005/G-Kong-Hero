using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Api;
using System;

public class AdMobManager : MonoBehaviour
{
    public static AdMobManager Instance;

    private RewardedAd rewardedAd;

    // This must be your Rewarded Ad Unit ID, not the App ID.
    // Use test id for initial debug: ca-app-pub-3940256099942544/5224354917
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
        // Initialize the Google Mobile Ads SDK
        MobileAds.Initialize(initStatus => { });

        LoadRewardedAd();
    }

    public void LoadRewardedAd()
    {
        // Clean up the old ad before loading a new one
        if (rewardedAd != null)
        {
            rewardedAd.Destroy();
            rewardedAd = null;
        }

        Debug.Log("Loading the rewarded ad.");

        // Create our request used to load the ad
        var adRequest = new AdRequest();

        // Send the request to load the ad
        RewardedAd.Load(adUnitId, adRequest,
            (RewardedAd ad, LoadAdError error) =>
            {
                // If error is not null, the load request failed
                if (error != null || ad == null)
                {
                    Debug.LogError("Rewarded ad failed to load an ad " +
                                   "with error : " + error);
                    return;
                }

                Debug.Log("Rewarded ad loaded with response : "
                          + ad.GetResponseInfo());

                rewardedAd = ad;

                // Register for ad events
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
                    GlobalValue.SavedCoins += 10; // Reward 10 coins, adjust as needed
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
        // Raised when the ad is estimated to have earned money
        ad.OnAdPaid += (AdValue adValue) =>
        {
            Debug.Log(String.Format("Rewarded ad paid {0} {1}.",
                adValue.Value,
                adValue.CurrencyCode));
        };
        // Raised when an impression is recorded for an ad
        ad.OnAdImpressionRecorded += () =>
        {
            Debug.Log("Rewarded ad recorded an impression.");
        };
        // Raised when a click is recorded for an ad
        ad.OnAdClicked += () =>
        {
            Debug.Log("Rewarded ad was clicked.");
        };
        // Raised when an ad opened full screen content
        ad.OnAdFullScreenContentOpened += () =>
        {
            Debug.Log("Rewarded ad full screen content opened.");
        };
        // Raised when the ad closed full screen content
        ad.OnAdFullScreenContentClosed += () =>
        {
            Debug.Log("Rewarded ad full screen content closed.");
        };
        // Raised when the ad failed to open full screen content
        ad.OnAdFullScreenContentFailed += (AdError error) =>
        {
            Debug.LogError("Rewarded ad failed to open full screen content " +
                           "with error : " + error);
        };
    }
}