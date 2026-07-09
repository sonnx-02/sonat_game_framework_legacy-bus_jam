using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[System.Serializable]
public class PositionByResolutionCondition
{
    public float aspect;
    public MyEnumCondition condition;
    public Vector3 anchoredPosition;
}

public class AnchorPositionByResolution : ByResolution
{
    public List<PositionByResolutionCondition> conditions = new List<PositionByResolutionCondition>();


    private void Start()
    {
        if (conditions.Count > 0)
            Apply();
    }

    private void Apply(PositionByResolutionCondition condition)
    {
        var rt = GetComponent<RectTransform>();
        rt.anchoredPosition = condition.anchoredPosition;
    }

    public override void Apply()
    {
        var condition = conditions[0];
        for (int i = 1; i < conditions.Count; i++)
            if (Mathf.Abs(conditions[i].aspect - Aspect) < Mathf.Abs(condition.aspect - Aspect))
                condition = conditions[i];
        Apply(condition);

        foreach (var transformByResolution in others)
            transformByResolution.Apply();
    }

    public override void AddCurrent()
    {
        var rt = GetComponent<RectTransform>();
        var condition = NearestCondition();
        if (condition == null)
        {
            conditions.Add(new PositionByResolutionCondition()
            {
                aspect =Aspect,
                anchoredPosition = rt.anchoredPosition,
            });
        }
        else
            condition.anchoredPosition = rt.anchoredPosition;
        
        this.SetDirty();
    }

    public PositionByResolutionCondition NearestCondition(float epsilon = 0.001f)
    {
        if (conditions == null)
            conditions = new List<PositionByResolutionCondition>();
        if (conditions.Count == 0)
            return null;

        var condition = conditions[0];
        for (int i = 1; i < conditions.Count; i++)
            if (Mathf.Abs(conditions[i].aspect - Aspect) < Mathf.Abs(condition.aspect - Aspect))
                condition = conditions[i];

        if (Mathf.Abs(Aspect - condition.aspect) < epsilon)
            return condition;
        return null;
    }


    public void SetPos()
    {
        var rt = GetComponent<RectTransform>();
        var aspect = Camera.main.pixelWidth * 1f / Camera.main.pixelHeight;

        for (int i = 0; i < conditions.Count; i++)
        {
            var condition = conditions[i];
            if (condition.condition == MyEnumCondition.Less)
            {
                if (aspect < condition.aspect)
                {
                    rt.anchoredPosition = condition.anchoredPosition;
                    break;
                }
            }

            if (condition.condition == MyEnumCondition.LessOrEqual)
            {
                Debug.Log((aspect <= condition.aspect));
                if (aspect <= condition.aspect)
                {
                    rt.anchoredPosition = condition.anchoredPosition;
                    break;
                }
            }

            if (condition.condition == MyEnumCondition.Equal)
            {
                if (Math.Abs(aspect - condition.aspect) < double.Epsilon)
                {
                    rt.anchoredPosition = condition.anchoredPosition;
                    break;
                }
            }


            if (condition.condition == MyEnumCondition.GreaterOrEqual)
            {
                if (aspect >= condition.aspect)
                {
                    rt.anchoredPosition = condition.anchoredPosition;
                    break;
                }
            }

            if (condition.condition == MyEnumCondition.Greater)
            {
                if (aspect > condition.aspect)
                {
                    rt.anchoredPosition = condition.anchoredPosition;
                    break;
                }
            }
        }
    }
}