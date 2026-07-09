using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UniRx;
using UnityEngine.UI;

public class RxIdleAnimator : BaseRxAnimator
{
    [SerializeField] private bool unscaledTime;
    [SerializeField] private  float waitToOut;
    [Header("Scale")]
    // Idle
    [SerializeField] public  bool scale;
    [ConditionalField("scale")] [SerializeField] private AnimatorLoopType scaleLoopType = AnimatorLoopType.PingPong;
    [ConditionalField("scale")] [SerializeField] private  Vector3 scaleFrom = new Vector3(1,1,1);
    [ConditionalField("scale")] [SerializeField] private  Vector3 scaleTo = new Vector3(1.1f,1.1f,1.1f);
    [ConditionalField("scale")] [SerializeField] private  AnimationCurve scaleCurve = AnimationCurve.Linear(0.0f, 0.0f, 1.0f, 1.0f);
    [ConditionalField("scale")] [SerializeField] private  float scaleTime = 1;
    [Header("Fade")]
    [SerializeField] public  bool fade;
    [ConditionalField("fade")] [SerializeField] private  AnimatorLoopType fadeLoopType = AnimatorLoopType.PingPong;
    [ConditionalField("fade")] [SerializeField] private  float fadeFrom = 1;
    [ConditionalField("fade")] [SerializeField] private  float fadeTo = 0;
    [ConditionalField("fade")] [SerializeField] private  float fadeTime = 1;
    [ConditionalField("fade")] [SerializeField] private  AnimationCurve fadeCurve = AnimationCurve.Linear(0.0f, 0.0f, 1.0f, 1.0f);
    [Header("Rotate")]
    [SerializeField] private  bool rotate;
    [ConditionalField("rotate")] [SerializeField] private  AnimatorLoopType rotateLoopType = AnimatorLoopType.OneDirection;
    [ConditionalField("rotate")] [SerializeField] private  Vector3 rotateFrom = Vector3.zero;
    [ConditionalField("rotate")] [SerializeField] private  Vector3 rotateTo = new Vector3(0, 0, 360);
    [ConditionalField("rotate")] [SerializeField] private  float rotateTime = 3;
    [ConditionalField("rotate")] [SerializeField] private  AnimationCurve rotateCurve = AnimationCurve.Linear(0.0f, 0.0f, 1.0f, 1.0f);
    
    [SerializeField] private  bool rotateAdd;
    [ConditionalField("rotateAdd")] [SerializeField] private  float rotateSpeed = 5;

    [ConditionalField("rotateAdd")] [SerializeField]
    private RotateCoordinate rotateCoordinate ;

    public enum RotateCoordinate
    {
        X,
        Y,
        Z
    }
    
    [SerializeField] private  bool move;
    [ConditionalField("move")] [SerializeField] private  AnimatorLoopType moveLoopType = AnimatorLoopType.OneDirection;
    [ConditionalField("move")] [SerializeField] private  Vector3 moveFrom = Vector3.zero;
    [ConditionalField("move")] [SerializeField] private  Vector3 moveTo = new Vector3(10, 0, 0);
    [ConditionalField("move")] [SerializeField] private  float moveTime = 3;
    [ConditionalField("move")] [SerializeField] private  AnimationCurve moveCurve = AnimationCurve.Linear(0.0f, 0.0f, 1.0f, 1.0f);
    [SerializeField] private  float idleTime = -1;
    [SerializeField] private int loopCount = -1;

    private Vector3 _originSize;
    private CanvasGroup _canvasGroup;
    private Image _image;
    private SpriteRenderer _spriteRenderer;
    private Text _text;
    private TMP_Text _textMesh;
    private float _startAlpha;
    private Vector3 _startRotation;

    private bool _stop;
    
    [MyButton(nameof(SetLocalIn), nameof(SetLocalOut))]
    [SerializeField] protected  int setlocal;

    [SerializeField] private float startFrom;
    public void SetLocalIn()
    {
        moveFrom = transform.localPosition;
    }
    public void SetLocalOut()
    {
        moveTo = transform.localPosition;
    }

    protected override void OnEnable()
    {
        base.OnEnable();
    }

    public override bool HasOut()
    {
        return false;
    }

    public override bool HasIn()
    {
        return scale || fade || rotate;
    }

    protected override void Register()
    {
        base.Register();
        _originSize = transform.localScale;
        _startRotation = transform.localEulerAngles;

        _canvasGroup = GetComponent<CanvasGroup>();
        _image = GetComponent<Image>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _text = GetComponent<Text>();
        _textMesh = GetComponent<TMP_Text>();

        if (_canvasGroup != null)
            _startAlpha = _canvasGroup.alpha;
        else if (_image != null)
            _startAlpha = _image.color.a;
        else if (_spriteRenderer != null)
            _startAlpha = _spriteRenderer.color.a;
        else if (_text != null)
            _startAlpha = _text.color.a;
        else if (_textMesh != null)
            _startAlpha = _textMesh.color.a;
    }

    public override void ResetAnimation(bool setAcive = false)
    {
        base.ResetAnimation(setAcive);
        _stop = true;
        if (settings.deactiveAfterOut)
            gameObject.SetActive(false);
        CheckRegister();

        if(move)
            transform.localPosition = moveFrom;
        transform.localScale = scale ? scaleFrom : _originSize;
        if (fade)
        {
            if (_canvasGroup != null)
                _canvasGroup.alpha = fadeFrom;
            else if (_image != null)
                _image.color = _image.color.Fade(fadeFrom);
            else if (_spriteRenderer != null)
                _spriteRenderer.color = _spriteRenderer.color.Fade(fadeFrom);
            else if (_textMesh != null)
                _textMesh.color = _textMesh.color.Fade(fadeFrom);
            else if (_text != null)
                _text.color = _text.color.Fade(fadeFrom);
        }
        else
        {
            if (_canvasGroup != null)
                _canvasGroup.alpha = _startAlpha;
            else if (_image != null)
                _image.color = _image.color.Fade(_startAlpha);
            else if (_spriteRenderer != null)
                _spriteRenderer.color = _spriteRenderer.color.Fade(_startAlpha);
            else if (_textMesh != null)
                _textMesh.color = _textMesh.color.Fade(_startAlpha);
            else if (_text != null)
                _text.color = _text.color.Fade(_startAlpha);
        }
      

        transform.localEulerAngles = _startRotation;
    }

    private IDisposable _dispose;
    public override void Animator_In()
    {
        //base.Animator_In();
        if (_dispose != null)
            _dispose.Dispose();
        //ResetAnimation();

        _dispose = IO_AnimatorIn().Subscribe();
    }

    public override void Animator_Out()
    {
        base.Animator_Out();
        if(_dispose != null)
            _dispose.Dispose();
        ResetAnimation();
    }

    protected override IEnumerator ThisDoAnimate_In()
    {
        ResetAnimation();
        Observable.FromCoroutine(DoAnimate_Loop).TakeUntilDisable(this).Subscribe();
        yield return null;
    }
    

    private IEnumerator DoAnimate_Loop()
    {
        CheckRegister();
        busy = true;
        state = RxAnimatorState.MovingIn;
        if (settings.deactiveAfterOut && HasIn())
            gameObject.SetActive(false);
        
        bool flipScale = false;
        bool flipFade = false;
        bool flipRotate = false;
        bool flipMove = false;
        float timeScale = startFrom * scaleTime;
        float timeFade = startFrom * fadeTime;
        float timeRotate =  startFrom * rotateTime;
        float timeMove = startFrom * moveTime;
        float time = 0;
        if (move)
        {
            transform.localPosition = Vector3.Lerp(moveFrom, moveTo,
                moveCurve.Evaluate(moveLoopType == AnimatorLoopType.PingPong && flipMove
                    ? (1 - timeMove / moveTime)
                    : timeMove / moveTime));
        }
        
        if(unscaledTime)
            yield return new WaitForSecondsRealtime(settings.DelayBeforeIn);
        else
            yield return new WaitForSeconds(settings.DelayBeforeIn);
    
        if (settings.deactiveAfterOut)
            gameObject.SetActive(true);

   
        
        

        _stop = false;
        int count = 0;
        while (!_stop)
        {
            if (idleTime < 0)
            {
                if (unscaledTime)
                {
                    timeScale += Time.unscaledDeltaTime;
                    timeFade += Time.unscaledDeltaTime;
                    timeRotate += Time.unscaledDeltaTime;
                    timeMove += Time.unscaledDeltaTime;
                }
                else
                {
                    timeScale += Time.deltaTime;
                    timeFade += Time.deltaTime;
                    timeRotate += Time.deltaTime;
                    timeMove += Time.deltaTime; 
                }

                if (scale)
                {
                    if (timeScale > scaleTime)
                    {
                        flipScale = !flipScale;
                        timeScale = timeScale % scaleTime;
                        
                        if (loopCount > 0)
                        {
                            count++;
                            if(count >= loopCount)
                                break;
                        }
                    }

                    var t = scaleLoopType == AnimatorLoopType.PingPong && flipScale
                        ? (1 - timeScale / scaleTime)
                        : timeScale / scaleTime;

                    t = scaleCurve.Evaluate(t);
                    transform.localScale = Vector3.Lerp(scaleFrom, scaleTo, t);
                }

                if (fade)
                {
                    if (timeFade > fadeTime)
                    {
                        flipFade = !flipFade;
                        timeFade = timeFade % fadeTime;
                        
                        if (loopCount > 0)
                        {
                            count++;
                            if(count >= loopCount)
                                break;
                        }
                    }

                    var t = fadeLoopType == AnimatorLoopType.PingPong && flipFade
                        ? (1 - timeFade / fadeTime)
                        : timeFade / fadeTime;

                    t = fadeCurve.Evaluate(t);

                    if (_canvasGroup != null)
                        _canvasGroup.alpha = Mathf.Lerp(fadeFrom, fadeTo, t);
                    else if (_image != null)
                        _image.color = _image.color.Fade(Mathf.Lerp(fadeFrom, fadeTo, t));
                    else if (_spriteRenderer != null)
                        _spriteRenderer.color = _spriteRenderer.color.Fade(Mathf.Lerp(fadeFrom, fadeTo, t));
                    else if (_textMesh != null)
                        _textMesh.color = _textMesh.color.Fade(Mathf.Lerp(fadeFrom, fadeTo, t));
                    else if (_text != null)
                        _text.color = _text.color.Fade(Mathf.Lerp(fadeFrom, fadeTo, t));
                }

                if (rotate)
                {
                    if (timeRotate > rotateTime)
                    {
                        flipRotate = !flipRotate;
                        timeRotate = timeRotate % rotateTime;
                        
                        if (loopCount > 0)
                        {
                            count++;
                            if(count >= loopCount)
                                break;
                        }
                    }

                    var t = rotateLoopType == AnimatorLoopType.PingPong && flipRotate
                        ? (1 - timeRotate / rotateTime)
                        : timeRotate / rotateTime;

                    t = rotateCurve.Evaluate(t);
                    transform.localEulerAngles = Vector3.Lerp(rotateFrom, rotateTo, t);
                }
                if (move)
                {
                    if (timeMove > moveTime)
                    {
                        flipMove = !flipMove;
                        timeMove = timeMove % moveTime;
                        
                        if (loopCount > 0)
                        {
                            count++;
                            if(count >= loopCount)
                                break;
                        }
                    }

                    var t = moveLoopType == AnimatorLoopType.PingPong && flipMove
                        ? (1 - timeMove / moveTime)
                        : timeMove / moveTime;

                    t = moveCurve.Evaluate(t);
                    transform.localPosition = Vector3.Lerp(moveFrom, moveTo, t);
                }

                if (rotateAdd)
                {
                    switch (rotateCoordinate)
                    {
                        case RotateCoordinate.X:
                            transform.Rotate(new Vector3(Time.deltaTime * rotateSpeed, 0, 0));
                            break;
                        case RotateCoordinate.Y:
                            transform.Rotate(new Vector3(0,Time.deltaTime * rotateSpeed,  0));
                            break;
                        case RotateCoordinate.Z:
                            transform.Rotate(new Vector3(0,0,Time.deltaTime * rotateSpeed ));
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }
            else
            {
                if (unscaledTime)
                    time += Time.unscaledDeltaTime;
                else
                    time += Time.deltaTime;
               
                if (time > idleTime)
                {
                    flipScale = !flipScale;
                    flipFade = !flipFade;
                    time = time % idleTime;
                    
                    if (settings.delayAfterOut > 0)
                    {
                        transform.localScale = Vector3.Lerp(scaleFrom, scaleTo, 1);
                        if (_canvasGroup != null)
                            _canvasGroup.alpha = Mathf.Lerp(fadeFrom, fadeTo, 1);
                        else if (_image != null)
                            _image.color = _image.color.Fade(Mathf.Lerp(fadeFrom, fadeTo, 1));
                        else if (_spriteRenderer != null)
                            _spriteRenderer.color = _spriteRenderer.color.Fade(Mathf.Lerp(fadeFrom, fadeTo, 1));
                        else if (_textMesh != null)
                            _textMesh.color = _textMesh.color.Fade(Mathf.Lerp(fadeFrom, fadeTo, 1));
                        else if (_text != null)
                            _text.color = _text.color.Fade(Mathf.Lerp(fadeFrom, fadeTo, 1));

                        if (unscaledTime)
                            yield return new WaitForSecondsRealtime(settings.delayAfterOut);
                        else
                            yield return new WaitForSeconds(settings.delayAfterOut);
                    }
                }

                var t = time / idleTime;
                var tScale = scaleLoopType == AnimatorLoopType.PingPong && flipFade  ? (1 - t) : t;
                tScale = scaleCurve.Evaluate(tScale);
                var tFade = fadeLoopType == AnimatorLoopType.PingPong && flipFade ? (1 - t) : t;
                tFade = fadeCurve.Evaluate(tFade);

                transform.localScale = Vector3.Lerp(scaleFrom, scaleTo, tScale);
                if (_canvasGroup != null)
                    _canvasGroup.alpha = Mathf.Lerp(fadeFrom, fadeTo, tFade);
                else if (_image != null)
                    _image.color = _image.color.Fade(Mathf.Lerp(fadeFrom, fadeTo, tFade));
                else if (_spriteRenderer != null)
                    _spriteRenderer.color = _spriteRenderer.color.Fade(Mathf.Lerp(fadeFrom, fadeTo, tFade));
                else if (_textMesh != null)
                    _textMesh.color = _textMesh.color.Fade(Mathf.Lerp(fadeFrom, fadeTo, tFade));
                else if (_text != null)
                    _text.color = _text.color.Fade(Mathf.Lerp(fadeFrom, fadeTo, tFade));
            }

            yield return  null;

           
        }

        ResetAnimation();
        busy = false;
        state = RxAnimatorState.Idle;
    }

    protected override IEnumerator ThisDoAnimate_Out()
    {
        yield return waitToOut;
        _stop = true;
        ResetAnimation();
        busy = false;
        state = RxAnimatorState.Idle;
    }

    protected override IEnumerator ThisDoAnimate_InThenOut()
    {
        ResetAnimation();
        _stop = false;
        CheckRegister();
        busy = true;
        state = RxAnimatorState.MovingIn;
        if (settings.deactiveAfterOut)
            gameObject.SetActive(true);

        yield return new WaitForSeconds(settings.DelayBeforeIn);


        bool flipScale = false;
        bool flipFade = false;
        bool flipRotate = false;
        float timeScale = 0;
        float timeFade = 0;
        float timeRotate = 0;
        float time = 0;
        int count = 0;
        while (!_stop)
        {
            if (idleTime < 0)
            {
                timeScale += Time.deltaTime;
                timeFade += Time.deltaTime;
                timeRotate += Time.deltaTime;

                if (scale)
                {
                    if (timeScale > scaleTime)
                    {
                        flipScale = !flipScale;
                        timeScale = timeScale % scaleTime;
                        
                        if (loopCount > 0)
                        {
                            count++;
                            if(count >= loopCount)
                                break;
                        }
                    }

                    var t = scaleLoopType == AnimatorLoopType.PingPong && flipScale
                        ? (1 - timeScale / scaleTime)
                        : timeScale / scaleTime;

                    t = scaleCurve.Evaluate(t);
                    transform.localScale = Vector3.Lerp(scaleFrom, scaleTo, t);
                }

                if (fade)
                {
                    if (timeFade > fadeTime)
                    {
                        flipFade = !flipFade;
                        timeFade = timeFade % fadeTime;
                        
                        if (loopCount > 0)
                        {
                            count++;
                            if(count >= loopCount)
                                break;
                        }
                    }

                    var t = fadeLoopType == AnimatorLoopType.PingPong && flipFade
                        ? (1 - timeFade / fadeTime)
                        : timeFade / fadeTime;

                    t = fadeCurve.Evaluate(t);

                    if (_canvasGroup != null)
                        _canvasGroup.alpha = Mathf.Lerp(fadeFrom, fadeTo, t);
                    else if (_image != null)
                        _image.color = _image.color.Fade(Mathf.Lerp(fadeFrom, fadeTo, t));
                    else if (_spriteRenderer != null)
                        _spriteRenderer.color = _spriteRenderer.color.Fade(Mathf.Lerp(fadeFrom, fadeTo, t));
                    else if (_textMesh != null)
                        _textMesh.color = _textMesh.color.Fade(Mathf.Lerp(fadeFrom, fadeTo, t));
                    else if (_text != null)
                        _text.color = _text.color.Fade(Mathf.Lerp(fadeFrom, fadeTo, t));
                }

                if (rotate)
                {
                    if (timeRotate > rotateTime)
                    {
                        flipRotate = !flipRotate;
                        timeRotate = timeRotate % rotateTime;
                        
                        if (loopCount > 0)
                        {
                            count++;
                            if(count >= loopCount)
                                break;
                        }
                    }

                    var t = rotateLoopType == AnimatorLoopType.PingPong && flipRotate
                        ? (1 - timeRotate / rotateTime)
                        : timeRotate / rotateTime;

                    t = rotateCurve.Evaluate(t);
                    transform.localEulerAngles = Vector3.Lerp(rotateFrom, rotateTo, t);
                }
            }
            else
            {
                time += Time.deltaTime;
                if (time > idleTime)
                {
                    flipScale = !flipScale;
                    flipFade = !flipFade;
                    time = time % idleTime;
                    
                    if (settings.delayAfterOut > 0)
                    {
                        transform.localScale = Vector3.Lerp(scaleFrom, scaleTo, 1);
                        if (_canvasGroup != null)
                            _canvasGroup.alpha = Mathf.Lerp(fadeFrom, fadeTo, 1);
                        else if (_image != null)
                            _image.color = _image.color.Fade(Mathf.Lerp(fadeFrom, fadeTo, 1));
                        else if (_spriteRenderer != null)
                            _spriteRenderer.color = _spriteRenderer.color.Fade(Mathf.Lerp(fadeFrom, fadeTo, 1));
                        else if (_textMesh != null)
                            _textMesh.color = _textMesh.color.Fade(Mathf.Lerp(fadeFrom, fadeTo, 1));
                        else if (_text != null)
                            _text.color = _text.color.Fade(Mathf.Lerp(fadeFrom, fadeTo, 1));
                        yield return new WaitForSeconds(settings.delayAfterOut);
                    }
                }

                var t = time / idleTime;
                var tScale = scaleLoopType == AnimatorLoopType.PingPong && flipFade ? (1 - t) : t;
                tScale = scaleCurve.Evaluate(tScale);
                var tFade = fadeLoopType == AnimatorLoopType.PingPong && flipFade ? (1 - t) : t;
                tFade = fadeCurve.Evaluate(tFade);

                transform.localScale = Vector3.Lerp(scaleFrom, scaleTo, tScale);
                if (_canvasGroup != null)
                    _canvasGroup.alpha = Mathf.Lerp(fadeFrom, fadeTo, tFade);
                else if (_image != null)
                    _image.color = _image.color.Fade(Mathf.Lerp(fadeFrom, fadeTo, tFade));
                else if (_spriteRenderer != null)
                    _spriteRenderer.color = _spriteRenderer.color.Fade(Mathf.Lerp(fadeFrom, fadeTo, tFade));
                else if (_textMesh != null)
                    _textMesh.color = _textMesh.color.Fade(Mathf.Lerp(fadeFrom, fadeTo, tFade));
                else if (_text != null)
                    _text.color = _text.color.Fade(Mathf.Lerp(fadeFrom, fadeTo, tFade));
            }

            yield return new WaitForEndOfFrame();
        }

        ResetAnimation();
        busy = false;
        state = RxAnimatorState.Idle;
    }


}
