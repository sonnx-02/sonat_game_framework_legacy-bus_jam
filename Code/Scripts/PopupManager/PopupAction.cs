using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupAction : BasePopup
{
    [SerializeField] private BindingToStringTextChange _textChange;
    [SerializeField] private IndexBindingScriptBase[] indexScrips;
    [SerializeField] private ToggleScript[] toggleScripts;
    [SerializeField] private Button btnNo;
    [SerializeField] private Button btnYes;

    
    private Action _action;
    private float _delayAction;
    
    protected override void OnKernelLoaded()
    {
        base.OnKernelLoaded();
        if(btnNo != null)
            btnNo.onClick.AddListener(() =>
            {
                _action = null;
                Close();
            });
        
        if(btnYes != null)
            btnYes.onClick.AddListener(() =>
            {
                Close();
                _delayAction.Timer(() =>
                {
                    _action.Invoke();
                    _action = null;
                });
            });
    }

    public PopupAction SetText(string text)
    {
        _textChange.OnChanged(text);
        return this;
    }
    
    public PopupAction SetIndex(int index)
    {
        indexScrips.OnChanged(index);
        return this;
    }
    
    public PopupAction SetToggle(bool toggle)
    {
        toggleScripts.OnChanged(toggle);
        return this;
    }
    
    
    public PopupAction SetAction(Action action,float delay = 0)
    {
        _action = action;
        _delayAction = delay;
        return this;
    }

    public PopupAction EnableNoButton(bool yes)
    {
        if (btnNo != null)
            btnNo.gameObject.SetActive(yes);
        return this;
    }
}
