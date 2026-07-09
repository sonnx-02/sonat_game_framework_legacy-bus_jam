using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

public class IndexBindingPlaySound : IndexBindingScript
{
    [SerializeField] [IndexAsEnum(BuiltInEnumType.SoundEnum)] private int[] sounds;
    [SerializeField] private float delay;

    protected override void LoadComponent()
    {
    }
   

    protected override void OnValueChanged(int computedValue)
    {
        base.OnValueChanged(computedValue);
        sounds[computedValue].PlaySound(delay);
    }
}
