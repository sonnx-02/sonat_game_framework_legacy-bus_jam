using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialSetter : BaseFrameworkView
{
    [SerializeField] private bool setOnRegister = true;
    [SerializeField] private bool sharedMaterial;
    [SerializeField] private string propertyName = "shader_property_name_here";
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private FlexibleCondition condition;
   
    private int _propertyId = -1;

    public void Reset()
    {
        if (meshRenderer == null)
            meshRenderer = GetComponent<MeshRenderer>();
    }

    public override void LogCustom()
    {
        base.LogCustom();
        Apply();
    }

    protected override void Register()
    {
        base.Register();
        _propertyId = Shader.PropertyToID(propertyName);
        if (setOnRegister)
            Apply();
    }
    
    public void Apply()
    {
        switch (condition.dataType)
        {
            case DataType.String:
//                meshRenderer.material.SetFloat(_propertyId, val);
                break;
            case DataType.Int:
                if(sharedMaterial)
                    meshRenderer.sharedMaterial.SetInteger(_propertyId, condition.valueInt);
                else
                    meshRenderer.material.SetInteger(_propertyId, condition.valueInt);
                break;
            case DataType.Float:
                if(sharedMaterial)
                    meshRenderer.material.SetFloat(_propertyId, condition.valueFloat);
                else
                    meshRenderer.material.SetFloat(_propertyId, condition.valueFloat);
                break;
            case DataType.Boolean:
                if(sharedMaterial)  
                    meshRenderer.material.SetInt(_propertyId, condition.valueBool ? 1 : 0);
                else
                    meshRenderer.material.SetInt(_propertyId, condition.valueBool ? 1 : 0);
                break;
            case DataType.Json:
                break;
            case DataType.Vector2:
                if(sharedMaterial) 
                    meshRenderer.material.SetVector(_propertyId,new Vector4( condition.valueFloat,condition.valueFloat2));
                else
                    meshRenderer.material.SetVector(_propertyId,new Vector4( condition.valueFloat,condition.valueFloat2));
                break;
            case DataType.Vector3:
                if(sharedMaterial)
                    meshRenderer.material.SetVector(_propertyId,new Vector4( condition.valueFloat,condition.valueFloat2,condition.valueFloat3));
                else
                    meshRenderer.material.SetVector(_propertyId,new Vector4( condition.valueFloat,condition.valueFloat2,condition.valueFloat3));
                break;
            case DataType.Point:
                break;
            case DataType.Point3:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

}
