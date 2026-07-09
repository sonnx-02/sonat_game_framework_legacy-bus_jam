using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleScriptReturnItem : ToggleScript
{
    [SerializeField] private PoolItem poolItem;
    protected override void LoadComponent()
    {
        poolItem = GetComponent<PoolItem>();
    }

 

    protected override void Trigger(bool val)
    {
        if(val)
            poolItem.Destroy();
        else
            poolItem.Return();
    }
}
