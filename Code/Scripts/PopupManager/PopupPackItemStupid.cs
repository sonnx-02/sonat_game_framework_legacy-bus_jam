using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupPackItemStupid : PopupPackItem
{
    [SerializeField] private CountDownView countDownView;

    public bool IsOffer() => countDownView != null;

    protected override void OnEnable()
    {
        base.OnEnable();
        StartCountDown();
    }

    public LongSerializableDictionary TimeTick => PlayerData.offerTimeTick;

    private void StartCountDown()
    {
        if (countDownView != null && gameObject.activeInHierarchy && TimeTick.Exist(key) &&
            DateTime.Now < DateTime.FromBinary(TimeTick[key]))
            countDownView.StartCountDown(DateTime.FromBinary(TimeTick[key]), () =>
            {
                Debug.Log("finish");
                SubjectController.OfferActivated.OnNext(key);
            });
    }

    protected override string GetName()
    {
        var n = "StupidPack" + "_" + ProjectSettings.GetKey(BuiltInEnumType.ShopItemKey.ToString(), key);

        if (Extra)
            n += "_Extra";
        if (IsOffer())
            n += "_Offer";
        return n;
    }
}