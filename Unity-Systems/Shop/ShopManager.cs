// Filename: ShopManager.cs
// Author: 0xFirekeeper
// Description: Shop System with UI Interfacing.

using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using RotaryHeart.Lib.SerializableDictionary;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

// ID Should Be: "Category_ItemDescriptor" or "Category_ItemIndex"

public enum ShopItemID
{
    BallSkin_0,
    BallSkin_1,
    BallSkin_2,
    BallSkin_3,
    BallSkin_4,

    BallTrail_0,
    BallTrail_1,
    BallTrail_2,
    BallTrail_3,
    BallTrail_4,

    BallSkin_5,
    BallSkin_6,

    PlayerSkin_0,
    PlayerSkin_1,
    PlayerSkin_2,
    PlayerSkin_3,
    PlayerSkin_4,
}

public enum ShopItemCategory
{
    BallSkin = 0,
    BallTrail = 1,
    PlayerSkin = 2,
}

// Serializeable singular shop item backend
[System.Serializable]
public class ShopItem
{
    // JSON-Serializeable
    public string id;
    public int category;
    public int cost;
    public bool purchased;
    public bool equipped;
    public bool uniqueInCategory;
    // Non Json-Serializeable
    public Sprite sprite;

    public ShopItem(string _id, int _category, int _cost, bool _purchased, bool _equipped, bool _uniqueInCategory, Sprite _sprite)
    {
        id = _id;
        category = _category;
        cost = _cost;
        purchased = _purchased;
        equipped = _equipped;
        uniqueInCategory = _uniqueInCategory;
        sprite = _sprite;
    }
}

// Json Serializeable List of ShopItems
[System.Serializable]
public class ShopItemList
{
    public List<ShopItem> shopItemList;

    public ShopItemList()
    {
        shopItemList = new List<ShopItem>();
    }
}

[System.Serializable]
public class MaterialListWrapper
{
    public List<Material> materialList;
}

[System.Serializable]
public class ShopItemDictionary : SerializableDictionaryBase<ShopItemID, ShopItem> { }

[System.Serializable]
public class PlayerSkinMaterials : SerializableDictionaryBase<ShopItemID, MaterialListWrapper> { }

[System.Serializable]
public class BallSkinMaterials : SerializableDictionaryBase<ShopItemID, Material> { }

// Link backend ShopItem and UIShopItem as needed
public class ShopManager : MonoBehaviour
{
    [Header("Shop Interfacing Dictionaries")]
    public BallSkinMaterials ballSkinMaterials;
    public PlayerSkinMaterials playerSkinMaterials;

    [Header("Persistent Data Path File Name")]
    public string shopItemsPath = "/ShopItems.json";

    [Header("Default Shop Item Data")]
    public ShopItemDictionary shopItemDictionary; // For Inspector Visibility
    [HideInInspector]
    public ShopItemList allShopItems;  // Backend Serializeable, based on shopItemDictionary
    private Dictionary<ShopItem, UIShopItem> allUIShopItems; // Frontend Data, based on dynamic UIShopItem Gameobject Spawning

    [Header("Shop Item UI")]
    public Transform uiShopItemParent; // Grid
    public GameObject uiShopItemPrefab;

    [Header("Category UI")]
    public Transform categoryParent; // Horizontal Layout
    public GameObject categoryPrefab;
    public Sprite categorySelected, categoryDeselected;

    [Header("Page UI")]
    public Transform pageParent; // Horizontal Layout
    public GameObject pagePrefab;
    public Sprite pageSelected, pageDeselected;

    [Header("Other UI Items")]
    public Image shopItemPreview;
    public Button unlockButton;
    public TMP_Text unlockButtonCostText;
    public Button unlockRewardedButton;
    public TMP_Text shopCoinText;

    public static ShopManager Instance;

    // Selected Category, Item, Page
    private ShopItemCategory selectedCategory;
    private int selectedPage;
    private int maxPages;
    private ShopItem selectedShopItem;

    // All Categories, Pages, UIShopItem Gameobjects Spawned;
    private List<GameObject> currentCategories;
    private List<GameObject> currentPages;
    private List<UIShopItem> currentUIShopItems;

    // Events
    public delegate void ShopItemEquipAction();
    public static event ShopItemEquipAction OnShopItemEquipped;

    // Random Shop Variables
    Dictionary<ShopItemCategory, int> categoryCost = new Dictionary<ShopItemCategory, int>(){
        {ShopItemCategory.BallSkin, 250},
        {ShopItemCategory.BallTrail, 500},
        {ShopItemCategory.PlayerSkin, 250}
    };

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

        InitializeShop();
    }

    private void InitializeShop()
    {
        allShopItems = new ShopItemList();
        allUIShopItems = new Dictionary<ShopItem, UIShopItem>();

        foreach (var inspectorItem in shopItemDictionary)
        {
            ShopItem inspectorShopItem = inspectorItem.Value;
            allShopItems.shopItemList.Add(inspectorShopItem);
        }

        // Those can be PlayerPrefs
        selectedCategory = 0;
        selectedPage = 0;
        selectedShopItem = allShopItems.shopItemList[0];

        currentCategories = new List<GameObject>();
        currentPages = new List<GameObject>();
        currentUIShopItems = new List<UIShopItem>();

        LoadItems();
        LoadUI();
    }

    #endregion

    #region Backend

    void LoadItems()
    {
        Debug.Log("Loading Purchased Shop Items");

        if (!File.Exists(Application.persistentDataPath + shopItemsPath))
        {
            Debug.Log("Shop Items File Not Found, Assuming No Items Purchased");
            return;
        }
        else
        {
            string loadedItems = File.ReadAllText(Application.persistentDataPath + shopItemsPath);
            ShopItemList savedShop = JsonUtility.FromJson<ShopItemList>(loadedItems);
            // Update current shop items with saved json ones, works through different shops between versions
            foreach (ShopItem savedItem in savedShop.shopItemList)
            {
                ShopItem currentShopItem = allShopItems.shopItemList.FirstOrDefault(currentItem => currentItem.id == savedItem.id);
                if (currentShopItem != null)
                {
                    // Not using currentShopItem = savedItem to keep the reference to dictionary from start
                    currentShopItem.cost = savedItem.cost; // Cost updates are optional
                    currentShopItem.purchased = savedItem.purchased;
                    currentShopItem.equipped = savedItem.equipped;
                    currentShopItem.category = savedItem.category;
                    currentShopItem.uniqueInCategory = savedItem.uniqueInCategory;
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
        string currentShopJson = JsonUtility.ToJson(allShopItems);
        File.WriteAllText(Application.persistentDataPath + shopItemsPath, currentShopJson);
    }

    public void UnlockRandomItem()
    {
        var unlockables = allShopItems.shopItemList.Where(x => !x.purchased && x.category == (int)selectedCategory);
        string randomItemID = unlockables.ElementAt(Random.Range(0, unlockables.Count())).id;

        if (UnlockShopItem(randomItemID))
            OnItemSelected(randomItemID);
    }

    public bool UnlockShopItem(string id)
    {
        Debug.Log("Attempting UnlockShopItem: " + id);
        var item = allShopItems.shopItemList.FirstOrDefault(x => x.id == id);
        bool canAfford = true;

        if (item == null)
        {
            Debug.LogWarning("Wrong ID provided when purchasing item: " + id);
            return false;
        }

        if (item.equipped) // Unequip on second click
        {
            // item.equipped = false;
        }
        else if (item.purchased) // Equip on first click
        {
            if (item.uniqueInCategory)
            {
                foreach (var sameCatItem in allShopItems.shopItemList.FindAll(x => x.category == item.category))
                {
                    sameCatItem.equipped = false;
                    allUIShopItems[sameCatItem].SetupUIShopItem(sameCatItem);
                }
            }
            item.equipped = true;

            OnShopItemEquipped?.Invoke();
        }
        else if (/*GameManager.Instance.Coins > item.cost*/ GameManager.Instance.Coins > categoryCost[selectedCategory]) // Purchase 
        {
            // GameManager.Instance.Coins -= item.cost;
            GameManager.Instance.Coins -= categoryCost[selectedCategory];
            // SoundManager.Instance.PlaySound(SoundTrigger.Success);
            item.purchased = true;
            if (item.uniqueInCategory)
            {
                foreach (var sameCatItem in allShopItems.shopItemList.FindAll(x => x.category == item.category))
                {
                    sameCatItem.equipped = false;
                    allUIShopItems[sameCatItem].SetupUIShopItem(sameCatItem);
                }
            }
            item.equipped = true;

            OnShopItemEquipped?.Invoke();

            // FireAnalytics.Instance.OnShopItemUnlock(id);

            AchievementManager.Instance.IncreaseAchievementProgress(AchievementItemID.Wealthy.ToString());

            if (allShopItems.shopItemList.FirstOrDefault(x => !x.purchased) == null)
            {
                AchievementManager.Instance.IncreaseAchievementProgress(AchievementItemID.Rich.ToString());
            }
        }
        else
        {
            Debug.Log("Not Enough Coins To Purchase: " + id);
            canAfford = false;
            // SoundManager.Instance.PlaySound(SoundTrigger.Fail);
        }

        allUIShopItems[item].SetupUIShopItem(item);

        SaveItems();

        shopCoinText.text = GameManager.Instance.Coins.ToString();

        return canAfford;
    }

    #endregion

    #region Frontend

    void LoadUI()
    {
        shopCoinText.text = GameManager.Instance.Coins.ToString();

        // Initializes all ShopItem, UIShopItem, and Categories UI Backend
        foreach (Transform item in uiShopItemParent)
            Destroy(item.gameObject);

        foreach (Transform item in categoryParent)
            Destroy(item.gameObject);

        int categoryCount = System.Enum.GetValues(typeof(ShopItemCategory)).Length;

        foreach (ShopItem shopItem in allShopItems.shopItemList)
        {
            GameObject uiObject = Instantiate(uiShopItemPrefab, uiShopItemParent);
            uiObject.name = "UIShopItem_" + shopItem.id;

            UIShopItem uiShopItem = uiObject.GetComponentInChildren<UIShopItem>();
            uiShopItem.SetupUIShopItem(shopItem);

            allUIShopItems.Add(shopItem, uiShopItem);
        }

        for (int i = 0; i < categoryCount; i++)
        {
            GameObject uiObject = Instantiate(categoryPrefab, categoryParent);
            uiObject.name = "Category_" + (ShopItemCategory)i;

            uiObject.GetComponentInChildren<Button>().onClick.RemoveAllListeners();
            int newI = i;
            uiObject.GetComponentInChildren<Button>().onClick.AddListener(() => OnCategorySelected(newI));

            TMP_Text categoryText = uiObject.GetComponentInChildren<TMP_Text>();
            categoryText.text = ((ShopItemCategory)i).ToString().AddSpaces().ToUpper();

            if (i == (int)selectedCategory)
                uiObject.GetComponentInChildren<Image>().sprite = categorySelected;
            else
                uiObject.GetComponentInChildren<Image>().sprite = categoryDeselected;

            currentCategories.Add(uiObject);
        }

        OnCategorySelected((int)selectedCategory);
    }

    public void OnChangePage(bool previous)
    {
        Debug.Log("OnChangePage");

        // SoundManager.Instance.PlaySound(SoundTrigger.Pop);

        if (maxPages < 1)
            return;

        // Update Selected Category
        if (previous)
        {
            if (selectedPage == 0)
                selectedPage = maxPages - 1;
            else
                selectedPage--;
        }
        else
        {
            if (selectedPage == maxPages - 1)
                selectedPage = 0;
            else
                selectedPage++;
        }

        RefreshShopUI();
    }

    public void OnCategorySelected(int category)
    {
        Debug.Log("OnCategorySelected: " + category);

        // SoundManager.Instance.PlaySound(SoundTrigger.Pop);

        selectedPage = 0;

        // Update Selected Category
        selectedCategory = (ShopItemCategory)category;

        for (int i = 0; i < currentCategories.Count; i++)
        {
            currentCategories[i].GetComponentInChildren<TMP_Text>().color = i == category ? Color.white : new Color(0.4f, 0.15f, 0.56f, 1f);
            currentCategories[i].GetComponentInChildren<Image>().sprite = i == category ? categorySelected : categoryDeselected;
        }

        // Select last equipped item, or first from the list if none are 
        var lastEquipped = allShopItems.shopItemList.FirstOrDefault(x => x.category == (int)selectedCategory && x.equipped);

        if (lastEquipped == null)
            OnItemSelected(allShopItems.shopItemList.First(x => x.category == (int)selectedCategory).id);
        else
            OnItemSelected(lastEquipped.id);

        RefreshShopUI();
    }

    public void OnItemSelected(string id)
    {
        Debug.Log("OnItemSelected: " + id);

        // SoundManager.Instance.PlaySound(SoundTrigger.Pop);

        // Update Selected Shop Item
        allUIShopItems[selectedShopItem].transform.Find("Selected").gameObject.SetActive(false);
        selectedShopItem = allShopItems.shopItemList.FirstOrDefault(x => x.id == id);
        allUIShopItems[selectedShopItem].transform.Find("Selected").gameObject.SetActive(true);

        if (selectedShopItem.purchased)
            UnlockShopItem(selectedShopItem.id);

        RefreshShopUI();
    }

    void RefreshShopUI()
    {
        Debug.Log("RefreshShopUI");

        // Set category items active
        foreach (var item in allUIShopItems)
        {
            if (item.Key.category != (int)selectedCategory)
                item.Value.gameObject.SetActive(false);
            else
                item.Value.gameObject.SetActive(true);
        }

        // Set preview image
        shopItemPreview.sprite = selectedShopItem.sprite;

        // Set unlock buttons active/inactive
        if (/*selectedShopItem.purchased*/ !allShopItems.shopItemList.Exists(x => !x.purchased && x.category == (int)selectedCategory))
        {
            unlockButton.gameObject.SetActive(false);
            unlockRewardedButton.gameObject.SetActive(false);
        }
        else
        {
            unlockButton.gameObject.SetActive(true);
            // unlockRewardedButton.gameObject.SetActive(true); // ADD THIS AFTER ADS
            // unlockButtonCostText.text = selectedShopItem.cost.ToString(); // Traditional Shop
            unlockButtonCostText.text = categoryCost[selectedCategory].ToString(); // Random Unlock Shop
            unlockButton.onClick.RemoveAllListeners();
            // unlockButton.onClick.AddListener(() => UnlockShopItem(selectedShopItem.id)); // Traditional Shop
            unlockButton.onClick.AddListener(() => UnlockRandomItem()); // Random Unlock Shop
            // Add listener for rewarded buttonwith callback UnlockShopItem(currentlySelectedItem.id);
        }

        // Handle Pages
        foreach (Transform item in pageParent)
            Destroy(item.gameObject);

        // Count UI Items in that category
        currentUIShopItems = allUIShopItems.Where(x => x.Key.category == (int)selectedCategory).Select(x => x.Value).ToList();
        maxPages = 1 + currentUIShopItems.Count / 9; // 9 being max displayed per page
        // Handle Page Icons UI
        currentPages = new List<GameObject>();
        for (int i = 0; i < maxPages; i++)
        {
            currentPages.Add(Instantiate(pagePrefab, pageParent));
            if (i == selectedPage)
                currentPages[i].GetComponentInChildren<Image>().sprite = pageSelected;
            else
                currentPages[i].GetComponentInChildren<Image>().sprite = pageDeselected;
        }
        // Handle UIShopItems to display
        for (int i = 0; i < currentUIShopItems.Count; i++)
        {
            if (i >= selectedPage * 9 && i < selectedPage * 9 + 9)
                currentUIShopItems[i].gameObject.SetActive(true);
            else
                currentUIShopItems[i].gameObject.SetActive(false);
        }
    }

    #endregion
}
