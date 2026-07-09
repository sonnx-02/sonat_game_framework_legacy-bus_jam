using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class IndexBindingMaterialChange : IndexBindingScript
{
    [SerializeField] MeshRenderer meshRenderer;
    [SerializeField] SkinnedMeshRenderer skinnedMeshRenderer;
    [SerializeField] ParticleSystem ps;
    [SerializeField] private Material[] materials;
    [SerializeField] private int[] otherIndex;
    protected override void LoadComponent()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        ps = GetComponent<ParticleSystem>();
    }
    
  

    protected override void OnValueChanged(int computedValue)
    {
        base.OnValueChanged(computedValue);
        if (meshRenderer != null)
        {
            meshRenderer.sharedMaterial = materials[computedValue];
            foreach (var i in otherIndex)
                meshRenderer.sharedMaterials[i] = materials[computedValue];
        }

        if (ps != null)
        {
            ps.GetComponent<ParticleSystemRenderer>().material = materials[computedValue];
            foreach (var i in otherIndex)
                ps.GetComponent<ParticleSystemRenderer>().sharedMaterials[i] = materials[computedValue];
        }

        if (skinnedMeshRenderer != null)
        {
            skinnedMeshRenderer.sharedMaterial = materials[computedValue];
            foreach (var i in otherIndex)
                skinnedMeshRenderer.sharedMaterials[i] = materials[computedValue];
        }
    }
    
    [MyButton("LoadOther1","LoadOther2")]
    public bool testLoad123;
    public void LoadOther1()
    {
        var load = GetComponents<IndexBindingScriptBase>().ToList();
        load.Remove(this);
        sharedSetting.others = load.ToArray();
    }
    public void LoadOther2()
    {
        var load = GetComponents<IndexBindingScriptBase>().ToList();
        load.Remove(this);
        sharedSetting.othersComputed = load.ToArray();
    }
}
