using System;
using System.Collections.Generic;
using System.Linq;
using TMPro.SpriteAssetUtilities;
using UnityEngine;
using UnityEngine.Serialization;


[Serializable]
public class ShopItemRelationship
{
    [IndexAsEnum(BuiltInEnumType.ShopItemKey)]
    public int key;
    [IndexAsEnum(BuiltInEnumType.ShopItemKey)]
    public IntBool isSaleOf;
    [IndexAsEnum(BuiltInEnumType.ShopItemKey)]
    public IntBool isNextOf;
}

[Serializable]
[CreateAssetMenu(fileName = "ShopDatabase.asset", menuName = "Duong/ShopDatabase")]
public class ShopDatabase : ScriptableObject
{
    [ArrayElementIndexTitleAttribute(nameof(StoreProductDescriptor.key), BuiltInEnumType.ShopItemKey)]
    public List<StoreProductDescriptor> storeProductDescriptors;
    [ArrayElementIndexTitleAttribute(nameof(ShopItemRelationship.key), BuiltInEnumType.ShopItemKey)]
    public List<ShopItemRelationship> relationships;

    [ArrayElementIndexTitleAttribute(nameof(Offer.key), BuiltInEnumType.ShopItemKey)] [SerializeField]
    protected Offer[] offers;

    [ArrayElementIndexTitleAttribute(new []{"price",nameof(ShopPrice.priceKey),nameof(ShopStock.key)},
        BuiltInEnumType.ShopItemKey)] 
    [SerializeField] protected ShopItem[] shopItems;
   
    [ArrayElementIndexTitleAttribute(nameof(AdsItem.key), "AdsItemKey")] 
    [SerializeField] protected AdsItem[] adsItems;
    [SerializeField] protected SubscriptionItem[] subscriptionItems;

    public SubscriptionItem[] SubscriptionItems => subscriptionItems;
//    [ArrayElementTitle()]
//    [ArrayEl(nameof(StoreProductDescriptor.key), BuiltInEnumType.RewardEnum)]
    [ArrayLabelIndexAsEnumAttribute(BuiltInEnumType.RewardEnum)]
    [SerializeField] protected Product[] rewards;

    public StoreProductDescriptor GetDescriptor(int key)
    {
        foreach (var storeProductDescriptor in storeProductDescriptors)
        {
            if (storeProductDescriptor.key == key)
                return storeProductDescriptor;
        }

        return null;
    }


    [SerializeField] private string defaultPackageName = "package.parking.jam.";
    [ContextMenu("rename store")]
    private void Rename()
    {
        for (var i = 0; i < storeProductDescriptors.Count; i++)
        {
            var name = storeProductDescriptors[i].StoreProductId.Split(".");
            var last = name.Last();
            storeProductDescriptors[i].SetName(defaultPackageName + last);
        }
    }
    
    
    [ContextMenu("export store")]
    private void Export()
    {
        string text = "";
        for (var i = 0; i < storeProductDescriptors.Count; i++)
        {
            text += storeProductDescriptors[i].StoreProductId + "\n";
            text += storeProductDescriptors[i].price + "\n";
        }
        Debug.Log(text);
    }

    
    public ShopItem GetShopItem(int key)
    {
        foreach (var shopItem in shopItems)
            if (shopItem.Key == key)
                return shopItem;

        return null;
    }
    
    
    
    public SubscriptionItem GetSubscription(int key)
    {
        foreach (var item in subscriptionItems)
            if (item.key == key)
                return item;

        return null;
    }


    // duong_comment_flow_offer_show1: shuffle list of offer and get an offer ready
    public Offer GetAnOfferReadyToShowWhenHomeScreen()
    {
      
        
        var popupOffer = RootView.rootView.screenRoot.popupManager.Resolve<PopupOffer>();
        if (popupOffer == null)
            return null;
        
        var list2 = offers.ToList();
        list2.Shuffle(new System.Random());
        foreach (var offer in list2)
        {
            if(!popupOffer.Contains(offer.key))
                continue;
            if (offer.activeWhenScreenShow && offer.IsReadyToShowOnSession() && offer.IsReadyToShowOnDay())
                return offer;}
        return null;
    }

    public Offer GetOffer(int key)
    {
        foreach (var offer in offers)
            if (offer.key == key)
                return offer;

        return null;
    }

    public SubscriptionItem GetSubscriptionItem(int key)
    {
        foreach (var shopItem in subscriptionItems)
        {
            if (shopItem.key == key)
                return shopItem;
        }

        return null;
    }

    public AdsItem GetAdsItem(int key)
    {
        foreach (var item in adsItems)
            if (item.key == key)
                return item;
        return null;
    }

    public Product GetReward(int enumAsInt)
    {
        if (enumAsInt >= 0 && enumAsInt < rewards.Length)
            return rewards[enumAsInt];
        Debug.LogError("not found reward"+enumAsInt);
        return null;
    }
    
    [MyButton(nameof(PrintJson))] [SerializeField]
    private int testShow;
    public void PrintJson()
    {
        Debug.Log(JsonUtility.ToJson(this));
    }
}