using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;


[System.Serializable]
public class DeltaByResolutionCondition
{
    public float aspect;
    public Vector2 size;
}


public class DeltaSizeByResolution : MonoBehaviour
{
    [SerializeField] private  List<DeltaByResolutionCondition> conditions;
//    [SerializeField] private ToggleScript[] triggerAfterSetPos;

#if UNITY_EDITOR
    [MyButton(nameof(AddCurrent), nameof(Apply))]
    [SerializeField] private  int testFunction;


    private string json;
// generic version:
 
   
    
    public void AddCurrent()
    {
        var condition = NearestCondition();
        if (condition == null)
        {
            conditions.Add(new DeltaByResolutionCondition()
            {
                aspect = Camera.main.pixelWidth * 1f / Camera.main.pixelHeight,
                size = GetComponent<RectTransform>().sizeDelta
            });
        }
        else
        {
            condition.size = GetComponent<RectTransform>().sizeDelta;
        }
    }
#endif
    
    public void Apply()
    {
        SetPos();
    }

    void Awake()
    {
        SetPos();
    }

    void Apply(DeltaByResolutionCondition condition)
    {
        GetComponent<RectTransform>().sizeDelta = condition.size;
    }

    public void SetPos()
    {
        var aspect = Camera.main.pixelWidth * 1f / Camera.main.pixelHeight;
        var condition = conditions[0];
        for (int i = 1; i < conditions.Count; i++)
            if (Mathf.Abs(conditions[i].aspect - aspect) < Mathf.Abs(condition.aspect - aspect))
                condition = conditions[i];
        Apply(condition);
        
//        foreach (var triggerAfterSetPo in triggerAfterSetPos)
//            triggerAfterSetPo.OnChanged(true);
    }

    public DeltaByResolutionCondition NearestCondition(float epsilon = 0.001f)
    {
        var aspect = Camera.main.pixelWidth * 1f / Camera.main.pixelHeight;

        if (conditions == null)
            conditions = new List<DeltaByResolutionCondition>();
        if (conditions.Count == 0)
            return null;

        var condition = conditions[0];
        for (int i = 1; i < conditions.Count; i++)
            if (Mathf.Abs(conditions[i].aspect - aspect) < Mathf.Abs(condition.aspect - aspect))
                condition = conditions[i];

        if (Mathf.Abs(aspect - condition.aspect) < epsilon)
            return condition;
        else
            return null;
    }
}