using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class CustomButtonAnimation : ToggleScript,IPointerEnterHandler,IPointerExitHandler,IPointerDownHandler,IPointerClickHandler
{
    [SerializeField] private Vector3 scaleOut = new Vector3(1.1f,1.1f,1.1f);
    [SerializeField] private float time = .05f;
    [SerializeField] private Transform[] targets;

    [SerializeField] private bool fixStartScaleOne;
    [ConditionalField(nameof(fixStartScaleOne))]
    [SerializeField] private Vector3 startScale = new Vector3(1,1,1);
    private Vector3[] _startScales;
    private Transform[] _realTargets;


    protected override void Awake()
    {
        base.Awake();
        var fix = fixStartScaleOne || GetComponent<RxAnimator>();
        if (targets.Length == 0)
        {
            var transform1 = transform;
            _startScales = new[] {fix ? startScale: transform1.localScale};
            _realTargets = new[] {transform1};
        }
        else
        {
            _startScales = targets.Select(x => fix ? startScale : x.localScale).ToArray();
            _realTargets = targets.ToArray();
        }
    }

    protected override void LoadComponent()
    {
        
    }
    
    private IDisposable _tween;

    [SerializeField] private bool useLast;
    [SerializeField] private bool onlyWhenChange;
    private bool _last;
    protected override void Trigger(bool val)
    {
        if (useLast)
        {
            if (val != _last)
                _last = val;
            else
                return;
        }
        if (custom)
        {
            _tween?.Dispose();
            _t = val ? 1f : 0;
            Set();
            busy = false;
        }
        else
        {
            if (!busy)
            {
                _tween?.Dispose();
                _tween = Observable.FromMicroCoroutine(Tween)
                    .TakeUntilDisable(this).Subscribe();
            }
        }
    }

    private void OnDisable()
    {
        _tween?.Dispose();
        CurrentValue = false;
//        0.5f.Timer(() => { }); // conflict 
        Set(0);
    }


    protected override void OnEnable()
    {
        base.OnEnable();
        _tween?.Dispose();
        CurrentValue = false;
        Set(0);
    }

    [SerializeField] private bool busy;
    private float _t;

    private IEnumerator Tween()
    {
        busy = true;
        CurrentValue = false;
        _t = CurrentValue ? 0 : 1;
        Set();
        yield return null;
        while (CurrentValue ? (_t < 1f) : (_t > 0f))
        {
            _t += Time.unscaledDeltaTime /  (CurrentValue ? time : -time);
            if (Mathf.Abs(_t) >= 1)
                _t = CurrentValue ? 1 : 0;
            Set();
            yield return null;
        }

        if (_t <= 0.01f)
        {
            for (var index = 0; index < _realTargets.Length; index++)
              _realTargets[index].localScale = _startScales[index];
        }
        
        CurrentValue = true;
        _t = CurrentValue ? 0 : 1;
        Set();
        yield return null;
        while (CurrentValue ? (_t < 1f) : (_t > 0f))
        {
            _t += Time.unscaledDeltaTime /  (CurrentValue ? time : -time);
            if (Mathf.Abs(_t) >= 1)
                _t = CurrentValue ? 1 : 0;
            Set();
            yield return null;
        }

        if (_t <= 0.01f)
        {
            for (var index = 0; index < _realTargets.Length; index++)
                _realTargets[index].localScale = _startScales[index];
        }
        busy = false;
    }
    
    private void Set()
    {
        Set(_t);
    }
    
    private void Set(float t)
    {
        for (var index = 0; index < _realTargets.Length; index++)
            _realTargets[index].localScale = Vector3.Lerp(_startScales[index], scaleOut, t);
    }

    
    public void OnPointerEnter(PointerEventData eventData)
    {
      //  OnChanged(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
      //  OnChanged(false);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
     //   OnChanged(true);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        OnChanged(true);
    }
}
