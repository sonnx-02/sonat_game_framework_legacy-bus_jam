using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionScriptPlaySounds : BaseBaseActionScript
{
    [SerializeField] private SoundPlayData data;

    protected override void DoAction()
    {
        base.DoAction();
        foreach (var soundDataItem in data.Data)
        {
            if (soundDataItem.delay > 0)
                soundDataItem.delay.Timer(Play);
            else
                Play();

            void Play()
            {
                if (soundDataItem.clip != null)
                    RootView.rootView.audioManager.Play(soundDataItem.clip,soundDataItem.volume);
                else
                    soundDataItem.key.PlaySound();
            }
                
        }
       
    }
}