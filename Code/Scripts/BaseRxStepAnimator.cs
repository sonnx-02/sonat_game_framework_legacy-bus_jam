using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseRxStepAnimator : BaseRxAnimator
{
    [SerializeField] [MyButtonVector2Int(nameof(StartAnim2))] private Vector2Int testAnim;
    protected int From;
    protected int To;

    protected void StartAnim2(Vector2Int vec)
    {
        StartAnim(vec.x,vec.y);
    }
    
    public virtual void StartAnim(int fromStep, int toStep)
    {
        From = fromStep;
        To = toStep;
        Animator_In();
    }
}