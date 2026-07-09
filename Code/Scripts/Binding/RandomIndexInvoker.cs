using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RandomIndexInvoker : BaseFrameworkView
{
    [SerializeField] private bool onEnable;
    [SerializeField] private int[] values;
    [SerializeField] private IndexBindingScriptBase[] targets;


    private void OnEnable()
    {
        if (onEnable)
            InvokeRandom();
    }

    private void InvokeRandom()
    {
        if (values.Length > 0)
            targets.OnChanged(UnityEngine.Random.Range(0, values.Length));
    }
}