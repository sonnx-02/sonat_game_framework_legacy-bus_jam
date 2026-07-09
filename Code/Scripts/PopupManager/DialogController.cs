using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using UniRx;


public class DialogController : DialogParent
{
    private Dictionary<Type, BaseDialog> _screenContainer = null;

    public T Resolve<T>() where T : BaseDialog
    {
        CheckRegister();
        if (!_screenContainer.ContainsKey(typeof(T)))
            return null;
        return (T) _screenContainer[typeof(T)];
    }
    
    public  T ResolveDirty<T>() where T : BaseDialog
    {
        foreach (var keyValuePair in _screenContainer)
        {
            if (keyValuePair.Key.IsSubclassOf(typeof(T)))
                return keyValuePair.Value as T;
        }

        return null;
    }
    
    public bool HasKey<T>() where T : BaseDialog
    {
        return _screenContainer.ContainsKey(typeof(T));
    }

    [SerializeField] private Transform dialogContainer;
    public ToggleScript[] blockUi;
    [SerializeField] private ToggleScript[] blockUiToggle;

    public override Transform Container() => dialogContainer;

    public bool AnyDialogShow { get; private set; }

    protected override void Register()
    {
        base.Register();
        CloseAll();
        gameObject.SetActive(true);
        var dialogs = dialogContainer.GetComponentsInChildren<BaseDialog>(true);
        _screenContainer = new Dictionary<Type, BaseDialog>();
        for (var i = 0; i < dialogs.Length; i++)
        {
            _screenContainer.Add(dialogs[i].GetType(), dialogs[i]);
            dialogs[i].CheckRegister();
            dialogs[i].transform.Release();
        }
    }

    public override void CloseAll()
    {
        if (CurrentDialog != null)
        {
            LastDialog = CurrentDialog;
            LastDialog.CloseHandler();
            LastDialog.transform.Release(true);
            CurrentDialog = null;
        }
        
        base.CloseAll();
        CurrentDialog = null;
        blockUi.OnChanged(false);
        AnyDialogShow = false;
        CheckStopInput();
    }

    public IObservable<Unit> CloseCurrentAsObservable(bool deactivate)
    {
        if (CurrentDialog == null)
            return null;
        return CurrentDialog.CloseAsObservable().SelectMany(() => Close(deactivate));
    }
    
    private IEnumerator Close(bool deactivate)
    {
        if (false)
            yield return null;
        
        LastDialog = CurrentDialog;
        LastDialog.CloseHandler();
        LastDialog.transform.Release(true);
        CurrentDialog = null;
        base.CloseAll();
        CurrentDialog = null;

        if (deactivate)
        {
            blockUi.OnChanged(false);
            AnyDialogShow = false;
            CheckStopInput();
        }
    }

   

    public override void OnItemClose()
    {
        base.OnItemClose();
        blockUi.OnChanged(false);
        if (CurrentDialog != null)
            LastDialog = CurrentDialog;
        CurrentDialog = null;
        AnyDialogShow = false;
    }

    public override void OnShow(bool showBlockUi)
    {
        base.OnShow(showBlockUi);
        blockUi.OnChanged(showBlockUi);
        blockUiToggle.OnChanged(showBlockUi);
        AnyDialogShow = true;
    }

    public bool ShowReward(Product product)
    {
        if (
            product.quantity == Quantity.Diamond ||
            product.quantity == Quantity.Stock ||
            product.quantity == Quantity.Energy ||
            product.quantity == Quantity.Gold 
            && ResolveDirty<DialogAddCurrencyBase>() != null)
        {
            ShowAddCurrency(product);
            return true;
        }

      //  if (Resolve<DialogAddCurrencyBase>() == null)
//        {
//            Resolve<DialogReward>().Show(product);
//            return true;
//        }

        return false;
    }

    private void ShowAddCurrency<T>(T product) where T : Product
    {
        var closeAsObservable = CloseCurrentAsObservable(false);
        if (closeAsObservable != null)
            closeAsObservable.Subscribe(data =>
            {
                ResolveDirty<DialogAddCurrencyBase>().Show(product);
            });
        else
        {
            ResolveDirty<DialogAddCurrencyBase>().Show(product);
        }
    }

    public void ShowMessage(string message)
    {
        Resolve<DialogMessage>().Show(message, () => { });
    }

    public void ShowMessage(string message, Action onClickOk)
    {
        Resolve<DialogMessage>().Show(message, onClickOk);
    }

    public void ShowMessage(string message, Action onClickOk, string buttonText)
    {
        Resolve<DialogMessage>().Show(message, onClickOk, buttonText);
    }

    public void ShowMessage(string message, Action onClickYes, Action onClickNo)
    {
        Resolve<DialogMessage>().Show(message, onClickYes, onClickNo);
    }

  
    public void ReopenLastDialog()
    {
        LastDialog.Show();
    }
}