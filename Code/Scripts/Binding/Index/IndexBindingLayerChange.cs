using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndexBindingLayerChange : IndexBindingScript
{
    [SerializeField] private bool useName;
    [SerializeField] private string[] layerNames;
    [SerializeField] private int[] layers;
    [SerializeField] private GameObject[] others;
    [SerializeField] private bool allChild;
    protected override void OnValueChanged(int computedValue)
    {
        base.OnValueChanged(computedValue);
        if (useName)
        {
            gameObject.layer = LayerMask.NameToLayer(layerNames[computedValue]);
            foreach (var other in others)
                other.layer = gameObject.layer;
            if(allChild)
                for (int i = 0; i < transform.childCount; i++)
                    transform.GetChild(i).gameObject.layer =  gameObject.layer;
        }
        else
        {
            gameObject.layer = layers[computedValue];
            foreach (var other in others)
                other.layer = gameObject.layer;
            
            if(allChild)
                for (int i = 0; i < transform.childCount; i++)
                    transform.GetChild(i).gameObject.layer =  gameObject.layer;
        }
    }
}