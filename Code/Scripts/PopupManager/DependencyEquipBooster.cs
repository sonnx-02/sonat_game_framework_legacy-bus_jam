using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

public class DependencyEquipBooster : BasePopupDependency
{
    [SerializeField] private ToggleScriptEquip[] equips;
    [SerializeField] private Button playButton;
    [SerializeField] private bool showInters;
    [SerializeField] private ValidScriptBase[] validInters;
    [SerializeField] [ConditionalField(nameof(showInters))] private string showIntersLog;
    [SerializeField] [ConditionalField(nameof(showInters))] private bool conditionLevel = true;
    [SerializeField] private int[] exceptLevels;
    [SerializeField] private bool skipAdsFirstTime = true;
    [SerializeField] private IndexPropertySourceRetrieverMultiple propertyEnergy;
    [SerializeField] private BaseBaseActionScript actionWhenNotEnough;
    [SerializeField] private ValidScriptBase[] forceValidScripts;
    [SerializeField] protected bool handlerAtTransition;

    private bool _firstTime = true;

    private bool IsValidShowAds()
    {
        foreach (var validScriptBase in validInters)
            if (!validScriptBase.IsValid())
                return false;

        return true;
    }

    [SerializeField] private BaseBaseActionScript[] ifValidOnClick;
    protected override void OnKernelLoaded()
    {
        base.OnKernelLoaded();
        playButton.onClick.AddListener(() =>
        {
            // View change
            bool valid = ValidToPlay();
            if(valid)
                ifValidOnClick.StartAction();
            if (!(_firstTime && skipAdsFirstTime))
                if (showInters && IsValidShowAds() && !exceptLevels.Contains(PlayerData.currentLevel.Value) && valid)
                    Kernel.Resolve<AdsManager>()
                        .ShowInterstitial(showIntersLog.CreateDefaultLogInterstitial(), false,conditionLevel);
            
            if(valid)
            {
                if(!handlerAtTransition)
                    Handler();
                else
                    ScreenRoot.IoUnloadScreen(Handler).Subscribe();
            
                void Handler()
                {
                    BaseGameController.HandlerButtonPlay(retry);
                    BaseGameController.Equip(GetEquipped().ToList());
                    _firstTime = false;
                }
            }
            else
            {
                Debug.LogError("not valid to play");
            }
        });
    }

    [SerializeField] private bool retry;

    private bool ValidToPlay()
    {
        if (propertyEnergy == null || actionWhenNotEnough == null)
            return true;
        foreach (var forceValidScript in forceValidScripts)
            if (forceValidScript.IsValid())
                return true;
        if (propertyEnergy.models[0].Property.Value == 0)
        {
            actionWhenNotEnough.StartAction();
            return false;
        }

        return true;
    }

    private IEnumerable<Product> GetEquipped()
    {
        for (var i = 0; i < equips.Length; i++)
        {
            var take = equips[i].TakeEquip();
            if (take != null)
            {
                equips[i].DoConsume();
                yield return take;
            }
        }
    }

 
    public override void OnShow()
    {
        base.OnShow();
        equips.OnChanged(false);
    }
}
