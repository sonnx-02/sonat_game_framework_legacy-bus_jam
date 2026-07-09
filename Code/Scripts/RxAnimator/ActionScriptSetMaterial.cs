using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionScriptSetMaterial : BaseBaseActionScript
{
    [SerializeField] SkinnedMeshRenderer skinnedMeshRenderer;
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] ParticleSystem ps;
    [SerializeField] private Material mat;


    protected virtual void LoadComponent()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        skinnedMeshRenderer = GetComponent<SkinnedMeshRenderer>();
        ps = GetComponent<ParticleSystem>();
    }

    protected virtual void Reset()
    {
        LoadComponent();
    }
    
    protected override void DoAction()
    {
        base.DoAction();
      
        if (meshRenderer != null)
        {
            meshRenderer.sharedMaterial = mat;
        }

        if (ps != null)
        {
            ps.GetComponent<ParticleSystemRenderer>().material = mat;
        }

        if (skinnedMeshRenderer != null)
        {
            skinnedMeshRenderer.sharedMaterial = mat;
        }
    }
}
