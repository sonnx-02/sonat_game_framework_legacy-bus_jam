using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class IndexBindingQuantity : IndexBindingEnumState
{
    public Quantity activeState;
    protected override int EnumState => (int) activeState;

    protected override void LoadComponent()
    {
    }
}