using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class ToggleScriptPlayAudioSource : ToggleScript
{
    [SerializeField] private AudioSource _audioSource;

    void Reset()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    void Start()
    {
        
    }
    
    protected override void LoadComponent()
    {
        
    }

    enum State
    {
        Playing,
        Stopped,
        NotPlayed
    }

    private State _state;
    private void OnEnable()
    {
        _state = State.NotPlayed;
        BaseFrameworkView.PlayerData.isSoundEnable.TakeUntilDisable(this).Subscribe(data =>
        {
            if (data == 0)
            {
                _audioSource.Pause();
                _state = State.NotPlayed;
            }
            else if (_last)
                Trigger(_last);
        });
        
        BaseView.Pause.TakeUntilDisable(this).Subscribe(data =>
        {
            if (data)
            {
                _audioSource.Pause();
                _state = State.NotPlayed;
            }
            else if (_last)
                Trigger(_last);
        });
    }
    
    private void OnDisable()
    {
        Trigger(false);
    }

    private bool _last;
    protected override void Trigger(bool val)
    {
        _last = val;
        if (val)
        {
            if (BaseFrameworkView.PlayerData.isSoundEnable.BoolValue && !BaseView.Pause.Value)
            {
                _state = State.Playing;
                _audioSource.Play();
            }
        }
        else
        {
            _state = State.Stopped;
            _audioSource.Stop();
        }
    }
}
