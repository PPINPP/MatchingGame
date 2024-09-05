using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public enum SoundType {
    Spawn,
    Click,
    CorrectMatch,
    WrongMatch
}

[CreateAssetMenu(fileName = "SoundEffect_", menuName = "ScriptableObjects/Sounds/SoundEffect")]
[Serializable]
public class SoundEffectSO : SerializedScriptableObject
{
    #region Header SOUND EFFECT DETAILS

    [Space(10)]
    [Header("SOUND EFFECT DETAILS")]

    #endregion

    #region Tooltip

    [Tooltip("The name for the sound effect")]

    #endregion

    public SoundType soundType;
    public AudioClip soundEffectClip;
}
