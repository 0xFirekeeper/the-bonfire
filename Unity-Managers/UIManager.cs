// Filename: UIManager.cs
// Author: 0xFirekeeper
// Description: UI Manager to Activate and Deactivate Panels with a serialized dictionary setup and an optional callback method.

using UnityEngine;
using UnityEngine.Events;
using RotaryHeart.Lib.SerializableDictionary;
using System;

public enum PanelNames
{
    MainMenuCanvas,
    MainGameCanvas
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
    [Header("UI PANEL OBJECTS AND EVENTS DICTIONARY")]
    public UIPanels UIPanelsDictionary;

    [Header("MAIN MENU CANVAS ITEMS")]

    [Header("MAIN GAME CANVAS ITEMS")]

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

    public void OnMainMenuCanvasOpened()
    {
        Debug.Log("Setting Up Main Menu Canvas");

    }

    public void OnMainGameCanvasOpened()
    {
        Debug.Log("Setting Up Main Game Canvas");
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

    public void OpenPanel(string panelString)
    {
        PanelNames panelName;
        if (Enum.TryParse<PanelNames>(panelString, out panelName))
            OpenPanel(panelName);
        else
            Debug.LogWarning("Did not find panel: " + panelString);
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

    public void ClosePanel(string panelString)
    {
        PanelNames panelName;
        if (Enum.TryParse<PanelNames>(panelString, out panelName))
            ClosePanel(panelName);
        else
            Debug.LogWarning("Did not find panel: " + panelString);
    }

    void CloseAllPanels()
    {
        foreach (PanelNames panelName in UIPanelsDictionary.Keys)
            ClosePanel(panelName);
    }

}