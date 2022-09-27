// Filename: EventCenter.cs
// Author: 0xFirekeeper
// Description: Manages all events in the game, a place to connect all scripts and be able
//              to keep track of Backend, UI, Analytics and Effects for each.

using UnityEngine;

public class EventManager : MonoBehaviour
{
    public static EventManager Instance;

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

    void Log(string str)
    {
        Debug.Log("[EventCenter] " + str);
    }

    #region MAIN MENU EVENTS

    #endregion

    #region GAME EVENTS

    #endregion

    #region GENERIC EVENTS

    public void OnLevelAdvanced()
    {
        Log("OnLevelAdvanced - " + (GameManager.Instance.Level - 1));

        // Backend
        // Analytics
        // UI
        // FX
        SoundManager.Instance.PlaySound(SoundTrigger.LevelWon);
    }

    public void OnSceneLoaded(SceneName sceneName)
    {
        Log("OnSceneLoaded - " + sceneName.ToString().AddSpaces());

        switch (sceneName)
        {
            case SceneName._preload:
                break;
            case SceneName.MainMenuScene:
                // Backend
                GameManager.Instance.SetGameState(GameState.MainMenu);
                // Analytics
                // FireAnalytics.Instance.SceneLoaded(sceneName);
                // UI
                UIManager.Instance.OpenPanel(PanelNames.MainMenuCanvas, true);
                // FX
                break;
            case SceneName.GameScene:
                // UI
                UIManager.Instance.OpenPanel(PanelNames.GameSceneCanvas, true);
                // Backend
                GameManager.Instance.SetGameState(GameState.Playing);
                // Analytics
                // FireAnalytics.Instance.SceneLoaded(sceneName);
                // FX
                break;
        }
    }

    public void OnSceneUnloaded(SceneName sceneName)
    {
        Log("OnSceneUnloaded - " + sceneName.ToString().AddSpaces());

        switch (sceneName)
        {
            case SceneName._preload:
                break;
            case SceneName.MainMenuScene:
                // Backend
                // Analytics
                // FireAnalytics.Instance.SceneUnloaded(sceneName);
                // UI
                // FX
                break;
            case SceneName.GameScene:
                // Backend
                // Analytics
                // FireAnalytics.Instance.SceneUnloaded(sceneName);
                // UI
                // FX
                break;
        }
    }

    #endregion

}
