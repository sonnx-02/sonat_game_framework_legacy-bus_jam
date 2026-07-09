using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class IndexBindingQuestState : IndexBindingFlagState
{
    [SerializeField] private QuestState activeState;
    protected override int FlagState => (int) activeState;

    
}