//using System;
//using System.Collections;
//using System.Collections.Generic;
//#if UNITY_EDITOR
//using System.IO;
//#endif
//using System.Linq;
//using DG.Tweening;
//using DG.Tweening.Core;
//using DG.Tweening.Plugins.Options;
//using TMPro;
//using UniRx;
//using UnityEngine;
//using UnityEngine.Events;
//using UnityEngine.UI;
//
//
//public class RxAnimator3D : BaseRxAnimator
//{
//    public float inThenOutIdleDelay;
//    public List<ParticleSystem> afterInParticleSystems;
//    public List<ParticleSystem> onStartParticleSystems;
//    public UnityEvent afterInAction;
//
//    public RxAnimatorMove moveIn;
//    public RxAnimatorMove moveOut;
//    public RxAnimatorFade fadeIn;
//    public RxAnimatorFade fadeOut;
//    public RxAnimatorScale scaleIn;
//    public RxAnimatorScale scaleOut;
//
//    // private
//    private Vector3 _startPos;
//    private Vector3 _startScale;
//    private Text _text;
//    private TMP_Text _textMesh;
//    private float _startAlpha;
//
//
//    [SerializeField] private RxAnimatorUpdatePos[] _updatePoses;
//
//    [MyButton(new[] {nameof(FixX), nameof(FixY)})]
//    public int test2;
//
//    public void FixX()
//    {
//        moveIn.value.x = transform.localPosition.x;
//        moveOut.value.x = transform.localPosition.x;
//    }
//
//    public void FixY()
//    {
//        moveIn.value.y = transform.localPosition.y;
//        moveOut.value.y = transform.localPosition.y;
//    }
//
//    public override bool HasOut()
//    {
//        return moveOut.active || scaleOut.active || fadeOut.active;
//    }
//
//    public override bool HasIn()
//    {
//        return moveIn.active || scaleIn.active || fadeIn.active;
//    }
//
//    protected override void Awake()
//    {
//        base.Awake();
//        CheckRegister();
//    }
//
//    protected override void Register()
//    {
//        base.Register();
//
//            _startPos = transform.localPosition;
//
//
//        _startScale = transform.localScale;
//        if (fadeIn.active || fadeOut.active)
//        {
//            _canvasGroup = GetComponent<CanvasGroup>();
//            _text = GetComponent<Text>();
//            _textMesh = GetComponent<TMP_Text>();
//
//            if (_canvasGroup != null)
//                _startAlpha = _canvasGroup.alpha;
//            else if (_image != null)
//                _startAlpha = _image.color.a;
//            else if (_spriteRenderer != null)
//                _startAlpha = _spriteRenderer.color.a;
//            else if (_text != null)
//                _startAlpha = _text.color.a;
//            else if (_textMesh != null)
//                _startAlpha = _textMesh.color.a;
//        }
//    }
//
//    public override void ResetAnimation(bool setAcive = false)
//    {
//        CheckRegister();
//        base.ResetAnimation(setAcive);
//        state = RxAnimatorState.Reset;
//
//        if (_canvasGroup != null)
//            _canvasGroup.DOKill();
//        else if (_image != null)
//            _image.DOKill();
//        else if (_spriteRenderer != null)
//            _spriteRenderer.DOKill();
//        else if (_textMesh != null)
//            _textMesh.DOKill();
//        else if (_text != null)
//            _text.DOKill();
//
//        foreach (var spriteRenderer in fadeRenderers)
//            spriteRenderer.DOKill();
//
//        if (moveOut.active && !moveIn.active)
//        {
//            if (moveIn.type == AnimatorMoveType.SpecificPosition || moveIn.type == AnimatorMoveType.LocationManager)
//            {
//                if (_rectTransform == null)
//                    transform.localPosition = _startPos;
//                else
//                    _rectTransform.anchoredPosition = _startPos;
//            }
//        }
//        else if (moveIn.active)
//        {
//            if (moveIn.type == AnimatorMoveType.SpecificPosition)
//            {
//                if (_rectTransform == null)
//                    transform.localPosition = moveIn.value;
//                else
//                    _rectTransform.anchoredPosition = moveIn.value;
//            }
//
//
//            if (moveIn.type == AnimatorMoveType.LocationManager)
//                _rectTransform.localPosition = RootView.rootView.screenRoot.GetLocationManager()
//                    .GetRelativeLocalPosition(moveIn.locationEnum, _rectTransform.parent);
//        }
//
//        if (moveOut.active && moveOut.type == AnimatorMoveType.LocationManager)
//        {
//            if (RootView.rootView.screenRoot.GetLocationManager() != null)
//                moveOut.value = RootView.rootView.screenRoot.GetLocationManager()
//                    .GetRelativeLocalPosition(moveOut.locationEnum, _rectTransform.parent);
//            else
//            {
//#if UNITY_EDITOR
//                Debug.LogError("not have location for this scene" + moveOut.locationEnum);
//#endif
//            }
//        }
//
//        if (scaleOut.active && !scaleIn.active)
//            transform.localScale = _startScale;
//        if (scaleIn.active)
//            transform.localScale = scaleIn.value;
//
//        if (fadeOut.active && !fadeIn.active)
//        {
//            if (_canvasGroup != null)
//                _canvasGroup.alpha = _startAlpha;
//            else if (_image != null)
//                _image.color = _image.color.Fade(_startAlpha);
//            else if (_spriteRenderer != null)
//                _spriteRenderer.color = _spriteRenderer.color.Fade(_startAlpha);
//            else if (_textMesh != null)
//                _textMesh.color = _textMesh.color.Fade(_startAlpha);
//            else if (_text != null)
//                _text.color = _text.color.Fade(_startAlpha);
//
//            foreach (var spriteRenderer in fadeRenderers)
//                spriteRenderer.color = Color.white;
//        }
//        else if (fadeIn.active)
//        {
//            if (_canvasGroup != null)
//            {
//                _canvasGroup.alpha = fadeIn.value;
//                if (fadeIn.turnOffRayCast)
//                    _canvasGroup.blocksRaycasts = false;
//            }
//            else if (_image != null)
//                _image.color = _image.color.Fade(fadeIn.value);
//            else if (_spriteRenderer != null)
//                _spriteRenderer.color = _spriteRenderer.color.Fade(fadeIn.value);
//            else if (_textMesh != null)
//                _textMesh.color = _textMesh.color.Fade(fadeIn.value);
//            else if (_text != null)
//                _text.color = _text.color.Fade(fadeIn.value);
//
//            foreach (var spriteRenderer in fadeRenderers)
//                spriteRenderer.color = Color.white.Fade(fadeIn.value);
//            ;
//        }
//
//        TurnOffStartParticle();
//    }
//
//    public override void SetAtAnimationIn(bool setActive = false)
//    {
//        CheckRegister();
//        base.SetAtAnimationIn(setActive);
//        state = RxAnimatorState.Idle;
//
//        //
//        if (_canvasGroup != null)
//            _canvasGroup.DOKill();
//        else if (_image != null)
//            _image.DOKill();
//        else if (_spriteRenderer != null)
//            _spriteRenderer.DOKill();
//        else if (_textMesh != null)
//            _textMesh.DOKill();
//        else if (_text != null)
//            _text.DOKill();
//        //
//        if (_rectTransform == null)
//            transform.localPosition = _startPos;
//        else
//            _rectTransform.anchoredPosition = _startPos;
//        transform.localScale = _startScale;
//        //
//        if (_canvasGroup != null)
//            _canvasGroup.alpha = _startAlpha;
//        else if (_image != null)
//            _image.color = _image.color.Fade(_startAlpha);
//        else if (_spriteRenderer != null)
//            _spriteRenderer.color = _spriteRenderer.color.Fade(_startAlpha);
//        else if (_textMesh != null)
//            _textMesh.color = _textMesh.color.Fade(_startAlpha);
//        else if (_text != null)
//            _text.color = _text.color.Fade(_startAlpha);
//
//        foreach (var spriteRenderer in fadeRenderers)
//        {
//            spriteRenderer.DOKill();
//            spriteRenderer.color = Color.white;
//        }
//
//        onStartParticleSystems.ForEach(x =>
//        {
//            x.gameObject.SetActive(true);
//            x.Stop();
//            x.Play();
//        });
//    }
//
//    public override bool IsAnimating()
//    {
//        return busy;
//    }
//
//
//    public override void LogCustom()
//    {
//        base.LogCustom();
//        ResetAnim();
//        Animator_In();
//    }
//
//
//#if UNITY_EDITOR
//    [SerializeField] private bool testDebug;
//#endif
//
//    protected override IEnumerator ThisDoAnimate_In()
//    {
//        CheckRegister();
//
//#if UNITY_EDITOR
//        if (testDebug)
//            Debug.Log("call from", gameObject);
//#endif
//
//
////        Debug.Log("_startScale"+_startScale +"/"+scaleIn.value);
//        TriggerStartParticle();
//        yield return CoroutineUtilities.WaitForRealTime(settings.DelayBeforeIn);
//
//
//        if (settings.deactiveAfterOut)
//            gameObject.SetActive(true);
//        if (settings.whenSoundStartIn == WhenSoundStart.OnStartAnim)
//            StartInSound();
//        var waitMoveIn = (moveIn.active ? moveIn.time + moveIn.delay : 0) + moveIn.beforeAfterDelay;
//        var waitScaleIn = (scaleIn.active ? scaleIn.time + scaleIn.delay : 0) + scaleIn.beforeAfterDelay;
//        var waitFadeIn = (fadeIn.active ? fadeIn.time + fadeIn.delay : 0) + fadeIn.beforeAfterDelay;
//
//        //ResetAnimation();
//        if (moveIn.active || scaleIn.active || fadeIn.active)
//        {
//            busy = true;
//            state = RxAnimatorState.MovingIn;
//        }
//
//        if (moveIn.active)
//        {
//            if (settings.whenSoundStartIn == WhenSoundStart.OnStartMove)
//                StartInSound(moveIn.delay);
//
//            if (_rectTransform == null)
//            {
//                var tween = transform.DOLocalMove(_startPos, moveIn.time)
//                    .SetUpdate(true);
//
//                if (_updatePoses.Length > 0)
//                    tween.OnUpdate(() =>
//                    {
//                        foreach (var rxAnimatorUpdatePose in _updatePoses)
//                            rxAnimatorUpdatePose.SetLocalPos(transform.localPosition);
//                    });
//                if (moveIn.delay > 0)
//                    tween.SetDelay(moveIn.delay);
//                if (settings.whenSoundStartIn == WhenSoundStart.OnFinishMove)
//                    tween.OnComplete(StartInSound);
//                if (moveIn.useCurve)
//                    tween.SetEase(moveIn.curve);
//                else
//                    tween.SetEase(moveIn.ease);
//            }
//            else
//            {
//                var tween = _rectTransform.DOAnchorPos(_startPos, moveIn.time)
//                    .SetUpdate(true);
//                if (_updatePoses.Length > 0)
//                    tween.OnUpdate(() =>
//                    {
//                        foreach (var rxAnimatorUpdatePose in _updatePoses)
//                            rxAnimatorUpdatePose.SetLocalPos(transform.localPosition);
//                    });
//                if (moveIn.delay > 0)
//                    tween.SetDelay(moveIn.delay);
//                if (settings.whenSoundStartIn == WhenSoundStart.OnFinishMove)
//                    tween.OnComplete(StartInSound);
//                if (moveIn.useCurve)
//                    tween.SetEase(moveIn.curve);
//                else
//                    tween.SetEase(moveIn.ease);
//            }
//        }
//
//        if (scaleIn.active)
//        {
//            if (settings.whenSoundStartIn == WhenSoundStart.OnStartScale)
//                StartInSound();
//            var tween = transform.DOScale(_startScale, scaleIn.time)
//                .SetUpdate(true);
//            if (scaleIn.delay > 0)
//                tween.SetDelay(scaleIn.delay);
//            if (settings.whenSoundStartIn == WhenSoundStart.OnFinishScale)
//                tween.OnComplete(StartInSound);
//            if (scaleIn.useCurve)
//                tween.SetEase(scaleIn.curve);
//            else
//                tween.SetEase(scaleIn.ease);
//        }
//
//        if (fadeIn.active)
//        {
//            if (settings.whenSoundStartIn == WhenSoundStart.OnStartFade)
//                StartInSound();
//
//            if (_canvasGroup != null)
//            {
//                var tween = _canvasGroup.DOFade(_startAlpha, fadeIn.time)
//                    .SetUpdate(true)
//                    .OnComplete(() =>
//                    {
//                        if (fadeIn.turnOffRayCast)
//                            _canvasGroup.blocksRaycasts = true;
//                        if (settings.whenSoundStartIn == WhenSoundStart.OnFinishFade)
//                            StartInSound();
//                    });
//
//                if (fadeIn.delay > 0)
//                    tween.SetDelay(fadeIn.delay);
//                if (fadeIn.useCurve)
//                    tween.SetEase(fadeIn.curve);
//                else
//                    tween.SetEase(fadeIn.ease);
//            }
//            else
//            {
//                TweenerCore<Color, Color, ColorOptions> tween = null;
//                if (_image != null)
//                    tween = _image.DOFade(_startAlpha, fadeIn.time);
//                else if (_spriteRenderer != null)
//                    tween = _spriteRenderer.DOFade(_startAlpha, fadeIn.time);
//                else if (_text != null)
//                    tween = _text.DOFade(_startAlpha, fadeIn.time);
//                else if (_textMesh != null)
//                    tween = _textMesh.DOFade(_startAlpha, fadeIn.time);
//                tween.SetUpdate(true);
//                if (fadeIn.delay > 0)
//                    tween.SetDelay(fadeIn.delay);
//                if (settings.whenSoundStartIn == WhenSoundStart.OnFinishFade)
//                    tween.OnComplete(StartInSound);
//                if (fadeIn.useCurve)
//                    tween.SetEase(fadeIn.curve);
//                else
//                    tween.SetEase(fadeIn.ease);
//            }
//
//            foreach (var spriteRenderer in fadeRenderers)
//                spriteRenderer.DOFade(1, fadeIn.time).SetDelay(fadeIn.delay).SetEase(fadeIn.ease)
//                    .SetUpdate(true);
//        }
//
//        var wait = Max(waitMoveIn, waitScaleIn, waitFadeIn);
//
//        TriggerAfterInParticle(wait - settings.particleTimeAdjust);
//
//        yield return CoroutineUtilities.WaitForRealTime(wait);
//        SubjectController.OnGlobalEffect.OnNext(settings.afterInCallBack);
//
//        busy = false;
//        state = RxAnimatorState.Idle;
//        afterInAction.Invoke();
//    }
//
//    protected override IEnumerator ThisDoAnimate_Out()
//    {
//        CheckRegister();
//        yield return CoroutineUtilities.WaitForRealTime(settings.delayAfterOut);
//        StartOutSound();
//
//        var waitMoveOut = (moveOut.active ? moveOut.time + moveOut.delay : 0) + moveOut.beforeAfterDelay;
//        var waitScaleOut = (scaleOut.active ? scaleOut.time + scaleOut.delay : 0) + scaleOut.beforeAfterDelay;
//        var waitFadeOut = (fadeOut.active ? fadeOut.time + fadeOut.delay : 0) + scaleOut.beforeAfterDelay;
//
//        if (moveOut.active || scaleOut.active || fadeOut.active)
//        {
//            busy = true;
//            state = RxAnimatorState.MovingOut;
//            yield return null;
//        }
//
//        if (moveOut.active)
//        {
//            if (moveOut.type == AnimatorMoveType.SpecificPosition || moveOut.type == AnimatorMoveType.LocationManager)
//            {
//                //transform.localPosition = _startPos;
//                if (_rectTransform == null)
//                {
//                    var tween = transform.DOLocalMove(moveOut.value, moveOut.time)
//                        .SetUpdate(true);
//                    if (moveOut.delay > 0)
//                        tween.SetDelay(moveOut.delay);
//                    if (_updatePoses.Length > 0)
//                        tween.OnUpdate(() =>
//                        {
//                            foreach (var rxAnimatorUpdatePose in _updatePoses)
//                                rxAnimatorUpdatePose.SetLocalPos(transform.localPosition);
//                        });
//                    if (moveOut.useCurve)
//                        tween.SetEase(moveOut.curve);
//                    else
//                        tween.SetEase(moveOut.ease);
//                }
//                else
//                {
//                    var tween = _rectTransform.DOAnchorPos(moveOut.value, moveOut.time)
//                        .SetUpdate(true);
//                    if (moveOut.delay > 0)
//                        tween.SetDelay(moveOut.delay);
//                    if (_updatePoses.Length > 0)
//                        tween.OnUpdate(() =>
//                        {
//                            foreach (var rxAnimatorUpdatePose in _updatePoses)
//                                rxAnimatorUpdatePose.SetLocalPos(transform.localPosition);
//                        });
//                    if (moveOut.useCurve)
//                        tween.SetEase(moveOut.curve);
//                    else
//                        tween.SetEase(moveOut.ease);
//                }
//            }
//        }
//
//        if (scaleOut.active)
//        {
//            //transform.localScale = _startScale;
//            //Debug.Log("scaleOut.value" + scaleOut.value + " - "+ "scaleOut.time" + scaleOut.time + " - ");
//            TweenerCore<Vector3, Vector3, VectorOptions> tween = null;
//            if (!scaleOut.pop)
//                tween = transform.DOScale(scaleOut.value, scaleOut.time).SetUpdate(true);
//            else
//                tween = transform.DOScale(scaleOut.value, scaleOut.time / 2).SetLoops(2, LoopType.Yoyo).SetUpdate(true);
//
//            if (scaleOut.delay > 0)
//                tween.SetDelay(scaleOut.delay);
//            if (scaleOut.useCurve)
//                tween.SetEase(scaleOut.curve);
//            else
//                tween.SetEase(scaleOut.ease);
//        }
//
//        if (fadeOut.active)
//        {
//            if (_canvasGroup != null)
//            {
//                if (fadeOut.turnOffRayCast)
//                    _canvasGroup.blocksRaycasts = false;
//                if (fadeOut.useCurve)
//                    _canvasGroup.DOFade(fadeOut.value, fadeOut.time).SetDelay(fadeOut.delay)
//                        .SetEase(fadeOut.curve)
//                        .SetUpdate(true);
//                else
//                    _canvasGroup.DOFade(fadeOut.value, fadeOut.time).SetDelay(fadeOut.delay)
//                        .SetEase(fadeOut.ease)
//                        .SetUpdate(true);
//            }
//            else
//            {
//                TweenerCore<Color, Color, ColorOptions> tween = null;
//                if (_image != null)
//                    tween = _image.DOFade(fadeOut.value, fadeOut.time);
//                else if (_spriteRenderer != null)
//                    tween = _spriteRenderer.DOFade(fadeOut.value, fadeOut.time);
//                else if (_textMesh != null)
//                    tween = _textMesh.DOFade(fadeOut.value, fadeOut.time);
//                else if (_text != null)
//                    tween = _text.DOFade(fadeOut.value, fadeOut.time);
//
//                tween.SetUpdate(true);
//                if (fadeOut.delay > 0)
//                    tween.SetDelay(fadeOut.delay);
//                if (fadeIn.useCurve)
//                    tween.SetEase(fadeOut.curve);
//                else
//                    tween.SetEase(fadeOut.ease);
//            }
//
//            foreach (var spriteRenderer in fadeRenderers)
//                spriteRenderer.DOFade(fadeOut.value, fadeOut.time).SetDelay(fadeOut.delay).SetEase(fadeOut.ease)
//                    .SetUpdate(true);
//        }
//
//        var wait = Max(waitMoveOut, waitScaleOut, waitFadeOut);
//        yield return CoroutineUtilities.WaitForRealTime(wait);
//
//        if (settings.deactiveAfterOut)
//            gameObject.SetActive(false);
//
//        busy = false;
//        state = RxAnimatorState.Reset;
//    }
//
//    protected override void OnDestroy()
//    {
//        base.OnDestroy();
//        transform.DOKill();
//    }
//
//    protected override IEnumerator ThisDoAnimate_InThenOut()
//    {
//        CheckRegister();
////        Debug.Log("_startScale"+_startScale +"/"+scaleIn.value);
//        TriggerStartParticle();
//        yield return CoroutineUtilities.WaitForRealTime(settings.DelayBeforeIn);
//        if (settings.whenSoundStartIn == WhenSoundStart.OnStartAnim)
//            StartInSound();
//
//        if (settings.deactiveAfterOut)
//            gameObject.SetActive(true);
//        var waitMoveIn = (moveIn.active ? moveIn.time + moveIn.delay : 0) + moveIn.beforeAfterDelay;
//        var waitScaleIn = (scaleIn.active ? scaleIn.time + scaleIn.delay : 0) + scaleIn.beforeAfterDelay;
//        var waitFadeIn = (fadeIn.active ? fadeIn.time + fadeIn.delay : 0) + fadeIn.beforeAfterDelay;
//
//        //ResetAnimation();
//        if (moveIn.active || scaleIn.active || fadeIn.active)
//        {
//            busy = true;
//            state = RxAnimatorState.MovingIn;
//        }
//
//        if (moveIn.active)
//        {
//            if (settings.whenSoundStartIn == WhenSoundStart.OnStartMove)
//                StartInSound(moveIn.delay);
//
//            if (_rectTransform == null)
//            {
//                var tween = transform.DOLocalMove(_startPos, moveIn.time)
//                    .SetUpdate(true);
//
//                if (_updatePoses.Length > 0)
//                    tween.OnUpdate(() =>
//                    {
//                        foreach (var rxAnimatorUpdatePose in _updatePoses)
//                            rxAnimatorUpdatePose.SetLocalPos(transform.localPosition);
//                    });
//                if (moveIn.delay > 0)
//                    tween.SetDelay(moveIn.delay);
//                if (settings.whenSoundStartIn == WhenSoundStart.OnFinishMove)
//                    tween.OnComplete(StartInSound);
//                if (moveIn.useCurve)
//                    tween.SetEase(moveIn.curve);
//                else
//                    tween.SetEase(moveIn.ease);
//            }
//            else
//            {
//                var tween = _rectTransform.DOAnchorPos(_startPos, moveIn.time)
//                    .SetUpdate(true);
//                if (_updatePoses.Length > 0)
//                    tween.OnUpdate(() =>
//                    {
//                        foreach (var rxAnimatorUpdatePose in _updatePoses)
//                            rxAnimatorUpdatePose.SetLocalPos(transform.localPosition);
//                    });
//                if (moveIn.delay > 0)
//                    tween.SetDelay(moveIn.delay);
//                if (settings.whenSoundStartIn == WhenSoundStart.OnFinishMove)
//                    tween.OnComplete(StartInSound);
//                if (moveIn.useCurve)
//                    tween.SetEase(moveIn.curve);
//                else
//                    tween.SetEase(moveIn.ease);
//            }
//        }
//
//        if (scaleIn.active)
//        {
//            if (settings.whenSoundStartIn == WhenSoundStart.OnStartScale)
//                StartInSound();
//            var tween = transform.DOScale(_startScale, scaleIn.time)
//                .SetUpdate(true);
//            if (scaleIn.delay > 0)
//                tween.SetDelay(scaleIn.delay);
//            if (settings.whenSoundStartIn == WhenSoundStart.OnFinishScale)
//                tween.OnComplete(StartInSound);
//            if (scaleIn.useCurve)
//                tween.SetEase(scaleIn.curve);
//            else
//                tween.SetEase(scaleIn.ease);
//        }
//
//        if (fadeIn.active)
//        {
//            if (settings.whenSoundStartIn == WhenSoundStart.OnStartFade)
//                StartInSound();
//
//            if (_canvasGroup != null)
//            {
//                var tween = _canvasGroup.DOFade(_startAlpha, fadeIn.time)
//                    .SetUpdate(true)
//                    .OnComplete(() =>
//                    {
//                        if (fadeIn.turnOffRayCast)
//                            _canvasGroup.blocksRaycasts = true;
//                        if (settings.whenSoundStartIn == WhenSoundStart.OnFinishFade)
//                            StartInSound();
//                    });
//
//                if (fadeIn.delay > 0)
//                    tween.SetDelay(fadeIn.delay);
//                if (fadeIn.useCurve)
//                    tween.SetEase(fadeIn.curve);
//                else
//                    tween.SetEase(fadeIn.ease);
//            }
//            else
//            {
//                TweenerCore<Color, Color, ColorOptions> tween = null;
//                if (_image != null)
//                    tween = _image.DOFade(_startAlpha, fadeIn.time);
//                else if (_spriteRenderer != null)
//                    tween = _spriteRenderer.DOFade(_startAlpha, fadeIn.time);
//                else if (_text != null)
//                    tween = _text.DOFade(_startAlpha, fadeIn.time);
//                else if (_textMesh != null)
//                    tween = _textMesh.DOFade(_startAlpha, fadeIn.time);
//
//                if (fadeIn.delay > 0)
//                    tween.SetDelay(fadeIn.delay);
//                if (settings.whenSoundStartIn == WhenSoundStart.OnFinishFade)
//                    tween.OnComplete(StartInSound);
//                if (fadeIn.useCurve)
//                    tween.SetEase(fadeIn.curve);
//                else
//                    tween.SetEase(fadeIn.ease);
//            }
//
//            foreach (var spriteRenderer in fadeRenderers)
//                spriteRenderer.DOFade(1, fadeIn.time).SetDelay(fadeIn.delay).SetEase(fadeIn.ease)
//                    .SetUpdate(true);
//        }
//
//        var wait = Max(waitMoveIn, waitScaleIn, waitFadeIn);
//
//        TriggerAfterInParticle(wait - settings.particleTimeAdjust);
//
//        yield return CoroutineUtilities.WaitForRealTime(wait);
//        SubjectController.OnGlobalEffect.OnNext(settings.afterInCallBack);
//        afterInAction.Invoke();
//
//        //||||||||||||||||||||||||||||| OUT |||||||||||||||||||||||||||||||||||
//        yield return CoroutineUtilities.WaitForRealTime(settings.delayAfterOut);
//        StartOutSound();
//
//        var waitMoveOut = (moveOut.active ? moveOut.time + moveOut.delay : 0) + moveOut.beforeAfterDelay;
//        var waitScaleOut = (scaleOut.active ? scaleOut.time + scaleOut.delay : 0) + scaleOut.beforeAfterDelay;
//        var waitFadeOut = (fadeOut.active ? fadeOut.time + fadeOut.delay : 0) + scaleOut.beforeAfterDelay;
//
//        if (moveOut.active || scaleOut.active || fadeOut.active)
//        {
//            busy = true;
//            state = RxAnimatorState.MovingOut;
//            yield return null;
//        }
//
//        if (moveOut.active)
//        {
//            if (moveOut.type == AnimatorMoveType.SpecificPosition || moveOut.type == AnimatorMoveType.LocationManager)
//            {
//                //transform.localPosition = _startPos;
//                if (_rectTransform == null)
//                {
//                    var tween = transform.DOLocalMove(moveOut.value, moveOut.time)
//                        .SetUpdate(true);
//                    if (moveOut.delay > 0)
//                        tween.SetDelay(moveOut.delay);
//                    if (_updatePoses.Length > 0)
//                        tween.OnUpdate(() =>
//                        {
//                            foreach (var rxAnimatorUpdatePose in _updatePoses)
//                                rxAnimatorUpdatePose.SetLocalPos(transform.localPosition);
//                        });
//                    if (moveOut.useCurve)
//                        tween.SetEase(moveOut.curve);
//                    else
//                        tween.SetEase(moveOut.ease);
//                }
//                else
//                {
//                    var tween = _rectTransform.DOAnchorPos(moveOut.value, moveOut.time)
//                        .SetUpdate(true);
//                    if (moveOut.delay > 0)
//                        tween.SetDelay(moveOut.delay);
//                    if (_updatePoses.Length > 0)
//                        tween.OnUpdate(() =>
//                        {
//                            foreach (var rxAnimatorUpdatePose in _updatePoses)
//                                rxAnimatorUpdatePose.SetLocalPos(transform.localPosition);
//                        });
//                    if (moveOut.useCurve)
//                        tween.SetEase(moveOut.curve);
//                    else
//                        tween.SetEase(moveOut.ease);
//                }
//            }
//        }
//
//        if (scaleOut.active)
//        {
//            //transform.localScale = _startScale;
//            //Debug.Log("scaleOut.value" + scaleOut.value + " - "+ "scaleOut.time" + scaleOut.time + " - ");
//            TweenerCore<Vector3, Vector3, VectorOptions> tween = null;
//            if (!scaleOut.pop)
//                tween = transform.DOScale(scaleOut.value, scaleOut.time).SetUpdate(true);
//            else
//                tween = transform.DOScale(scaleOut.value, scaleOut.time / 2).SetLoops(2, LoopType.Yoyo).SetUpdate(true);
//
//            if (scaleOut.delay > 0)
//                tween.SetDelay(scaleOut.delay);
//            if (scaleOut.useCurve)
//                tween.SetEase(scaleOut.curve);
//            else
//                tween.SetEase(scaleOut.ease);
//        }
//
//        if (fadeOut.active)
//        {
//            if (_canvasGroup != null)
//            {
//                if (fadeOut.turnOffRayCast)
//                    _canvasGroup.blocksRaycasts = false;
//                if (fadeOut.useCurve)
//                    _canvasGroup.DOFade(fadeOut.value, fadeOut.time).SetDelay(fadeOut.delay)
//                        .SetEase(fadeOut.curve)
//                        .SetUpdate(true);
//                else
//                    _canvasGroup.DOFade(fadeOut.value, fadeOut.time).SetDelay(fadeOut.delay)
//                        .SetEase(fadeOut.ease)
//                        .SetUpdate(true);
//            }
//            else
//            {
//                TweenerCore<Color, Color, ColorOptions> tween = null;
//                if (_image != null)
//                    tween = _image.DOFade(fadeOut.value, fadeOut.time);
//                else if (_spriteRenderer != null)
//                    tween = _spriteRenderer.DOFade(fadeOut.value, fadeOut.time);
//                else if (_textMesh != null)
//                    tween = _textMesh.DOFade(fadeOut.value, fadeOut.time);
//                else if (_text != null)
//                    tween = _text.DOFade(fadeOut.value, fadeOut.time);
//
//                tween.SetUpdate(true);
//                if (fadeOut.delay > 0)
//                    tween.SetDelay(fadeOut.delay);
//                if (fadeIn.useCurve)
//                    tween.SetEase(fadeOut.curve);
//                else
//                    tween.SetEase(fadeOut.ease);
//            }
//
//            foreach (var spriteRenderer in fadeRenderers)
//                spriteRenderer.DOFade(fadeOut.value, fadeOut.time).SetDelay(fadeOut.delay).SetEase(fadeOut.ease)
//                    .SetUpdate(true);
//        }
//
//        yield return CoroutineUtilities.WaitForRealTime(Max(waitMoveOut, waitScaleOut, waitFadeOut));
//
//        if (settings.deactiveAfterOut && gameObject != null)
//            gameObject.SetActive(false);
//        state = RxAnimatorState.Reset;
//        busy = false;
//
//        onStartParticleSystems.ForEach(x => { x.gameObject.SetActive(false); });
//
//        if (settings.nextInThenOut != null)
//            yield return settings.nextInThenOut.DoAnimate_InThenOut();
//    }
//
//    protected override IEnumerator ThisDoAnimate_InThenOutIdle()
//    {
//        CheckRegister();
//        if (settings.nextInThenOut != null)
//            settings.nextInThenOut.ResetAnimation();
//        yield return CoroutineUtilities.WaitForRealTime(inThenOutIdleDelay);
//
//        while (true)
//        {
//            ResetAnimation();
//            TriggerStartParticle();
//            yield return CoroutineUtilities.WaitForRealTime(settings.DelayBeforeIn);
//            if (settings.whenSoundStartIn == WhenSoundStart.OnStartAnim)
//                StartInSound();
//
//            if (settings.deactiveAfterOut)
//                gameObject.SetActive(true);
//            var waitMoveIn = (moveIn.active ? moveIn.time + moveIn.delay : 0) + moveIn.beforeAfterDelay;
//            var waitScaleIn = (scaleIn.active ? scaleIn.time + scaleIn.delay : 0) + scaleIn.beforeAfterDelay;
//            var waitFadeIn = (fadeIn.active ? fadeIn.time + fadeIn.delay : 0) + fadeIn.beforeAfterDelay;
//
//            //ResetAnimation();
//            if (moveIn.active || scaleIn.active || fadeIn.active)
//            {
//                busy = true;
//                state = RxAnimatorState.MovingIn;
//            }
//
//            if (moveIn.active)
//            {
//                if (settings.whenSoundStartIn == WhenSoundStart.OnStartMove)
//                    StartInSound(moveIn.delay);
//
//                if (_rectTransform == null)
//                {
//                    var tween = transform.DOLocalMove(_startPos, moveIn.time)
//                        .SetUpdate(true);
//
//                    if (_updatePoses.Length > 0)
//                        tween.OnUpdate(() =>
//                        {
//                            foreach (var rxAnimatorUpdatePose in _updatePoses)
//                                rxAnimatorUpdatePose.SetLocalPos(transform.localPosition);
//                        });
//                    if (moveIn.delay > 0)
//                        tween.SetDelay(moveIn.delay);
//                    if (settings.whenSoundStartIn == WhenSoundStart.OnFinishMove)
//                        tween.OnComplete(StartInSound);
//                    if (moveIn.useCurve)
//                        tween.SetEase(moveIn.curve);
//                    else
//                        tween.SetEase(moveIn.ease);
//                }
//                else
//                {
//                    var tween = _rectTransform.DOAnchorPos(_startPos, moveIn.time)
//                        .SetUpdate(true);
//                    if (_updatePoses.Length > 0)
//                        tween.OnUpdate(() =>
//                        {
//                            foreach (var rxAnimatorUpdatePose in _updatePoses)
//                                rxAnimatorUpdatePose.SetLocalPos(transform.localPosition);
//                        });
//                    if (moveIn.delay > 0)
//                        tween.SetDelay(moveIn.delay);
//                    if (settings.whenSoundStartIn == WhenSoundStart.OnFinishMove)
//                        tween.OnComplete(StartInSound);
//                    if (moveIn.useCurve)
//                        tween.SetEase(moveIn.curve);
//                    else
//                        tween.SetEase(moveIn.ease);
//                }
//            }
//
//            if (scaleIn.active)
//            {
//                if (settings.whenSoundStartIn == WhenSoundStart.OnStartScale)
//                    StartInSound();
//                var tween = transform.DOScale(_startScale, scaleIn.time)
//                    .SetUpdate(true);
//                if (scaleIn.delay > 0)
//                    tween.SetDelay(scaleIn.delay);
//                if (settings.whenSoundStartIn == WhenSoundStart.OnFinishScale)
//                    tween.OnComplete(StartInSound);
//                if (scaleIn.useCurve)
//                    tween.SetEase(scaleIn.curve);
//                else
//                    tween.SetEase(scaleIn.ease);
//            }
//
//            if (fadeIn.active)
//            {
//                if (settings.whenSoundStartIn == WhenSoundStart.OnStartFade)
//                    StartInSound();
//
//                if (_canvasGroup != null)
//                {
//                    var tween = _canvasGroup.DOFade(_startAlpha, fadeIn.time)
//                        .SetUpdate(true)
//                        .OnComplete(() =>
//                        {
//                            if (fadeIn.turnOffRayCast)
//                                _canvasGroup.blocksRaycasts = true;
//                            if (settings.whenSoundStartIn == WhenSoundStart.OnFinishFade)
//                                StartInSound();
//                        });
//
//                    if (fadeIn.delay > 0)
//                        tween.SetDelay(fadeIn.delay);
//                    if (fadeIn.useCurve)
//                        tween.SetEase(fadeIn.curve);
//                    else
//                        tween.SetEase(fadeIn.ease);
//                }
//                else
//                {
//                    TweenerCore<Color, Color, ColorOptions> tween = null;
//                    if (_image != null)
//                        tween = _image.DOFade(_startAlpha, fadeIn.time);
//                    else if (_spriteRenderer != null)
//                        tween = _spriteRenderer.DOFade(_startAlpha, fadeIn.time);
//                    else if (_text != null)
//                        tween = _text.DOFade(_startAlpha, fadeIn.time);
//                    else if (_textMesh != null)
//                        tween = _textMesh.DOFade(_startAlpha, fadeIn.time);
//
//                    if (fadeIn.delay > 0)
//                        tween.SetDelay(fadeIn.delay);
//                    if (settings.whenSoundStartIn == WhenSoundStart.OnFinishFade)
//                        tween.OnComplete(StartInSound);
//                    if (fadeIn.useCurve)
//                        tween.SetEase(fadeIn.curve);
//                    else
//                        tween.SetEase(fadeIn.ease);
//                }
//
//                foreach (var spriteRenderer in fadeRenderers)
//                    spriteRenderer.DOFade(1, fadeIn.time).SetDelay(fadeIn.delay).SetEase(fadeIn.ease)
//                        .SetUpdate(true);
//            }
//
//            var wait = Max(waitMoveIn, waitScaleIn, waitFadeIn);
//
//            TriggerAfterInParticle(wait - settings.particleTimeAdjust);
//
//            yield return CoroutineUtilities.WaitForRealTime(wait);
//            SubjectController.OnGlobalEffect.OnNext(settings.afterInCallBack);
//            afterInAction.Invoke();
//
//            //||||||||||||||||||||||||||||| OUT |||||||||||||||||||||||||||||||||||
//            yield return CoroutineUtilities.WaitForRealTime(settings.delayAfterOut);
//            StartOutSound();
//
//            var waitMoveOut = (moveOut.active ? moveOut.time + moveOut.delay : 0) + moveOut.beforeAfterDelay;
//            var waitScaleOut = (scaleOut.active ? scaleOut.time + scaleOut.delay : 0) + scaleOut.beforeAfterDelay;
//            var waitFadeOut = (fadeOut.active ? fadeOut.time + fadeOut.delay : 0) + scaleOut.beforeAfterDelay;
//
//            if (moveOut.active || scaleOut.active || fadeOut.active)
//            {
//                busy = true;
//                state = RxAnimatorState.MovingOut;
//                yield return null;
//            }
//
//            if (moveOut.active)
//            {
//                if (moveOut.type == AnimatorMoveType.SpecificPosition ||
//                    moveOut.type == AnimatorMoveType.LocationManager)
//                {
//                    //transform.localPosition = _startPos;
//                    if (_rectTransform == null)
//                    {
//                        var tween = transform.DOLocalMove(moveOut.value, moveOut.time)
//                            .SetUpdate(true);
//                        if (moveOut.delay > 0)
//                            tween.SetDelay(moveOut.delay);
//                        if (_updatePoses.Length > 0)
//                            tween.OnUpdate(() =>
//                            {
//                                foreach (var rxAnimatorUpdatePose in _updatePoses)
//                                    rxAnimatorUpdatePose.SetLocalPos(transform.localPosition);
//                            });
//                        if (moveOut.useCurve)
//                            tween.SetEase(moveOut.curve);
//                        else
//                            tween.SetEase(moveOut.ease);
//                    }
//                    else
//                    {
//                        var tween = _rectTransform.DOAnchorPos(moveOut.value, moveOut.time)
//                            .SetUpdate(true);
//                        if (moveOut.delay > 0)
//                            tween.SetDelay(moveOut.delay);
//                        if (_updatePoses.Length > 0)
//                            tween.OnUpdate(() =>
//                            {
//                                foreach (var rxAnimatorUpdatePose in _updatePoses)
//                                    rxAnimatorUpdatePose.SetLocalPos(transform.localPosition);
//                            });
//                        if (moveOut.useCurve)
//                            tween.SetEase(moveOut.curve);
//                        else
//                            tween.SetEase(moveOut.ease);
//                    }
//                }
//            }
//
//            if (scaleOut.active)
//            {
//                //transform.localScale = _startScale;
//                //Debug.Log("scaleOut.value" + scaleOut.value + " - "+ "scaleOut.time" + scaleOut.time + " - ");
//                TweenerCore<Vector3, Vector3, VectorOptions> tween = null;
//                if (!scaleOut.pop)
//                    tween = transform.DOScale(scaleOut.value, scaleOut.time).SetUpdate(true);
//                else
//                    tween = transform.DOScale(scaleOut.value, scaleOut.time / 2).SetLoops(2, LoopType.Yoyo)
//                        .SetUpdate(true);
//
//                if (scaleOut.delay > 0)
//                    tween.SetDelay(scaleOut.delay);
//                if (scaleOut.useCurve)
//                    tween.SetEase(scaleOut.curve);
//                else
//                    tween.SetEase(scaleOut.ease);
//            }
//
//            if (fadeOut.active)
//            {
//                if (_canvasGroup != null)
//                {
//                    if (fadeOut.turnOffRayCast)
//                        _canvasGroup.blocksRaycasts = false;
//                    if (fadeOut.useCurve)
//                        _canvasGroup.DOFade(fadeOut.value, fadeOut.time).SetDelay(fadeOut.delay)
//                            .SetEase(fadeOut.curve)
//                            .SetUpdate(true);
//                    else
//                        _canvasGroup.DOFade(fadeOut.value, fadeOut.time).SetDelay(fadeOut.delay)
//                            .SetEase(fadeOut.ease)
//                            .SetUpdate(true);
//                }
//                else
//                {
//                    TweenerCore<Color, Color, ColorOptions> tween = null;
//                    if (_image != null)
//                        tween = _image.DOFade(fadeOut.value, fadeOut.time);
//                    else if (_spriteRenderer != null)
//                        tween = _spriteRenderer.DOFade(fadeOut.value, fadeOut.time);
//                    else if (_textMesh != null)
//                        tween = _textMesh.DOFade(fadeOut.value, fadeOut.time);
//                    else if (_text != null)
//                        tween = _text.DOFade(fadeOut.value, fadeOut.time);
//
//                    tween.SetUpdate(true);
//                    if (fadeOut.delay > 0)
//                        tween.SetDelay(fadeOut.delay);
//                    if (fadeIn.useCurve)
//                        tween.SetEase(fadeOut.curve);
//                    else
//                        tween.SetEase(fadeOut.ease);
//                }
//
//                foreach (var spriteRenderer in fadeRenderers)
//                    spriteRenderer.DOFade(fadeOut.value, fadeOut.time).SetDelay(fadeOut.delay).SetEase(fadeOut.ease)
//                        .SetUpdate(true);
//            }
//
//            yield return CoroutineUtilities.WaitForRealTime(Max(waitMoveOut, waitScaleOut, waitFadeOut));
//            if (settings.nextInThenOut != null)
//                yield return settings.nextInThenOut.DoAnimate_InThenOut();
//        }
//    }
//
//    /// ________ Helper _______________________
//    private static float Max(float value1, float value2)
//    {
//        float max = value1;
//        if (value2 > max)
//            max = value2;
//
//        return max;
//    }
//
//    private static float Max(float value1, float value2, float value3)
//    {
//        float max = value1;
//        if (value2 > max)
//            max = value2;
//        if (value3 > max)
//            max = value3;
//
//        return max;
//    }
//
//    private static float Max(float value1, float value2, float value3, float value4)
//    {
//        float max = value1;
//        if (value2 > max)
//            max = value2;
//        if (value3 > max)
//            max = value3;
//        if (value4 > max)
//            max = value4;
//
//        return max;
//    }
//}