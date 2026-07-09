using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupTutorial : BasePopup
{
    [SerializeField] private Button[] buttons;
    [SerializeField] private IndexBindingScript[] keyBinds;
    [SerializeField] private IntBool showPopupNext = new IntBool(0,true);

    [SerializeField] private VideoTutorialManager videoTutorialManager;
    [SerializeField] private RawImage rawImg;
    [SerializeField] private Image[] tutImgs;

    protected override void Register()
    {
        base.Register();
        foreach (var button in buttons)
            button.onClick.AddListener(Invoke);
    }

    public override void OnShowHandler(WhenPopupClose whenPopupClosed)
    {
        base.OnShowHandler(whenPopupClosed);
        rawImg.gameObject.SetActive(videoTutorialManager.IsTutVideoReady());

        for (int i = 0; i < tutImgs.Length; i++)
        {
            tutImgs[i].enabled = !videoTutorialManager.IsTutVideoReady();
        }
    }

    protected override void CloseHandler()
    {
        base.CloseHandler();
        videoTutorialManager.StopVideo();
    }

    private void Invoke()
    {
        Close();
        _action?.Invoke();
    }

    public override void LogCustomInt(int value)
    {
        base.LogCustomInt(value);
        keyBinds.OnChanged(value);
        ScreenRoot.popupManager.ShowPopupNext(this, WhenPopupClose.Close, new CustomParameter()); 
    }

    private Action _action;

    private PlayerPrefListInt _list;
    public bool CheckTut(int key,Action action)
    {
        if(_list == null)
            _list = new PlayerPrefListInt("popup_tutorial_key",new List<int>());

        if (!_list.Contains(key))
        {
            _action = action;
            keyBinds.OnChanged(key);
            if(showPopupNext.value2)
                ScreenRoot.popupManager.ShowPopupNext(this, WhenPopupClose.Close, new CustomParameter());
            else
                ScreenRoot.popupManager.ShowPopup(this, showPopupNext.value,WhenPopupClose.Close, new CustomParameter());
            _list.Add(key);
            _list.Save();

            videoTutorialManager.SetupVideo(key);

            return true;
        }

        return false;
    }

    
    // display content by key and after click button invoke action
    public void SetAction(int key,Action action)
    {
        _action = action;
        keyBinds.OnChanged(key);
    }

    public void SetTutorial(int key)
    {
        videoTutorialManager.SetupVideo(key);
    }
}
