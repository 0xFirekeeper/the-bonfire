// Filename: AchievementManager.cs
// Author: 0xFirekeeper
// Description: Achievement System with UI Interfacing.

using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using RotaryHeart.Lib.SerializableDictionary;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
// using DG.Tweening;

public enum AchievementItemID
{
    Rookie,
    Newbie,
    Amateur,
    Sharpshooter,
    Swaggy,
    Pro,
    Bullseye,
    Stylish,
    Silver,
    Gold,
    Titanium,
    Platinum,
    Palladium,
    Jade,
    Ruby,
    Sapphire,
    Diamond,
    Quick,
    Epic,
    Selfless,
    Selfish,
    LeBron,
    Kerr,
    Nash,
    Wealthy,
    Rich,
    Competitive,
    Master,
    Challenger,
    Godlike
}

public enum AchievementItemCategory
{
    Default = 0
}

// Serializeable singular Achievement item backend
[System.Serializable]
public class AchievementItem
{
    // JSON-Serializeable
    public string id;
    public int category;
    public int rewardAmount;
    public string description;
    public int currentProgress;
    public int totalProgress;
    public bool claimed;
    // Non Json-Serializeable
    public Sprite sprite;

    public AchievementItem(string _id, int _category, int _rewardAmount, string _description, int _currentProgress, int _totalProgress, bool _claimed, Sprite _sprite)
    {
        id = _id;
        category = _category;
        rewardAmount = _rewardAmount;
        description = _description;
        currentProgress = _currentProgress;
        totalProgress = _totalProgress;
        claimed = _claimed;
        sprite = _sprite;
    }
}

// Json Serializeable List of AchievementItems
[System.Serializable]
public class AchievementItemList
{
    public List<AchievementItem> achievementItemList;

    public AchievementItemList()
    {
        achievementItemList = new List<AchievementItem>();
    }
}

[System.Serializable]
public class AchievementItemDictionary : SerializableDictionaryBase<AchievementItemID, AchievementItem> { }

// Link backend AchievementItem and UIAchievementItem as needed
public class AchievementManager : MonoBehaviour
{
    [Header("Persistent Data Path File Name")]
    public string achievementItemsPath = "/AchievementItems.json";

    [Header("Default Achievement Item Data")]
    public AchievementItemDictionary achievementItemDictionary; // For Inspector Visibility
    private AchievementItemList allAchievementItems;  // Backend Serializeable, based on achievementItemDictionary
    private Dictionary<AchievementItem, UIAchievementItem> allUIAchievementItems; // Frontend Data, based on dynamic UIAchievementItem Gameobject Spawning

    [Header("Achievement Item UI")]
    public Transform uiAchievementItemParent; // Grid
    public GameObject uiAchievementItemPrefab;

    // [Header("Coin Animation")]
    // public DOTweenAnimation coinTween;

    public GameObject coinsParent;
    public GameObject coinsTarget;

    public static AchievementManager Instance;

    #region Initialization

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

        InitializeAchievements();
    }

    private void InitializeAchievements()
    {
        allAchievementItems = new AchievementItemList();
        allUIAchievementItems = new Dictionary<AchievementItem, UIAchievementItem>();

        foreach (var inspectorItem in achievementItemDictionary)
        {
            AchievementItem inspectorAchievementItem = inspectorItem.Value;
            allAchievementItems.achievementItemList.Add(inspectorAchievementItem);
        }

        LoadItems();
        LoadUI();
    }

    #endregion

    #region Backend

    void LoadItems()
    {
        Debug.Log("Loading Purchased Achievement Items");

        if (!File.Exists(Application.persistentDataPath + achievementItemsPath))
        {
            Debug.Log("Achievement Items File Not Found, Assuming No Items Purchased");
            return;
        }
        else
        {
            string loadedItems = File.ReadAllText(Application.persistentDataPath + achievementItemsPath);
            AchievementItemList savedAchievement = JsonUtility.FromJson<AchievementItemList>(loadedItems);
            // Update current Achievement items with saved json ones, works through different Achievements between versions
            foreach (AchievementItem savedItem in savedAchievement.achievementItemList)
            {
                AchievementItem currentAchievementItem = allAchievementItems.achievementItemList.FirstOrDefault(currentItem => currentItem.id == savedItem.id);
                if (currentAchievementItem != null)
                {
                    currentAchievementItem.currentProgress = savedItem.currentProgress;
                    currentAchievementItem.claimed = savedItem.claimed;
                }
                else
                {
                    Debug.LogWarning("Could not find old item in current item list: " + savedItem.id);
                }
            }

            SaveItems();
        }
    }

    void SaveItems()
    {
        Debug.Log("Saving Current Items Locally.");
        string currentAchievementJson = JsonUtility.ToJson(allAchievementItems);
        File.WriteAllText(Application.persistentDataPath + achievementItemsPath, currentAchievementJson);
    }

    public void ClaimAchievement(Vector3 position, string id, bool x2 = false)
    {
        var item = allAchievementItems.achievementItemList.FirstOrDefault(x => x.id == id);

        if (item == null)
        {
            Debug.LogWarning("Wrong ID provided when purchasing item: " + id);
            return;
        }

        item.claimed = true;

        if (x2)
            GameManager.Instance.Coins += item.rewardAmount * 2;
        else
            GameManager.Instance.Coins += item.rewardAmount;

        // coinTween.DORestart();

        // SoundManager.Instance.PlaySound(SoundTrigger.Success);

        allUIAchievementItems[item].SetupUIAchievementItem(item);

        SaveItems();

        // FireAnalytics.Instance.OnAchievementClaim(id);

        // UIManager.Instance.PlayCoinAnimation(coinsParent, coinsTarget, position);
    }

    [ContextMenu("TestIncrease")]
    public void IncreaseTest()
    {
        IncreaseAchievementProgress(AchievementItemID.Bullseye.ToString());
    }

    public void IncreaseAchievementProgress(string id)
    {
        Debug.Log("Attempting UnlockAchievementItem: " + id);
        var item = allAchievementItems.achievementItemList.FirstOrDefault(x => x.id == id);

        if (item == null)
        {
            Debug.LogWarning("Wrong ID provided when purchasing item: " + id);
            return;
        }

        if (item.claimed || item.currentProgress == item.totalProgress)
        {
            Debug.LogWarning("Item already completed");
            return;
        }
        else
        {
            item.currentProgress++;

            if (item.currentProgress == item.totalProgress)
            {
                Debug.LogWarning("Showing Achievement Popup for " + id);
                AchievementPopup.Instance.DisplayPopup(id);
                // FireAnalytics.Instance.OnAchievementComplete(id);
            }
        }

        allUIAchievementItems[item].SetupUIAchievementItem(item);

        SaveItems();
    }

    #endregion

    #region Frontend

    void LoadUI()
    {
        // Initializes all AchievementItem, UIAchievementItem, and Categories UI Backend
        foreach (Transform item in uiAchievementItemParent)
            Destroy(item.gameObject);

        foreach (AchievementItem AchievementItem in allAchievementItems.achievementItemList)
        {
            GameObject uiObject = Instantiate(uiAchievementItemPrefab, uiAchievementItemParent);
            uiObject.name = "UIAchievementItem_" + AchievementItem.id;

            UIAchievementItem uiAchievementItem = uiObject.GetComponentInChildren<UIAchievementItem>();
            uiAchievementItem.SetupUIAchievementItem(AchievementItem);

            allUIAchievementItems.Add(AchievementItem, uiAchievementItem);
        }
    }

    #endregion
}
