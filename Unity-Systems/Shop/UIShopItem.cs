// Filename: UIShopItem.cs
// Author: 0xFirekeeper
// Description: Shop item setup logic, preferably add this script onto a prefab.

using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIShopItem : MonoBehaviour
{
    // Include any Dynamic UI Elements and update SetupUIShopItem function
    public Button button;
    public Image uiBox;
    public Sprite uiBoxLocked, uiBoxUnlocked;
    public TMP_Text itemPriceText;
    public Image itemSpriteImage, lockedImage, equippedImage;

    public void SetupUIShopItem(ShopItem item)
    {
        button.onClick.RemoveAllListeners();

        itemSpriteImage.sprite = item.sprite;
        itemPriceText.text = item.cost.ToString();

        if (item.equipped) // Equipped
        {
            itemSpriteImage.gameObject.SetActive(true);
            itemPriceText.gameObject.SetActive(false);
            equippedImage.gameObject.SetActive(true);
            lockedImage.gameObject.SetActive(false);

            uiBox.sprite = uiBoxUnlocked;
            // itemSpriteImage.color = Color.white;

            button.onClick.AddListener(() => ShopManager.Instance.OnItemSelected(item.id));
        }
        else if (item.purchased) // Unlocked
        {
            itemSpriteImage.gameObject.SetActive(true);
            itemPriceText.gameObject.SetActive(true);
            equippedImage.gameObject.SetActive(false);
            lockedImage.gameObject.SetActive(false);

            uiBox.sprite = uiBoxUnlocked;
            itemPriceText.text = "equip";
            itemPriceText.color = Color.blue;
            // itemSpriteImage.color = Color.white;

            button.onClick.AddListener(() => ShopManager.Instance.OnItemSelected(item.id));
        }
        else // Locked
        {
            itemSpriteImage.gameObject.SetActive(false);
            itemPriceText.gameObject.SetActive(false);
            equippedImage.gameObject.SetActive(false);
            lockedImage.gameObject.SetActive(true);

            uiBox.sprite = uiBoxLocked;
            // itemPriceText.text = item.cost.ToString();
            // itemPriceText.color = Color.gray;
            // itemSpriteImage.color = Color.gray;
        }
    }
}
