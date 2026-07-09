using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

[Serializable]
public class DoTweenMoveData
{
    public float positionValue;
    public float duration = 0.35f;
    public Ease ease = Ease.Linear;
    public float delay;

    public DoTweenMoveData(Ease ease)
    {
        this.ease = ease;
    }

    public DoTweenMoveData()
    {
        
    }
}

public class DoTweenAnimationScript : MonoBehaviour
{
//    public Vector2 position;

    public enum TweenType
    {
        MoveX,
        MoveY
    }
    public RectTransform rectTransform;
    public TweenType moveType;
    
    public DoTweenMoveData dataIn = new DoTweenMoveData(Ease.OutSine);
    public DoTweenMoveData dataOut = new DoTweenMoveData(Ease.InSine);

    private Vector2 _position;
    private bool _loaded = false;

    [SerializeField] private bool tweenOnEnable;
    private void OnEnable()
    {
        if(tweenOnEnable)
            TweenIn();
    }

    private void ResetIn()
    {
        if (!_loaded)
        {
            _loaded = true;
            _position = rectTransform.anchoredPosition;
        }
        if (moveType == TweenType.MoveX)
            rectTransform.anchoredPosition = new Vector3(dataIn.positionValue, _position.y);
        if (moveType == TweenType.MoveY)
            rectTransform.anchoredPosition = new Vector3(_position.x, dataIn.positionValue);
    }

    public void TweenIn()
    {
        ResetIn();
        if (moveType == TweenType.MoveX)
            rectTransform.DOAnchorPosX(_position.x, dataIn.duration).SetDelay(dataIn.delay).SetEase(dataIn.ease);
        if (moveType == TweenType.MoveY)
            rectTransform.DOAnchorPosY(_position.y,  dataIn.duration).SetDelay(dataIn.delay).SetEase(dataIn.ease);
    }
    
    public void TweenOut()
    {
        if (moveType == TweenType.MoveX)
            rectTransform.DOAnchorPosX(dataOut.positionValue, dataOut.duration).SetDelay(dataOut.delay).SetEase(dataOut.ease);
        if (moveType == TweenType.MoveY)
            rectTransform.DOAnchorPosY(dataOut.positionValue,  dataOut.duration).SetDelay(dataOut.delay).SetEase(dataOut.ease);
    }
}
