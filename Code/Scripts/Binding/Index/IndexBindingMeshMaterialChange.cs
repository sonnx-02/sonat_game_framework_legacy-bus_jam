using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class IndexBindingMeshMaterialChange : IndexBindingScript
{
    [SerializeField] SkinnedMeshRenderer meshRenderer;
    [SerializeField] MeshRenderer meshRenderer2;
    [SerializeField] private Material[] materials;

    protected override void LoadComponent()
    {
        meshRenderer = GetComponent<SkinnedMeshRenderer>();
        meshRenderer2 = GetComponent<MeshRenderer>();
    }

    protected override bool IsNoComponent()
    {
        return meshRenderer == null;
    }

    protected override void OnValueChanged(int computedValue)
    {
        base.OnValueChanged(computedValue);
        if (computedValue >= materials.Length || computedValue < 0)
        {
            Debug.LogError("out range" + computedValue);
            return;
        }
        if(meshRenderer != null)
            meshRenderer.sharedMaterial = materials[computedValue];
        if(meshRenderer2 != null)
            meshRenderer2.sharedMaterial = materials[computedValue];
    }
    
//    [MyButton(new [] {"LoadOther","LoadOther2"})]
//    [MyButton("LoadOther","LoadOther2")]
       //    public bool testLoad;
       //    public void LoadOther()
       //    {
       //        var load = GetComponents<IndexBindingScript>().ToList();
       //        load.Remove(this);
       //        others = load.ToArray();
       //    }
       //    public void LoadOther2()
       //    {
       //        var load = GetComponents<IndexBindingScript>().ToList();
       //        load.Remove(this);
       //        othersComputed = load.ToArray();
       //    }
}
