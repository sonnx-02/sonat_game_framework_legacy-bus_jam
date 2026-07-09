using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class RxAnimatorBlinkSpriteRenderer : BaseRxAnimator
{
    [SerializeField] private SpriteRenderer spriteRenderer;

    [SerializeField] private int loop = -1;
    [SerializeField] private float time = .3f;
    [SerializeField] private float delayEach = 0f;
    [SerializeField] private float fadeTo = 0f;
    [SerializeField] private bool useCurve;
    [SerializeField] private AnimationCurve curve;
    
    public override bool HasOut()
    {
        return false;
    }

    public override void ResetAnimation(bool setActive = false)
    {
        base.ResetAnimation(setActive);
        spriteRenderer.color = spriteRenderer.color.Fade(1);
    }

    public override bool HasIn()
    {
        return true;
    }
    
    protected override IEnumerator ThisDoAnimate_In()
    {
        int l = loop > 0 ? loop : int.MaxValue;
        while (l > 0)
        {
            if(!useCurve)
                yield return spriteRenderer.DOFade(fadeTo, time).SetLoops(2, LoopType.Yoyo).WaitForCompletion();
            else
                yield return spriteRenderer.DOFade(fadeTo, time).SetLoops(2, LoopType.Yoyo).SetEase(curve).WaitForCompletion();
            
            if(delayEach > 0)
                yield return new WaitForSeconds(delayEach);
            l--;
        }
    }

    protected override IEnumerator ThisDoAnimate_Out()
    {
        yield return null;
    }

    protected override IEnumerator ThisDoAnimate_InThenOut() => ThisDoAnimate_In();
}
