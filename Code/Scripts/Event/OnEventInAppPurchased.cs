using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UniRx;

public class OnEventInAppPurchased : BaseOnEvent
{
   [IndexAsEnum(BuiltInEnumType.ShopItemKey)] [SerializeField]
   private int[] keys;

   [SerializeField] private IndexBindingScript[] bindShopItemKey;
   [SerializeField] private BaseBaseActionScript[] actionOnBuy;

   protected override IDisposable RegisterEvent()
   {
      return SubjectController.InAppPurchasedEvent.Subscribe(data =>
      {
         if (keys.Contains(data))
         {
            actionOnBuy.StartAction();
            bindShopItemKey.OnChanged(data);
         }
      });
   }
}
