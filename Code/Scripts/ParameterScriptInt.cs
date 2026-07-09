using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParameterScriptInt : MonoBehaviour
{
    private enum ParamType
    {
        Independent,
        Increment,
    }

    [SerializeField] private ParamType paramType;
    [SerializeField] private int[] parameters;

    public int GetValue(int i)
    {
        switch (paramType)
        {
            case ParamType.Independent:
                return parameters[i];
            case ParamType.Increment:
                int p = 0;
                for (int j = 0; j <= i; j++)
                    p += parameters[j];
                return p;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}
