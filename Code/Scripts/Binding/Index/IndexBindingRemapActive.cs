using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndexBindingRemapActive : IndexBindingScript
{
    [SerializeField] private GameObject[] corresponds;
    [SerializeField] private int remapAdd;
    protected override void OnValueChanged(int computedValue)
    {
        base.OnValueChanged(computedValue);
        for (var i = 0; i < corresponds.Length; i++)
        {
            if(corresponds[i] != null)
                corresponds[i].SetActive(i + remapAdd< computedValue);
        }
    }
}
