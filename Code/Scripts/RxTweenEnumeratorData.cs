using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

    
    
    public class RxTweenEnumeratorData
    {
        public Transform Target;
        public bool IsLocal;
        public Vector2 From;
        public Vector2 To;
        public bool UseSpeed;
        public float Speed;
        public float Duration;
        public float DelayBefore;
        public float DelayAfter;
        public bool UseCurve;
        public EaseEnum Ease = EaseEnum.Linear;
        public AnimationCurve Curve;
        public Action OnUpdate;
        public Action OnFinish;
    }

