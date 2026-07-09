using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RxPopRepeat : BaseRxAnimator
{
    [SerializeField] private Vector3 scaleFrom = Vector3.one;
    [SerializeField] private Vector3 scaleTo = new Vector3(1.1f, 1.1f, 1.1f);
    [SerializeField] private float duration = 1;
    [SerializeField] private Loop loop;
    [SerializeField] private BaseBaseActionScript[] onEachLoop;

    [SerializeField] private AnimationCurve curve = new AnimationCurve(
        new Keyframe(0, 0),
        new Keyframe(0.25f, 1.0f),
        new Keyframe(0.5f, 0),
        new Keyframe(0.75f, 1.0f),
        new Keyframe(1, 0f)
    );

    [Serializable]
    private class Loop
    {
        public int loop;
        public float delayLoopRandomFrom = 1f;
        public float delayLoopRandomTo = 1f;
    }

    private float _t;

    public override bool HasOut()
    {
        return false;
    }

    public override bool HasIn()
    {
        return true;
    }

    public override void Animator_In()
    {
        if (!busy)
        {
            base.Animator_In();
        }
    }

    private void OnDisable()
    {
        busy = false;
        state = RxAnimatorState.Reset;
        transform.localScale = scaleFrom;
    }

    protected override IEnumerator ThisDoAnimate_In()
    {
        int count = 0;
        busy = true;
        state = RxAnimatorState.MovingIn;

        TriggerStartParticle();
        yield return CoroutineUtilities.WaitForRealTime(settings.DelayBeforeIn);
        if (settings.whenSoundStartIn == WhenSoundStart.OnStartAnim)
            StartInSound();
        if (settings.deactiveAfterOut)
            gameObject.SetActive(true);

        do
        {
            count++;
            onEachLoop.StartAction();
            _t = 0;
            transform.localScale = scaleFrom;
            while (_t < duration)
            {
                _t += Time.unscaledDeltaTime;
                transform.localScale = Vector3.Lerp(scaleFrom, scaleTo, curve.Evaluate(_t / duration));
                yield return null;
            }
            if(count < loop.loop)
                yield return CoroutineUtilities.WaitForRealTime(UnityEngine.Random.Range(loop.delayLoopRandomFrom,loop.delayLoopRandomTo));
        } 
        while (count < loop.loop);
        
        if (settings.delayAfterOut > 0)
            yield return CoroutineUtilities.WaitForRealTime(settings.delayAfterOut);
        if (settings.deactiveAfterOut)
            gameObject.SetActive(false);
        busy = false;
        state = RxAnimatorState.Reset; 
    }

    protected override IEnumerator ThisDoAnimate_Out() => ThisDoAnimate_In();
    protected override IEnumerator ThisDoAnimate_InThenOut() => ThisDoAnimate_In();
}