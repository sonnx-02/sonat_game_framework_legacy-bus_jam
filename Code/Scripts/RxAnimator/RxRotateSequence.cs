using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class RxRotateSequence: BaseRxAnimatorInOnly
{
    [SerializeField] private Ease ease = Ease.Linear;
    [SerializeField] private bool useCurve;
    [SerializeField] private AnimationCurve curve;
    [SerializeField] private Vector3[] rotates;
    [SerializeField] private float[] delayEach;
    [SerializeField] private float[] timeEach;
    [SerializeField] private BaseBaseActionScript[] actionEach;
    [SerializeField] private bool loop = true;
    public bool Stop { get; set; }
    protected override IEnumerator ThisDoAnimate_In()
    {
        int current = 0;
        Stop = false;
        transform.localEulerAngles = rotates[current];
        if(settings.DelayBeforeIn > 0)
            yield return new WaitForSeconds(settings.DelayBeforeIn);
        while (gameObject.activeInHierarchy && !Stop)
        {
            var nextPoint = rotates[(current + 1) % rotates.Length];
            foreach (var baseActionScript in actionEach)
                baseActionScript.StartAction();
            if (useCurve)
                yield return transform.DOLocalRotate(nextPoint, timeEach[current]).SetEase(curve).WaitForCompletion();
            else
                yield return transform.DOLocalRotate(nextPoint, timeEach[current]).SetEase(ease).WaitForCompletion();

            yield return new WaitForSeconds(delayEach[current]);

            current = (current + 1) % rotates.Length;
            if(!loop && current == 0)
                break;
        }
    }

    public override void StopAnim()
    {
        Stop = true;
        transform.DOKill();
        base.StopAnim();
    }
}
