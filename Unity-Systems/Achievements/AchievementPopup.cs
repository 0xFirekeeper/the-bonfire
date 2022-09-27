// Filename: AchievementPopup.cs
// Author: 0xFirekeeper
// Description: Handle user achievement events with an animation.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class AchievementPopup : MonoBehaviour
{
    public TMP_Text achievementName;
    public TMP_Text achievementDescription;

    public static AchievementPopup Instance;

    private Animator animator;

    private void Awake()
    {
        Instance = this;

        animator = GetComponentInChildren<Animator>();
    }

    public void DisplayPopup(string id)
    {
        StartCoroutine(PopupAnimation(id));
    }

    IEnumerator PopupAnimation(string id)
    {
        // Wait for any previous anim
        while (animator.GetCurrentAnimatorStateInfo(0).IsName("Popup Animation") &&
                animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f)
            yield return null;

        AchievementItem item = AchievementManager.Instance.achievementItemDictionary[(AchievementItemID)Enum.Parse(typeof(AchievementItemID), id)];

        achievementName.text = item.id;
        achievementDescription.text = item.description;

        animator.SetTrigger("Play");
        // SoundManager.Instance.PlaySound(SoundTrigger.Success);

        while (animator.GetCurrentAnimatorStateInfo(0).IsName("Popup Animation") &&
                animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f)
            yield return null;

        Debug.Log("Done showing popup");

        yield return null;
    }
}
