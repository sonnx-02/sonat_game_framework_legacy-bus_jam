using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialPropertySetterToggleScript : ToggleScript
{
    [SerializeField] private string nameOfP;
    
    [SerializeField] private int onValue = 0;
    [SerializeField] private int offValue = 100;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private SpriteRenderer[] otherRenderers;
    [SerializeField] private Material grayMaterial;
    [SerializeField] private Material defaultMaterial;
  
    
    [MyButtonInt(nameof(SetMaterial100))] public int abc;
    public void SetMaterial100(int a)
    {
        spriteRenderer.material.SetFloat(nameOfP,a / 100f);
    }

    
    protected override void LoadComponent()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    protected override void Trigger(bool isOn)
    {
        if (!isOn)
        {
            spriteRenderer.material = grayMaterial;
            foreach (var otherRenderer in otherRenderers)
                otherRenderer.material = grayMaterial;
        }
        else
        {
            spriteRenderer.sharedMaterial = defaultMaterial;
            foreach (var otherRenderer in otherRenderers)
                otherRenderer.material = defaultMaterial;
        }
        SetMaterial100(isOn ? onValue : offValue);
    }
}