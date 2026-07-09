using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(ParticleSystem))]
public class ParticleSoundScript : MonoBehaviour
{
    private ParticleSystem _parentParticleSystem;

    [IndexAsEnum(BuiltInEnumType.SoundEnum)] [SerializeField]
    private int sound;
    [IndexAsEnum(BuiltInEnumType.SoundEnum)] [SerializeField]
    private int[] soundNext;
    
    [SerializeField] private int particleCountToPlaySound = 50;

    private void Start()
    {
        _parentParticleSystem = this.GetComponent<ParticleSystem>();
        if (_parentParticleSystem == null)
            Debug.LogError("Missing ParticleSystem!", this);
    }

    private int times = 0;
    private void OnEnable()
    {
        times = 0;
    }

    private bool _played;

    void Update()
    {
        var liveParticles = new ParticleSystem.Particle[_parentParticleSystem.particleCount];
        int current = _parentParticleSystem.GetParticles(liveParticles);

        if (_parentParticleSystem.particleCount > particleCountToPlaySound && !_played)
        {
            if (times == 0 || soundNext.Length == 0)
                sound.PlaySound();
            else 
                soundNext[times% soundNext.Length].PlaySound();
            _played = true;
            times++;
        }

        if (_parentParticleSystem.particleCount < particleCountToPlaySound && _played)
            _played = false;
    }
}