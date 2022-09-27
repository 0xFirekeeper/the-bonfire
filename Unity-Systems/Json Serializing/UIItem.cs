// Filename: UIItem.cs
// Author: 0xFirekeeper
// Description: The frontend of the JsonDataManager serialized items, also a template.

using UnityEngine;
using UnityEngine.UI;

// UIShopItem monobehavior to quickly edit the UI using its related shopItem
public class UIItem : MonoBehaviour
{
    // Include any Dynamic UI Elements and update SetupUIShopItem function
    public Image image;
    public Text costText;

    public void SetupUIItem(Item item)
    {
        costText.text = item.cost + "";
        if (item.equipped)
        {
            image.color = Color.blue;
            costText.gameObject.SetActive(false);
        }
        else if (item.purchased)
        {
            image.color = Color.green;
            costText.gameObject.SetActive(false);
        }
        else
        {
            image.color = Color.white;
            costText.gameObject.SetActive(true);
        }
    }
}
