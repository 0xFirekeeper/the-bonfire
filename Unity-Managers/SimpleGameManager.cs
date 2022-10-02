// Filename: SimpleGameManager.cs
// Author: 0xFirekeeper
// Description: Simpler version of GameManager.cs for games with a small amount of scenes or no local saving required.

using System.Collections;
using System.Collections.Generic;
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
    public int Coins { get; private set; }
    public int Level { get; private set; }
    private SimpleGameState gameState;

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
        switch (simpleGameState)
        {
            case (SimpleGameState.MainMenu):
                UIManager.Instance.OpenPanel(PanelNames.MainMenuPanel, true);
                CameraManager.Instance.MoveToView(CameraTransforms.View1);
                break;
            case (SimpleGameState.MainGame):
                UIManager.Instance.OpenPanel(PanelNames.MainGamePanel, true);
                CameraManager.Instance.MoveToView(CameraTransforms.View2);
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
