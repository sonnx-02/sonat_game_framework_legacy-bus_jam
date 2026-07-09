using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct TriggerAnimationItem
{
    public string name;
    public int value;
}

public class IndexBindingTriggerAnimation : IndexBindingScript
{
    [SerializeField] private  Animator animator;
    [SerializeField] private TriggerAnimationItem[] items;

    

  

    protected override void OnValueChanged(int computedValue)
    {
        base.OnValueChanged(computedValue);
        for (var i = 0; i < items.Length; i++)
        {
            bool active = items[i].value == computedValue;
            if (active)
            {
                animator.SetTrigger(items[i].name);
//                Debug.Log(items[i].name + "/"+computedValue);
            }
        }
    }
}