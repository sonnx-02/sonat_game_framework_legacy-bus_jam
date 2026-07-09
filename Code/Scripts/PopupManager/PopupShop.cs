using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.Serialization;

public class PopupShop : BasePopup
{
   [SerializeField] private AdsItemView[] ads;
   [SerializeField] private BaseShopItemView[] shop;
   [SerializeField] [DisplayAsDisable] private bool closeWhenAddGem;

   protected override void Register()
   {
       base.Register();
       SubjectController.NotifyEvent
           .Where(x => gameObject.activeInHierarchy).Subscribe(n =>
       {
           //if (n == NotifyEnum.AdsRewarded)
               CheckHideItem();
       });

       SubjectController.CustomEvent.Subscribe(data =>
       {
           switch ((CustomEvent)data)
           {
               case CustomEvent.DialogAddGemOpen:
                   if (closeWhenAddGem)
                       Close();
                   break;
               default:
                   throw new ArgumentOutOfRangeException(nameof(data), data, null);
           }
       });
   }

   public override void SetParameter(CustomParameter parameter)
   {
       base.SetParameter(parameter);
       closeWhenAddGem = parameter?.boolValue ?? false;
   }


   public override void OnShowHandler(WhenPopupClose whenPopupClosed)
   {
       base.OnShowHandler(whenPopupClosed);
       CheckHideItem();
   }

  

   private void CheckHideItem()
   {
       for (var i = 0; i < ads.Length; i++)
       {
           ads[i].CheckActiveIfInStock();
       }
       for (var i = 0; i < shop.Length; i++)
       {
           shop[i].CheckActiveIfInStock();
       }
   }
}
