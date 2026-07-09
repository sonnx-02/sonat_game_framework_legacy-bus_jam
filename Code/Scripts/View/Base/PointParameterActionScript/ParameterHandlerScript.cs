using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ParameterHandlerScript : BaseActionScript
{
    [SerializeField] protected PointParameter parameters;
    
#if UNITY_EDITOR
    [SplitHeader] [SerializeField]
    private string split1;
#endif
    public void SetParameter(PointParameter pointParameter)
    {
        parameters = pointParameter;
    }
}
