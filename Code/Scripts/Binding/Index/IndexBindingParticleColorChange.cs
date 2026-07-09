using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndexBindingParticleColorChange : IndexBindingScript
{
    // ReSharper disable once InconsistentNaming
    [SerializeField] private ParticleSystem _particleSystem;

    [SerializeField] private  Color[] colors;
    [SerializeField] private bool clampMode;
    protected override void OnValueChanged(int computedValue)
    {
        var main = _particleSystem.main;
        main.startColor = colors[clampMode ? computedValue : computedValue % colors.Length];
    }

    protected override void LoadComponent()
    {
        _particleSystem = GetComponent<ParticleSystem>();
    }
}
