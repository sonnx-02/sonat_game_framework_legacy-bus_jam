using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class RentEffectScriptBase : BaseActionScript
{
    [IndexAsEnum(BuiltInEnumType.SoundEnum)]
    [SerializeField] protected int soundOnRent;
    [SerializeField] protected Transform parent;
    [SerializeField] protected int actionIndex = -1;
    [IndexAsEnum(BuiltInEnumType.CoolDownKey)]
    [SerializeField] protected IntFloat coolDown;
    [SerializeField] protected IntFloat multiAndInterval = new IntFloat(1, 0.5f);
    [SerializeField] protected Vector3 actionOffset;
    [SerializeField] private bool randomOffset;
    [ConditionalField(nameof(randomOffset))]
    [SerializeField] private Vector3 actionOffsetFrom;
    [ConditionalField(nameof(randomOffset))]
    [SerializeField] private Vector3 actionOffsetTo;
    [SerializeField] private bool presetOffset;
   // [ConditionalField(nameof(presetOffset))]
    [SerializeField] private Vector3[] presetOffsets;
    [SerializeField] protected bool rentWorldCurrentPos;


    protected override void DoAction()
    {
        base.DoAction();
        Count = 0;
        if (Delay > 0)
            Delay.Timer(Rent);
        else
            Rent();
    }

    protected virtual void Rent()
    {
        CheckRegister();
        var poolManager = RootView.rootView.poolManager;
        if (coolDown.value == 0 || coolDown.value2 <= 0 ||
            poolManager.CoolDownValidAndFill(coolDown.value, coolDown.value2))
        {
            soundOnRent.PlaySound();
            if (multiAndInterval.value <= 1)
                DoRent();
            else
                for (int i = 0; i < multiAndInterval.value; i++)
                    (multiAndInterval.value2 * i).Timer(DoRent);
        }
    }

    protected virtual void DoRent()
    {
       

    }

    protected int Count;
    protected Vector3 GetOffset()
    {
        if (presetOffset && presetOffsets.Length > 0)
        {
            Count++;
            return actionOffset + presetOffsets[Count % presetOffsets.Length];
        }
        return !randomOffset ? actionOffset : actionOffsetFrom.Random(actionOffsetTo);
    }
}