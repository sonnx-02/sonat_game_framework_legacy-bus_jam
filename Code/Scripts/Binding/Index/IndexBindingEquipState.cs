using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class IndexBindingEquipState : IndexBindingFlagState
{
    [SerializeField] private UnlockEquipState activeState;
    [SerializeField] private UnlockEquipState exceptState;
    protected override int FlagState => (int) activeState;
    protected override int FlagStateExcept => (int) exceptState;

    
}