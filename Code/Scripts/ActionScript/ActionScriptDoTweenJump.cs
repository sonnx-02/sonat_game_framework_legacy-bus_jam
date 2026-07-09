using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UniRx;
using UnityEngine;

public class ActionScriptDoTweenJump : BaseActionScript
{
    [SerializeField] private bool startOnEnable;

    
    [SerializeField] private ResetType resetFrom;
    [SerializeField] private Vector3 fromFrom;
    [SerializeField] private Vector3 fromTo;

    [Serializable]
    private enum ResetType
    {
        None,
        Reset,
        Add
    }
    
    [SerializeField] private Vector3 targetFrom;
    [SerializeField] private Vector3 targetTo;
    [SerializeField] private bool setRotation;
  
    [SerializeField] private Vector3 eulerFrom;
    [SerializeField] private Vector3 eulerTo;
    
    [SerializeField] private bool rotate;
    [SerializeField] private Vector3 rotateFrom;
    [SerializeField] private Vector3 rotateAfter;
    
    [SerializeField] private Vector2 force;
    [SerializeField] private Vector2 duration;
    [SerializeField] private bool snap;

    [SerializeField] private PoolItem returnOnFinish;
    [SerializeField] private BaseBaseActionScript[] onFinishScript;

    [SerializeField] private bool useCurve;
    [SerializeField] private AnimationCurve curve;
    protected override void OnEnable()
    {
        base.OnEnable();
        if(startOnEnable)
            StartAction();
    }

    private void OnDisable()
    {
        transform.DOKill();
    }

    protected override void DoAction()
    {
        base.DoAction();
        switch (resetFrom)
        {
            case ResetType.None:
                break;
            case ResetType.Reset:
                transform.localPosition = fromFrom.Random(fromTo);
                break;
            case ResetType.Add:
                transform.localPosition += fromFrom.Random(fromTo);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
       
        if (setRotation)
        {
            transform.localEulerAngles = eulerFrom.Random(eulerTo);
            
        }

        if (rotate)
        {
            var _transform = transform;
            var vec = rotateFrom.Random(rotateAfter);
            Observable.EveryUpdate().TakeUntilDisable(this).Subscribe(data =>
            {
                _transform.localEulerAngles += Time.deltaTime * vec;
            });
        }

        if (useCurve)
        {
            transform.DOLocalJump(targetFrom.Random(targetTo), force.GetRandom(), 1, duration.GetRandom(), snap)
                .SetEase(curve)
                .OnComplete(() =>
                {
                    if(returnOnFinish != null)
                        returnOnFinish.DestroyView();
                    foreach (var baseActionScript in onFinishScript)
                        baseActionScript.StartAction();
                });
        }
        else
        {
            transform.DOLocalJump(targetFrom.Random(targetTo), force.GetRandom(), 1, duration.GetRandom(), snap)
                .OnComplete(() =>
                {
                    if(returnOnFinish != null)
                        returnOnFinish.DestroyView();
                    foreach (var baseActionScript in onFinishScript)
                        baseActionScript.StartAction();
                });
        }
    
    }

    
}