using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class PopupPreviewShopItem : BasePopup
{
    [SerializeField] private ShopItemViewPreview[] previews;

    private int _key;
    public void CheckShow(int key)
    {
        _key = key;
        var shopItem = Kernel.GetDatabase<ShopDatabase>().GetShopItem(key);
        if (shopItem.Price.IsInStock())
        {
            previews.SetActive(false);
            Show();
            SessionData.offerChecker[key] = true;
            bool isCurrent = false;
            for (var i = 0; i < previews.Length; i++)
            {
                isCurrent = key == previews[i].Key();
                if (isCurrent)
                {
                    previews[i].gameObject.SetActive(true);
                    break;
                }
            }

            if (!isCurrent)
                Debug.LogError("not found offer", gameObject);
        }
    }
}