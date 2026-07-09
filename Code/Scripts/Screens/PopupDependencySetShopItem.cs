
using UnityEngine;

public class PopupDependencySetShopItem : BasePopupDependency
{
    [SerializeField] private BaseActionProductScript[] productBindings;
    [SerializeField] private ShopItemViewToSetup shopItemView;

    public virtual void SetData(BaseBaseShopItemView data)
    {
        var shopItem = data.GetShopItem().Clone();
        foreach (var baseProductBinding in productBindings)
            baseProductBinding.StartAction(shopItem.Product);
        if(shopItemView != null)
            shopItemView.Setup(shopItem);
    }

    public override void OnShow()
    {
        base.OnShow();
    }
}
