using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class IndexBindingProgressState : IndexBindingFlagState
{
    [SerializeField] private ProgressState activeState;
    protected override int FlagState => (int) activeState;

    
}