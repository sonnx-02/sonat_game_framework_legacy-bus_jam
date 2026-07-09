using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[System.Serializable]
public struct LevelStatePair
{
    public StateCompareType compareType;
    public LevelSelectState active;
    public LevelSelectState except;
}

public class IndexBindingLevelStateConditionCorrespond : IndexBindingScript
{
    [SerializeField] private IndexBindingScript[] correspondBindings;// correspond with conditions index
    [SerializeField] private IndexBindingScript[] correspondSetIndex2; // correspond with conditions index
    [SerializeField] private int defaultNotFound; // default correspond if conditions not found
    [SerializeField] private int defaultNotFoundIndex2;// default correspond 2 if conditions not found 
    [SerializeField] private LevelStatePair[] conditions;// correspond = condition

    [SerializeField] private bool activeIfCondition0;

    
    
  

   
    [DisplayAsDisable]
    [SerializeField] private LevelSelectState state;
    protected override void OnValueChanged(int computedValue)
    {
        state = (LevelSelectState)computedValue;
        base.OnValueChanged(computedValue);
        bool found = false;
        for (var i = 0; i < conditions.Length; i++)
        {
            bool valid;
            switch (conditions[i].compareType)
            {
                case StateCompareType.HasKey:
                    valid = (computedValue & (int)conditions[i].active) != 0;
                    break;
                case StateCompareType.Exact:
                    valid = computedValue == (int) conditions[i].active;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            var valid2 = (int)conditions[i].except == 0 || (computedValue & (int)conditions[i].except) == 0;
            if (valid && valid2)
            {
                correspondBindings.OnChanged(i);
                foreach (var script in correspondSetIndex2)
                    script.SetIndex2(i);
                found = true;
                if (activeIfCondition0 && i ==0)
                    gameObject.SetActive(true);
                break;
                #if UNITY_EDITOR
                    if(!Application.isPlaying)
                        Debug.Log("Match at i");
                #endif
            }
            if (activeIfCondition0 && i ==0)
                gameObject.SetActive(false);
        }

        if (!found)
        {
            correspondBindings.OnChanged(defaultNotFound);
            foreach (var script in correspondSetIndex2)
                script.SetIndex2(defaultNotFoundIndex2);
        }
    }

    [SerializeField] private LevelSelectState testState;

    [MyButton("TestState")]
    [SerializeField]
    private int testInvokeState;
    public void TestState()
    {
        bool found = false;
        for (var i = 0; i < conditions.Length; i++)
        {
            var valid = ((int)testState & (int)conditions[i].active) != 0;
            var valid2 = (int)conditions[i].except == 0 || ((int)testState & (int)conditions[i].except) == 0;
            if (valid && valid2)
            {
                Debug.Log("index"+i);
                correspondBindings.OnChanged(i);
                foreach (var script in correspondSetIndex2)
                    script.SetIndex2(i);
                
                found = true;
                break;
            }
        }

        if (!found)
        {
            Debug.Log("not found");
        }
    }
}
