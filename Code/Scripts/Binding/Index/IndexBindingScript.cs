using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

// ReSharper disable Unity.InefficientPropertyAccess

public enum IndexBindingType
{
    Index,
    AdsState,
}

[Serializable]
public struct IndexBindingKey
{
    public IndexBindingType type;
    public int index;
}


[Serializable]
public class IndexScriptSharedSetting
{
#if UNITY_EDITOR
    [SerializeField] [TextArea] private string descriptions;
#endif
    [DisplayAsDisable] public int lastValue;

    [DisplayAsDisable] public int index2 = 0;

//    [IndexAsEnum(BuiltInEnumType.SoundEnum)]
//    public int sound;
    [SerializeField] [IndexAsEnum(BuiltInEnumType.SoundEnum)]
    public IntFloat[] sounds;

    public int add = 0;
    public int divide = -1;
    public int mod = -1;
    public IndexBindingScriptBase[] others;
    public IndexBindingScriptBase[] othersComputed;
    public IndexBindingScriptBase[] othersIndex2;
    public IndexBindingScriptBase[] othersComputedIndex2;
    public IndexBindingScriptBase[] index2ToOther1;
}


[Serializable]
public class ListIndexBindingScript
{
    public IndexBindingScriptBase[] scripts;
}

public abstract class IndexBindingScript : IndexBindingScriptBase
{
    [SerializeField] protected IndexScriptSharedSetting sharedSetting;
    [SerializeField] protected bool ignoreValidate;
    [SerializeField] private bool onChangedRandomOnEnable;

    public virtual void CopySetting()
    {
//        if (sound > 0)
//            sharedSetting.sounds = new IntFloat[] {new IntFloat() {value = sound},};
//        sharedSetting.add = add;
//        sharedSetting.divide = divide;
//        sharedSetting.mod = mod;
//        sharedSetting.others = others;
//        sharedSetting.othersComputed = othersComputed;
//        sharedSetting.othersIndex2 = othersIndex2;
//        sharedSetting.othersComputedIndex2 = othersComputedIndex2;
//        sharedSetting.index2ToOther1 = index2ToOther1;
    }

//    [HideInInspector] [SerializeField] [IndexAsEnum(BuiltInEnumType.SoundEnum)]
//    private int sound;
//    [HideInInspector] [SerializeField] private int lastValue;
//    [HideInInspector] [SerializeField] protected int index2 = 0;
//    [HideInInspector] [SerializeField] private int add = 0;
//    [HideInInspector] [SerializeField] private int divide = -1;
//    [HideInInspector] [SerializeField] private int mod = -1;
//    [HideInInspector] [SerializeField] protected IndexBindingScriptBase[] others;
//    [HideInInspector] [SerializeField] protected IndexBindingScriptBase[] othersComputed;
//    [HideInInspector] [SerializeField] protected IndexBindingScriptBase[] othersIndex2;
//    [HideInInspector] [SerializeField] protected IndexBindingScriptBase[] othersComputedIndex2;
//    [HideInInspector] [SerializeField] protected IndexBindingScriptBase[] index2ToOther1;

    public virtual int Size => 0;
    public bool Ignore { get; set; }

    protected virtual bool IsNoComponent() => false;

#if UNITY_EDITOR
    void Reset()
    {
        if (IsNoComponent())
            LoadComponent();
    }
#endif

    [MyButtonIntParameterPreset(nameof(OnChanged), "", new int[] {-1, 0, 1, 2,3})] [SerializeField]
    private int test_btn2;

    [MyButtonInt(nameof(OnChanged), nameof(SetIndex2))] [SerializeField]
    private int test_btn;

//    [MyButtonInt(nameof(Load), nameof(LoadOther), nameof(CopySetting))] [SerializeField]
    [MyButton(new[]
    {
        nameof(LoadComponent), nameof(LoadOther), nameof(LoadOtherCompute)
    })]
    [SerializeField]
    private int testLoad;

    [MyButtonInt(new[]
    {
        nameof(LoadIndex),
    })]
    [SerializeField]
    private int testLoad2;

    public virtual void LoadIndex(int index)
    {
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        if (onChangedRandomOnEnable)
            OnChangedRandom();
    }

    private bool _overwritten;

#if UNITY_EDITOR
    [SerializeField] protected bool logDebug;
    protected override void Register()
    {
        base.Register();
        if (sharedSetting.others.Any(x => x == null))
            Debug.Log("other null", gameObject);
        if (sharedSetting.othersComputed.Any(x => x == null))
            Debug.Log("other null", gameObject);
        if (sharedSetting.othersIndex2.Any(x => x == null))
            Debug.Log("other null", gameObject);
        if (sharedSetting.othersComputedIndex2.Any(x => x == null))
            Debug.Log("other null", gameObject);
        if (sharedSetting.index2ToOther1.Any(x => x == null))
            Debug.Log("other null", gameObject);
    }
#endif

    protected virtual void LoadComponent()
    {
    }

    protected virtual void LoadOther()
    {
        var list = GetComponents<IndexBindingScriptBase>().ToList();
        list.Remove(this);
        sharedSetting.others = list.ToArray();
        foreach (var indexBindingScript in sharedSetting.others)
        {
            if (indexBindingScript is IndexBindingScript cast)
            {
                foreach (var bindingScript in cast.sharedSetting.others)
                {
                    if (bindingScript == this)
                    {
                        var list2 = cast.sharedSetting.others.ToList();
                        list2.Remove(this);
                        cast.sharedSetting.others = list2.ToArray();
                    }
                }
            }
        }
    }

    public virtual void LoadOtherCompute()
    {
        var list = GetComponents<IndexBindingScriptBase>().ToList();
        list.Remove(this);
        sharedSetting.othersComputed = list.ToArray();
        foreach (var indexBindingScript in sharedSetting.othersComputed)
        {
            if (indexBindingScript is IndexBindingScript)
            {
                var cast = indexBindingScript as IndexBindingScript;
                foreach (var bindingScript in cast.sharedSetting.othersComputed)
                    if (bindingScript == this)
                    {
                        var list2 = cast.sharedSetting.othersComputed.ToList();
                        list2.Remove(this);
                        cast.sharedSetting.others = list2.ToArray();
                    }
            }
        }
    }

    protected override void OnShow(bool overwritten)
    {
        _overwritten = overwritten;
        OnChanged(_index);
    }

    public void OnChangedBoolean(bool toggle)
    {
        OnChanged(toggle ? 1 : 0);
    }


    public void OnChangedRandom()
    {
        OnChanged(UnityEngine.Random.Range(0, Size));
    }

    public void SetIndex2(int value)
    {
        sharedSetting.index2 = value;
        OnValueChanged(LastComputed);
        sharedSetting.index2ToOther1.OnChanged(value);
    }

    private int _index;

    protected virtual void PlaySoundOnChanged()
    {
        foreach (var sharedSettingSound in sharedSetting.sounds)
            sharedSettingSound.PlaySound();
    }

    public void OnChangedTypeValue(int type, int index)
    {
        OnChangedType(type);
        OnChanged(index);
    }

    protected virtual void OnChangedType(int type)
    {
    }

    public override void OnChanged(int index)
    {
        if (Ignore)
            return;
        sharedSetting.lastValue = index;
        PlaySoundOnChanged();
        if (_overwritten)
            _index = index;

        _overwritten = true;

        //if(Others != null)
        foreach (var baseIndexBindingAction in sharedSetting.others)
        {
#if UNITY_EDITOR
            if (baseIndexBindingAction == null)
                Debug.LogError("Others null on" + gameObject, gameObject);
#endif

            baseIndexBindingAction.OnChanged(index);
        }

        foreach (var baseIndexBindingAction in sharedSetting.othersIndex2)
        {
#if UNITY_EDITOR
            if (baseIndexBindingAction == null)
                Debug.LogError("null on others " + gameObject, gameObject);
#endif
            baseIndexBindingAction.OnChanged(index);
        }

        int computedValue;
        if (sharedSetting.divide > 0)
        {
            var value = index / sharedSetting.divide;
            if (sharedSetting.mod > 0)
                value = value % sharedSetting.mod;
            computedValue = (value + sharedSetting.add);
        }
        else if (sharedSetting.mod > 0)
            computedValue = (index % sharedSetting.mod + sharedSetting.add);
        else
            computedValue = (index + sharedSetting.add);
        
       
        OnValueChanged(GetRemap(computedValue));
    }

    [SerializeField] private int[] computedRemaps;
    private int GetRemap(int computed)
    {
        for (var i1 = 0; i1 < computedRemaps.Length; i1++)
            if (computedRemaps[i1] == computed)
                return i1;

        return computed;
    }

    public void OnChangedLastValue()
    {
        OnValueChanged(LastComputed);
    }

    protected int LastComputed;

    protected bool IsIncreased;
    protected virtual void OnValueChanged(int computedValue)
    {
#if UNITY_EDITOR
        if (logDebug)
            Debug.LogError(computedValue, gameObject);
#endif
        IsIncreased = computedValue > LastComputed;
        LastComputed = computedValue;
        foreach (var baseIndexBindingAction in sharedSetting.othersComputed)
        {
            baseIndexBindingAction.OnChanged(computedValue);

#if UNITY_EDITOR
            if (baseIndexBindingAction == null)
                Debug.LogError("Others null on" + gameObject, gameObject);
#endif
        }

        foreach (var baseIndexBindingAction in sharedSetting.othersComputedIndex2)
            if (baseIndexBindingAction is IndexBindingScript script)
                script.SetIndex2(computedValue);
    }


#if UNITY_EDITOR
    GameObject _parentRegister;

    [ContextMenu("Focus Parent")]
    void FocusParent()
    {
        Debug.Log("ParentRegister of this" + gameObject.name + " is " + _parentRegister, _parentRegister);
    }

    public void SetParent(GameObject parent)
    {
        _parentRegister = parent;
    }
#endif
}