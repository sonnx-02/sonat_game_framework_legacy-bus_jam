using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class ToggleScriptChangeMaterial : ToggleScript
{
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private MeshRenderer[] othersRenderers;
    [SerializeField] private Material onMaterial;
    [SerializeField] private Material offMaterial;

    protected override void LoadComponent()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }

    protected override void Trigger(bool val)
    {
        if(meshRenderer != null)
            meshRenderer.sharedMaterial = val ? onMaterial : offMaterial;
        foreach (var othersRenderer in othersRenderers)
            othersRenderer.sharedMaterial = val ? onMaterial : offMaterial;
    }
}