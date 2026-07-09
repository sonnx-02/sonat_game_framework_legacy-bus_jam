using System;
using System.Collections.Generic;
using System.Linq;
using TMPro.SpriteAssetUtilities;
using UnityEngine;
using UnityEngine.Serialization;



[Serializable]
[CreateAssetMenu(fileName = "PresetColorDatabase.asset", menuName = "Duong/PresetColorDatabase")]
public class PresetColorDatabase : ScriptableObject
{
    [SerializeField] private Color[] colors;
    public Color this[int index] => colors.InRange(index) ? colors[index] : Color.white;

    public Color[] Colors => colors;
}