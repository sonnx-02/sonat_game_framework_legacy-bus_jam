using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CopyPosition : ToggleScript
{
    [SerializeField] private Transform transform1;
    [SerializeField] private Transform transform2;

    

    protected override void LoadComponent()
    {
        
    }

    protected override void Trigger(bool val)
    {
        if (val)
            transform2.position = transform1.position;
        else
            transform1.position = transform2.position;
    }
}
