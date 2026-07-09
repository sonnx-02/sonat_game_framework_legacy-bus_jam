using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicPoolItem : PoolItem
{
    [SerializeField] private int index;
    public override int Index => index;
    [SerializeField] private IndexBindingScriptBase[] indexBindings;
    [SerializeField] private ToggleScript[] toggles;

    public void OnChanged(bool value) => toggles.OnChanged(value);
    public void OnChanged(int value) => indexBindings.OnChanged(value);

    [SerializeField] private BaseBaseActionScript[] actions;

    public BaseBaseActionScript[] Actions => actions;
}
