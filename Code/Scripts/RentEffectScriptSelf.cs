using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class RentEffectScriptSelf : RentEffectScriptBase
{
    [SerializeField] private PoolItem prefab;
    [SerializeField] private float poolCheckTime;
    private Pool<PoolItem> _pool;

    protected override void Register()
    {
        base.Register();
        _pool = new Pool<PoolItem>(prefab, parent, poolCheckTime);
    }

    [SerializeField] private bool localRotation = true;


    protected override void DoRent()
    {
        base.DoRent();

        if (rentWorldCurrentPos)
        {
            var rent = _pool.Rent(parent, Vector3.zero);
            if (localRotation)
                rent.transform.localRotation = Quaternion.identity;
        }
        else
        {
            var rent = _pool.RentWorld(null, parent != null ? parent.position : transform.position);
            if (localRotation)
                rent.transform.localRotation = Quaternion.identity;
        }
    }
}