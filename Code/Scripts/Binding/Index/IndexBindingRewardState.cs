using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class IndexBindingRewardState : IndexBindingFlagState
{
    [SerializeField] private RewardState activeState;
    protected override int FlagState => (int) activeState;

    

    protected override string GetName()
    {
        return base.GetName() + "_" + activeState;
    }
}