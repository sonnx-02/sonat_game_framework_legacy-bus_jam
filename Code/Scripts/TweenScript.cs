using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;


public abstract class BaseTweenScript : BaseView
{

    public BaseTweenScript[] others;
    
    public bool startOnEnable;
    protected virtual  void OnEnable()
    {
        if(startOnEnable)
            Tween();
    }
    
    [MyButton(nameof(Tween),nameof(ResetTween))] public int abc;

    public virtual void Tween()
    {
        CheckRegister();
        foreach (var baseTweenScript in others)
            baseTweenScript.Tween();
    }

    public virtual void ResetTween()
    {
        foreach (var baseTweenScript in others)
            baseTweenScript.ResetTween(); 
    }
    
    public virtual void StopTween()
    {
        transform.DOKill();
    }
}

public class TweenScript : BaseTweenScript
{
    public Vector3 to1;
    public Vector3 to2;
    public bool fromStartPos;
    public float speed = 1;


    private Vector3 _startPos;
    protected override void Register()
    {
        base.Register();
        _startPos = transform.localPosition;
    }


    public override void Tween()
    {
        base.Tween();
        if (fromStartPos)
        {
            transform.localPosition = _startPos;
            transform.DOLocalMove(to1, Vector3.Magnitude(_startPos - to1)/speed).SetEase(Ease.Linear)
                .OnComplete(() =>
                    {
                        transform.DOLocalMove(to2, Vector3.Magnitude(to2 - to1)/speed).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.Linear);
                    });
        }
        else
        {
            transform.localPosition = to1;
            transform.DOLocalMove(to2, Vector3.Magnitude(to2 - to1)/speed).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.Linear);
        }
    }

    public override void ResetTween()
    {
        base.ResetTween();
        transform.localPosition = _startPos;
    }

 
    private void OnDisable()
    {
        transform.DOKill();
    }
}
