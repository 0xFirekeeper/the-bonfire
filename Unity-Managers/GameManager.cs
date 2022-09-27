// Filename: GameManager.cs
// Author: 0xFirekeeper
// Description: Manages Scene Loading Game States, Binary Save System and Generic Player Prefs
//              Handles Levels, Sounds, Vibration

using System.IO;
using UnityEngine;
using RotaryHeart.Lib.SerializableDictionary;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Linq;
//using DG.Tweening;

public enum GameState
{
    None,
    MainMenu,
    Paused,
    Playing,
};

public enum SceneName
{
    _preload,
    MainMenuScene,
    GameScene
}

[System.Serializable]
public class Scenes : SerializableDictionaryBase<SceneName, int> { }

public class GameManager : MonoBehaviour, IGameManager
{
    #region Generic Variables

    public Scenes Scenes;

    [SerializeField]
    private SceneName currentScene;
    [SerializeField]
    private int currentSceneId;
    [SerializeField]
    private GameState currentGameState;
    private bool loadingScene;

    #endregion

    #region Generic Properties

    private int _score;
    [SerializeField]
    private int _level;
    [SerializeField]
    private string _version;
    [SerializeField]
    private int _coins;
    private bool _soundOn;
    private bool _vibrationOn;
    private int _accumulatedCoin;

    public int Score
    {
        get => this._score;
        set { if (value >= 0) _score = value; }
    }
    public int Coins
    {
        get => this._coins;
        set
        {
            if (value >= 0)
            {
                _coins = value;

                if (UIManager.Instance != null)
                {
                    // Animate Coin Text
                    // UIManager.Instance.CoinText.transform.DOShakeScale(0.5f, Vector3.one, 10, 90, true);
                }
            }
        }
    }
    public int Level
    {
        get => this._level;
        set { if (value >= 0) _level = value; }
    }

    public string Version
    {
        get => this._version;
        set { if (value != "") _version = value; }
    }
    public int AccumulatedCoin
    {
        get => this._accumulatedCoin;
        set { if (value >= 0) _accumulatedCoin = value; }
    }

    public bool SoundOn
    {
        get
        {
            if (PlayerPrefs.HasKey("SoundOn"))
                return PlayerPrefs.GetInt("SoundOn") != 0;
            else
            {
                SoundOn = true;
                return true;
            }
        }
        set
        {
            PlayerPrefs.SetInt("SoundOn", (value ? 1 : 0));
            PlayerPrefs.Save();
            _soundOn = value;
            AudioListener.pause = !_soundOn;
        }
    }

    public bool VibrationOn
    {
        get
        {
            if (PlayerPrefs.HasKey("VibrationOn"))
                return PlayerPrefs.GetInt("VibrationOn") != 0;
            else
            {
                VibrationOn = true;
                return true;
            }

        }
        set
        {
            PlayerPrefs.SetInt("VibrationOn", (value ? 1 : 0));
            PlayerPrefs.Save();
            _vibrationOn = value;
            //Taptic.tapticOn = _vibrationOn;

        }
    }

    public float LastSavedGameState
    {
        get
        {
            return PlayerPrefs.GetFloat("LastSavedGameState", -1.0f);
        }
        set
        {

            PlayerPrefs.SetFloat("LastSavedGameState", value);
            PlayerPrefs.Save();
        }
    }

    #endregion

    #region Generic Methods

    // Mainly for buttons
    public void LoadSceneAsyncByID(int id)
    {
        if (Scenes.ContainsValue(id))
        {
            SceneName sceneName = Scenes.FirstOrDefault(x => x.Value == id).Key;
            LoadSceneAsync(sceneName);
        }
        else
        {
            Debug.LogWarning("Cannot find scene by that id");
        }

    }

    // Recommended
    public void LoadSceneAsync(SceneName sceneName)
    {
        int tempID = -100;
        if (Scenes.TryGetValue(sceneName, out tempID))
        {
            StartCoroutine(LoadMySceneAsync(sceneName, tempID));
        }
    }

    IEnumerator LoadMySceneAsync(SceneName sceneName, int tempID)
    {
        if (loadingScene)
            yield break;

        loadingScene = true;

        EventManager.Instance.OnSceneUnloaded(currentScene);

        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneName.ToString());

        while (!asyncOperation.isDone)
        {
            //loadingSlider.value = asyncOperation.progress;
            yield return null;
        }

        // Initialize vars
        currentScene = sceneName;
        currentSceneId = tempID;
        // Initialize Scene Managers

        EventManager.Instance.OnSceneLoaded(currentScene);

        loadingScene = false;
    }

    public void AdvanceLevel()
    {
        Level++;

        Coins += AccumulatedCoin;
        AccumulatedCoin = 0;

        EventManager.Instance.OnLevelAdvanced();

        Debug.Log("Advancing to Level: " + Level);

    }

    public void DecreaseLevel()
    {
        Level--;

        Debug.Log("Decreasing Level to: " + Level);
    }

    public void ReloadLevel()
    {
        Debug.Log("Reload Level " + Level);

        LoadSceneAsync(currentScene);
    }

    public bool IsScene(SceneName sceneName)
    {
        return currentScene == sceneName;
    }

    public bool IsGameState(GameState gameState)
    {
        return currentGameState == gameState;
    }

    public void LoadBinaryData()
    {
        Debug.Log("Loading Game State");

        PlayerData data = SaveSystem.LoadGameState();

        if (Version != data._version) // New Version - Load Only Static Vars
        {
            // Generic Persistent Through Versions
            Score = data._score;
            Level = data._level;
            Coins = data._coins;
            // Game Specific
        }
        else // Same Version - Load Everything
        {
            // Generic Persistent Through Versions
            Score = data._score;
            Level = data._level;
            Coins = data._coins;
            // Game Specific
        }
    }

    public void SaveBinaryData()
    {
        float interval = (float)System.DateTime.Now.TimeOfDay.TotalSeconds - LastSavedGameState;
        Debug.Log("Attempting Save Game State - interval: " + interval);

        if (Mathf.Abs(interval) > 5f)
        {
            LastSavedGameState = (float)System.DateTime.Now.TimeOfDay.TotalSeconds;
            SaveSystem.SaveGameState(this);
            Debug.Log("Last Saved: " + LastSavedGameState);
        }

    }

    public void SetGameState(GameState gameState)
    {
        Debug.Log("Setting GameState " + gameState.ToString());

        switch (gameState)
        {
            case (GameState.Paused):
                Time.timeScale = 0f;
                //PlayerControls.Disabled = true;
                break;
            case (GameState.Playing):
                Time.timeScale = 1f;
                //PlayerControls.Disabled = false;
                break;
            case (GameState.MainMenu):
                Time.timeScale = 1f;
                //PlayerControls.Disabled = false;
                break;
        }

        currentGameState = gameState;
    }

    #endregion

    #region Game Specific Vars

    #endregion

    #region Game Specific Properties

    #endregion

    public static GameManager Instance;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        currentScene = SceneName._preload;
        currentGameState = GameState.None;
        currentSceneId = 0;

        string path = Application.persistentDataPath + "/controller.state";

        Version = Application.version;

        if (File.Exists(path))
        {
            LoadBinaryData();
        }
        else
        {
            Score = 0;
            Level = 0;
            Coins = 0;
        }
    }

    private void Start()
    {
        AudioListener.pause = !SoundOn;
        // Taptic.tapticOn = VibrationOn;

        LoadSceneAsync(SceneName.MainMenuScene);
    }

    private void OnApplicationFocus(bool focus)
    {
        if (!focus)
            SaveBinaryData();

    }

    private void OnApplicationPause(bool pause)
    {
        if (pause)
            SaveBinaryData();
    }

    private void OnApplicationQuit()
    {
        SaveBinaryData();
    }
}
