using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleScriptEnableMesh : ToggleScript
{
    public MeshRenderer meshRenderer;
    protected override void Trigger(bool val)
    {
        meshRenderer.enabled = val;
    }
    
    protected override void LoadComponent()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }

}
