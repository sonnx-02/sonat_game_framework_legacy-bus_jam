using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class ShopExchangeAdsActivator : BaseFrameworkView
{
    [SerializeField] private IndexPropertySourceRetriever stockSource;

    [SerializeField] [IndexAsEnum(BuiltInEnumType.CustomGameCondition)]
    private int[] customConditions;

    [SerializeField] private ShopItemView shopItemView;
    [SerializeField] private BaseExchangeView exchangeView;
    [SerializeField] private AdsItemView adsItemView;
    [SerializeField] private IntegerPropertyExchangeButton exchangeButton;

    private IntReactiveProperty _property;

    private List<GameObject> _toActives;

    protected override void Register()
    {
        base.Register();
        _property = stockSource.GetProperty();
        _toActives = new List<GameObject>();
        if (shopItemView != null)
        {
            shopItemView.CheckRegister();
            _toActives.Add(shopItemView.gameObject);
        }

        if (exchangeView != null)
        {
            exchangeView.CheckRegister();
            _toActives.Add(exchangeView.gameObject);
        }

        if (adsItemView != null)
        {
            adsItemView.CheckRegister();
            _toActives.Add(adsItemView.gameObject);
        }

        if (exchangeButton != null)
        {
            exchangeButton.CheckRegister();
            _toActives.Add(exchangeButton.gameObject);
        }
    }

    protected override void OnKernelLoaded()
    {
        base.OnKernelLoaded();
        
        if (shopItemView != null)
            shopItemView.CheckKernelLoaded();
        if (exchangeView != null)
            exchangeView.CheckKernelLoaded();

        if (adsItemView != null)
            adsItemView.CheckKernelLoaded();
        if (exchangeButton != null)
            exchangeButton.CheckKernelLoaded();

        CheckSubscribe();
        // UpdateState(_property.Value);
    }

    private void CheckSubscribe()
    {
        if (!_subscribe)
        {     
            _property.Subscribe(UpdateState);
            SubjectController.NotifyCustomGameCondition.TakeUntilDisable(this).Subscribe(data => UpdateState(_property.Value));
            SubjectController.OnShopItemBought.TakeUntilDisable(this).Subscribe(data => UpdateState(_property.Value));
            SubjectController.OnExchanged.TakeUntilDisable(this).Subscribe(data => UpdateState(_property.Value));
            SubjectController.PopupChanged.Subscribe(data => _property.Subscribe(UpdateState));
            _subscribe = true;
        }
    }

    private void OnDisable()
    {
        _subscribe = false;
    }

    private bool _subscribe;

    private void OnEnable()
    {
        if (Kernel.IsReady())
            CheckSubscribe();
    }


    private void Active(MonoBehaviour go)
    {
        foreach (var active in _toActives)
            active.SetActive(go.gameObject == active);
    }

    private void UpdateState(int i)
    {
        if (exchangeButton != null && exchangeButton.IsAffordable())
            Active(exchangeButton);
        else if (exchangeView != null && exchangeView.IsAffordable())
            Active(exchangeView);
        else if (shopItemView != null && shopItemView.IsAffordable())
            Active(shopItemView);
        else if (adsItemView != null)
            Active(adsItemView);
        else
            Active(null);
    }


    protected bool ConditionValid()
    {
        var gc = BaseGameController;
        foreach (var customCondition in customConditions)
        {
            if (!gc.GetCustomCondition(customCondition))
                return false;
        }

        return true;
    }
}