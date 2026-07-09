using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UniRx;

public class ShopItemViewPreview : ShopItemView
{
    public enum AfterBought
    {
        None,
        CloseCurrentPopup,
    }

    [SerializeField] private AfterBought afterBought;

    protected override void OnKernelLoaded()
    {
        base.OnKernelLoaded();
    }
    


    protected override void OnSuccessful()
    {
        base.OnSuccessful();
        switch (afterBought)
        {
            case AfterBought.None:
                break;
            case AfterBought.CloseCurrentPopup:
                ScreenRoot.popupManager.CloseCurrentPopupAsObservable().Subscribe();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}
