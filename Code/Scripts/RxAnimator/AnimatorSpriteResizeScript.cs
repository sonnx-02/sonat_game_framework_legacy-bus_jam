using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class AnimatorSpriteResizeScript : BaseRxAnimator
{
    public SpriteRenderer spriteRenderer;
    [SerializeField] private Vector2 startSize;
    [SerializeField] private Vector2 animatorInFrom;
    [SerializeField] private Vector2 animatorOutTo;
    [SerializeField] private float timeIn = 0.5f;
    [SerializeField] private float timeOut = 0.5f;
    [SerializeField] private Ease easeIn = Ease.Linear;
    [SerializeField] private Ease easeOut = Ease.Linear;

    protected override IEnumerator ThisDoAnimate_In()
    {
        spriteRenderer.size = animatorInFrom;
        yield return DOTween.To(() => animatorInFrom, x => { spriteRenderer.size = x; }, startSize, timeIn)
            .SetEase(easeIn)
            .WaitForCompletion();
    }

    protected override IEnumerator ThisDoAnimate_Out()
    {
        yield return DOTween.To(() => startSize, x => { spriteRenderer.size = x; }, animatorOutTo, timeOut)
            .SetEase(easeOut)
            .WaitForCompletion();
    }

    protected override IEnumerator ThisDoAnimate_InThenOut()
    {
        spriteRenderer.size = animatorInFrom;
        yield return DOTween.To(() => animatorInFrom, x => { spriteRenderer.size = x; }, startSize, timeIn)
            .SetEase(easeIn)
            .WaitForCompletion();
        
        yield return DOTween.To(() => startSize, x => { spriteRenderer.size = x; }, animatorOutTo, timeOut)
            .SetEase(easeOut)
            .WaitForCompletion();
    }

    public override void ResetAnimation(bool setActive = false)
    {
        base.ResetAnimation(setActive);
        spriteRenderer.size = HasIn() ? startSize  : animatorInFrom;
    }

    public override bool HasIn() => timeIn > 0;
    public override bool HasOut() => timeOut > 0;
}
