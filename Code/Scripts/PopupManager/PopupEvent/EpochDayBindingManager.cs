using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct EpochBinding
{
    public string key;
    public Vector2Int dateRange;
}

public class EpochDayBindingManager : BaseFrameworkView
{
    public EpochBinding[] data;
    public IndexBindingScriptBase[] bindings;

    protected override void Register()
    {
        base.Register();
        var currentEpochDate = DuongExtensions.GetEpochDayOfYear();

        bool find = false;
        for (var index = data.Length - 1; index >= 0; index--)
            if (data[index].dateRange.InRange(currentEpochDate))
            {
                bindings.OnChanged(index);
//                foreach (var indexBindingScript in bindings)
//                    Debug.LogError(indexBindingScript.gameObject + " "+index);
                find = true;
                break;
            }

        if (!find)
            bindings.OnChanged(0);
    }
}
