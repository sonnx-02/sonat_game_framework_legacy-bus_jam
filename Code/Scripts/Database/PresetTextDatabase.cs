using System;
using System.Collections.Generic;
using System.Linq;
using TMPro.SpriteAssetUtilities;
using UnityEngine;
using UnityEngine.Serialization;



[Serializable]
[CreateAssetMenu(fileName = "PresetTextDatabase.asset", menuName = "Duong/PresetTextDatabase")]
public class PresetTextDatabase : ScriptableObject
{
    [SerializeField] private string[] texts;
    
    public string this[int index] => texts.InRange(index) ? texts[index] : "Not found";

}