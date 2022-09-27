// Filename: UIAchievementItem.cs
// Author: 0xFirekeeper
// Description: Achievement item setup logic, preferably add this script onto a prefab.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIAchievementItem : MonoBehaviour
{
    // Include any Dynamic UI Elements and update SetupUIShopItem function
    public Image achievementSprite;
    public Button rewardButton, getx2Button;
    public Slider progressBar;
    public TMP_Text sliderText, progressText, descriptionText, nameText, coinText;

    public void SetupUIAchievementItem(AchievementItem item)
    {
        Debug.Log("Setting Up UI Achievement Item: " + item.id.ToString());
        rewardButton.onClick.RemoveAllListeners();
        getx2Button.onClick.RemoveAllListeners();

        achievementSprite.sprite = item.sprite;
        descriptionText.text = item.description;
        nameText.text = item.id.ToUpper();
        coinText.text = item.rewardAmount.ToString("N2");

        progressText.text = item.currentProgress + "/" + item.totalProgress;

        progressBar.maxValue = item.totalProgress;
        progressBar.value = item.currentProgress;

        if (item.currentProgress == item.totalProgress)
            sliderText.text = "COMPLETE";
        else
            sliderText.text = "";


        if (item.claimed)
        {
            rewardButton.gameObject.SetActive(false);
            getx2Button.gameObject.SetActive(false);
        }
        else if (item.currentProgress != item.totalProgress)
        {
            rewardButton.interactable = false;
            getx2Button.interactable = false;
        }
        else
        {
            rewardButton.interactable = true;
            getx2Button.interactable = true;

            rewardButton.onClick.AddListener(() => AchievementManager.Instance.ClaimAchievement(rewardButton.transform.position, item.id));
            getx2Button.onClick.AddListener(() => AchievementManager.Instance.ClaimAchievement(getx2Button.transform.position, item.id, true));
        }

    }
}
