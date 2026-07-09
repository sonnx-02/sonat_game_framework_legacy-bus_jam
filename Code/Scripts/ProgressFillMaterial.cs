using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressFillMaterial : ProgressScript
{
    [SerializeField] private string propertyName = "shader_property_name_here";
    [SerializeField] private MeshRenderer meshRenderer;

    private int _propertyId = -1;

    protected override void Register()
    {
        base.Register();
        _propertyId = Shader.PropertyToID(propertyName);
    }

    [SerializeField] private bool debug;
    protected override void Trigger(float val)
    {
        if(debug)
            Debug.Log(val);
        CheckRegister();
        #if UNITY_EDITOR
        if(_propertyId == -1)
            Debug.LogError("check register"+gameObject,gameObject);
        #endif
        base.Trigger(val);
        if(meshRenderer == null)
            return;
        meshRenderer.material.SetFloat(_propertyId, val);
    }
}