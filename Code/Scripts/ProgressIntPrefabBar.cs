using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressIntPrefabBar : ProgressScriptInt
{
    [SerializeField] private PoolItemGeneric poolItem;
    [SerializeField] private Transform container;
    private Pool<PoolItemGeneric> _pool;

    public override void SetTarget(int value)
    {
        if (_pool == null)
        {
            _pool = new Pool<PoolItemGeneric>(poolItem, container,-0.1f);
            _pool.ViewFirst(container);
        }

        _pool.ReturnAll();
        for (int i = 0; i < value; i++)
        {
            var rent = _pool.Rent();
            rent.TogglesIsLast.OnChanged(i == value - 1);
        }

        base.SetTarget(value);
    }

    protected override void Trigger(int val)
    {
        base.Trigger(val);
        if (_pool == null)
        {
            _pool = new Pool<PoolItemGeneric>(poolItem, container);
            _pool.ViewFirst(container);
        }
        
        for (var i = 0; i < _pool.ActiveItems.Count; i++)
        {
            _pool.ActiveItems[i].Toggles.OnChanged(val >= i);
        }
    }
}