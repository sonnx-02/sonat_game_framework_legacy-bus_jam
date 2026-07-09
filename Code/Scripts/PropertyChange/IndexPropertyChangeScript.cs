using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class IndexPropertyChangeScript : IndexPropertyChangeScriptBase
{
    [SerializeField] private int value;
    
    protected override int Value
    {
        get => value;
        set => this.value = value;
    }

    
}
