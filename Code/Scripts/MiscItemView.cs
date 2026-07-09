using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct MiscItemPoolDescriptor
{
    [IndexAsEnum(BuiltInEnumType.MiscEnum)]
    public int key;
    public MiscItemView prefab;
    public int prePool;
}

public class MiscItemView : ParameterOnRentPoolItem
{
    [IndexAsEnum(BuiltInEnumType.MiscEnum)]
    [SerializeField] protected int index;
    public override int Index => index;
  
    public override void SetParams(Point3Float para)
    {
        
    }
}
