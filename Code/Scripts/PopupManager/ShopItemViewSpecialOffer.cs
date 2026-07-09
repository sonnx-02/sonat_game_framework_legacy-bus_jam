using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;



public class ShopItemViewSpecialOffer : ShopItemView
{
    [SerializeField] private BaseRxAnimator[] anims;
    [SerializeField] private RewardView[] rewardViews;
    [SerializeField] private CountDownView countDownView;
    
    private Offer _offer;

    protected override bool FillOffer => true;
    private Offer Offer => _offer ?? (_offer = RootView.rootView.ShopDatabase.GetOffer(key));

//    protected override bool FillOffer => true;

    [SerializeField] private IndexBindingScriptBase txtTotalDiamond;
    [SerializeField] private IndexBindingScriptBase txtTotalGold;


    public BaseRxAnimator[] Anims => anims;
    protected override IEnumerable<Product> Products => Offer.GetProducts();

    protected override void OnKernelLoaded()
    {
        base.OnKernelLoaded();
        var arr = Offer.GetProducts().ToArray();
        int countDiamond = 0;
        int countGold = 0;
        foreach (var product in arr)
        {
            if (product.quantity == Quantity.Diamond)
                countDiamond += product.amount;
            if (product.quantity == Quantity.Gold)
                countGold += product.amount;
        }
        if(txtTotalDiamond != null)
            txtTotalDiamond.OnChanged(countDiamond);
        if(txtTotalGold != null)
            txtTotalGold.OnChanged(countGold);
        
        
        for (var i = 0; i < rewardViews.Length; i++)
            if (rewardViews[i] != null && arr.Length > i)
                rewardViews[i].SetData(arr[i]);
    }

//    public void SetActiveOffer(bool force = false)
//    {
//        if (!Offer.IsOfferActive() || force)
//        {
//            Offer.ActiveOffer();
//           
//        }
//        // start log count down
//    }

    protected override void OnEnable()
    {
        base.OnEnable();
        StartCountDown();

    }

    private void StartCountDown()
    {
        if (countDownView != null &&  gameObject.activeInHierarchy && Offer.IsOfferActive())
            countDownView.StartCountDown(DateTime.FromBinary(Offer.TimeTick[key]), () =>
            {
                Debug.Log("finish");
                SubjectController.OfferActivated.OnNext(key);
            });
    }
    
    protected override string GetName()
    {
        return base.GetName() +"_"+ ProjectSettings.GetKey(BuiltInEnumType.ShopItemKey.ToString(),key);
    }

    public void AnimIn()
    {
        foreach (var baseRxAnimator in anims)
            baseRxAnimator.Animator_In();
    }
    
    public void AnimOut()
    {
        foreach (var baseRxAnimator in anims)
            baseRxAnimator.Animator_Out();
    }
}
