using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ArrayIntHandler
{
    public int index;
    public IndexBindingScriptBase[] binds;

    public void Handler(int[] arrayInt)
    {
        if (arrayInt != null && (index < 0 || index >= arrayInt.Length-1))
            foreach (var indexBindingScript in binds)
                indexBindingScript.OnChanged(arrayInt[index]);
    }
}

public class ArrayIntSplitScript : BaseFrameworkView
{
    [SerializeField] private ArrayIntHandler[] handlers;
    public void OnChanged(int[] arrayInt)
    {
        foreach (var arrayIntHandler in handlers)
        {
            arrayIntHandler.Handler(arrayInt);
        }
    }
}
