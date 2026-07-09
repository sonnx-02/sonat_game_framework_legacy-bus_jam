using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sonat;
using UniRx;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

[Serializable]
public class ShowRewardInput
{
    public Trade<Product, Product> trade;

    [IndexAsEnumFlag(BuiltInEnumType.RewardTypeFlag)]
    public int rewardTypeFlag;
    public int parameter; // for type, ex : gold or gem
    public int parameter2; // for set index, for display bag of gold
    public float multipleByAds; // multiple value when watch ads
    public TradeLog log;
    public bool byAds;
    public SonatLogShowInterstitial logInterstitial; // if have log show interstitial when claim normal
    public BaseSonatAnalyticLog[] logWhenClaim;
    public BaseSonatAnalyticLog[] logWhenAdsClaim;
    public bool closeCurrentPopup;
    public bool showRewardAsNext;
    public CustomClaimScript CustomClaimScript { get; set; }
    
    public ShowRewardInput()
    {
        
    }

    public ShowRewardInput(Trade<Product, Product> trade,int rewardTypeFlag,int parameter,int parameter2,float multiple)
    {
        this.trade = trade;
        this.rewardTypeFlag = rewardTypeFlag;
        this.parameter = parameter;
        this.parameter2 = parameter2;
        this.log = log;
        multipleByAds = multiple;
    }
    
    public ShowRewardInput(Trade<Product, Product> trade,int rewardTypeFlag,bool byAds = true)
    {
        this.trade = trade;
        this.rewardTypeFlag = rewardTypeFlag;
        this.byAds = byAds;
    }
    
    
    
    public ShowRewardInput(Trade<Product, Product> trade)
    {
        this.trade = trade;
        rewardTypeFlag = (int)RewardTypeFlag.ClaimInstantInPopup;
    }
}
