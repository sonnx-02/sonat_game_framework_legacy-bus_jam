using System.Collections;
using System.Collections.Generic;
using UniRx;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.UI;


//[RequireComponent(typeof(CanvasScaler))]
public class CanvasScalerByResolution : ByResolution
{
    [SerializeField] private List<SettingByResolution> conditions = new List<SettingByResolution>();
    [SerializeField] private CanvasScaler canvasScaler;
    [SerializeField] private bool applyOnAwake;
    [SerializeField] private ToggleScript[] copyPos;
    [SerializeField] private Paginator[] paginators;
    [System.Serializable]
    public class SettingByResolution
    {
        public float aspect;
        public float match;
    }

    protected override void Awake()
    {
        base.Awake();
        if (applyOnAwake)
        {
            if (canvasScaler != null)
                canvasScaler = GetComponent<CanvasScaler>();
            if (conditions.Count > 0)
                Apply();
        }
    }

    
    void Apply(SettingByResolution condition)
    {
        if (canvasScaler == null)
            canvasScaler = ScreenRoot.canvasScaler;

        canvasScaler.matchWidthOrHeight = condition.match;
    }

    void Apply(SettingByResolution condition, CanvasScaler cs)
    {
        cs.matchWidthOrHeight = condition.match;
    }

    public SettingByResolution NearestCondition(float epsilon = 0.001f)
    {
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

    public void ApplyInject(CanvasScaler cs)
    {
        var condition = conditions[0];
        for (int i = 1; i < conditions.Count; i++)
            if (Mathf.Abs(conditions[i].aspect - Aspect) < Mathf.Abs(condition.aspect - Aspect))
                condition = conditions[i];
        Apply(condition, cs);
    }


    public override void Apply()
    {
        if (conditions.Count > 0)
        {
            var condition = conditions[0];
            for (int i = 1; i < conditions.Count; i++)
                if (Mathf.Abs(conditions[i].aspect - Aspect) < Mathf.Abs(condition.aspect - Aspect))
                    condition = conditions[i];
            Apply(condition);
            foreach (var transformByResolution in others)
                transformByResolution.Apply();
        }

        foreach (var paginator in paginators)
        {
            DuongExtensions.ExecuteInNextFrame(() =>
            {
                paginator.Populate();
                paginator.SetCurrentPos();
            });
        }
        0.1f.Timer(() => {    copyPos.OnChanged(true); });
    
    }

    public override void AddCurrent()
    {
        foreach (var transformByResolution in others)
            transformByResolution.AddCurrent();

        var condition = NearestCondition();
        if (condition == null)
        {
            conditions.Add(new SettingByResolution()
            {
                aspect = Aspect,
                match = canvasScaler.matchWidthOrHeight
            });
        }
        else
            condition.match = canvasScaler.matchWidthOrHeight;

        this.SetDirty();
    }
}