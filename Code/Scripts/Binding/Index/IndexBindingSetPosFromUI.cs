using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndexBindingSetPosFromUI : IndexBindingScript
{
    [SerializeField] private Vector3 adjust;
    [SerializeField] private Transform[] targets;
    protected override void OnValueChanged(int computedValue)
    {
        base.OnValueChanged(computedValue);
        if (computedValue < 0 || computedValue >= targets.Length)
        {
            gameObject.SetActive(false);
        }
        else
        {
            gameObject.SetActive(true);
            transform.position = targets[computedValue].position + adjust;
        }
        
    }

    
}
