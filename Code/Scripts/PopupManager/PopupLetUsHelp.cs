using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopupLetUsHelp : BasePopup
{
    [SerializeField] private Button[] buttons;
    [SerializeField] private IndexBindingScript[] keyBinds;

    [SerializeField] private TextMeshProUGUI itemPrice;
    [SerializeField] private Button buyBtn;
    [SerializeField] private Button watchAdsBtn;
    [SerializeField] private ShopItemView shopItemView;

    private ShopItem currentShopItem;

    protected override void Register()
    {
        base.Register();
        foreach (var button in buttons)
            button.onClick.AddListener(Invoke);
    }

    private void Invoke()
    {
        Close();
    }

    public override void LogCustomInt(int value)
    {
        base.LogCustomInt(value);
        keyBinds.OnChanged(value);
        ScreenRoot.popupManager.ShowPopupNext(this, WhenPopupClose.Close, new CustomParameter()); 
    }

    public void SetData(ShopItem shopItem)
    {
        currentShopItem = shopItem;

        int key = 0;

        switch (shopItem.Key)
        {
            //Undo
            case GameDefine.BoosterUndoKey:
                key = 1;
                shopItemView.OverrideKey(GameDefine.BoosterUndoKey);
                break;
            //More3Slot
            case GameDefine.BoosterMore3SlotKey:
                key = 2;
                shopItemView.OverrideKey(GameDefine.BoosterMore3SlotKey);
                break;
            //GoStraight
            case GameDefine.BoosterGoStraightKey:
                key = 3;
                shopItemView.OverrideKey(GameDefine.BoosterGoStraightKey);
                break;
        }

        keyBinds.OnChanged(key);
        itemPrice.text = currentShopItem.Price.amount.ToString();
    }

    // display content by key and after click button invoke action
    public void SetAction(int key,Action action)
    {
        keyBinds.OnChanged(key);
    }
}
