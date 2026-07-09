using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum FlagState
{
    QuestState,
    GameState,
    LevelSelectState,
}

public class IndexBindingButtonState : IndexBindingScript
{

    [SerializeField]
    private Button btn;
    [SerializeField] private FlagState flagStateEnum;

    [SerializeField]
    [IndexAsEnumFlagCondition(
        new object[]
        {
            FlagState.QuestState,
            FlagState.GameState,
            FlagState.LevelSelectState,
        },
        new[]
        {
            nameof(QuestState),
            "GameState",
            nameof(LevelSelectState),
        },nameof(flagStateEnum))]
    private int inactiveFlag;
    
    [SerializeField]
    [IndexAsEnumFlagCondition(
        new object[]
        {
            FlagState.QuestState,
            FlagState.GameState,
            FlagState.LevelSelectState,
        },
        new[]
        {
            nameof(QuestState),
            "GameState",
            nameof(LevelSelectState),
        },nameof(flagStateEnum))]
    private int disableFlag;
//    
    [SerializeField]
    [IndexAsEnumFlagCondition(
        new object[]
        {
            FlagState.QuestState,
            FlagState.GameState,
            FlagState.LevelSelectState,
        },
        new[]
        {
            nameof(QuestState),
            "GameState",
            nameof(LevelSelectState),
        },nameof(flagStateEnum))]
    private int otherToggleFlag;

    
    [SerializeField] private ToggleScript[] toggleOthers;

    protected override void OnValueChanged(int computedValue)
    {
        base.OnValueChanged(computedValue);
        if((inactiveFlag & computedValue) != 0)
            btn.gameObject.SetActive(false);
        else
        {
            btn.gameObject.SetActive(true);
            if ((disableFlag & computedValue) != 0)
                btn.interactable = false;
            else 
                btn.interactable = true;
        }
        
        toggleOthers.OnChanged((disableFlag & computedValue) != 0);
    }
}
