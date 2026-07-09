using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ValidScriptMath : ValidScriptBase
{
    [SerializeField] private MathCompareDataAction math;
    [SerializeField] private IntValueRetrieverBase intValueRetriever;
    protected override bool IsThisValid()
    {
        return math.CheckStartAction(intValueRetriever.Value);
    }
}
