using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class PopupPackItem : ShopItemView
{
    [SerializeField] private Button btnClose;
    [SerializeField] private RxAnimator[] animators;
    [IndexAsEnum(BuiltInEnumType.ShopItemKey)]
    [SerializeField] private IntBool originalPriceKey;
    [SerializeField] private BindingToStringTextChange originalPriceBind;
    public RxAnimator[] Animators => animators;

    public int ShowTimes { get; set; }
    
    public Action OnCloseAction { get; set; }

    private void LoadPack()
    {
        rewardView = GetComponentsInChildren<RewardView>(true);
    }

    public bool Extra => withExtra;
   
    protected override void OnKernelLoaded()
    {
        base.OnKernelLoaded();
        if(btnClose != null)
            btnClose.onClick.AddListener(() =>
            {
                OnCloseAction?.Invoke(); 
            });
    }

    protected override void UpdateView()
    {
        CheckKernelLoaded();
        base.UpdateView();
        var products = Products.ToArray();
        if (rewardView.Length == 0 || rewardView == null)
        {
            Debug.LogError("not set"+gameObject,gameObject);
            LoadPack();
            #if UNITY_EDITOR
            this.SetDirty();
            #endif
        }
        for (var i = 0; i < rewardView.Length; i++)
        {
            if (rewardView[i] != null)
            {
                if (i >= products.Length)
                    rewardView[i].gameObject.SetActive(false);
                else
                    rewardView[i].SetData(products[i]);
            }
        }

        if (originalPriceKey.value2 && originalPriceBind != null)
            originalPriceBind.OnChanged(GetPriceText(originalPriceKey.value));
    }

    public void Show()
    {
        PlayerData.shopItemShowTimes[key]++;
        SessionData.shopItemShowTimes[key]++;
        DailyData.shopItemShowTimes[key]++;
        gameObject.SetActive(true);
        foreach (var rxAnimator in animators)
            rxAnimator.Animator_In();
    }

    private void Close()
    {
        if(gameObject.activeInHierarchy)
            foreach (var rxAnimator in animators)
                rxAnimator.Animator_Out();
    }
}