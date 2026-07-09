using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class RxAnimatorTweenFrom : BaseRxAnimator
{ 
    [SerializeField] private Transform tweenFromPos;
    [SerializeField] private Transform tweenFromScale;
    [SerializeField] private float time = 1;
    public AnimationCurve curve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1));


    public override bool HasOut()
    {
        return false;
    }

    public override bool HasIn()
    {
        return true;
    }

    protected override IEnumerator ThisDoAnimate_In()
    {
        var from = tweenFromPos.position;
        var transform1 = transform;
        var to = transform1.position;
        var fromScale = tweenFromScale.localScale;
        var toScale = transform1.localScale;
        
        MyIEnumeratorExtensions.IoValue(0, 1, time,curve, x =>
            {
                transform.position = Vector3.Lerp(from, to, x);
                transform.localScale = Vector3.Lerp(fromScale, toScale, x);
            })
            .TakeUntilDisable(this)
            .Subscribe(data => { });
        
   
        yield return new WaitForSeconds(time);
    }

    protected override IEnumerator ThisDoAnimate_Out() => ThisDoAnimate_In();
    protected override IEnumerator ThisDoAnimate_InThenOut() => ThisDoAnimate_In();
}