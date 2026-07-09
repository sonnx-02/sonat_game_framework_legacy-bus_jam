using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class PopupOffer : BasePopup
{
    [SerializeField] private ShopItemViewSpecialOffer[] offers;
    public override void LogCustomInt(int value)
    {
        base.LogCustomInt(value);
        offers.SetActive(false);
        Show();
        SessionData.offerChecker[value] = true;
        offers[value].gameObject.SetActive(true);
    }
    
    private int _key;
    // duong_comment_flow_offer_show7 : check to show
    public void CheckShow(int key)
    {
        _key = key;
        var offer = Kernel.GetDatabase<ShopDatabase>().GetOffer(key);
        if (offer.IsOfferActive())
        {
            offers.SetActive(false);
            // duong_comment_flow_offer_show8 : show offer
            Show();
            SessionData.offerChecker[key] = true;
            bool isCurrent = false;
            for (var i = 0; i < offers.Length; i++)
            {
                isCurrent = key == offers[i].Key();
                if (isCurrent)
                {
                    offers[i].gameObject.SetActive(true);
                    offers[i].AnimIn();
                    PlayerData.shopItemShowTimes[key]++;
                    SessionData.shopItemShowTimes[key]++;
                    DailyData.shopItemShowTimes[key]++;
                    SubjectController.OfferActivated.Where(x => x == _key).Subscribe(data =>
                    {
                        if (!offer.IsOfferActive())
                        {
                            if(ScreenRoot.popupManager.GetCurrentPopup() is PopupOffer)
                                Close();
                        }
                    });
                    break;
                }
            }

            if (!isCurrent)
            {
                 Debug.LogError("not found offer" +key,gameObject);
                #if UNITY_EDITOR
//                Debug.Break();
                #endif
            }
        }
        else
        {
            SubjectController.OfferActivated.OnNext(key);
        }
    }


//    [MyButtonInt(nameof(TestActiveAndShow))] [SerializeField]
//    private int testActiveShow;
//    private void TestActiveAndShow(int key)
//    {
//        Show();
//        for (var i = 0; i < offers.Length; i++)
//        {
//            offers[i].gameObject.SetActive(key == offers[i].Key());
//            if (key == offers[i].Key())
//            {
//                offers[i].CheckKernelLoaded();
//                offers[i].SetActiveOffer();
//                offers[i].StartCountDown();
//            }
//        }
//    }
    public bool Contains( int key)
    {
        foreach (var shopItemViewSpecialOffer in offers)
        {
            if (shopItemViewSpecialOffer.Key() == key)
                return true;
            
        }

        return false;
    }
}
