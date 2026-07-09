using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySoundScript : MonoBehaviour
{
    [SerializeField] [IndexAsEnum(BuiltInEnumType.SoundEnum)] private int sound;

    [SerializeField] private bool playOnEnable;
    [SerializeField] private float delay;

    private void OnEnable()
    {
        if(playOnEnable)
            sound.PlaySound(delay);
    }

    public void Play()
    {
        sound.PlaySound();
    }
}
