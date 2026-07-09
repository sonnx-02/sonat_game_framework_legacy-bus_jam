using System;
using System.Collections.Generic;
using System.Linq;
using TMPro.SpriteAssetUtilities;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public class SoundDataItem
{
    [IndexAsEnum(BuiltInEnumType.SoundEnum)]
    public int key;
    public AudioClip clip;
    public float volume = 1;
    public float delay;
}


[Serializable]
[CreateAssetMenu(fileName = "SoundData.asset", menuName = "Duong/SoundPlayData")]
public class SoundPlayData : ScriptableObject
{
    [SerializeField] private SoundDataItem[] data;

    public SoundDataItem[] Data => data;
}