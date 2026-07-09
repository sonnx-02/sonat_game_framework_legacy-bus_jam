using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UniRx;

public class SubscribeScriptOnItemBought : SubscribeScriptBase
{
    [IndexAsEnum(BuiltInEnumType.ShopItemKey)]
    [SerializeField] private int[] key;

    [SerializeField] private float delayInvoke;
    protected override void RegisterSubscribe()
    {
        SubjectController.OnShopItemBought.TakeUntilDisable(this).Subscribe(data =>
        {
            if (key.Length == 0 || key.Contains(data))
            {
                if(delayInvoke <= 0)
                    DuongExtensions.ExecuteInNextFrame(Invoke);
                else
                    delayInvoke.Timer(Invoke);
            }
        });
    }
}
