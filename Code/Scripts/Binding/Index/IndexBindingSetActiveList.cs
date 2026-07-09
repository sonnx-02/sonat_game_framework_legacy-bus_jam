using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public struct ActiveListItem
{
    public GameObject target;
    public GameObject[] targets;
    public int activeValue;
    public bool reverse;
}

public class IndexBindingSetActiveList : IndexBindingScript
{
    [SerializeField] private ActiveListItem[] items;
    
    protected override void OnValueChanged(int computedValue)
    {
        base.OnValueChanged(computedValue);
        for (var i = 0; i < items.Length; i++)
        {
            bool active = items[i].activeValue == computedValue;
            if(items[i].target != null)
                items[i].target.SetActive(items[i].reverse ? !active : active);
            items[i].targets.SetActiveAll(items[i].reverse ? !active : active);
        }
    }
}