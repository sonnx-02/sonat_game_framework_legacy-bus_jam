using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.Events;

public class ActionScriptInvokeIndexBindingScript : BaseBaseActionScript
{
    [SerializeField] private InvokeIndexScriptType type;
    [SerializeField] private IndexBindingScriptBase[] scripts;

    [SerializeField] private IntBool defaultOnStart = new IntBool(0,true);
    private enum InvokeIndexScriptType
    {
        OnChanged,
        OnChanged2,
    }


    [SerializeField] private IntReactiveProperty current = new IntReactiveProperty();
    private void OnDisable()
    {
        if (KernelLoadedRegistered && defaultOnStart.value2)
        {
            current.Value = defaultOnStart.value;
            scripts.OnChanged(current.Value);
        }
    }

    public void Test2(int test)
    {
        scripts.OnChanged(test);
    }

    public void OnChangedAdd(int add)
    {
        StartAction(current.Value + add);
    }

    public override void StartAction(int index)
    {
        base.StartAction(index);
        current.Value = index;
        switch (type)
        {
            case InvokeIndexScriptType.OnChanged:
                scripts.OnChanged(index);
                break;
            case InvokeIndexScriptType.OnChanged2:
                foreach (var indexBindingScriptBase in scripts)
                    indexBindingScriptBase.OnChanged2(index);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}
