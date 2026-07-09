using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;

public class PopupPack : BasePopup
{
    [SerializeField] private PopupPackItem[] items;
    private PopupPackItem _current;

    public PopupPackItem[] Items => items;

    protected override void Register()
    {
        base.Register();
        foreach (var popupPackItem in items)
            popupPackItem.CheckRegister();
    }

    protected override void OnKernelLoaded()
    {
        base.OnKernelLoaded();
    }

    public override void Start()
    {
        base.Start();
        foreach (var popupPackItem in items)
            popupPackItem.OnCloseAction += Close;
    }
    
    [MyButtonInt(nameof(ShowPack), BuiltInEnumType.ShopItemKey)] [SerializeField] private int k;

    public void ShowPack(int key)
    {
        SetPack(key);
        Show();
    }

    public override void OnShowHandler(WhenPopupClose whenPopupClosed)
    {
        base.OnShowHandler(whenPopupClosed);
        _current.Show();
    }

    public PopupPackItem SetPack(int key)
    {
        items.SetActiveAll(false);
        foreach (var popupPackItem in items)
            if (popupPackItem.Key() == key)
            {
                _current = popupPackItem;
                return _current;
            }

        return null;
    }
    
    public override IObservable<Unit> ObservableAnim_Out(PopupManagerLevel level, bool closeUIblock)
    {
        var parallel = Observable.WhenAll(_current.Animators.Select(x => x.IO_AnimatorOut()).ToArray())
            .SelectMany(() => AfterOut(level, closeUIblock));
        return parallel;
    }

    public bool Exist(int shopItemKey)
    {
        return items.Any(x => x.Key() == shopItemKey);
    }
}
