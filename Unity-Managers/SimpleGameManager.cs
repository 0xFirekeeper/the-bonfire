// Filename: SimpleGameManager.cs
// Author: 0xFirekeeper
// Description: Simpler version of GameManager.cs for games with a small amount of scenes or no local saving required.

using UnityEngine;

public enum SimpleGameState
{
    MainMenu,
    MainGame,
    Playing,
    Paused
}
public class SimpleGameManager : MonoBehaviour
{
    private int _coins;
    public int Coins
    {
        get
        {
            return _coins;
        }
        set
        {
            _coins = value;
            PlayerPrefs.SetInt("Coins", _coins);
        }
    }

    private int _level;
    public int Level
    {
        get
        {
            return _level;
        }
        set
        {
            _level = value;
            PlayerPrefs.SetInt("Level", _level);
        }
    }

    private SimpleGameState _gameState;

    public static SimpleGameManager Instance;

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
    }

    void Start()
    {
        Coins = PlayerPrefs.GetInt("Coins", 0);
        Level = PlayerPrefs.GetInt("Level", 0);
        SetGameState(SimpleGameState.MainMenu);
    }

    public void SetGameState(SimpleGameState simpleGameState)
    {
        _gameState = simpleGameState;

        switch (_gameState)
        {
            case (SimpleGameState.MainMenu):
                UIManager.Instance.OpenPanel(PanelNames.MainMenuCanvas, true);
                break;
            case (SimpleGameState.MainGame):
                UIManager.Instance.OpenPanel(PanelNames.MainGameCanvas, true);
                break;
            case (SimpleGameState.Playing):
                // Do something here. Activate controls, deal with panels etc.
                break;
            case (SimpleGameState.Paused):
                // Do something here. Disable controls, pause time, etc.
                break;
        }
    }
}