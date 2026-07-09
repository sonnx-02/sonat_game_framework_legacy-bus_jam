using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sonat;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

public class ActionStartInterstitial : BaseBaseActionScript
{
    
    [SerializeField] private bool showInters;
    [SerializeField] [ConditionalField(nameof(showInters))]
    private string showIntersLog;
    [SerializeField] [ConditionalField(nameof(showInters))]
    private bool conditionLevel = true;
    [SerializeField] private int[] exceptLevels;
    [SerializeField] private float delay;

    protected override void DoAction()
    {
        base.DoAction();
        if (showInters && !exceptLevels.Contains(PlayerData.currentLevel.Value))
        {
            if (delay > 0)
                delay.Timer(() =>
                {
                    Kernel.Resolve<AdsManager>()
                        .ShowInterstitial(showIntersLog.CreateDefaultLogInterstitial(), false, conditionLevel);
                });
            else
            {
                Kernel.Resolve<AdsManager>()
                    .ShowInterstitial(showIntersLog.CreateDefaultLogInterstitial(), false, conditionLevel);
            }
        }
    }

}
