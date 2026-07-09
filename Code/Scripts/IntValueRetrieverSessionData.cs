using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntValueRetrieverSessionData : IntValueRetrieverBase
{
    [IndexAsEnum(BuiltInEnumType.CustomSessionDataProperty)]
    [SerializeField] private int[] keys;
    public override int Length => keys.Length;

    public override int this[int index] => keys[index].GetSessionDataCustomProperty().Value;
}
