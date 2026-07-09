using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ConditionFloatArray
{
    [SerializeField] private ValidScriptBase valid;
    [SerializeField] float[] parameters;

    public float[] Valid()
    {
        if (valid.IsValid())
            return parameters;
        return null;
    }
}

public class ParameterScriptFloat : ParameterScriptFloatBase
{
    private enum ParamType
    {
        Independent,
        Increment,
        RemoteConfigKey,
        Random,
    }

    [SerializeField] private ParamType paramType;
    [SerializeField] private ConditionFloatArray[] conditions;
    [SerializeField] private float[] parameters;
    [ConditionalField(nameof(paramType),new object[]{ParamType.Random})]
    [SerializeField] private Vector2 random;
    [Tooltip("if null use parameters 0")]
    [SerializeField] private string remoteConfigKey;


    public override float GetValue(int i)
    {
        var par = Parameters();
        switch (paramType)
        {
            case ParamType.Independent:
                return par[i];
            case ParamType.Increment:
                float p = 0;
                for (int j = 0; j <= i; j++)
                    p += par[j];
                return p;
            case ParamType.RemoteConfigKey:
                if (string.IsNullOrEmpty(remoteConfigKey))
                    return par[0];
                return (float)RemoteConfigHelper.GetValueDouble(remoteConfigKey);
            case ParamType.Random:
                return random.GetRandom();
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private float[] Parameters()
    {
        foreach (var conditionFloatArray in conditions)
        {
            var p = conditionFloatArray.Valid();
            if (p != null)
                return p;
        }

        return parameters;
    }
}
