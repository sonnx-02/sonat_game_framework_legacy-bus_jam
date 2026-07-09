using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;


public class ToggleScriptSelectItemOld : ToggleScript
{ 
    [DisplayAsDisable] [SerializeField] 
    private int index = 0;
    [SerializeField] private BlockTweenDataSet[] dataSet;
    
    [SerializeField]
    private Vector3 localPos = new Vector3(0, 0, 0);
    private float _t = 0f;
    [SerializeField] private float speed = 1f;
    [SerializeField] private float maxTime = 1.5f;

    public BlockTweenDataSet[] DataSet => dataSet;
    
    protected override void LoadComponent()
    {
    }
    
    
    public void SetIndex(int i)
    {
        index = i;
    }
    
    public void SetAddLocalPos(Vector3 i)
    {
        localPos = i;
    }
    

    [DisplayAsDisable] [SerializeField] public bool busy;
    private IDisposable _tween;

    [SerializeField] private bool useLast;
    private bool _last;
    protected override void Trigger(bool val)
    {
        if (useLast)
        {
            if (val != _last)
                _last = val;
            else
                return;
        }
        if (custom)
        {
            _tween?.Dispose();
            _t = val ? 1f : 0;
            Set();
            busy = false;
        }
        else
        {
            if (!busy)
            {
                _tween?.Dispose();
                _tween = Observable.FromCoroutine(Tween).Subscribe();
            }
        }
    }

    private void Set()
    {
        Set(_t);
    }
    
    public void Set(float t)
    {
        transform.localPosition = Vector3.Lerp(dataSet[index].offLocalPos, dataSet[index].onLocalPos + localPos, t);
        transform.localScale = Vector3.Lerp(dataSet[index].offScale, dataSet[index].onScale, t);
    }

    IEnumerator Tween()
    {
        busy = true;

        _t = CurrentValue ? 0 : 1;
        var length = transform.localPosition.Magnitude2();
        var s = speed;
        if (length > 0)
        {
            s = s / length;
        }

        if (s < maxTime)
            s = maxTime;
        Set();
        yield return null;
        while (CurrentValue ? (_t < 1f) : (_t > 0f))
        {
            var d = Time.deltaTime * s;
            _t += CurrentValue ? d : -d;
            if (Mathf.Abs(_t) >= 1)
            {
                _t = CurrentValue ? 1 : 0;
            }

           
            Set();
            yield return null;
        }
        if(_t <= 0.01f)
            SetAddLocalPos(Vector3.zero);
        busy = false;
    }

    public void Release()
    {
        _tween?.Dispose();
        var transform1 = transform;
        transform1.localScale = Vector3.one;
        transform1.localPosition = Vector3.zero;
    }
}