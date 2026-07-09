using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleScriptPlaySound : ToggleScript
{
    [SerializeField] [IndexAsEnum(BuiltInEnumType.SoundEnum)] private int soundOn;
    [SerializeField] [IndexAsEnum(BuiltInEnumType.SoundEnum)] private int soundOff;
    [SerializeField]  private float delayOn;
    [SerializeField]  private float delayOff;
    

    // Start is called before the first frame update
    protected override void LoadComponent()
    {
    }

    protected override void Trigger(bool val)
    {
        if (val)
            soundOn.PlaySound(delayOn);
        else
            soundOff.PlaySound(delayOff);
    }
}
