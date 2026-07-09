using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class RxAnimatorPopFloatBinding : BaseRxAnimator
{
    [SerializeField] private FloatBindingReactiveProperty property = new FloatBindingReactiveProperty();
    [SerializeField] private ProgressScript[] progressScripts;
    [SerializeField] private float speed = 1;
    [SerializeField] private float delayAfter = 1;
    protected override void Register()
    {
        base.Register();
        property.Init();
    }

    private bool _isTweening;
    private bool _reverse;

    protected override void DoAction()
    {
        
     //   base.StartAction();
        _reverse = false;
        
        if (_isTweening)
        {
           
        }
        else
        {
            Animator_In();
        }
    }
    
    public override void ResetAnimation(bool setActive = false)
    {
        base.ResetAnimation(setActive);
        _isTweening = false;
        _reverse = false;
        property.Value = 0;
        OnChanged();
    }

    private void OnDisable()
    {
        ResetAnimation();
    }

    public override bool HasOut()
    {
        return true;
    }

    public override bool HasIn()
    {
        return true;
    }

    protected override IEnumerator ThisDoAnimate_In()
    {
        _isTweening = true;
        OnChanged();
        yield return new WaitForSeconds(settings.DelayBeforeIn);
        StartInSound();
        _reverse = false;
        
        while (_isTweening)
        {
            property.Value += _reverse ? Time.deltaTime * -speed : Time.deltaTime * speed;
            if (!_reverse && property.Value >= 1)
            {
                property.Value = 1-(property.Value % 1);
                _reverse = true;
                if(delayAfter > 0)
                    yield return new WaitForSeconds(delayAfter);
            }

            if (property.Value <= 0 && _reverse)
            {
                _reverse = false;
                property.Value = 0;
                _isTweening = false;
                OnChanged();
                break;
            }   
            OnChanged();
            yield return null;
        }
    }

    private void OnChanged()
    {
        foreach (var progressScript in progressScripts)
            progressScript.OnChanged(property.Value);
    }
    protected override IEnumerator ThisDoAnimate_Out() => ThisDoAnimate_In();


    protected override IEnumerator ThisDoAnimate_InThenOut() => ThisDoAnimate_In();
}
