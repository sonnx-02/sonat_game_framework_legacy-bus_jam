using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ParameterScriptFloatBase : MonoBehaviour,IDelayScript
{
    public abstract float GetValue(int i);
    public float GetDelay(int i = 0) => GetValue(i);
}
