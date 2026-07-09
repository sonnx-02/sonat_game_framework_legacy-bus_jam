using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class ProgressLevelModel
{
    public int currentLevel;
    public int progressValue;
    public int[] conditions;

    public bool IsMaxLevel() =>  currentLevel >= conditions.Length ;
    public string GetFragmentProgressString() =>IsMaxLevel() ? "COMPLETE" : $"{GetFragmentProgressValue(currentLevel)}/{conditions[currentLevel]}";
    public float GetCurrentProgress() => IsMaxLevel()  ? 1 : GetFragmentProgressValue(currentLevel) * 1f / conditions[currentLevel];
    public int GetCurrentCondition() => conditions[Mathf.Min(currentLevel,conditions.Length-1)];
    public int GetCurrentProgressRemain() => IsMaxLevel() ? 0 : conditions[currentLevel] - GetFragmentProgressValue(currentLevel);
    public int GetCurrentProgressValue() => IsMaxLevel() ? 0 : GetFragmentProgressValue(currentLevel);
    

    private int GetFragmentProgressValue(int index)
    {
        var value = progressValue; 
        for (int i = 0; i < index; i++)
        {
            value -= conditions[i];
        }
        return value;
    }


    public int CalculateLevel()
    {
        currentLevel = 0;
        var value = progressValue;
        for (int i = 0; i < conditions.Length; i++)
        {
            if (value < conditions[i])
                break;
            currentLevel =  i + 1;
            value -= conditions[i];
        }

        return currentLevel;
    }

    public bool AddValue(int take)
    {
        return AddValue(take, GetCurrentCondition());
    }
    
    public bool AddValue(int take,int toCompare)
    {
        progressValue += take;
        if (GetCurrentProgressValue() >= toCompare)
        {
             CalculateLevel();
            return true;
        }

        return false;
    }
}