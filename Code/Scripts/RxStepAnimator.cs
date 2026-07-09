using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[System.Serializable]
public class StepAnimatorData
{
    public RxAnimatorMove move;
    public RxAnimatorFade fade;
    public RxAnimatorScale scale;
}

public class RxStepAnimator : BaseRxStepAnimator
{
    [SerializeField] private bool moveLocal;
    [SerializeField] private bool useCurrentPos;

    [SerializeField] [ConditionalField(nameof(useCurrentPos), new object[] {true}, true)]
    private Vector3 startPos;

    [SerializeField] private bool useCurrentFade;

    [SerializeField] [ConditionalField(nameof(useCurrentFade), new object[] {true}, true)]
    private float startFade;

    [SerializeField] private bool useCurrentScale;

    [SerializeField] [ConditionalField(nameof(useCurrentScale), new object[] {true}, true)]
    private Vector3 startScale = new Vector3(1, 1, 1);

    [SerializeField] private StepAnimatorData[] steps;
    [SerializeField] private SpriteRenderer[] fadeRenderers;
    [SerializeField] private RxAnimatorUpdatePos[] _updatePoses;

    private RectTransform _rectTransform;


    private CanvasGroup _canvasGroup;
    private Image _image;
    private SpriteRenderer _spriteRenderer;
    private Text _text;
    private TMP_Text _textMesh;
    [SerializeField] private bool moveTowards;
    [SerializeField] private float speed = 10;

    protected override void Register()
    {
        base.Register();
        _rectTransform = GetComponent<RectTransform>();
        foreach (var stepAnimatorData in steps)
            if (stepAnimatorData.fade.active)
            {
                _canvasGroup = GetComponent<CanvasGroup>();
                _image = GetComponent<Image>();
                _spriteRenderer = GetComponent<SpriteRenderer>();
                _text = GetComponent<Text>();
                _textMesh = GetComponent<TMP_Text>();
                break;
            }
    }

    public override void Animator_In()
    {
        From = 0;
        To = steps.Length - 1;
        base.Animator_In();
    }

    public override void Animator_InThenOut()
    {
        From = 0;
        To = steps.Length - 1;
        base.Animator_InThenOut();
    }

    public override void Animator_Out()
    {
        From = 0;
        To = steps.Length - 1;
        base.Animator_In();
    }

    public override bool HasOut()
    {
        return steps.Length > 0;
    }

    public override bool HasIn()
    {
        return steps.Length > 0;
    }

    public override void ResetAnimation(bool setAcive = false)
    {
        CheckRegister();
        base.ResetAnimation(setAcive);
        state = RxAnimatorState.Reset;

        if (_canvasGroup != null)
            _canvasGroup.DOKill();
        else if (_image != null)
            _image.DOKill();
        else if (_spriteRenderer != null)
            _spriteRenderer.DOKill();
        else if (_textMesh != null)
            _textMesh.DOKill();
        else if (_text != null)
            _text.DOKill();

        foreach (var spriteRenderer in fadeRenderers)
            spriteRenderer.DOKill();


        if (!useCurrentPos)
        {
            if (moveLocal)
            {
                if (_rectTransform == null)
                    transform.localPosition = startPos;
                else
                    _rectTransform.anchoredPosition = startPos;
            }
            else
            {
                if (_rectTransform == null)
                    transform.position = startPos;
                else
                    _rectTransform.anchoredPosition = startPos;
            }
        }

        if (!useCurrentScale)
            transform.localScale = startScale;
        settings.onStartParticleSystems.ForEach(x => { x.gameObject.SetActive(false); });
    }


    public override void SetAtAnimationIn(bool setActive = false)
    {
        CheckRegister();
        base.SetAtAnimationIn(setActive);
        state = RxAnimatorState.Idle;
        ResetAnimation();
    }

    protected override IEnumerator ThisDoAnimate_In()
    {
        CheckRegister();
        TriggerStartParticle();
        yield return CoroutineUtilities.WaitForRealTime(settings.DelayBeforeIn);
        if (settings.whenSoundStartIn == WhenSoundStart.OnStartAnim)
            StartInSound();
        if (settings.deactiveAfterOut)
            gameObject.SetActive(true);

        for (int i = From; i <= To; i++)
        {
            var data = steps[i];
            var waitMoveIn = (data.move.active ? data.move.time + data.move.delay : 0) + data.move.beforeAfterDelay;
            var waitScaleIn = (data.scale.active ? data.scale.time + data.scale.delay : 0) +
                              data.scale.beforeAfterDelay;
            var waitFadeIn = (data.fade.active ? data.fade.time + data.fade.delay : 0) + data.fade.beforeAfterDelay;

            //ResetAnimation();
            if (data.move.active || data.scale.active || data.fade.active)
            {
                busy = true;
                state = RxAnimatorState.MovingIn;
            }

            if (data.scale.active)
            {
                if (settings.whenSoundStartIn == WhenSoundStart.OnStartScale)
                    StartInSound(data.scale.delay);
                TweenerCore<Vector3, Vector3, VectorOptions> tween =
                    transform.DOScale(data.scale.value, data.scale.time);
                if (data.scale.pop)
                    tween.SetLoops(2, LoopType.Yoyo);
                tween.SetUpdate(true);
                if (settings.whenSoundStartIn == WhenSoundStart.OnFinishScale)
                    tween.OnComplete(StartInSound);

                if (data.scale.delay > 0)
                    tween.SetDelay(data.scale.delay);
                if (data.scale.useCurve)
                    tween.SetEase(data.scale.curve);
                else
                    tween.SetEase(data.scale.ease);
            }

            if (data.fade.active)
            {
                if (settings.whenSoundStartIn == WhenSoundStart.OnStartFade)
                    StartInSound(data.fade.delay);

                if (_canvasGroup != null)
                {
                    var tween = _canvasGroup.DOFade(data.fade.value, data.fade.time)
                        .SetUpdate(true)
                        .OnComplete(() =>
                        {
                            if (data.fade.turnOffRayCast)
                                _canvasGroup.blocksRaycasts = true;
                            if (settings.whenSoundStartIn == WhenSoundStart.OnFinishFade)
                                StartInSound();
                        });

                    if (data.fade.delay > 0)
                        tween.SetDelay(data.fade.delay);
                    if (data.fade.useCurve)
                        tween.SetEase(data.fade.curve);
                    else
                        tween.SetEase(data.fade.ease);
                }
                else
                {
                    TweenerCore<Color, Color, ColorOptions> tween = null;
                    if (_image != null)
                        tween = _image.DOFade(data.fade.value, data.fade.time);
                    else if (_spriteRenderer != null)
                        tween = _spriteRenderer.DOFade(data.fade.value, data.fade.time);
                    else if (_text != null)
                        tween = _text.DOFade(data.fade.value, data.fade.time);
                    else if (_textMesh != null)
                        tween = _textMesh.DOFade(data.fade.value, data.fade.time);

                    if (data.fade.delay > 0)
                        tween.SetDelay(data.fade.delay);
                    if (settings.whenSoundStartIn == WhenSoundStart.OnFinishFade)
                        tween.OnComplete(StartInSound);
                    if (data.fade.useCurve)
                        tween.SetEase(data.fade.curve);
                    else
                        tween.SetEase(data.fade.ease);
                }

                if (fadeRenderers.Length > 0)
                    foreach (var spriteRenderer in fadeRenderers)
                        spriteRenderer.DOFade(1, data.fade.time).SetDelay(data.fade.delay).SetEase(data.fade.ease)
                            .SetUpdate(true);
            }

            if (data.move.active)
            {
                if (settings.whenSoundStartIn == WhenSoundStart.OnStartMove)
                    StartInSound(data.move.delay);

                if (!moveTowards)
                {
                    if (_rectTransform == null)
                    {
                        var tf = transform;
                        var target = data.move.GetTarget(tf, moveLocal, startPos);
                        var tween = moveLocal
                            ? tf.DOLocalMove(target, data.move.time)
                            : tf.DOMove(target, data.move.time);
                        tween.SetUpdate(true);
                        if (_updatePoses.Length > 0)
                            tween.OnUpdate(() =>
                            {
                                foreach (var rxAnimatorUpdatePose in _updatePoses)
                                    rxAnimatorUpdatePose.SetLocalPos(tf.localPosition);
                            });
                        if (data.move.delay > 0)
                            tween.SetDelay(data.move.delay);
                        if (settings.whenSoundStartIn == WhenSoundStart.OnFinishMove)
                            tween.OnComplete(StartInSound);
                        if (data.move.useCurve)
                            tween.SetEase(data.move.curve);
                        else
                            tween.SetEase(data.move.ease);
                    }
                    else
                    {
                        var target = data.move.GetTargetRt(_rectTransform, moveLocal, startPos);
                        var tween = _rectTransform.DOAnchorPos(target, data.move.time)
                            .SetUpdate(true);
                        if (_updatePoses.Length > 0)
                            tween.OnUpdate(() =>
                            {
                                foreach (var rxAnimatorUpdatePose in _updatePoses)
                                    rxAnimatorUpdatePose.SetLocalPos(transform.localPosition);
                            });
                        if (data.move.delay > 0)
                            tween.SetDelay(data.move.delay);
                        if (settings.whenSoundStartIn == WhenSoundStart.OnFinishMove)
                            tween.OnComplete(StartInSound);
                        if (data.move.useCurve)
                            tween.SetEase(data.move.curve);
                        else
                            tween.SetEase(data.move.ease);
                    }
                }
                else
                {
                    if (data.move.delay > 0)
                        yield return new WaitForSeconds(data.move.delay);

                    var target = RootView.rootView.screenRoot.GetLocation(data.move.locationEnum);
                    var sp = speed;
                    if(target == null)
                        Debug.LogError("null target");
                    while (target != null && Mathf.Abs((transform.position - target.position).Magnitude2()) > 0.1f)
                    {
                        var d = Time.deltaTime * sp;
                        if (moveTowardVelocity > 0.1f)
                            sp += Time.deltaTime * moveTowardVelocity;
                        transform.position = Vector2.MoveTowards(transform.position, target.position, d);
                        yield return null;
                    }
                }
            }


            if (!moveTowards)
            {
                var wait = Max(waitMoveIn, waitScaleIn, waitFadeIn);
                TriggerAfterInParticle(wait - settings.particleTimeAdjust);
                yield return CoroutineUtilities.WaitForRealTime(wait);
            }
            else
            {
                TriggerAfterInParticle(0);
            }
        }


        busy = false;
        state = RxAnimatorState.Idle;
        InvokeCallBack();

        TriggerStopParticle();
        yield return CoroutineUtilities.WaitForRealTime(settings.delayAfterOut);
        settings.toggleOnOut.OnChanged(settings.toggleOutValue);

        if (settings.deactiveAfterOut)
        {
            gameObject.SetActive(false);
            ResetAnimation();
        }
    }

    [SerializeField] private float moveTowardVelocity;

    protected override IEnumerator ThisDoAnimate_Out()
    {
        yield return null;
    }

    protected override IEnumerator ThisDoAnimate_InThenOut() => ThisDoAnimate_In();


    private static float Max(float value1, float value2)
    {
        float max = value1;
        if (value2 > max)
            max = value2;

        return max;
    }

    private static float Max(float value1, float value2, float value3)
    {
        float max = value1;
        if (value2 > max)
            max = value2;
        if (value3 > max)
            max = value3;

        return max;
    }

    private static float Max(float value1, float value2, float value3, float value4)
    {
        float max = value1;
        if (value2 > max)
            max = value2;
        if (value3 > max)
            max = value3;
        if (value4 > max)
            max = value4;

        return max;
    }
}