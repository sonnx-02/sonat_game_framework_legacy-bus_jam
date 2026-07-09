using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class RxMoveAround : BaseRxAnimatorInOnly
{
    [SerializeField] private Ease ease = Ease.Linear;
    [SerializeField] private bool useCurve;
    [SerializeField] private AnimationCurve curve;
    [SerializeField] private Vector3[] localPoints;
    [SerializeField] private float[] timeEach;
    [SerializeField] private float[] delayEach;
    [SerializeField] private BaseBaseActionScript[] actionEach;
    
    public bool Stop { get; set; }
    protected override IEnumerator ThisDoAnimate_In()
    {
        int current = 0;
        Stop = false;
        transform.localPosition = localPoints[current];
        yield return new WaitForSeconds(settings.DelayBeforeIn);
        while (gameObject.activeInHierarchy && !Stop)
        {

            var nextPoint = localPoints[(current + 1) % localPoints.Length];
            foreach (var baseActionScript in actionEach)
                baseActionScript.StartAction();
            if (useCurve)
                yield return transform.DOLocalMove(nextPoint, timeEach[current]).SetEase(curve).WaitForCompletion();
            else
                yield return transform.DOLocalMove(nextPoint, timeEach[current]).SetEase(ease).WaitForCompletion();

            yield return new WaitForSeconds(delayEach[current]);

            current = (current + 1) % localPoints.Length;
        }
    }

    public override void StopAnim()
    {
        Stop = true;
        transform.DOKill();
        base.StopAnim();
    }
}
