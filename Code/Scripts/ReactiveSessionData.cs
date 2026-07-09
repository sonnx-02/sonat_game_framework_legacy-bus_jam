using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UniRx;
using UnityEngine;




[System.Serializable]
public class ReactiveSessionData : BaseReactiveData
{
    public IntReactiveProperty playMode = new IntReactiveProperty();
    public IntReactiveProperty combo = new IntReactiveProperty();
    public BooleanSerializableDictionary offerChecker = new BooleanSerializableDictionary();
    public IntegerSerializableDictionary shopItemShowTimes = new IntegerSerializableDictionary();
    public string[] sharedString = new string[20];
    public bool skipIntersOnce;

    protected override int CustomListSize => 30;


    public override void Clear()
    {
        base.Clear();
        offerChecker.ClearDict();
        shopItemShowTimes.ClearDict();
        if(sharedString == null || sharedString.Length == 0)
            sharedString = new string[20];
    }
}

[Serializable]
public class ReactiveWaveData : BaseReactiveData
{
    public IntReactiveProperty coin = new IntReactiveProperty(0);
    public IntReactiveProperty currentScore = new IntReactiveProperty(0);
    public IntReactiveProperty phase = new IntReactiveProperty(0);
    public IntegerSerializableDictionary adsRewardCounter = new IntegerSerializableDictionary();
    public IntegerSerializableDictionary adsRewardTaken = new IntegerSerializableDictionary();
    public LongSerializableDictionary adsTimeTick = new LongSerializableDictionary();

    public override void Clear()
    {
        base.Clear();
        coin.Value = 0;
        currentScore.Value = 0;
        phase.Value = 0;
        adsRewardCounter.ClearDict();
        adsRewardTaken.ClearDict();
        adsTimeTick.ClearDict();
        SubjectController.FireEventToUpdateAdsAndQuest();
    }
}


