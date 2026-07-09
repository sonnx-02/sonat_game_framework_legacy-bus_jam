using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RxAnimatorMoveToLocation : BaseRxAnimator
{
    [SerializeField] private LocationEnum locationEnum;
    [SerializeField] private float delay = 1f;
    [SerializeField] private float timeTween = 1f;
    [SerializeField] private AnimationCurve curve =  new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1));
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
        yield return new WaitForSeconds(settings.DelayBeforeIn);
        var rt = ScreenRoot.GetLocation(locationEnum);
        if(delay > 0)
            yield return new WaitForSeconds(delay);

        float t = 0f;
        Vector3 start = transform.position;
        Vector3 to = rt.position;
        while (t < timeTween)
        {
            t += Time.deltaTime;
            transform.position = Vector3.Lerp(start,to,curve.Evaluate(t/timeTween));
            yield return null;
        }
    }

    protected override IEnumerator ThisDoAnimate_Out() => ThisDoAnimate_In();
    protected override IEnumerator ThisDoAnimate_InThenOut() => ThisDoAnimate_In();
}
