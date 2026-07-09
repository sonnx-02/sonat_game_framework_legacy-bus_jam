using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[System.Serializable]
public class ShowPopupItem
{
    [SerializeField] [IndexAsEnum(BuiltInEnumType.SoundEnum)]
    private int soundClick;

    public Button[] showButtons;
    public BaseDialog popup;

    public void Register()
    {
        foreach (var showButton in showButtons)
        {
            showButton.onClick.AddListener(() =>
            {
                soundClick.PlaySound();
                popup.Show();
            });
        }
    }
}

[RequireComponent(typeof(GraphicRaycaster))]
[RequireComponent(typeof(Canvas))]
public abstract class DialogParent : BaseFrameworkView
{
    public abstract Transform Container();

    public bool ShowOverride { get; set; }

    private BaseDialog[] _allPopups;

    private IEnumerable<BaseDialog> AllPopups
    {
        get
        {
            if (_allPopups == null)
                Load();
            return _allPopups;
        }
    }


    public override void LogCustom()
    {
        base.LogCustom();
        Debug.Log(Busy);
    }

    public bool Busy { get; set; }

    public bool IsBusy()
    {
        if (CurrentDialog != null && !CurrentDialog.stopInput)
            return false;
        return Busy;
    }

    public void CheckStopInput()
    {
        0.2f.Timer(() =>
        {
            if (CurrentDialog == null)
                Busy = false;
        });
    }

    public BaseDialog CurrentDialog { get; set; }
    protected BaseDialog LastDialog { get; set; }

    public override void Start()
    {
        base.Start();
        Load();
    }

    public void Load()
    {
        _allPopups = GetComponentsInChildren<BaseDialog>(true);
        foreach (var uiPopup in _allPopups)
            uiPopup.SetParent(this);
    }

    public virtual void CloseAll()
    {
        foreach (var uiDialog in AllPopups)
            uiDialog.gameObject.SetActive(false);
    }

    public virtual void OnItemClose()
    {
        CheckStopInput();
    }

    public virtual void OnShow(bool showBlockUi)
    {
        Busy = true;
    }
}