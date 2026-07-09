using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public abstract class Point3FloatScript : MonoBehaviour
{
    [FormerlySerializedAs("autoStart")] public bool autoStartDefault;
    protected virtual void OnEnable()
    {
        if (autoStartDefault)
            SetDefault();
    }

    [SerializeField] private Point3Float defaultParams;
    [MyButton(nameof(SetDefault))] [SerializeField] private  int testSetDefault;
    public virtual void SetDefault()
    {
        SetParam(defaultParams);
    }
    
    public abstract void SetParam(Point3Float para);

}

