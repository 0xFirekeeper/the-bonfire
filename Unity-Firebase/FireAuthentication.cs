// Filename: FireAuthentication.cs
// Author: 0xFirekeeper
// Description: Initializes Firebase Anonymous Authentication and Stores User ID

using UnityEngine;
using Firebase.Auth;

public class FireAuthentication : MonoBehaviour
{
    public string userID;

    private FirebaseAuth auth;
    private FirebaseUser user;

    public static FireAuthentication Instance;

    public bool HasInternet()
    {
        return !(Application.internetReachability == NetworkReachability.NotReachable);
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

    public void InitializeAuthentication()
    {
        if (!HasInternet())
            return;

        Debug.Log("INITIALIZING AUTHENTICATION");
        auth = FirebaseAuth.DefaultInstance;

        if (auth.CurrentUser != null)
        {
            userID = auth.CurrentUser.UserId;
        }
        else
        {
            auth.SignInAnonymouslyAsync().ContinueWith(logintask =>
            {
                if (logintask.IsCanceled || logintask.IsFaulted)
                {
                    Debug.Log("Error Authenticating");
                    return;
                }
                if (logintask.IsCompleted)
                {
                    user = auth.CurrentUser;
                    userID = user.UserId;
                }
            });

        }
    }
}
