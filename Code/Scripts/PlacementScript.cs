using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacementScript : BaseFrameworkView
{
    [IndexAsEnum(BuiltInEnumType.LogPlacement)]
    [SerializeField] private int placement;
    [SerializeField] private int level;

    [SerializeField] private BoolBool enableDisable;
    private void OnEnable()
    {
        if(enableDisable.value)
            BaseGameController.OnPlacement(placement,level,true);
    }
    
    private void OnDisable()
    {
        if(enableDisable.value2)
            BaseGameController.OnPlacement(placement,level,false);
    }
}
