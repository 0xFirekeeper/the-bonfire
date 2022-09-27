// Filename: FireAnalytics.cs
// Author: 0xFirekeeper
// Description: Initializes Firebase Analytics and Links to EventManager

using UnityEngine;
using Firebase.Analytics;


public class FireAnalytics : MonoBehaviour
{
    public static FireAnalytics Instance;

    private bool ANALYTICS_AVAILABLE()
    {
        return FireApp.Instance != null && FireApp.Instance.FIREBASE_INITIALIZED;
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void InitializeAnalytics()
    {
        if (!Extensions.HasInternet())
            return;

        Debug.Log("INITIALIZING ANALYTICS");

        FirebaseAnalytics.LogEvent(Firebase.Analytics.FirebaseAnalytics.EventLogin);
    }

    #region General Events Logs

    public void SceneLoaded(SceneName name)
    {
        if (ANALYTICS_AVAILABLE())
            FirebaseAnalytics.LogEvent("Scene Loaded", "Scene Name", name.ToString());
    }

    public void SceneUnloaded(SceneName name)
    {
        if (ANALYTICS_AVAILABLE())
            FirebaseAnalytics.LogEvent("Scene Unloaded", "Scene Name", name.ToString());
    }

    #endregion

    #region Ad Events Logs

    public void OnInterstitialWatched()
    {
        if (ANALYTICS_AVAILABLE())
            FirebaseAnalytics.LogEvent("InterstitialWatched");
    }

    public void OnRewardedWatched(string rewardId)
    {
        if (ANALYTICS_AVAILABLE())
            FirebaseAnalytics.LogEvent("RewardedWatched", "RewardId", rewardId);
    }

    #endregion



    #region User Event Logs
    public void OnItemSold(string itemName)
    {
        if (ANALYTICS_AVAILABLE())
            FirebaseAnalytics.LogEvent("SellItem", "Name", itemName);
    }

    public void RoundStart()
    {
        if (ANALYTICS_AVAILABLE())
            FirebaseAnalytics.LogEvent("RoundStarted");
    }

    public void RoundWon()
    {
        if (ANALYTICS_AVAILABLE())
            FirebaseAnalytics.LogEvent("RoundWon", "LevelIndex", GameManager.Instance.Level);
    }

    public void RoundLost()
    {
        if (ANALYTICS_AVAILABLE())
            FirebaseAnalytics.LogEvent("RoundLost", "LevelIndex", GameManager.Instance.Level);
    }

    public void GameWon()
    {
        if (ANALYTICS_AVAILABLE())
            FirebaseAnalytics.LogEvent("GameWon");
    }

    public void GameLost()
    {
        if (ANALYTICS_AVAILABLE())
            FirebaseAnalytics.LogEvent("GameLost");
    }

    #endregion

}
