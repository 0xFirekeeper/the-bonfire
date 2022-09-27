// Filename: FireApp.cs
// Author: 0xFirekeeper
// Description: Initializes Firebase Core

using UnityEngine;
using Firebase;

public class FireApp : MonoBehaviour
{
    //public UnityEvent OnFirebaseInitialized = new UnityEvent();

    public static FireApp Instance;

    public FirebaseApp app;

    [Header("SET MODULES YOU WANT")]
    public bool AUTHENTICATION = true;
    public bool REMOTE_CONFIG = true;
    public bool REALTIME_DATABASE = true;
    public bool ANALYTICS = true;

    [HideInInspector]
    public bool FIREBASE_INITIALIZED = false;
    private bool MODULES_INITIALIZED = false;

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

    void Start()
    {
        FIREBASE_INITIALIZED = false;

        Debug.Log("INITIALIZING FIREBASE APP");

        Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            var dependencyStatus = task.Result;
            if (dependencyStatus == Firebase.DependencyStatus.Available)
            {
                // Create and hold a reference to your FirebaseApp,
                // where app is a Firebase.FirebaseApp property of your application class.
                app = Firebase.FirebaseApp.DefaultInstance;

                // Set a flag here to indicate whether Firebase is ready to use by your app.
                FIREBASE_INITIALIZED = true;

            }
            else
            {
                UnityEngine.Debug.LogError(System.String.Format(
                  "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
                // Firebase Unity SDK is not safe to use here.
                FIREBASE_INITIALIZED = false;
            }
        });

    }

    private void Update()
    {
        if (FIREBASE_INITIALIZED && !MODULES_INITIALIZED)
        {
            MODULES_INITIALIZED = true;

            if (AUTHENTICATION)
                FireAuthentication.Instance.InitializeAuthentication();
            if (ANALYTICS)
                FireAnalytics.Instance.InitializeAnalytics();
            if (REMOTE_CONFIG)
                FireRemoteConfig.Instance.InitializeRemoteConfig();
            if (REALTIME_DATABASE)
                FireDatabase.Instance.InitializeDatabase();
        }
    }

}
