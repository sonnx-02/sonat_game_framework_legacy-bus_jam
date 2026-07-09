using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupIntroduce : BasePopup
{
    [SerializeField] private Button[] buttons;
    [SerializeField] private IndexBindingScript[] keyBinds;

    [SerializeField] private IntBool showPopupNext = new IntBool(0,true);
    private Action _action;

    protected override void Register()
    {
        base.Register();
        foreach (var button in buttons)
            button.onClick.AddListener(Invoke);
    }

    private void Invoke()
    {
        Close();
        _action?.Invoke();
    }

    private PlayerPrefListInt _list;

    public bool CheckIntroduce(int key,Action action,bool force)
    {
        if(_list == null)
            _list = new PlayerPrefListInt("popup_introduce_key",new List<int>());
        
        if (!_list.Contains(key) || force)
        {
            _action = action;
            keyBinds.OnChanged(key);
            if(showPopupNext.value2)
                ScreenRoot.popupManager.ShowPopupNext(this, WhenPopupClose.Close, new CustomParameter());
            else
                ScreenRoot.popupManager.ShowPopup(this, showPopupNext.value,WhenPopupClose.Close, new CustomParameter());
            _list.Add(key);
            _list.Save();
            return true;
        }

        return false;
    }
    
    public void SetAction(int key,Action action)
    {
        _action = action;
        keyBinds.OnChanged(key);
    }
}
