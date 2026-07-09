using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;


[System.Serializable]
public abstract class ResolutionAdjustApply
{
     public float aspect;
}

public abstract class BaseAdjustByResolution : MonoBehaviour {

    public abstract ResolutionAdjustApply this[int index] { get; }
    public abstract int Count { get; }

    void Awake()
    {
        //ApplyNearestCondition();
        NotchAdjustByResolution.NotchRate = 0;
    }

    [ContextMenu("Apply 0")]
    private void Apply0()
    {
        DoApply(this[0]);
    }

    public ResolutionAdjustApply NearestCondition(float epsilon = 0.001f)
    {
        var aspect = Camera.main.pixelWidth * 1f / Camera.main.pixelHeight;

        if (Count == 0)
            return null;

        var condition = this[0];
        for (int i = 1; i < Count; i++)
            if (Mathf.Abs(this[i].aspect - aspect) < Mathf.Abs(condition.aspect - aspect))
                condition = this[i];

        if (Mathf.Abs(aspect - condition.aspect) < epsilon)
            return condition;
        else
            return null;
    }


    protected abstract void DoApply<T>(T apply) where T : ResolutionAdjustApply;

    [ContextMenu("ApplyNearestCondition")]
    protected void ApplyNearestCondition()
    {
        var aspect =Camera.main.pixelWidth * 1f / Camera.main.pixelHeight;
        var condition = this[0];
        for (int i = 1; i < this.Count; i++)
            if (Mathf.Abs(this[i].aspect - aspect) < Mathf.Abs(condition.aspect - aspect))
                condition = this[i];
        DoApply(condition);
    }

    [ContextMenu("AddOrAdjust Condition")]
    void AddOrAdjust()
    {
        var condition = NearestCondition();
        if (condition == null)
            AddCurrentAdjust();
        else
            ModifyCondition(condition);
    }

    public abstract void AddCurrentAdjust();
    public abstract void ModifyCondition(ResolutionAdjustApply apply);
}
