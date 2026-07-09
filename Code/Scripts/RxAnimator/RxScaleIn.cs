using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

[Serializable]
public class RxAnimatorScaleAdvanced
{
    public Vector3 value;
    public float time = .3f;
    public bool useCurve;
    public AnimationCurve curveX = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1));
    public AnimationCurve curveY = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1));
    public AnimationCurve curveZ = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1));
    public Ease easeX = Ease.Linear;
    public Ease easeY = Ease.Linear;
    public Ease easeZ = Ease.Linear;
}


public class RxScaleIn : BaseRxAnimator
{
    [SerializeField] private bool yoyo;
    [SerializeField] private RxAnimatorScaleAdvanced scaleIn;
    [SerializeField] private RxAnimatorScaleAdvanced scaleOut;

    public override bool HasOut()
    {
        return false;
    }

    public override bool HasIn()
    {
        return true;
    }

    private Vector3 _startScale;

    protected override void Register()
    {
        base.Register();
        _startScale = transform.localScale;
    }

    public override void ResetAnimation(bool setActive = false)
    {
        CheckRegister();
        base.ResetAnimation(setActive);
        state = RxAnimatorState.Reset;
        TurnOffStartParticle();
    }

    protected override IEnumerator ThisDoAnimate_In()
    {
        CheckRegister();
        TriggerStartParticle();
        yield return new WaitForSeconds(settings.DelayBeforeIn);

        if (settings.deactiveAfterOut)
            gameObject.SetActive(true);

        if (settings.whenSoundStartIn == WhenSoundStart.OnStartScale)
            StartInSound();

        transform.localScale = scaleIn.value;
        float t = 0f;
        while (t < scaleIn.time)
        {
            yield return null;
            t += Time.deltaTime;
            var d = t / scaleIn.time;

            if (!yoyo)
            {
                if (scaleIn.useCurve)
                {
                    float x = DOVirtual.EasedValue(scaleIn.value.x, _startScale.x, d, scaleIn.curveX);
                    float y = DOVirtual.EasedValue(scaleIn.value.y, _startScale.y, d, scaleIn.curveY);
                    float z = DOVirtual.EasedValue(scaleIn.value.z, _startScale.z, d, scaleIn.curveZ);
                    transform.localScale = new Vector3(x, y, z);
                }
                else
                {
                    float x = DOVirtual.EasedValue(scaleIn.value.x, _startScale.x, d, scaleIn.easeX);
                    float y = DOVirtual.EasedValue(scaleIn.value.y, _startScale.y, d, scaleIn.easeY);
                    float z = DOVirtual.EasedValue(scaleIn.value.z, _startScale.z, d, scaleIn.easeZ);
                    transform.localScale = new Vector3(x, y, z);
                }
            }
            else
            {
                d = Mathf.PingPong(d * 2, 1);
                if (scaleIn.useCurve)
                {
                    float x = DOVirtual.EasedValue(_startScale.x, scaleIn.value.x, d, scaleIn.curveX);
                    float y = DOVirtual.EasedValue(_startScale.y, scaleIn.value.y, d, scaleIn.curveY);
                    float z = DOVirtual.EasedValue(_startScale.z, scaleIn.value.z, d, scaleIn.curveZ);
                    transform.localScale = new Vector3(x, y, z);
                }
                else
                {
                    float x = DOVirtual.EasedValue(_startScale.x, scaleIn.value.x, d, scaleIn.easeX);
                    float y = DOVirtual.EasedValue(_startScale.y, scaleIn.value.y, d, scaleIn.easeY);
                    float z = DOVirtual.EasedValue(_startScale.z, scaleIn.value.z, d, scaleIn.easeZ);
                    transform.localScale = new Vector3(x, y, z);
                }
            }
        }

        if (settings.whenSoundStartIn == WhenSoundStart.OnFinishScale)
            StartInSound();
    }

    protected override IEnumerator ThisDoAnimate_Out()
    {
        yield return null;
    }

    protected override IEnumerator ThisDoAnimate_InThenOut() => ThisDoAnimate_In();
}