using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class IndexBindingScriptSetMaterialPropertyFloat : IndexBindingScript
{
    [SerializeField] private string propertyName = "shader_property_name_here";
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private Vector2Int source;
    [SerializeField] private Vector2 remapTo;
    [SerializeField] private BoolFloat animate;
    [SerializeField] private Ease ease = Ease.Linear;
    private int _propertyId = -1;

    protected override void Register()
    {
        base.Register();
        _propertyId = Shader.PropertyToID(propertyName);
    }

    private float _currentProperty;

    protected override void OnValueChanged(int computedValue)
    {
        base.OnValueChanged(computedValue);
#if UNITY_EDITOR
        if(!Application.isPlaying)
            _propertyId = Shader.PropertyToID(propertyName);
#endif
        base.OnValueChanged(computedValue);

        this.DOKill();
        if (!animate.value)
        {
            _currentProperty = computedValue;
            var val  = DuongExtensions.Remap(source,remapTo,computedValue);
            _currentProperty = val;
            meshRenderer.material.SetFloat(_propertyId, val);  
        }
        else
        {
            DOTween.To(() => _currentProperty, data =>
            {            
                var val  = DuongExtensions.Remap((float)source.x,(float)source.y,remapTo.x,remapTo.y,data);
                _currentProperty = data;
                meshRenderer.material.SetFloat(_propertyId, val);  
            }, (float) computedValue, animate.value2);
        }
    }

    public override void OnChanged2(int index)
    {
        base.OnChanged2(index);
        this.DOKill();
        _currentProperty = index;
        var val  = DuongExtensions.Remap(source,remapTo,index);
        _currentProperty = index;
        meshRenderer.material.SetFloat(_propertyId, val);
    }
}