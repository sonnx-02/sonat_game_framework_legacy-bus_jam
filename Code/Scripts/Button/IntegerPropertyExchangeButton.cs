using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UnityEngine.UI;

public class IntegerPropertyExchangeButton : BaseFrameworkView
{
    [SerializeField] private IndexPropertySourceRetriever source;
    [SerializeField] private Button button;
    [SerializeField] private int spend = 1;
    [Header("Return")]
    [SerializeField] private Product @return;
    [SerializeField] private IndexBindingScriptBase[] bindings; // bind with amount
    [SerializeField] private ToggleScript[] toggles; // bind if affordable
    [SerializeField] private IndexPropertySourceRetriever otherToggleSource;
    [IndexAsEnum(BuiltInEnumType.CustomGameCondition)] [SerializeField]
    private int customCondition;
    [SerializeField] private ToggleScript[] customConditionBinding;
    private BoolReactiveProperty _toggleProperty;
    [SerializeField] private BaseButton showPopupButton;
    public string boosterName;

    public bool IsAffordable() => source.GetProperty().Value >= spend;

    private bool Valid() => source.GetProperty().Value >= spend &&
                            (otherToggleSource == null || otherToggleSource.GetProperty().BoolValue);
    protected override void OnKernelLoaded()
    {
        base.OnKernelLoaded();
        
        source.GetProperty().Subscribe(data =>
        {
            bindings.OnChanged(data);
            bool valid = Valid();
            toggles.OnChanged(valid); 
        });

        if (otherToggleSource != null)
            otherToggleSource.GetProperty().Subscribe(data =>
            {
                bool valid = Valid();
                toggles.OnChanged(valid);
            });
        
        button.onClick.AddListener(() =>
        {
            if (customCondition != 0 && !BaseGameController.GetCustomCondition(customCondition))
                return;
            ((int) BaseSoundEnum.ButtonClick).PlaySound();
            if (IsAffordable())
            {
                source.GetProperty().Value -= spend;
                PlayerData.Claim(@return);
                SubjectController.OnExchanged.OnNext(true);
                if(!string.IsNullOrEmpty(boosterName))
                    BaseGameController.LogUseBooster(boosterName);
            }
            else
            {
                if (showPopupButton != null)
                    showPopupButton.TriggerOnClick();
            }
        });
        
        if (customCondition != 0)
        {
            CheckCustomCondition();
            SubjectController.NotifyCustomGameCondition.Subscribe(data => CheckCustomCondition());
        }
    }
    
    protected virtual void CheckCustomCondition()
    {
        var valid = BaseGameController.GetCustomCondition(customCondition);
        customConditionBinding.OnChanged(valid);
    }

    protected override string GetName()
    {
        return "btn_Exchange";
    }
    
    
}
