using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleScriptPlayParticle : ToggleScript
{
    [SerializeField]
    private ParticleSystem ps;
    protected override void LoadComponent()
    {
        ps = GetComponent<ParticleSystem>();
    }

    protected override void Trigger(bool val)
    {
        if(val)
            ps.Play();
        else
            ps.Stop();
    }
}
