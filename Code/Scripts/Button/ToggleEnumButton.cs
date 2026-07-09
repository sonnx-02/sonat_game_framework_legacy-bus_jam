using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class ToggleEnumButton : BaseButton
{
    [SerializeField] private int selectValue;
    [SerializeField] private bool isFlagEnum;
    [SerializeField] private IndexBindingScript[] bindSelectValue;
    [SerializeField] private ToggleScript[] bindCurrent;
    [SerializeField] private BindingToStringTextChange bindName;
  
    protected override void Register()
    {
        base.Register();
        bindSelectValue.OnChanged(selectValue);
    }

    private IntReactiveProperty _property;
    public void SetProperty(IntReactiveProperty property)
    {
        CheckRegister();
        _property = property;
        _property.Subscribe(OnChanged);
    }

    public void SetProperty(IntReactiveProperty property,int setValue)
    {
        _property = property;
        selectValue = setValue;
        _property.Subscribe(OnChanged);
        bindSelectValue.OnChanged(selectValue);
    }

    public void SetSelectValue(int val)
    {
        selectValue = val;
        bindSelectValue.OnChanged(selectValue);
    }
    
    public void SetProperty(IntReactiveProperty property,int setValue,string buttonName)
    {
        if(bindName != null)
            bindName.OnChanged(buttonName);
        SetProperty(property, setValue);
    }

    protected override void OnClick()
    {
        base.OnClick();
        _property.Value = selectValue;
    }

    private void OnChanged(int val)
    {
        bindCurrent.OnChanged(_property.Value == selectValue);
    }
}