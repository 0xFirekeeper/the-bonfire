// Filename: UIManager.cs
// Author: 0xFirekeeper
// Description: UI Manager to Activate and Deactivate Panels with a serialized dictionary setup and an optional callback method.

using UnityEngine;
using UnityEngine.Events;
using RotaryHeart.Lib.SerializableDictionary;
using UnityEngine.UI;
using System;
using TMPro;

public enum PanelNames
{
    MainMenuCanvas,
    GameSceneCanvas
}

[System.Serializable]
public class UIPanels : SerializableDictionaryBase<PanelNames, UIPanelAndSetup> { }

[System.Serializable]
public class UIPanelAndSetup
{
    public GameObject UIPanel;
    public UnityEvent UIPanelSetup;
}

public class UIManager : MonoBehaviour
{
    public UIPanels UIPanelsDictionary;

    [Header("MAIN MENU CANVAS ITEMS")]
    public Image vibrationImage;
    public Image soundImage;
    public Sprite onSprite;
    public Sprite offSprite;

    [Header("GAME CANVAS ITEMS")]
    public TMP_Text coins;
    public TMP_Text level;

    public static UIManager Instance;

    void Awake()
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

    public void OnMainMenuSceneCanvasOpened()
    {
        Debug.Log("Setting Up Main Menu Scene Canvas");

        vibrationImage.sprite = GameManager.Instance.VibrationOn ? onSprite : offSprite;
        soundImage.sprite = GameManager.Instance.SoundOn ? onSprite : offSprite;
    }

    public void OnGameSceneCanvasOpened()
    {
        Debug.Log("Setting Up Game Scene Canvas");

        coins.text = GameManager.Instance.Coins.FormatNumber();
        level.text = GameManager.Instance.Level.ToString("N2");
    }

    public void ToggleVibration()
    {
        GameManager.Instance.VibrationOn = !GameManager.Instance.VibrationOn;
        vibrationImage.sprite = GameManager.Instance.VibrationOn ? onSprite : offSprite;
    }
    public void ToggleSound()
    {
        GameManager.Instance.SoundOn = !GameManager.Instance.SoundOn;
        soundImage.sprite = GameManager.Instance.SoundOn ? onSprite : offSprite;
    }

    public void OpenPanel(string panel)
    {
        PanelNames panelName;
        if (Enum.TryParse<PanelNames>(panel, out panelName))
            OpenPanel(panelName);
        else
            Debug.LogWarning("Did not find panel: " + panel);
    }

    public void OpenPanel(PanelNames panelName, bool closeOtherPanels = false)
    {
        UIPanelAndSetup panelToOpen;
        if (UIPanelsDictionary.TryGetValue(panelName, out panelToOpen))
        {
            if (closeOtherPanels)
                CloseAllPanels();

            panelToOpen.UIPanel.SetActive(true);
            panelToOpen.UIPanelSetup?.Invoke();
        }
        else
        {
            Debug.LogWarning("No value for key: " + panelName + " exists");
        }

    }

    public void ClosePanel(PanelNames panelName)
    {
        UIPanelAndSetup currentPanel;
        if (UIPanelsDictionary.TryGetValue(panelName, out currentPanel))
        {
            currentPanel.UIPanel.SetActive(false);
            Debug.Log(panelName + " closed");
        }
    }

    void CloseAllPanels()
    {
        foreach (PanelNames panelName in UIPanelsDictionary.Keys)
            ClosePanel(panelName);
    }

}



