using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleScriptRentEffect : ToggleScript
{
    
    [SerializeField] private Transform parentRent;

    [IndexAsEnum(BuiltInEnumType.EffectEnum)] [SerializeField]
    private int effect;

    [SerializeField] private int setIndex = -1;
    private EffectView _effectView;

    protected override void LoadComponent()
    {
        parentRent = transform;
    }

    protected override void Trigger(bool val)
    {
        if(disable)
            return;
        if (val)
        {
            if (parentRent != null)
                _effectView = RootView.rootView.poolManager.effects[effect].Rent(parentRent, Vector3.zero);
            else
                _effectView = RootView.rootView.poolManager.effects[effect].RentWorld(null, transform.position);
            
            if(setIndex >= 0)
                _effectView.SetIndex(setIndex);
        }
    }
}