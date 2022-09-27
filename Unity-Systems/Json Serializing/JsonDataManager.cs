// Filename: JsonDataManager.cs
// Author: 0xFirekeeper
// Description: Template script for typical shop or achievement panels that need to be serialized and have UI counterparts.

using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using RotaryHeart.Lib.SerializableDictionary;

// Serializeable singular shop item backend
[System.Serializable]
public class Item
{
    public string id;
    public int cost;
    public bool purchased;
    public bool equipped;

    public Item(string _id, int _cost, bool _purchased, bool _equipped)
    {
        id = _id;
        cost = _cost;
        purchased = _purchased;
        equipped = _equipped;
    }
}

// Json Serializeable List of ShopItems
[System.Serializable]
public class ItemList
{
    public List<Item> itemList;

    public ItemList()
    {
        itemList = new List<Item>();
    }
}

// Link backend ShopItem and UIShopItem as needed
[System.Serializable]
public class UIItems : SerializableDictionaryBase<Item, UIItem> { }

public class JsonDataManager : MonoBehaviour
{
    [Header("Persistent Data Path File Name")]
    public string itemsPath = "/ShopItems.json";
    [Header("Link your serializeable data with ui items")]
    public UIItems uiItems;
    public static JsonDataManager Instance;
    private ItemList currentItems;

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

        LoadItems();
    }

    public void LoadItems()
    {
        Debug.Log("Loading Purchased Shop Items");

        currentItems = new ItemList();
        foreach (var item in uiItems)
        {
            currentItems.itemList.Add(item.Key);
            item.Value.SetupUIItem(item.Key);
        }

        if (!File.Exists(Application.persistentDataPath + itemsPath))
        {
            Debug.Log("Shop Items File Not Found, Assuming No Items Purchased");
            return;
        }
        else
        {
            string loadedItems = File.ReadAllText(Application.persistentDataPath + itemsPath);
            ItemList savedShop = JsonUtility.FromJson<ItemList>(loadedItems);
            // Update current shop items with saved json ones, works through different shops between versions
            foreach (Item savedItem in savedShop.itemList)
            {
                var currentShopItem = currentItems.itemList.FirstOrDefault(currentItem => currentItem.id == savedItem.id);
                if (currentShopItem != null)
                {
                    // Not using currentShopItem = savedItem to keep the reference to dictionary from start
                    currentShopItem.cost = savedItem.cost; // Cost updates are optional
                    currentShopItem.purchased = savedItem.purchased;
                    currentShopItem.equipped = savedItem.equipped;

                    uiItems[currentShopItem].SetupUIItem(currentShopItem);
                }
                else
                {
                    Debug.LogWarning("Could not find old item in current item list: " + savedItem.id);
                }
            }

            SaveItems();
        }
    }

    public void SaveItems()
    {
        Debug.Log("Saving Current Items Locally.");
        string currentShopJson = JsonUtility.ToJson(currentItems);
        File.WriteAllText(Application.persistentDataPath + itemsPath, currentShopJson);
    }

    public void UnlockItem(string id)
    {
        var item = currentItems.itemList.FirstOrDefault(x => x.id == id);

        if (item == null)
        {
            Debug.LogWarning("Wrong ID provided when purchasing item: " + id);
            return;
        }

        if (item.equipped) // Unequip on second click
        {
            item.equipped = false;
        }
        else if (item.purchased) // Equip on first click
        {
            item.equipped = true;
        }
        else if (/*GameManager.Instance.Coins > item.cost*/ true) // Purchase 
        {
            // GameManager.Insance.Coins -= item.cost;
            // SoundManager.Instance.PlaySound(SoundTrigger.ItemPurchased);
            item.purchased = true;
            item.equipped = true;
        }

        uiItems[item].SetupUIItem(item);


        SaveItems();
    }
}
