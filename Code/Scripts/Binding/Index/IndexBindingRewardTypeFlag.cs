using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class IndexBindingRewardTypeFlag : IndexBindingFlagState
{
    [SerializeField] private RewardTypeFlag activeState;
    [SerializeField] private RewardTypeFlag exceptState;
    protected override int FlagState => (int) activeState;
    protected override int FlagStateExcept => (int) exceptState;

    
}