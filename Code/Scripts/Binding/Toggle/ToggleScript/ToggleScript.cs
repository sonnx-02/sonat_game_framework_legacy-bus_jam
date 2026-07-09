using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;



public abstract class ToggleScript : BaseBindingAction
{
    [SerializeField] protected bool disable;
    //[SerializeField] [IndexAsEnum(BuiltInEnumType.SoundEnum)] private int sound;
    
    [SerializeField] [IndexAsEnum(BuiltInEnumType.SoundEnum)]
    private IntFloat[] sounds;

    private bool _overwritten;
    private bool _toggle;
    protected override void OnShow(bool overwritten)
    {
        _overwritten = overwritten;
        OnChanged(_toggle);
    }

    [SerializeField] private BaseRxAnimator[] whenStartInAnimations = new BaseRxAnimator[0];

    [MyButton(nameof(LoadOther), nameof(LoadOn), nameof(LoadOff))] [SerializeField]
    private int test2;
    [MyButton(nameof(ToggleOn),nameof(ToggleOff))] [SerializeField]
    private bool testToggle2;
    [MyButtonToggle(nameof(OnChanged))] [SerializeField]
    private bool testToggle;
    
    public void ToggleOn()
    {
        OnChanged(true);
    }
    
    public void ToggleOff()
    {
        OnChanged(false);
    }

    public ToggleScript[] others = new ToggleScript[0];
    public IndexBindingScript[] others2 = new IndexBindingScript[0];
    [SerializeField] private bool reverse;
    [SerializeField] protected bool custom;

    public void OnChanged(bool value, bool c)
    {
        if (_overwritten)
            _toggle = value;
        this.custom = c;
        OnChanged(value);
    }

    [MyButton(nameof(LoadComponent))] [SerializeField]
    private int testLoadComponent;
    [MyButton(nameof(LoadOn),nameof(LoadOff))] [SerializeField]
    private int testLoadComponent2;

    
    protected abstract void LoadComponent();

    protected virtual void Reset()
    {
        LoadComponent();
    }


    #if UNITY_EDITOR
    [SerializeField] private bool testBreak;
    public override void Start()
    {
        base.Start();
        if( others.Any(x => x== null))
            Debug.Log("other null",gameObject);
    }
#endif

    public bool CurrentValue { get; protected set; }
    public void OnChanged(bool val)
    {
        var go = gameObject;
#if UNITY_EDITOR
        if(testBreak)
            Debug.LogError("break "+go,go);
#endif
        CurrentValue = val;
        if (val)
            foreach (var whenStartInAnimation in whenStartInAnimations)
            {
                if(whenStartInAnimation == null)
                    Debug.Log(go,go);
                whenStartInAnimation.Animator_In();
            }

        if((!reverse && val) || (reverse && !val))
            foreach (var intFloat in sounds)
                intFloat.PlaySound();

        if (!disable)
        {
            if (reverse)
                Trigger(!val);
            else
                Trigger(val);
        }

        foreach (var toggleScript in others)
        {
#if UNITY_EDITOR
            if(toggleScript == null)
                Debug.LogError("null on others "+go,go);
#endif
            toggleScript.OnChanged(val);
        }
        
        foreach (var toggleScript in others2)
        {
#if UNITY_EDITOR
            if(toggleScript == null)
                Debug.LogError("null on others "+go,go);
#endif
            toggleScript.OnChanged(val ? 1 : 0);
        }
    }
    
    public void OnChanged(int val)
    {
        CurrentValue = val != 0;
        if (CurrentValue)
            foreach (var whenStartInAnimation in whenStartInAnimations)
                whenStartInAnimation.Animator_In();

        if((!reverse && CurrentValue) || (reverse && !CurrentValue))
            foreach (var intFloat in sounds)
                intFloat.PlaySound();
        if (!disable)
        {
            if (reverse)
                Trigger(!CurrentValue);
            else
                Trigger(CurrentValue);
        }
        foreach (var toggleScript in others)
            toggleScript.OnChanged(val);
        foreach (var toggleScript in others2)
            toggleScript.OnChanged(val);
    }

    protected abstract void Trigger(bool val);

    protected virtual void LoadOn()
    {
    }

    protected virtual void LoadOff()
    {
    }

    private void LoadOther()
    {
        var scripts = GetComponents<ToggleScript>();
        others = scripts.Where(x => x != this).ToArray();
        foreach (var baseRxAnimator in others)
            baseRxAnimator.others = new ToggleScript[0];
    }
}