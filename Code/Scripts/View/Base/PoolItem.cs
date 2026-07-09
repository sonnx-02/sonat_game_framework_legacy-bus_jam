using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;

// ReSharper disable UnassignedGetOnlyAutoProperty
// ReSharper disable PossibleNullReferenceException



public abstract class PoolItem : BaseFrameworkView
{
    public IPool pool;

    [IndexAsEnum(BuiltInEnumType.GlobalCallBackEnum)]
    [SerializeField] private IntIntInt callBackWhenReturn;

    public virtual int Index { get; }

    [Tooltip("To override rent Position")]
    [InLine(nameof(overridePositionAdjust),20)]
    public bool overridePosition;
    [HideInInspector] public Vector3 overridePositionAdjust;
#if UNITY_EDITOR
    [SplitHeader] [SerializeField]
    private string split1;
#endif

    public PointParameter Parameter => _parameter;
    private PointParameter _parameter; // to SetParameter for pool item or get
    // ReSharper disable once InconsistentNaming

    public PointParameter CallBackWhenReturn { get; set; }
    public virtual void OnRent()
    {
        Active = true;
        _parameter = null;
        CallBackWhenReturn = null;
    }

    public virtual void SetParameter(PointParameter param)
    {
        _parameter = param;
    }
    
    public virtual void SetReturnCallBack(int callBack,int param,int param2 = 0,int param3 = 0,int param4 = 0)
    {
        var point = new PointParameter(BuiltInEnumType.GlobalCallBackEnum,callBack);
        point.value = param;
        point.value2 = param2;
        point.value3 = param3;
        point.value4 = param4;
    }
    
    public virtual void OverrideCallbackValue(int param,int param2 = 0)
    {
        callBackWhenReturn.value = param;
        callBackWhenReturn.value2 = param2;
    }


    // for setup data then init something
    public virtual void AfterRent()
    {
    }

    private float _timeReturn;
    public bool Active { get; protected set; }

    protected virtual void InvokeReturnCallBack()
    {
        if (CallBackWhenReturn != null)
            SubjectController.OnPointParameterCallBack.OnNext(CallBackWhenReturn);
        if(callBackWhenReturn.value > 0)
            SubjectController.OnPointParameterCallBack.OnNext(
                new PointParameter(BuiltInEnumType.GlobalCallBackEnum,callBackWhenReturn.value,callBackWhenReturn.value2,callBackWhenReturn.value3));
    }
    

    public virtual void Return()
    {
        InvokeReturnCallBack();
        if (pool != null)
        {
            pool.Return(this);
            Deactivate();
            _timeReturn = Time.timeSinceLevelLoad;
        }
        else
            Destroy(gameObject);
    }

    public virtual void Deactivate()
    {
        gameObject.SetActive(false);
        Active = false;
    }

    public virtual void Destroy()
    {
        if (pool != null)
            pool.Destroy(this);
        else
            Destroy(gameObject);
    }

    public virtual void DestroyView()
    {
        Return();
    }

    // for active animation
    public virtual void DestroyView(int action)
    {
        if (action == 0)
            Return();
        else
            DestroyViewBy(action);
    }

    public virtual void DestroyViewBy(int action)
    {
    }

    public bool IsUnused()
    {
        return !Active;
    }

    public bool CheckRent(float minTime)
    {
        if (minTime <= 0)
            return true;
        return (Time.timeSinceLevelLoad - _timeReturn > minTime);
    }
}