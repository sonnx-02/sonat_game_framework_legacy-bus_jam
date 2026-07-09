using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseRxPingPong : BaseRxAnimator
{
    [SerializeField] protected int times = -1;
    [SerializeField] protected float duration = 0.2f;
    [SerializeField] protected float startT = 0f;
    [SerializeField] protected Vector3 fromValue;
    [SerializeField] protected Vector3 toValue;
    [SerializeField] protected AnimationCurve curve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1));

    protected override IEnumerator ThisDoAnimate_Out()
    {
        yield return null;
    }
}

public class RxPingPongScale : BaseRxPingPong
{

    public override bool HasOut()
    {
        return true;
    }

    public override bool HasIn()
    {
        return true;
    }

    public override void ResetAnimation(bool setActive = false)
    {
        base.ResetAnimation(setActive);
        transform.localScale = Vector3.Lerp(fromValue, toValue, curve.Evaluate(startT));
    }

    protected override IEnumerator ThisDoAnimate_In()
    {
        CheckRegister();
        TriggerStartParticle();
        state = RxAnimatorState.MovingIn;

        if (settings.DelayBeforeIn > 0)
            yield return CoroutineUtilities.WaitForRealTime(settings.DelayBeforeIn);
        if (settings.whenSoundStartIn == WhenSoundStart.OnStartAnim)
            StartInSound();
        if (settings.deactiveAfterOut)
            gameObject.SetActive(true);
        ResetAnimation();

        int count = 0;
        float t = duration * startT;
        bool reverse;
        while (count != times)
        {
            reverse = count % 2 == 1;
            while (t < duration)
            {
                transform.localScale = reverse
                    ? Vector3.Lerp(toValue, fromValue, curve.Evaluate(t / duration))
                    : Vector3.Lerp(fromValue, toValue, curve.Evaluate(t / duration));
                t += Time.deltaTime;
                yield return null;
            }

            count += (int) (t / duration);
            t = t % duration;
        }

        reverse = count % 2 == 1;
        while (t < duration * startT)
        {
            transform.localScale = reverse
                ? Vector3.Lerp(toValue, fromValue, curve.Evaluate(t / duration))
                : Vector3.Lerp(fromValue, toValue, curve.Evaluate(t / duration));
            t += Time.deltaTime;
            yield return null;
        }

        transform.localScale = Vector3.Lerp(fromValue, toValue, curve.Evaluate(startT));

        TriggerAfterInParticle(settings.DelayBeforeIn - settings.particleTimeAdjust);
        InvokeCallBack();
        state = RxAnimatorState.Idle;
    }

    protected override IEnumerator ThisDoAnimate_InThenOut() => ThisDoAnimate_In();


}