// Filename: FireDatabase.cs
// Author: 0xFirekeeper
// Description: Initializes Realtime Database with Local Parsing and other options

using System.Collections.Generic;
using UnityEngine;
using Firebase.Database;
using System;


[System.Serializable]
public class ListOfData
{
    public List<Highscore> highscoreList;

    public ListOfData()
    {
        highscoreList = new List<Highscore>();
    }

    public ListOfData(List<Highscore> _highScoreList)
    {
        highscoreList = _highScoreList;
    }
}

[System.Serializable]
public class Highscore
{
    public string name;
    public int score;

    public Highscore(string _name, int _score)
    {
        name = _name;
        score = _score;
    }
}

public class FireDatabase : MonoBehaviour
{
    [Header("MUST SET CONSOLE URL")]
    public string EDITOR_DATABASE_URL = "";
    [Header("LOCAL FILE TO UPLOAD TO DB - DO NOT SET TRUE FOR BUILDS")]
    public bool uploadSampleData = false;
    public TextAsset sampleTextData;
    [Header("REALTIME MAY INCREASE COSTS")]
    public bool FULLY_REALTIME = false;
    [Header("DONT GO OVER 1000")]
    public int QUERY_LIMIT = 100;

    public bool isDatabaseInitialized = false;
    public bool localListReady = false;

    private DatabaseReference dbRef; // Database Reference
    public ListOfData localList; // Local Reference

    public static FireDatabase Instance;

    private bool saveCache = false;

    public bool HasInternet() { return !(Application.internetReachability == NetworkReachability.NotReachable); }

    public int LastUpdatedDayOfYear
    {
        get
        {
            return PlayerPrefs.GetInt("LastUpdated", -1);
        }
        set
        {
            if (FireDatabase.Instance.isDatabaseInitialized &&
                    FireDatabase.Instance.localList != null &&
                    FireDatabase.Instance.localList.highscoreList.Count > 0)
            {
                PlayerPrefs.SetInt("LastUpdated", value);
                PlayerPrefs.Save();
            }
        }
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

    private void Update()
    {
        if (saveCache)
        {
            saveCache = false;
            SaveCachedList();
        }
    }

    public void SaveCachedList()
    {
        Debug.Log("Attempting Save Cache");

        LastUpdatedDayOfYear = DateTime.Today.DayOfYear;

        string json = JsonUtility.ToJson(localList);
        System.IO.File.WriteAllText(Application.persistentDataPath + "/ListOfData.json", json);

        Debug.Log("Saved ListOfData.json Cache");
    }

    public void GetCachedList()
    {
        Debug.Log("Getting cached List");

        try
        {
            string cachedListOfData = System.IO.File.ReadAllText(Application.persistentDataPath + "/ListOfData.json");
            localList = JsonUtility.FromJson<ListOfData>(cachedListOfData);
            localListReady = true;
        }
        catch (System.IO.FileNotFoundException e)
        {
            Debug.Log(e.Message);
            Debug.LogWarning("Could not find cached list of data, requesting new list from DB");
            GetSnapshot();
        }
    }

    public void InitializeDatabase()
    {
        Debug.Log("INITIALIZING FIREBASE LEADERBOARD");

        if (EDITOR_DATABASE_URL == "")
        {
            Debug.LogWarning("Aborting Leaderboard Initialization - Set Editor Database URL");
            return;
        }

        if (!HasInternet())
        {
            Debug.LogWarning("Aborting Leaderboard Initialization - No Network Reachability");
            return;
        }

        FireApp.Instance.app.Options.DatabaseUrl = new Uri(EDITOR_DATABASE_URL); // Set your database URL here

        dbRef = FirebaseDatabase.DefaultInstance.GetReference("highscores"); // Set the path you will be using mostly, reuse this reference

        isDatabaseInitialized = true;

        // Uncomment if needed 

        // if (uploadSampleData)
        //     UploadLocalData();
        // else
        //     FetchData();
    }

    #region Database Updating

    void UploadLocalData()
    {
        string json;

        string[] linesFromfile = sampleTextData.text.Split("\n"[0]);

        // This will need to match your db structure to work
        string localName = "";
        int localScore = 0;

        ListOfData listOfData = new ListOfData();

        for (int i = 0; i < linesFromfile.Length; i += 3)
        {
            linesFromfile[i] = linesFromfile[i].Replace("\r", "");

            localName = linesFromfile[i + 1];
            localScore = int.Parse(linesFromfile[i + 2]);

            listOfData.highscoreList.Add(new Highscore(localName, localScore));
        }

        json = JsonUtility.ToJson(listOfData);

        dbRef/*.Child(FireAuthentication.Instance.userID)*/.SetRawJsonValueAsync(json).ContinueWith(task =>
            {
                if (task.IsFaulted)
                    Debug.LogWarning(task.Exception);
                else
                    Debug.Log("Local Upload Successful");
            });

    }

    [ContextMenu("Fetch Data")]
    void FetchData()
    {
        Debug.Log("Fetching Data");

        if (FULLY_REALTIME) // Careful, ValueChanged is called whenever any client has a value updated, must not be called on big queries
        {
            dbRef.OrderByChild("score").LimitToLast(QUERY_LIMIT)
                .ValueChanged += HandleValueChanged;
        }
        else
        {
            localListReady = false;

            if (LastUpdatedDayOfYear != DateTime.Today.DayOfYear) // One snapshot a day
            {
                GetSnapshot();
            }
            else
            {
                GetCachedList();
            }
        }

    }

    void HandleValueChanged(object sender, ValueChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }

        Debug.Log("HandleValueChanged");

        UpdateLocalList(args.Snapshot);
    }

    [ContextMenu("Get Snapshot")]
    void GetSnapshot()
    {
        Debug.Log("Getting Snapshot");

        dbRef.OrderByChild("score").LimitToLast(QUERY_LIMIT)
          .GetValueAsync().ContinueWith(task =>
          {
              if (task.IsFaulted)
              {
                  // Handle the error...
                  Debug.LogWarning("Could not update local List: " + task.Exception.Message);
              }
              else if (task.IsCompleted)
              {
                  DataSnapshot snapshot = task.Result;
                  // Do something with snapshot...
                  UpdateLocalList(snapshot);
              }
          });
    }

    void UpdateLocalList(DataSnapshot snapshot)
    {
        Debug.Log("Updating Questions List");

        localList = new ListOfData();

        // You can use this to store a local cache if you want, but I recommend using localList and ToJson()
        // string finaljson = snapshot.GetRawJsonValue();

        string dbName = "";
        int dbScore = 0;

        // One way of reading a snapshot
        foreach (DataSnapshot entry in snapshot.Children)
        {
            try
            {
                if (entry.Child("name").Value == null || entry.Child("score").Value == null)
                    continue;

                dbName = entry.Child("name").Value.ToString();
                if (!int.TryParse(entry.Child("score").Value.ToString(), out dbScore))
                {
                    Debug.LogWarning("Could not parse score");
                    dbScore = 0;
                }

                localList.highscoreList.Add(new Highscore(dbName, dbScore));

                Debug.Log("Fetched Data: Name: " + dbName + " - Score: " + dbScore);
            }
            catch (Exception e)
            {
                Debug.LogWarning(e);
                continue;
            }
        }

        localList.highscoreList.Reverse();

        localListReady = true;
        //Set a saveCache bool to do SaveCachedList(finaljson); 
        saveCache = true;
        //(bool is needed if you update LastTimeFetched as the setter uses Player Prefs which doesn't work in async chains)
    }

    [ContextMenu("Save Score")]
    public void TestSave()
    {
        SaveScore();
    }

    public void SaveScore(bool displayLeaderboard = false)
    {
        if (!isDatabaseInitialized || !HasInternet())
            return;

        string updatedUid = FireAuthentication.Instance.userID;
        string updatedName = GameManager.Instance.PlayerName;
        int updatedScore = GameManager.Instance.Level;

        Debug.Log("Updating Score With Given Name: " + updatedName + " to: " + updatedScore);
        Debug.Log("User ID: " + updatedUid);

        // You don't need to use Json if you end up not using SetRawJsonValueAsync, and use SetValueAsync instead
        Highscore highscore = new Highscore(updatedName, updatedScore);
        string json = JsonUtility.ToJson(highscore);

        // Prevent crashes
        if (FireAuthentication.Instance.userID == null ||
            FireAuthentication.Instance.userID == "" ||
            dbRef == null || updatedName == null || updatedUid == "" || json == null || json == "")
            return;


        dbRef.Child(updatedUid).SetRawJsonValueAsync(json).ContinueWith(task =>
        {
            if (task.IsFaulted)
            {
                Debug.Log("Could not update score");
            }
            else if (task.IsCompleted)
            {
                Debug.Log("Score Updated!");

                if (displayLeaderboard)
                {
                    Debug.Log("Displaying Leaderboards");
                    // Handle the UI here, first sorting locally or fetching again to auto sort
                    // DisplayLeaderboards();
                }
            }
        });

    }

    #endregion

}
