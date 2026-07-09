using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;
using UnityEngine.Serialization;

public enum TextTweenType
{
    Set,
    Add,
    TweenTo
}

public struct TweenCommand
{
    public TextTweenType TweenType;
    public int Value;
}

public class LoadMono : MonoBehaviour
{
    [MyButton(nameof(Load))] public int testLoad123;

    public virtual void Load()
    {
        gameObject.name = GetType().ToString();
    }
}

public abstract class TweenDisplayScriptComponent : BaseFrameworkView
{
    public abstract void Set(int val);
    public abstract void Add(int val);
}


public abstract class TweenDisplayScript : BaseFrameworkView
{
    [MyButtonInt(nameof(ResetValue),nameof(ResetValue2),nameof(ResetValue3))] [SerializeField]
    int testValueTween3;

    [MyButtonInt(nameof(SetTo), nameof(TweenTo), nameof(DoAdd))] [SerializeField]
    int testValueTween;

    [MyButtonInt(nameof(SetDisplayValue), nameof(SetTarget), nameof(SetTarget))] [SerializeField]
    int testValueTween2;

    [Tooltip("true value of tween")]
    [DisplayAsDisable] [SerializeField] protected int trueValue; // true value of tween
    [Tooltip("display value that animate")]
    [DisplayAsDisable] [SerializeField] protected int displayValue; // display value that animate
    [Tooltip("multiple target on progress")]
    [DisplayAsDisable] [SerializeField] protected int[] targets; // multiple target on progress
    [Tooltip("claimed target")]
    [DisplayAsDisable] [SerializeField] protected int claimedTarget = -1; // claimed target
    protected int TargetValue => targets[targets.Length - 1]; // largest target

    [SerializeField] protected int intervalTime = 10; // ms
    [SerializeField] protected float maxTimeTween = .3f; //s

    [IndexAsEnum(BuiltInEnumType.SoundEnum)] [SerializeField]
    protected int scoreSound;


#if UNITY_EDITOR
    [LabelHeader("Caution this",1,0,0)] [SerializeField]
    private int testHeader;
#endif
    [SerializeField] private SetTargetData setTargetData;
    [SerializeField] private BaseIndexPropertySource tweenOnEnable;


    [SerializeField] private float scoreSoundInterval = 0.05f;
    [SerializeField] protected bool instantSub = true;

    [FormerlySerializedAs("progressScripts")] [SerializeField]
    [Tooltip("to set position of target")]
    protected ProgressScript[] targetProgressScript; // to set position of targets, example =====(1)=====(2)====(3), position of (1) is (1)/(3)

    [SerializeField]
    [Tooltip("to set position of target [SerializeField] protected ToggleScript[] claimedToggles; // claimed target toggles")]
    protected IndexBindingScriptBase[]
        targetIndexScript; // to set position of target [SerializeField] protected ToggleScript[] claimedToggles; // claimed target toggles

    [Tooltip("claimed target toggles")]
    [SerializeField] protected ToggleScript[] claimedToggles; // claimed target toggles
    [Tooltip("claimed target toggles")]
    [SerializeField] protected IndexBindingScriptBase triggerClaimTarget; // claimed target toggles

    [SerializeField]
    [Tooltip("standalone not a displayer, ex : slot machine text")]
    protected TweenDisplayScriptComponent[] components; // standalone not a displayer, ex : slot machine text

    [Tooltip("a display bind to display value ex : 0/1")]
    [SerializeField] protected IndexBindingScriptBase[] displayValueBindings; // a display bind to display value ex : 0/1
    [Tooltip("a display bind to display value ex : 0/1")]
    [SerializeField] protected ProgressScriptInt[] intProgresses; // a display bind to display value ex : 0/1
    [Tooltip("claimed target toggles")]
    [SerializeField] protected ToggleScript[] onFinishAllTargets; // claimed target toggles
    public Action<bool> onFinish;

    private IDisposable _soundDisposable;
    private IDisposable _scoreTweenDisposable;

    public int TrueValue => trueValue;

    private void OnEnable()
    {
        if (tweenOnEnable != null)
        {
            Dispose();
            SetTo(0);
            DoAdd(tweenOnEnable.GetProperty().Value);
        }
    }

    protected override void OnKernelLoaded()
    {
        base.OnKernelLoaded();
   
    }

    public override void CheckKernelLoaded()
    {
        base.CheckKernelLoaded();
        if (setTargetData.Self)
            Inject(setTargetData.GetTargets());
    }

    public void Inject(int[] targetValues)
    {
        Inject(targetValues, trueValue);
    }

    public void Inject(int[] targetValues, int initValue)
    {
        SetTargets(targetValues);
        foreach (var script in intProgresses)
            script.SetTarget(TargetValue);
        SetTo(initValue);
    }

    public void SetIntervalTime(int interval)
    {
        intervalTime = interval;
    }

    public void SetTarget(int data)
    {
        targets = new[] {data};
        foreach (var script in intProgresses)
            script.SetTarget(TargetValue);
    }

    public void SetAll(int[] targetValues, int initValue, int claimed, bool checkFinishTarget)
    {
        Debug.Log("duong" + targetValues.Length + "-" + initValue + "-" + claimed);
        SetTargets(targetValues);
        SetTo(initValue);
        SetClaimed(claimed);
        if (checkFinishTarget)
            CheckFinishTarget();
    }

    public void SetTargets(int[] data)
    {
        targets = data;
        targetProgressScript.SetActiveAll(false);
        for (var i = 0; i < Mathf.Min(data.Length, targetProgressScript.Length); i++)
        {
            targetProgressScript[i].gameObject.SetActive(true);
            targetProgressScript[i].OnChanged(data[i] * 1f / TargetValue);
        }

        targetIndexScript.SetActiveAll(false);
        for (var i = 0; i < Mathf.Min(data.Length, targetIndexScript.Length); i++)
        {
            targetIndexScript[i].gameObject.SetActive(true);
            targetIndexScript[i].OnChanged(data[i]);
        }
        
        foreach (var script in intProgresses)
            script.SetTarget(TargetValue);
    }

    public void CheckFinishTarget()
    {
        var current = CurrentTargetFinish();
        if (claimedTarget < current)
        {
            claimedTarget++; // = CurrentTargetFinish();
            if (triggerClaimTarget != null)
                triggerClaimTarget.OnChanged(claimedTarget);
            UpdateClaimed();

            if (claimedTarget >= TargetValue)
            {
                onFinishAllTargets.OnChanged(true);
                onFinish?.Invoke(true);
            }
        }
    }

    protected virtual void ProgressInt(int i)
    {
        foreach (var script in intProgresses)
            script.OnChanged(i);
        displayValueBindings.OnChanged(i);
    }

    [SerializeField] private bool disableTweenTo;
    public virtual float Handler(TweenCommand tween)
    {
        switch (tween.TweenType)
        {
            case TextTweenType.Set:
                SetTo(tween.Value);
                return 0;
            case TextTweenType.Add:
                return DoAdd(tween.Value);
            case TextTweenType.TweenTo:
                if (disableTweenTo)
                    return 0;
                return TweenTo(tween.Value);
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void SetValue()
    {
        if (_onValueUpdate != null)
            _onValueUpdate.Invoke(displayValue);
        SetDisplayValue(displayValue);
        ProgressInt(displayValue);
    }

    public abstract void SetDisplayValue(int value);

    protected Action<int> _onValueUpdate;

    public virtual float TweenToWithAction(int val, Action<int> onUpdate)
    {
        _onValueUpdate = onUpdate;
        return DoAdd(val - trueValue);
    }

    public virtual void SetTo(int value)
    {
        Dispose();
        trueValue = value;
        displayValue = trueValue;
        SetValue();
        foreach (var tweenDisplayScriptComponent in components)
            tweenDisplayScriptComponent.Set(trueValue);
    }

    public virtual void ResetValue(int value)
    {
        SetTo(0);
    }
    public virtual void ResetValue2(int value)
    {
        SetTo(0);
    }

    public virtual void ResetValue3(int value)
    {
        SetTo(0);
    }


    public virtual void SetClaimed(int claimed = -1)
    {
        claimedTarget = claimed;
        UpdateClaimed();
    }

    private void UpdateClaimed()
    {
        for (int i = 0; i < claimedToggles.Length; i++)
            claimedToggles[i].OnChanged(i <= claimedTarget);
    }

    private int CurrentTargetFinish()
    {
        int currentTargetFinish = -1;
        for (int i = 0; i < targets.Length; i++)
        {
            if (trueValue >= targets[i])
                currentTargetFinish = i;
        }

        return currentTargetFinish;
    }


    public virtual float TweenTo(int val)
    {
        _onValueUpdate = null;
        return DoAdd(val - trueValue);
    }

    public virtual float TweenTo(int val, Action afterTween)
    {
        OnCompleteTween = afterTween;
        return TweenTo(val);
    }

    
    // implement continue update here
    
    // this handle int only
    public virtual float DoAdd(int val)
    {
        if (val <= 0 && instantSub)
        {
            SetTo(trueValue + val);
            return 0;
        }

        foreach (var tweenDisplayScriptComponent in components)
            tweenDisplayScriptComponent.Add(val);

        _soundDisposable?.Dispose();
        _scoreTweenDisposable?.Dispose();
        int delta = trueValue - displayValue;
        int distance = delta + val;

        var abs = Mathf.Abs(distance);
        int maxInterval = Mathf.CeilToInt(1000f / intervalTime * maxTimeTween);
        int add = Mathf.FloorToInt(abs * 1f / maxInterval);

        add = Mathf.Clamp(add, 1, add);
        int interval = Mathf.Abs(Mathf.FloorToInt(abs * 1f / add));
        int lastAdd = abs - (add * interval);

        _scoreTweenDisposable = Observable.Interval(TimeSpan.FromMilliseconds(intervalTime),Scheduler.MainThreadIgnoreTimeScale)
            .Take(interval + 1)
            .TakeUntilDisable(this)
            .DoOnCompleted(OnCompleteTweenFunc)
            .Subscribe(l =>
            {
                var value = (l == interval) ? lastAdd : add;
                if (distance > 0)
                    displayValue += value;
                else
                    displayValue -= value;
                SetValue();
                ProgressInt(displayValue);
            });

        if (scoreSound != 0)
        {
            var time = (intervalTime / 1000f) * (interval + 1);
            int soundCount = Mathf.CeilToInt(time / scoreSoundInterval);
            _soundDisposable = Observable.Interval(TimeSpan.FromSeconds(scoreSoundInterval),Scheduler.MainThreadIgnoreTimeScale)
                .Take(soundCount)
                .TakeUntilDisable(this)
                .Subscribe(l => { RootView.rootView.audioManager.Play(scoreSound); });
        }

        trueValue += val;
        return intervalTime / 1000f * (interval + 1);
    }

    private void OnCompleteTweenFunc()
    {
        OnCompleteTween?.Invoke();
        OnCompleteTween = null;
    }
    
    public Action OnCompleteTween { get; set; }


    private void Dispose()
    {
        _soundDisposable?.Dispose();
        _scoreTweenDisposable?.Dispose();
    }


    public void SetMax(int data)
    {
        if (displayValue < data)
        {
            displayValue = data;
            SetValue();
        }
    }


    [Serializable]
    public class SetTargetData
    {
        [SerializeField] private SetTargetType setTarget = SetTargetType.InjectByOther;
        [SerializeField] public IntValueRetrieverBase retriever;
        [SerializeField] private int[] manualTargets = {10};
        [SerializeField] private int retrieverIndex;

        public bool Self => setTarget != SetTargetType.InjectByOther;

        private enum SetTargetType
        {
            InjectByOther, // by script
            ByRetriever,
            ByRetrieverArray,
            ByRetrieverMultiTarget,
            Manual,
        }

        public int[] GetTargets()
        {
            switch (setTarget)
            {
                case SetTargetType.Manual:
                    return (manualTargets);
                case SetTargetType.ByRetriever:
                    return (new[] {retriever[retrieverIndex]});
                case SetTargetType.ByRetrieverMultiTarget:
                    return (retriever.Values);
                case SetTargetType.ByRetrieverArray:
                    return (retriever.GetArrayInt(retrieverIndex));
                case SetTargetType.InjectByOther:
                    return new[] {10};
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}