using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public abstract class IndexBindingFlagState : IndexBindingScript
{
    protected abstract int FlagState { get; }
    protected virtual int FlagStateExcept => 0;
    [SerializeField] private ToggleScript[] toggles;
    [SerializeField] private bool setActive = true;
    [SerializeField] private bool reverse;
    [SerializeField] private bool activeWhenZero; // for zero state still active
    [SerializeField] private bool containsAllState; // for zero state still active
    [SerializeField] private bool containsAllStateExcept; // for zero state still active

    [SerializeField] [IndexAsEnum(BuiltInEnumType.SoundEnum)]
    private IntFloat soundWhenOn;
//    [SerializeField]  private float delaySound;
    
    protected override void OnValueChanged(int computedValue)
    {
        base.OnValueChanged(computedValue);
        var validExcept = containsAllStateExcept
            ? FlagStateExcept == 0 || (computedValue & FlagStateExcept) == FlagStateExcept
            : FlagStateExcept == 0 || (computedValue & FlagStateExcept) == 0;

        var valid = containsAllState ? validExcept && (computedValue & FlagState) == FlagState || (activeWhenZero && computedValue == 0)
            : validExcept && (computedValue & FlagState) != 0 || (activeWhenZero && computedValue == 0);

        if (reverse)
            valid = !valid;
        if (setActive)
            gameObject.SetActive(valid);
        toggles.OnChanged(valid);
        
        if(valid)
            soundWhenOn.value.PlaySound(soundWhenOn.value2);
    }
}