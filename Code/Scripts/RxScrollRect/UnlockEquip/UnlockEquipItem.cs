using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class UnlockEquipItem : RxScrollItemBase
{
    [SerializeField] [DisplayAsDisable] private UnlockEquipState display;
    [SerializeField] private IndexBindingScriptBase[] conditionBindings;
    [SerializeField] private IndexBindingScriptBase[] unlockConditionTypeBinding;

    [SerializeField] private ShopItemViewToSetup shopItemView;
    public UnlockEquipState State => (UnlockEquipState)state;
    public override void SetState(int value)
    {
        base.SetState(value);
        display = (UnlockEquipState)value;
    }

    public ShopItemViewToSetup ShopItemView => shopItemView;

    protected ProductUnlock Condition;
    protected int EquipDataEnum;
    [SerializeField] private BaseActionProductScript[] bindProduct;
    [SerializeField] private bool log;
    [SerializeField] private IndexBindingScript[] priceQuantityBindings;
    [SerializeField] private IndexBindingScriptBase[] parameterBinds;
    [SerializeField] private IndexBindingScriptBase[] parameter2Binds;
    
    
    public void SetCondition(ProductUnlock condition,int equipDataEnum)
    {
        if (Condition != null && Condition.Equal(condition))
            return;
        
        Condition = condition;
        if (log)
        {
            Debug.LogError(gameObject,gameObject);
        }
        priceQuantityBindings.OnChanged((int)condition.Quantity);
        parameterBinds.OnChanged(condition.parameter);
        parameter2Binds.OnChanged(condition.parameter2);
        if(shopItemView != null)
            shopItemView.CheckKernelLoaded();

        if (condition == null)
        {
            Debug.LogError("null"+gameObject,gameObject);
            return;
        }
        
        Condition = condition;
        EquipDataEnum = equipDataEnum;
        conditionBindings.OnChanged(condition.amount);
        unlockConditionTypeBinding.OnChanged((int)condition.condition);
        
        var product = new Product(Quantity.EquipData,1,equipDataEnum,Index,0);
        foreach (var baseProductBinding in bindProduct)
            baseProductBinding.StartAction(product);
        
        switch (condition.condition)
        {
            case UnlockEquipConditionEnum.GameControllerCalculate:
                break;
            case UnlockEquipConditionEnum.FinishLevel:
                break;
            case UnlockEquipConditionEnum.AlwaysUnlocked:
                break;
            case UnlockEquipConditionEnum.Shopping:
                var shopItem = new ShopItem(new ShopPrice()
                {
                    Quantity = condition.quantity,
                    amount = condition.amount,
                    priceKey = new ShopStock()
                    {
                        key = condition.index,
                        stockType = StockType.None
                    }
                    
                }, product);
                shopItemView.Setup(shopItem, () =>
                {
                    // have to delay because show reward popup have delay to claim
                 //   SubjectController.OnEquipDataUpdate.OnNext(EquipDataEnum); 
                });
                break;
            case UnlockEquipConditionEnum.BuilderFinish:
                break;
            case UnlockEquipConditionEnum.None:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public void SyncTo(UnlockEquipItem other)
    {
        other.index.Value = index.Value;
        other.SetCondition(Condition,EquipDataEnum);
        other.SetState(state);
    }

    public void ShopTryToBuy()
    {
        if(shopItemView != null)
            shopItemView.ClickHandler();
    }
}
