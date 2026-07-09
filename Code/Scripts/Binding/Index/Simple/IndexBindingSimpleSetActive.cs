using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndexBindingSimpleSetActive : IndexBindingScriptBase
{
    [SerializeField] private bool reverse;
    [SerializeField] private int activeValue;
    public override void OnChanged(int value) => gameObject.SetActive(reverse ? value != activeValue : value == activeValue);
}
