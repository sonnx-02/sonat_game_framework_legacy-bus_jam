using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PopupShopItemBase : BasePopup
{
   [SerializeField] protected GetShopItemView shopItemView;

   public virtual void SetShopItem(int shopItemKey)
   {
        shopItemView.OverrideKey(shopItemKey);
   }
}
