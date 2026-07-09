using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RxHit : BaseRxAnimator
{
    [SerializeField] private Vector3 scaleFrom = Vector3.one;
    [SerializeField] private Vector3 scaleTo = new Vector3(1.1f, 1.1f, 1.1f);
    [SerializeField] private float duration = 1;

    [SerializeField] private AnimationCurve curve = new AnimationCurve(
        new Keyframe(0, 0),
        new Keyframe(0.25f, 1.0f),
        new Keyframe(0.5f, 0),
        new Keyframe(0.75f, 1.0f),
        new Keyframe(1, 0f)
    );

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
        _t = 0;
        if (gameObject.activeInHierarchy)
        {
            TriggerStartParticle();
            if (settings.whenSoundStartIn == WhenSoundStart.OnStartAnim)
                StartInSound();
        }
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
        busy = true;
        state = RxAnimatorState.MovingIn;
        _t = 0;
        transform.localScale = scaleFrom;
        while (_t < duration)
        {
            _t += Time.deltaTime;
            transform.localScale = Vector3.Lerp(scaleFrom, scaleTo, curve.Evaluate(_t / duration));
            yield return null;
        }
        busy = false;
        state = RxAnimatorState.Reset; 
    }

    protected override IEnumerator ThisDoAnimate_Out() => ThisDoAnimate_In();
    protected override IEnumerator ThisDoAnimate_InThenOut() => ThisDoAnimate_In();
}