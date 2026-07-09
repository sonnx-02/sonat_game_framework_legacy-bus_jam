using System;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using System.IO;
#endif
using System.Linq;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;



public class RxAnimatorList : BaseRxAnimator
{
    [SerializeField] private BaseBaseActionScript[] onMoveInActions;
    [SerializeField] private float duration;
    [SerializeField] private float delayEach;
    [SerializeField] private Ease ease;
    [SerializeField] private bool useCurve;
    [SerializeField] private AnimationCurve curve;

    private Vector3[] _startPositions;
    [SerializeField] private MoveInListItem[] items;

    public override void LogCustomInt(int value)
    {
        base.LogCustomInt(value);
        for (var i = 0; i < items.Length; i++)
        {
            items[i].toPos = items[i].target.anchoredPosition;
            items[i].fromPos = items[i].toPos - new Vector2(0, value);
        }
    }

    [Serializable]
    public class MoveInListItem
    {
        public RectTransform target;
        public Vector2 fromPos;
        public Vector2 toPos;
    }
    
    public override bool HasOut()
    {
        return false;
    }

    public override bool HasIn()
    {
        return true;
    }

    protected override IEnumerator ThisDoAnimate_In()
    {
        for (var i = 0; i < items.Length; i++)
        {
            items[i].target.gameObject.SetActive(false);
            items[i].target.anchoredPosition = items[i].fromPos;
        }

        CheckRegister();
        TriggerStartParticle();

        
        yield return CoroutineUtilities.WaitForRealTime(settings.DelayBeforeIn);
        
        if (settings.deactiveAfterOut)
            gameObject.SetActive(true);
        if (settings.whenSoundStartIn == WhenSoundStart.OnStartAnim)
            StartInSound();
        onMoveInActions.StartAction();
        state = RxAnimatorState.MovingIn;
        
       
        for (var i = 0; i < items.Length; i++)
        {
            items[i].target.gameObject.SetActive(true);
            items[i].target.anchoredPosition = items[i].fromPos;
            if (useCurve)
                items[i].target.DOAnchorPos(items[i].toPos, duration).SetEase(curve).SetUpdate(true);
            else
                items[i].target.DOAnchorPos(items[i].toPos, duration).SetEase(ease).SetUpdate(true);
            yield return CoroutineUtilities.WaitForRealTime(delayEach);
        }
        
        InvokeCallBack();

        busy = false;
        state = RxAnimatorState.Idle;
    }

    protected override IEnumerator ThisDoAnimate_Out()
    {
        yield return null;
    }

    protected override IEnumerator ThisDoAnimate_InThenOut() => ThisDoAnimate_In();


    public override void SetAtAnimationIn(bool setActive)
    {
        base.SetAtAnimationIn(setActive);
        for (var i = 0; i < items.Length; i++)
        {
            items[i].target.anchoredPosition = items[i].toPos;
        }
    }
}