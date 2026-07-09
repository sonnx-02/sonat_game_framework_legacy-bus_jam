using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class ToggleScriptEquip : ToggleScript
{
    [SerializeField] private Product equip;
    [SerializeField] private BaseBaseActionScript actionWhenNotAffordable;
    [SerializeField] private BaseIndexPropertySourceRetriever source;
    [SerializeField] private Button btn;
    [SerializeField] private ToggleScript[] equipedBinds;
    [SerializeField] private int consume = 1;
    public IntReactiveProperty Property => source.GetProperty();

    public int Consume => consume;
    public string boosterName;

    public Product TakeEquip()
    {
        if (CurrentValue && Property.Value >= consume)
            return equip;
        return null;
    }

    public void DoConsume()
    {
        Property.Value -= consume;
        if(!string.IsNullOrEmpty(boosterName))
            RootView.rootView.gameController.LogUseBooster(boosterName);
    }


//    private void OnEnable()
//    {
//        OnChanged(false);
//    }

    private void Start()
    {
        OnChanged(false);
        btn.onClick.AddListener(() =>
        {
            ShortHand.LightFeedBack();
            if (Property.Value < consume)
            {
                if (actionWhenNotAffordable != null)
                    actionWhenNotAffordable.StartAction();
            }
            else
            {
                
            }
            
            if (!CurrentValue && Property.Value < consume)
            {
                return;
            }
            else
            {
                OnChanged(!CurrentValue);
            }
        });
    }

    public void Check()
    {
        if (Property.Value < consume)
            OnChanged(false);
    }

    protected override void Trigger(bool val)
    {
        if (val && Property.Value < consume)
        {
            OnChanged(false);
            return;
        }

        equipedBinds.OnChanged(val);
    }
    
    protected override void LoadComponent()
    {
        
    }
}
