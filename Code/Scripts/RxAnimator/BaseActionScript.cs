using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;


public enum AutoApplyWhen
{
    None,
    Awake,
    Enable,
    Start,
}

//
public abstract class BaseActionProductScript : BaseActionScript
{
    [SerializeField] private BaseActionProductScript[] others;
    [SerializeField] protected ProductValueRetrieverBase input;
    [SerializeField] protected Product defaultProduct;
    [SerializeField] private RewardView[] rewards;

    protected virtual Product Product
    {
        get
        {
            if (input == null)
                return defaultProduct;
            return input.Product;
        }
    }

  
    
    protected override void DoAction()
    {
        base.DoAction();
        StartAction(Product);
    }

    public override void StartAction(Product product)
    {
//        base.StartAction(product);
        others.StartAction(product);
        foreach (var rewardView in rewards)
            rewardView.SetData(product);
    }
}


public abstract class BaseActionScriptBoolean : BaseActionScript, ISetArrayInt
{
    [SerializeField] protected BooleanValueRetrieverBase input;
    [SerializeField] protected bool defaultValue;

    protected virtual bool Value
    {
        get
        {
            if (input == null)
                return defaultValue;
            return input.Valid;
        }
    }

    public void SetArrayInt(int[] arr)
    {
    }
}


public abstract class BaseActionScriptInput : BaseActionScript, ISetArrayInt
{
    [SerializeField] protected IntValueRetrieverBase input;
    
    [SerializeField] protected int defaultValue;

    protected virtual int Value
    {
        get
        {
            if (SetArrayValues != null)
                return SetArrayValues[0];
            if (input == null)
                return defaultValue;
            return input.Value;
        }
    }

    protected override void DoAction()
    {
        base.DoAction();
        if (input != null)
            SetArrayInt(input.Values);
    }

    public override void Prepare()
    {
        base.Prepare();
        if (input != null)
            SetArrayInt(input.Values);
    }

    protected int[] SetArrayValues;
    public virtual void SetArrayInt(int[] arr)
    {
        SetArrayValues = arr;
    }
}

public abstract class BaseActionScript : BaseBaseActionScript
{
    [SerializeField] private AutoApplyWhen applyWhen;

    [IndexAsEnum(BuiltInEnumType.SoundEnum)] [SerializeField]
    private int actionScriptSound;

    [SerializeField] protected int setIndex = -1;

//    [HideInInspector]
    [DisplayAsDisable] 
    [FormerlySerializedAs("delayActionForIndex")]
    [SerializeField]
    [LabelColor(1,.5f,.5f,"delayAction(obsolete)")]
    private float delayAction;

    [SerializeField] private IntFloatScript delayInput;

    protected virtual float Delay
    {
        get
        {
            CheckRegister();
            if (_delayScript != null)
                return _delayScript.GetDelay(delayInput.value) + delayInput.value2;
            return delayInput.value2;
        }
    }

    [ContextMenu("validate script")]
    public void ValidateTst()
    {
        if (delayAction > 0)
            Debug.LogError("update this delay", gameObject);
    }

    private IDelayScript _delayScript;

    protected override void Register()
    {
        base.Register();
        if (delayAction > 0)
            Debug.LogError("update this delay", gameObject);

        if (delayInput.value3 != null)
        {
            _delayScript = delayInput.value3.GetComponent<IDelayScript>();
            if (_delayScript == null)
                Debug.LogError("it is not IDelayScript");
        }
    }

    public override void StartAction()
    {
        if (IsValidToDoAction())
        {
            if (Delay > 0)
                Delay.Timer(DoAction);
            else
                DoAction();
        }
//        base.StartAction();
    }

    protected override void DoAction()
    {
        actionScriptSound.PlaySound();
    }

    public override void StartAction(int index)
    {
        setIndex = index;
        StartAction();
    }


    protected override void Awake()
    {
        base.Awake();
        if (applyWhen == AutoApplyWhen.Awake)
            StartAction();
    }
    
    public override void Start()
    {
        base.Start();
        if (applyWhen == AutoApplyWhen.Start)
            StartAction();
    }


    protected virtual void OnEnable()
    {
        if (applyWhen == AutoApplyWhen.Enable)
            StartAction();
    }

//    public bool CalculatedValue { get; set; }
}