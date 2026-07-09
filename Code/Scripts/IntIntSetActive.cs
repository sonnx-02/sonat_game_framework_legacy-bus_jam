using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class IntIntSetActive
{
    public int value;
    public int value2;
    public bool reverse;
    public GameObject go;
    public ToggleScript toggle;

    public void OnChanged(int val, int val2)
    {
        var valid = (value == val || value <= -999) && (value2 == val2 || value2 <= -999);
        if (reverse)
            valid = !valid;
        if(go != null)
            go.SetActive(valid);
        if(toggle != null)
            toggle.OnChanged(valid);
    }
    
    public void OnChanged(ArrayInt arrayInt)
    {
        if(arrayInt == null || arrayInt.values.Length < 2)
            return;
        OnChanged(arrayInt.values[0],arrayInt.values[1]);
    }
}
