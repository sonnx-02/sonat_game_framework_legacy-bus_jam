using System;
using System.Collections.Generic;
using System.Linq;
using TMPro.SpriteAssetUtilities;
using UnityEngine;
using UnityEngine.Serialization;


[Serializable]
[CreateAssetMenu(fileName = "BuilderDatabase.asset", menuName = "Duong/BuilderDatabase")]
public class BuilderDatabase : ScriptableObject
{
   public ArrayInt[] stages;
}