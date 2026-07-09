using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntValueRetrieverRandom : IntValueRetrieverBase
{
    [SerializeField] private bool byCurrentLevel;
    [SerializeField] private Vector2Int[] randoms;

    public override int this[int index]
    {
        get
        {
            if(byCurrentLevel)
                return randoms[(Kernel.ParameterValueRetriever.LevelDisplay-1) % randoms.Length].GetRandom();
            return randoms[index % randoms.Length].GetRandom();
        }
    }

    public override int Length => randoms.Length;
}
