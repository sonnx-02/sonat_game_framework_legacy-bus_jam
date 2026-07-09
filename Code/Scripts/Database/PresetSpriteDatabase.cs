using System;
using System.Collections.Generic;
using System.Linq;
using TMPro.SpriteAssetUtilities;
using UnityEngine;
using UnityEngine.Serialization;



[Serializable]
[CreateAssetMenu(fileName = "PresetSpriteDatabase.asset", menuName = "Duong/PresetSpriteDatabase")]
public class PresetSpriteDatabase : ScriptableObject
{
    [SerializeField] private Sprite[] sprites;
    public Sprite this[int index] => sprites.InRange(index) ? sprites[index] : null;

    public Sprite[] Sprites => sprites;
}