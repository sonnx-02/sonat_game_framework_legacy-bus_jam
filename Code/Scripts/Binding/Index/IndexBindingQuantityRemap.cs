using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class IndexBindingQuantityRemap : IndexBindingScript
{
    [SerializeField] private Quantity[] quantities;
    [SerializeField] private IndexBindingScript[] remapInvoke;

    public override void OnChanged(int index)
    {
        base.OnChanged(index);
        for (var i = 0; i < quantities.Length; i++)
            if (quantities[i] == (Quantity) index)
            {
                remapInvoke.OnChanged(i);
                break;
            }
    }

    protected override void LoadComponent()
    {
    }
}