using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;


[System.Serializable]
public enum AnimatorMoveType
{
    SpecificPosition,
    ParentPosition,
    CurrentPosition,
    LocationManager,
    AddPosition,
}

[System.Serializable]
public enum AnimatorLoopType
{
    OneDirection,
    PingPong
}

public enum RxAnimatorState
{
    Reset,
    MovingIn,
    Idle,
    MovingOut
}

public enum WhenSoundStart
{
    OnStartAnim,
    OnStartMove,
    OnStartFade,
    OnStartScale,
    OnFinishMove,
    OnFinishFade,
    OnFinishScale,
}

public enum AnimatorAction
{
    Reset,
    AnimIn,
    AnimOut
}

[Serializable]
public class RxAnimatorSharedSetting
{
    public bool startOnEnable;
    public bool inThenOutOnStart;
    public bool inIncludeOut;
    public List<BaseRxAnimator> others;
    public BaseRxAnimator nextInThenOut; // for sequence animator
    public bool resetOnStart;
    public bool deactiveAfterOut = true;
    public bool reverse;
    public AudioClip animInClip;
    public AudioClip animOutClip;

    [IndexAsEnum(BuiltInEnumType.SoundEnum)]
    public int animInSound;

    [IndexAsEnum(BuiltInEnumType.SoundEnum)]
    public int animOutSound;

    public WhenSoundStart whenSoundStartIn;


    [SerializeField] protected float delayStart;
    [SerializeField] [ConditionalField(nameof(delayScript), new object[] {null})]
    protected float delayBeforeIn;
    [IndexParam(nameof(delayScriptIndex))] public ParameterScriptFloatBase delayScript;
    [HideInInspector] [SerializeField] private int delayScriptIndex;
    public float delayAfterOut;
    
    // override Delay Before In
    public float DelayBeforeIn => (delayScript != null ? delayScript.GetValue(delayScriptIndex) : delayBeforeIn) + delayStart;

    /// <summary>
    /// if true will trigger True on toggleScripts on this and others, you can reverse true in ToggleScript
    /// </summary>
    public bool toggleTrueOnIn = true; 
    public ToggleScript[] toggleScripts;
    /// <summary>
    /// // trigger value of toggleOnOut when animator out, no matter true or false
    /// </summary>
    public bool toggleOutValue = true; 
    public ToggleScript[] toggleOnOut;
    // for external script can trigger SetIndexBindings(int)
    public IndexBindingScriptBase[] indexBindings;
    // call all call back event after animation in
    [IndexAsEnum(BuiltInEnumType.GlobalCallBackEnum)]
    [SerializeField] private int afterInCallBack;
    [SerializeField] private GetPointScriptBase afterInParameterScript;

    public PointParameter AfterInCallBack
    {
        get
        {
            if (afterInCallBack == 0)
                return null;
            if (afterInParameterScript == null)
                return new PointParameter(BuiltInEnumType.GlobalCallBackEnum,afterInCallBack); // none parameter call back
            return afterInParameterScript.GetPoint(BuiltInEnumType.GlobalCallBackEnum,afterInCallBack); // custom parameter call back
        }
    }


    // time adjust to start particle
    public float particleTimeAdjust;
    public List<ParticleSystem> afterInParticleSystems;
    public List<ParticleSystem> onStartParticleSystems;
    
    
    
}


public abstract class BaseRxAnimationIdle : BaseRxAnimator
{
    public override bool HasOut()
    {
        return false;
    }

    public override bool HasIn()
    {
        return true;
    }


    protected override IEnumerator ThisDoAnimate_Out()
    {
        yield return null;
    }

    protected override IEnumerator ThisDoAnimate_InThenOut() => ThisDoAnimate_In();
}



public abstract class BaseRxAnimator : BaseActionScript,ISetIndex,IDelayScript
{
    [SerializeField] private bool turnOff; // to turn off from other script, not change structure of animators
#if UNITY_EDITOR
    [MyButton(new[] {nameof(Animator_In), nameof(Animator_Out), nameof(Animator_InThenOut)}
        , new[] {"MoveIn", "MoveOut", "In=>Out"})]
    [SerializeField]
    protected int t1;

    [MyButton(new[] {nameof(ResetAnim), nameof(SetAtAnimationIn), nameof(StopAnim),}
        , new[] {"ResetAnim", "SetAtIn", "Stop"})]
    [SerializeField]
    protected int t2;

    [MyButton(nameof(AddAllOther), nameof(AddFirstOther), nameof(AddLastOther))] [SerializeField]
    protected int t3;
    
    [MyButton(nameof(SetPlayOnEnable))] [SerializeField]
    protected int t4;
#endif

    [SerializeField] protected RxAnimatorSharedSetting settings;


    public float GetDelay(int i = 0) => settings.DelayBeforeIn;

    [DisplayAsDisable] [SerializeField] protected RxAnimatorState state = RxAnimatorState.Idle;

    [DisplayAsDisable] [SerializeField] protected int index; // for custom by index
    [DisplayAsDisable] [SerializeField] protected bool busy;
    public bool Busy => busy;

    private Action _afterAnimationInAction;
    public void SetActionAfterIn(Action action)
    {
        _afterAnimationInAction = action;
    }

    protected void InvokeAfterInAction()
    {
        if(_afterAnimationInAction != null)
            _afterAnimationInAction.Invoke();
        _afterAnimationInAction = null;
    }

    public virtual void SetIndex(int value)
    {
        this.index = value;
    }

    public void SetTurnOff(bool val)
    {
        turnOff = val;
    }

    public void AddLastOther()
    {
#if UNITY_EDITOR

        var list = settings.others.ToList();
        var rxs = GetComponents<BaseRxAnimator>();
        for (var i = rxs.Length - 1; i >= 0; i--)
        {
            if (!list.Contains(rxs[i]))
            {
                list.Add(rxs[i]);
                settings.others = list.ToList();
                UnityEditor.EditorUtility.SetDirty(this);
            }

            break;
        }
#endif
    }

    public void AddFirstOther()
    {
#if UNITY_EDITOR

        var list = settings.others.ToList();
        var rxs = GetComponents<BaseRxAnimator>();
        for (var i = 0; i < rxs.Length; i++)
        {
            if (!list.Contains(rxs[i]))
            {
                list.Add(rxs[i]);
                settings.others = list.ToList();
                UnityEditor.EditorUtility.SetDirty(this);
            }

            break;
        }
#endif
    }

    public void AddAllOther()
    {
#if UNITY_EDITOR

        var list = settings.others.ToList();
        var rxs = GetComponents<BaseRxAnimator>();
        for (var i = 0; i < rxs.Length; i++)
        {
            if (!list.Contains(rxs[i]))
            {
                list.Add(rxs[i]);
                settings.others = list.ToList();
                UnityEditor.EditorUtility.SetDirty(this);
            }
        }
#endif
    }
    
    public void SetPlayOnEnable()
    {
#if UNITY_EDITOR

        settings.startOnEnable = true;
        settings.deactiveAfterOut = false;
        this.SetDirty();
#endif
    }

    protected void TriggerStartParticle()
    {
        if (settings.onStartParticleSystems.Count == 0)
            return;
        settings.onStartParticleSystems.ForEach(x =>
        {
            x.gameObject.SetActive(true);
            x.Stop();
            x.Play();
        });
    }
    
    protected void TriggerStopParticle()
    {
        if (settings.onStartParticleSystems.Count == 0)
            return;
        settings.onStartParticleSystems.ForEach(x =>
        {
            x.Stop();
        });
    }

    protected void TurnOffStartParticle()
    {
        settings.onStartParticleSystems.ForEach(x => { x.gameObject.SetActive(false); });
    }

    protected void TriggerAfterInParticle(float delay)
    {
        if (settings.afterInParticleSystems.Count == 0)
            return;

        if (delay <= 0)
            Trigger();
        else
            delay.Timer(Trigger);

        void Trigger()
        {
            foreach (var x in settings.afterInParticleSystems)
            {
                x.gameObject.SetActive(true);
                x.Play();
            }
        }
    }

    protected void StartInSound(float delay)
    {
        if (!gameObject.activeInHierarchy) return;

        AudioManager.Play(settings.animInClip, delay);
        AudioManager.Play(settings.animInSound, delay);
    }

    protected void StartInSound()
    {
        if (!gameObject.activeInHierarchy) return;
        AudioManager.Play(settings.animInClip);
        AudioManager.Play(settings.animInSound);
    }

    protected void StartOutSound(float delay)
    {
        if (!gameObject.activeInHierarchy) return;
        AudioManager.Play(settings.animOutClip, delay);
        AudioManager.Play(settings.animOutSound, delay);
    }

    protected void StartOutSound()
    {
        if (!gameObject.activeInHierarchy) return;
        AudioManager.Play(settings.animOutClip);
        AudioManager.Play(settings.animOutSound);
    }

    public void SetToggle(bool value)
    {
        settings.toggleScripts.OnChanged(value);
    }

    public void SetIndexBindings(int value)
    {
        settings.indexBindings.OnChanged(value);
    }

    protected override void OnEnable()
    {
      // base.OnEnable();
        if (settings.startOnEnable)
        {
            if (settings.inThenOutOnStart)
                Animator_InThenOut();
            else
                Animator_In();
        }
    }

    public void ResetAnim()
    {
        ResetAnimation();
    }

    public void SetAtIn(bool active = false)
    {
        CheckRegister();
        SetAtAnimationIn(active);
    }

    public virtual void StopAnim()
    {
        if (_disposable != null)
            _disposable.Dispose();
    }


    public RxAnimatorSharedSetting GetSetting() => settings;


    public abstract bool HasOut();
    public abstract bool HasIn();

    protected override void DoAction()
    {
        base.DoAction();
        Animator_In();
    }

    public override void Prepare()
    {
        ResetAnimation();
    }

    protected override void Awake()
    {
        base.Awake();
        if (settings.resetOnStart)
            ResetAnimation();

        settings.others.ForEach(x =>
        {
            if (x.settings.resetOnStart)
                x.ResetAnimation();
        });
    }


    protected override void Register()
    {
        base.Register();
        
#if UNITY_EDITOR

//        if (settings.deactiveAfterOut && settings.startOnEnable)
//        {
//            Debug.LogError("can have both");
//        }
        if (GetComponents<BaseRxAnimator>().Length > 1)
        {
//            if (GetComponents<BaseRxAnimator>()[0].GetType() == GetComponents<BaseRxAnimator>()[1].GetType())
//            {
//                Debug.LogError("Cái này có 2 rxAnimator, xóa bớt đi",gameObject);
//                Debug.Break();
//            }
        }
#endif
        
        if (settings.deactiveAfterOut)
        {
            if (HasIn())
                gameObject.SetActive(false);
        }

        for (var i = 0; i < settings.others.Count; i++)
            if (settings.others[i] == null)
                Debug.LogError("other null " + gameObject, gameObject);
    }
    

    public virtual void ResetAnimation(bool setActive = false)
    {
        if (setActive)
            gameObject.SetActive(true);
        transform.DOKill();
        settings.others.ForEach(x => { x.ResetAnimation(); });
    }

    public virtual void SetAtAnimationIn()
    {
        SetAtAnimationIn(false);
    }
    
    public virtual void SetAtAnimationIn(bool setActive)
    {
        if (setActive)
            gameObject.SetActive(true);
        transform.DOKill();
        settings.others.ForEach(x => { x.SetAtAnimationIn(); });
    }



    private IDisposable _disposable;


    public virtual void Animator_In()
    {
        if (turnOff)
        {
            gameObject.SetActive(false);
            return;
        }

        CheckRegister();
        _disposable?.Dispose();
        _idleDisposable?.Dispose();
        if (settings.inIncludeOut)
            _disposable = IO_AnimatorInThenOut().TakeUntilDisable(this).Subscribe();
        else
            _disposable = IO_AnimatorIn().TakeUntilDisable(this).Subscribe();
        //others.ForEach(x =>
        //{
        //    x.IO_AnimatorIn().Subscribe();
        //});
    }

    private IDisposable _idleDisposable;


    public void DestroyIdle()
    {
        foreach (var baseRxAnimator in settings.others)
            baseRxAnimator.DestroyIdle();

        _idleDisposable?.Dispose();
        ResetAnimation();
        gameObject.SetActive(false);
    }

    protected virtual IEnumerator ThisDoAnimate_InThenOutIdle()
    {
        yield return null;
    }


    public virtual void Animator_Out()
    {
        if (turnOff)
        {
            gameObject.SetActive(false);
            return;
        }

        CheckRegister();
        _disposable?.Dispose();
        _idleDisposable?.Dispose();
        _disposable = IO_AnimatorOut().TakeUntilDisable(this).Subscribe();

        //others.ForEach(x =>
        //{
        //    x.IO_AnimatorOut().Subscribe();
        //});
    }

    public virtual void Animator_InThenOut()
    {
        CheckRegister();
        _disposable?.Dispose();
        _idleDisposable?.Dispose();
        _disposable = IO_AnimatorInThenOut().TakeUntilDestroy(this).Subscribe().DisposeWith(this);

        //others.ForEach(x =>
        //{
        //    x.IO_AnimatorInThenOut().Subscribe();
        //});
    }
    
#if UNITY_EDITOR
    private float _lastTimeCall = -10;
#endif
    public  void CheckDuplicateCall()
    {
#if UNITY_EDITOR
        if (Time.unscaledTime - _lastTimeCall<= 0.2f)
        {
            Debug.LogError("Duplicate invoke "+this,this);
         //   Debug.Break();
        }
        else
        {
            _lastTimeCall = Time.unscaledTime;
        }
#endif
    }
    

    public IObservable<Unit> IO_AnimatorIn()
    {
        CheckDuplicateCall();
        PrepareIn();
        settings.others.ForEach(x => { x.Animator_In(); });
        if (MircoCoroutine)
        {
            if (!settings.reverse)
                return (Observable.FromMicroCoroutine(ThisDoAnimate_In));
            return Observable.FromMicroCoroutine(ThisDoAnimate_Out);
        }

        if (!settings.reverse)
            return (Observable.FromCoroutine(ThisDoAnimate_In));
        return Observable.FromCoroutine(ThisDoAnimate_Out);
    }

    protected virtual bool MircoCoroutine => false;


    public IObservable<Unit> IO_AnimatorOut()
    {
        //ResetAnimation();
        settings.others.ForEach(x => { x.Animator_Out(); });
        if (settings.reverse)
        {
            if (MircoCoroutine)
            {
                if (settings.toggleOnOut.Length > 0)
                
                    return Observable.FromMicroCoroutine(ThisDoAnimate_In).SelectMany(AfterOut);
                return Observable.FromMicroCoroutine(ThisDoAnimate_In);  
            }
            if (settings.toggleOnOut.Length > 0)
                
                return Observable.FromCoroutine(ThisDoAnimate_In).SelectMany(AfterOut);
            return Observable.FromCoroutine(ThisDoAnimate_In);
        }

        if (MircoCoroutine)
        {
            if (settings.toggleOnOut.Length > 0)
                return Observable.FromMicroCoroutine(ThisDoAnimate_Out).SelectMany(AfterOut);
            return Observable.FromMicroCoroutine(ThisDoAnimate_Out);
        }
        
        if (settings.toggleOnOut.Length > 0)
            return Observable.FromCoroutine(ThisDoAnimate_Out).SelectMany(AfterOut);
        return Observable.FromCoroutine(ThisDoAnimate_Out);
    }

    public IObservable<Unit> IO_AnimatorInThenOut()
    {
        PrepareIn();
        settings.others.ForEach(x => { x.Animator_InThenOut(); });
        if (MircoCoroutine)
        {
            if (settings.toggleOnOut.Length > 0)
                return Observable.FromMicroCoroutine(ThisDoAnimate_InThenOut).SelectMany(AfterOut);
            return Observable.FromMicroCoroutine(ThisDoAnimate_InThenOut);
        }
        if (settings.toggleOnOut.Length > 0)
            return Observable.FromCoroutine(ThisDoAnimate_InThenOut).SelectMany(AfterOut);
        return Observable.FromCoroutine(ThisDoAnimate_InThenOut);
    }

    private void PrepareIn()
    {
        ResetAnimation();
        TriggerToggle();
    }

    private void TriggerToggle()
    {
        if (settings.toggleTrueOnIn)
            SetToggle(true);
        foreach (var baseRxAnimator in settings.others)
            if (baseRxAnimator.settings.toggleTrueOnIn)
                baseRxAnimator.SetToggle(true);
    }

    private IEnumerator AfterOut()
    {
        settings.toggleOnOut.OnChanged(settings.toggleOutValue);
        if (false)
            yield return null;
    }

    // for sequence in then out
    public virtual IEnumerator DoAnimate_InThenOut()
    {
        settings.others.ForEach(x => { x.Animator_InThenOut(); });
        yield return ThisDoAnimate_InThenOut();
    }

    protected abstract IEnumerator ThisDoAnimate_In();

    protected abstract IEnumerator ThisDoAnimate_Out();

    protected abstract IEnumerator ThisDoAnimate_InThenOut();

    public virtual bool IsAnimating()
    {
        return busy;
    }

    public void Deactivate()
    {
        gameObject.SetActive(false);
    }

    public void Activate()
    {
        gameObject.SetActive(true);
    }

    protected virtual void InvokeCallBack()
    {
        if (settings.AfterInCallBack != null)
            SubjectController.OnPointParameterCallBack.OnNext(settings.AfterInCallBack);
    }

//    public virtual void Animator_InThenOutIdle()
//    {
//        CheckRegister();
//        _disposable?.Dispose();
//        _idleDisposable?.Dispose();
//        foreach (var baseRxAnimator in settings.others)
//            baseRxAnimator.Animator_InThenOutIdle();
//        _idleDisposable = Observable.FromCoroutine(ThisDoAnimate_InThenOutIdle).TakeUntilDisable(this).Subscribe();
//    }
}