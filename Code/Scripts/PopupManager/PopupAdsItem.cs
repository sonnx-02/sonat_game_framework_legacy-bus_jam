using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupAdsItem : BasePopup
{
    [SerializeField] private AdsItemView adsButton;

    [MyButtonInt(nameof(SetData),BuiltInEnumType.AdsItemKey)]
    [SerializeField] private int testKey;
    public void SetData(int adsItemKey)
    {
        adsButton.OverrideAdsItemKey(adsItemKey);
    }
    
    
}
