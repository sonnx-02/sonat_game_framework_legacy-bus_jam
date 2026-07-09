using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class IndexBindRules : MonoBehaviour
{
    [SerializeField] private int[] indexList; 
    [SerializeField] private int[] exceptList;

    public bool Valid(int inputIndex)
    {
        foreach (var i in exceptList)
        {
            if (inputIndex == i)
                return false;
        }
        foreach (var i in indexList)
        {
            if (inputIndex == i)
                return true;
        }

        return false;
    }
}
