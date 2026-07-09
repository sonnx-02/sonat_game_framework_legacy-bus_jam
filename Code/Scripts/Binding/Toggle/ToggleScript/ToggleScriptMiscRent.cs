using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleScriptMiscRent : ToggleScript
{
    [IndexAsEnum(BuiltInEnumType.MiscEnum)]
    [SerializeField] private int key;
    private PoolItem _rent;
    private bool _lastValue;

    protected override void LoadComponent()
    {
        
    }

    protected override void Trigger(bool val)
    {
        if (_lastValue != val)
        {
            if (val)
                _rent = RootView.rootView.poolManager.miscs[key].Rent(transform, Vector3.zero);
            else
                _rent.Return();

            _lastValue = val;
        }
    }
}