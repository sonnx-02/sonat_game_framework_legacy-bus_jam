using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class IndexBindingLevelState : IndexBindingFlagState
{
    [SerializeField] private LevelSelectState activeState;
    [SerializeField] private LevelSelectState exceptState;
    protected override int FlagState => (int) activeState;
    protected override int FlagStateExcept => (int) exceptState;

    
}