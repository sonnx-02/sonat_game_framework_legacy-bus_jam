using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndexBindingGameState : IndexBindingFlagState
{
    [IndexAsEnumFlag(BuiltInEnumType.GameState)] [SerializeField] private int activeState;
    protected override int FlagState => (int) activeState;

    
}
