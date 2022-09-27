// Filename: EffectsManager.cs
// Author: 0xFirekeeper
// Description: Provides a serializable dictionary setup and interface to spawn VFX Prefabs 
//              easily, with its own camera.

using UnityEngine;
using RotaryHeart.Lib.SerializableDictionary;
using TMPro;

public enum EffectTrigger
{
    PerfectMatch,
    GreatMatch,
    GoodMatch,
    BadMatch,
    LevelWon,
    LevelLost,
    FeedbackText,
}

[System.Serializable]
public class Effects : SerializableDictionaryBase<EffectTrigger, GameObject> { }

public class EffectsManager : MonoBehaviour
{
    [Header("This script requires CameraEffects Layer and an EffectsCamera Tag")]
    public Effects effects;

    public static EffectsManager Instance;

    private Camera effectsCamera;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        effectsCamera = GameObject.FindGameObjectWithTag(Tags.EFFECTS_CAMERA).GetComponent<Camera>();
    }

    public void PlayEffect(EffectTrigger effectTrigger, string text = "") // Add any parameters your effect may need
    {
        var effect = Instantiate(effects[effectTrigger], effectsCamera.transform);
        effect.transform.localPosition = Vector3.forward * 3;
        effect.layer = LayerMask.NameToLayer("CameraEffects");

        foreach (Transform child in effect.transform)
            child.gameObject.layer = LayerMask.NameToLayer("CameraEffects");

        effect.AddComponent<SelfDestruct>();

        // Add Special Behavior Here
        switch (effectTrigger)
        {
            case EffectTrigger.FeedbackText:
                effect.GetComponentInChildren<TMP_Text>().text = text;
                effect.GetComponent<SelfDestruct>().lifetime = 1f;
                break;
            default:
                break;

        }
    }

}