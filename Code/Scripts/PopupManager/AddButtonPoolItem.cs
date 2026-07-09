using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AddButtonPoolItem : PoolItem
{
    public Button onClick;
    public Button onRemove;
    public Button onApply;

    public IndexBindingScript[] bindings;
    public ToggleScript[] isCurrent;
    public ToggleScript[] isApplied;
    public int Index { get; set; }
 
    public void SetIndex(int i)
    {
        bindings.OnChanged(i);
        this.Index = i;
    }
}
