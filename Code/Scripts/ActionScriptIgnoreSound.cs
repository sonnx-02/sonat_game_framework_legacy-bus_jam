using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionScriptIgnoreSound : BaseBaseActionScript
{
    [SerializeField] private bool clear;
    [IndexAsEnum(BuiltInEnumType.SoundEnum)]
    [SerializeField] private int[] sounds;
    protected override void DoAction()
    {
        base.DoAction();
        if(!clear)
            RootView.rootView.audioManager.ignoreSounds.AddRange(sounds);
        else
            RootView.rootView.audioManager.ClearIgnoreSounds();
    }
}
