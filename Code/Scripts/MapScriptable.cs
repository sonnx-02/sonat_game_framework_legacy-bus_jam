using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MapScriptable.asset", menuName = "Duong/MapScriptable")]
public class MapScriptable : ScriptableObject
{
   public MapListPoint map;
   public ListPoint[] items;
}
