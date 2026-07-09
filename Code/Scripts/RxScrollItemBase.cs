using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;


[RequireComponent(typeof(RectTransform))]
public class RxScrollItemBase : PoolItem
{
    [SerializeField] protected Button[] buttons;
    [SerializeField] protected IntReactiveProperty index;
    [SerializeField] protected IndexBindingScriptBase[] indexBindings = new IndexBindingScriptBase[0];
    [SerializeField] protected IndexBindingScriptBase[] stateBindings = new IndexBindingScriptBase[0];

    private int _poolIndex;

    protected int state;
    public virtual void SetState(int value)
    {
        state = value;
        stateBindings.OnChanged(state);
    }

    public override int Index => index.Value;

    public int PoolIndex => _poolIndex;

    private RectTransform _rt;

    public RectTransform RectTransform
    {
        get
        {
            if (_rt == null) _rt = transform as RectTransform;
            return _rt;
        }
    }

    public Button[] Buttons => buttons;

    protected override void Register()
    {
        base.Register();
        index.Subscribe(data =>
        {
            OnValueChanged();
            UpdatePos();
        });
    }

  


    private IReactiveScrollRect _parent;

    public bool Setup(IReactiveScrollRect parent)
    {
        if (_parent == null)
        {
            _parent = parent;
            index.Subscribe(data =>
            {
                OnValueChanged();
                UpdatePos();
            });
            return true;
        }

        return false;
    }

    public virtual void SetIndex(int i)
    {
        index.Value = i;
        OnValueChanged();
        UpdatePos();
    }

    public void SetPoolIndex(int i)
    {
        _poolIndex = i;
    }

    public virtual void OnValueChanged()
    {
        indexBindings.OnChanged(Index);
    }

    public void UpdatePos()
    {
        if(_parent != null)
            RectTransform.anchoredPosition = _parent.GetItemAnchorPos(index.Value);
    }
}