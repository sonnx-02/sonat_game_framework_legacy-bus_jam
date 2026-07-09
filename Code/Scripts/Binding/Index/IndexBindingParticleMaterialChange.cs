using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndexBindingParticleMaterialChange : IndexBindingScript
{
    // ReSharper disable once InconsistentNaming
    [SerializeField] private ParticleSystem _particleSystem;

    [SerializeField] private  Material[] materials;
    [SerializeField] private bool clampMode;

    private ParticleSystemRenderer _rend;
    protected override void OnValueChanged(int computedValue)
    {
        if (_rend == null)
             _rend = _particleSystem.GetComponent<ParticleSystemRenderer>();
        _rend.sharedMaterial = materials[clampMode ? computedValue : computedValue % materials.Length];
    }

    protected override void LoadComponent()
    {
        _particleSystem = GetComponent<ParticleSystem>();
    }

    protected override bool IsNoComponent()
    {
        return _particleSystem == null;
    }
}
