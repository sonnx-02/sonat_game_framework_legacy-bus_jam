using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class ActionScriptTweenPosition : BaseActionScript
{
    [SerializeField] private bool defaultValue;
    [SerializeField] private ValidScriptBase toggle;
    [SerializeField] private Vector2 startPos;
    [SerializeField] private Vector2 toPos;
    [SerializeField] private RectTransform target;
    [SerializeField] private float duration = 0.5f;
    [SerializeField] private Ease ease;
    [SerializeField] private bool useCurve;
    [SerializeField] private AnimationCurve curve;
    [SerializeField] private float delay;
    [SerializeField] private BaseBaseActionScript[] onStartMove;
    [SerializeField] private BaseBaseActionScript[] onFinishMove;

    [SerializeField] int actionSoundIndex;

    public bool valid;
    
    protected override void DoAction()
    {
        base.DoAction();
        valid = toggle != null ?  toggle.IsValid() : defaultValue;

        if (!valid)
        {
            onStartMove.StartAction();
            target.anchoredPosition = toPos;
            onFinishMove.StartAction();
        }
        else
        {
            onStartMove.StartAction();
            target.anchoredPosition = startPos;
            if (useCurve)
                target.DOAnchorPos(toPos, duration).SetEase(curve).SetDelay(delay)
                    .OnComplete(() => onFinishMove.StartAction());
            else
                target.DOAnchorPos(toPos, duration).SetEase(ease).SetDelay(delay)
                    .OnComplete(() => onFinishMove.StartAction());

            if (actionSoundIndex != 0)
            {
                (actionSoundIndex).PlaySound();
            }
        }
    }
}
