using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

[Serializable]
public class BlockTweenDataSet
{
    public Vector3 offScale = Vector3.zero;
    public Vector3 onScale = Vector3.one;
    public Vector3 offLocalPos = Vector3.zero;
    public Vector3 onLocalPos = new Vector3(0, 2, 0);
}

public class ToggleScriptSelectItem : ToggleScript
{ 
    [DisplayAsDisable] [SerializeField] 
    private int index = 0;
    [SerializeField] private BlockTweenDataSet[] dataSet;
    [SerializeField] private float motionSpeed = 20f;
    [SerializeField] private Vector3 localPos = new Vector3(0, 0, 0);
    public BlockTweenDataSet[] DataSet => dataSet;

    protected override void LoadComponent()
    {
    
    }
    
    public void SetIndex(int i)
    {
        index = i;
    }
    
    public void SetAddLocalPos(Vector3 add,bool reset)
    {
        if (reset)
            localPos = add;
        else
            localPos += add;
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
        
        if (custom) // custom is instant
        {
            _tween?.Dispose();
            busy = false;
        }
        else
        {
            busy = true;
            _tween?.Dispose();

            var transform1 = transform;
            Vector3 from = transform1.localPosition;
            Vector3 to = val ? dataSet[index].onLocalPos + localPos : dataSet[index].offLocalPos;
            Vector3 fromScale = transform1.localScale;
            Vector3 toScale = val ? dataSet[index].onScale  : dataSet[index].offScale;

            MyIEnumeratorExtensions.IoValue(0, 1, motionSpeed, x =>
                {
                    transform.localPosition = Vector3.Lerp(from, to, x);
                    transform.localScale = Vector3.Lerp(fromScale, toScale, x);
                })
                .TakeUntilDisable(this)
                .Subscribe(data => { busy = false; });
        }
    }

    
    public void Release()
    {
        _last = false;
        busy = false;
        _tween?.Dispose();
        var transform1 = transform;
        transform1.localScale = dataSet[index].offScale;
        transform1.localPosition = dataSet[index].offLocalPos;
    }
}