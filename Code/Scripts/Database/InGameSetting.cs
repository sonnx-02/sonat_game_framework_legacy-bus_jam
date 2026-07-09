using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;


[CreateAssetMenu(fileName = nameof(InGameSetting) + ".asset", menuName = "Duong/" + nameof(InGameSetting))]
public class InGameSetting : ScriptableObject
{
    [SerializeField] [IndexAsEnum(BuiltInEnumType.SoundEnum)] private int[] comboSounds;

    
    public int GetSound(int combo)
    {
        if (comboSounds.Length == 0)
            return 0;
        return comboSounds[Mathf.Clamp(combo, 0, comboSounds.Length-1)];
    }
}