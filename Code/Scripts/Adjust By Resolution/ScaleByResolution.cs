using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;


public class ScaleByResolution : MonoBehaviour
{
    [SerializeField] private bool disable;
     public List<ScaleAndAnchorByResolutionCondition> conditions = new List<ScaleAndAnchorByResolutionCondition>();

    
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
        if(!disable)
        transform.localScale = condition.scale;
    }

    public void SetPos()
    {
        Apply(NearestCondition(1));
    }

    public ScaleAndAnchorByResolutionCondition NearestCondition(float epsilon = 0.001f)
    {
        var cam = Camera.main;
        if (cam == null)
            cam = RootView.rootView.gameController.cam;
        var aspect = cam.pixelWidth * 1f / cam.pixelHeight;

        if (conditions.Count == 0)
            return null;

        var condition = conditions[0];
        for (int i = 1; i < conditions.Count; i++)
            if (Mathf.Abs(conditions[i].aspect - aspect) < Mathf.Abs(condition.aspect - aspect))
                condition = conditions[i];

        if(Mathf.Abs(aspect - condition.aspect) < epsilon || aspect >= 1)
            return condition;
        return null;
    }

}
