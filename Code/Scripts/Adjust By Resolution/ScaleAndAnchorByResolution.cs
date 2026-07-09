using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[System.Serializable]
public class ScaleAndAnchorByResolutionCondition
{
     public float aspect;
     public Vector3 scale;
}


[RequireComponent(typeof(RectTransform))]
public class ScaleAndAnchorByResolution : MonoBehaviour
{
     public List<ScaleAndAnchorByResolutionCondition> conditions = new List<ScaleAndAnchorByResolutionCondition>();
     public bool anchor = true;

    
#if UNITY_EDITOR
    [MyButton(nameof(AddCurrent), nameof(Apply))]
    public int testFunction3;

    public void Apply()
    {
        SetPos();
    }

    public void AddCurrent()
    {
        var condition = NearestCondition();
        if (condition == null)
        {
            conditions.Add(new ScaleAndAnchorByResolutionCondition()
            {
                aspect = Camera.main.pixelWidth * 1f / Camera.main.pixelHeight,
                scale = transform.localScale
            });
        }
        else
        {
            condition.scale = transform.localScale;
        }
    }
#endif
    
    private void Awake()
    {
        SetPos();
    }

    void Apply(ScaleAndAnchorByResolutionCondition condition)
    {
        GetComponent<RectTransform>().localScale = condition.scale;
        if(anchor)
            GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
    }

    public void SetPos()
    {
        Apply(NearestCondition(1));
    }

    public ScaleAndAnchorByResolutionCondition NearestCondition(float epsilon = 0.001f)
    {
        var aspect = Camera.main.pixelWidth * 1f / Camera.main.pixelHeight;

        if (conditions.Count == 0)
            return null;

        var condition = conditions[0];
        for (int i = 1; i < conditions.Count; i++)
            if (Mathf.Abs(conditions[i].aspect - aspect) < Mathf.Abs(condition.aspect - aspect))
                condition = conditions[i];

        if(Mathf.Abs(aspect - condition.aspect) < epsilon)
            return condition;
        else
            return null;
    }

}
