// Filename: FireRemoteConfig.cs
// Author: 0xFirekeeper
// Description: Initializes Remote Config and Sets Local Variables

using Firebase.Extensions;
using System.Threading.Tasks;
using UnityEngine;

public class FireRemoteConfig : MonoBehaviour
{
    public static FireRemoteConfig Instance;

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

    // Initialize remote config, and set the default values.
    public void InitializeRemoteConfig()
    {
        // [START set_defaults]
        System.Collections.Generic.Dictionary<string, object> defaults =
          new System.Collections.Generic.Dictionary<string, object>();

        // Presto defaults
        //defaults.Add("interstitialShowFrequency", 75.0);

        Firebase.RemoteConfig.FirebaseRemoteConfig.SetDefaults(defaults);
        // [END set_defaults]
        Debug.Log("RemoteConfig configured and ready!");

        FetchDataAsync();
    }

    // Generic Fetch Task
    public Task FetchDataAsync()
    {
        Debug.Log("Fetching data...");
        System.Threading.Tasks.Task fetchTask = Firebase.RemoteConfig.FirebaseRemoteConfig.FetchAsync(); // Add TimeSpan.Zero argument for testing
        return fetchTask.ContinueWithOnMainThread(FetchComplete);
    }

    // Generic Fetch Callback
    void FetchComplete(Task fetchTask)
    {
        if (fetchTask.IsCanceled)
            Debug.Log("Fetch canceled.");
        else if (fetchTask.IsFaulted)
            Debug.Log("Fetch encountered an error.");
        else if (fetchTask.IsCompleted)
            Debug.Log("Fetch completed successfully!");

        var info = Firebase.RemoteConfig.FirebaseRemoteConfig.Info;
        switch (info.LastFetchStatus)
        {
            case Firebase.RemoteConfig.LastFetchStatus.Success:
                Firebase.RemoteConfig.FirebaseRemoteConfig.ActivateFetched();
                Debug.Log(string.Format("Remote data loaded and ready (last fetch time {0}).",
                                       info.FetchTime));
                UpdateValues(); // Updating local vars
                break;
            case Firebase.RemoteConfig.LastFetchStatus.Failure:
                switch (info.LastFetchFailureReason)
                {
                    case Firebase.RemoteConfig.FetchFailureReason.Error:
                        Debug.Log("Fetch failed for unknown reason");
                        break;
                    case Firebase.RemoteConfig.FetchFailureReason.Throttled:
                        Debug.Log("Fetch throttled until " + info.ThrottledEndTime);
                        break;
                }
                break;
            case Firebase.RemoteConfig.LastFetchStatus.Pending:
                Debug.Log("Latest Fetch call still pending.");
                break;
        }
    }

    // Callback if fetch is done
    void UpdateValues()
    {
        Debug.Log("Updated local values from remote config");

        // Interstitial Frequency
        //float _remoteInterstitialShowFrequency = (float)Firebase.RemoteConfig.FirebaseRemoteConfig.GetValue("interstitialShowFrequency").DoubleValue;
        //AdManager.Instance.interstitialShowFrequency = _remoteInterstitialShowFrequency;
    }

}
