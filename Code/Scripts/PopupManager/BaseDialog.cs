using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class BaseDialog : BaseFrameworkView
{
    [SerializeField] protected bool showBlockUi = true;
    [MyButton(nameof(TestShow))] [SerializeField] private int testShowDialog;
    public virtual void TestShow()
    {
        Show();
    }

    public bool turnOffInFocusAds;
    
    [SerializeField] [IndexAsEnum(BuiltInEnumType.SoundEnum)] private int closeSound;
    [SerializeField] private Button closeButton;
    [SerializeField] private bool stopTimeWhenShow;
    [SerializeField] private List<BaseRxAnimator> anims = new List<BaseRxAnimator>();
    [IndexAsEnum(BuiltInEnumType.SoundEnum)] [SerializeField]
    private int soundIn;
    [SerializeField] private bool notUseParentBg;
    protected DialogParent parent;
    public bool stopInput = true;

    [SerializeField] [IndexAsEnum(BuiltInEnumType.ToggleLink)] private int toggleLink;

    protected override void Register()
    {
        base.Register();
        if (closeButton != null)
            closeButton.onClick.AddListener(Close);
    }

    protected virtual void Close()
    {
        closeSound.PlaySound();
        gameObject.SetActive(false);
        transform.Release();
        if (stopTimeWhenShow)
            Time.timeScale = 1;
      
        if (_isOverride)
             _isOverride = false;
        else
            parent.OnItemClose();

        CloseHandler();
    }

    public void CloseHandler()
    {
        SubjectController.OnDialogClosed.OnNext(this);
        SubjectController.ToggleLink.OnNext(new IndexToggle(toggleLink,false));
    }
    
    
    public IObservable<Unit> CloseAsObservable()
    {
        var parallel = Observable.WhenAll(anims.Select(x => x.IO_AnimatorOut()).ToArray());
        return parallel;
    }

    public void SetParent(DialogParent parentPopup)
    {
        parent = parentPopup;
    }

    [SerializeField] private ToggleScript[] overrideToggle;

    private bool _isOverride;
    public virtual void Show()
    {
        SubjectController.ToggleLink.OnNext(new IndexToggle(toggleLink,true));
        _isOverride = parent.ShowOverride;
        parent.ShowOverride = false;
        
        transform.SetParentAndExpand(parent.Container(),false,true);
        
        AudioManager.Play(soundIn);
        if (!_isOverride)
        {
            parent.CloseAll();
            parent.CurrentDialog = this;
            parent.OnShow(showBlockUi);
        }
       
        overrideToggle.OnChanged(_isOverride);
        gameObject.SetActive(true);
        AudioManager.Play(soundIn);
        foreach (var anim in anims)
            anim.Animator_In();

        if (stopTimeWhenShow)
            Time.timeScale = 0;
    }
}