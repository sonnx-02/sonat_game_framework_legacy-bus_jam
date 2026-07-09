using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntValueRetrieverDummy : IntValueRetrieverBase
{
    [SerializeField] private int[] values;
    public override int this[int index] => values[index];

    public override int Length => values.Length;
}
