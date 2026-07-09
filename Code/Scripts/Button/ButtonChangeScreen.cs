using System.Collections;
using System.Collections.Generic;
using Sonat;
using UnityEngine;

public enum SetPreShowPopup
{
    No,
    ByEnum,
    ByName
} 
public class ButtonChangeScreen : BaseButton
{
    [IndexAsEnum(BuiltInEnumType.ScreenEnum)] [SerializeField]
    private int key;
    [ConditionalField(nameof(key),new object[]{-1})]
    [SerializeField] private string screenClassName;

    [SerializeField] private bool transition = true;
    [SerializeField] private bool clearGame = false;
    [SerializeField] private bool showInters;
    [SerializeField] [ConditionalField(nameof(showInters))] private string showIntersLog;
    [SerializeField] [ConditionalField(nameof(showInters))] private bool conditionLevel = true;
    [SerializeField] private int visitCondition = 0;
    private PlayerPrefInt _visitCount;
    
    [SerializeField] private SetPreShowPopup setPreShowPopup;
    [SerializeField] 
    [ConditionalField(nameof(setPreShowPopup),new object[]{SetPreShowPopup.ByEnum})] 
    
    private InScreenPopupEnum popupEnum;
    [SerializeField] [ConditionalField(nameof(setPreShowPopup),new object[]{SetPreShowPopup.ByName})]
    private string popupName;
        
    protected override void OnClick()
    {
        base.OnClick();
        if (showInters)
        {
            if( _visitCount == null)
                _visitCount = new PlayerPrefInt("count_visit_screen_"+key);
            _visitCount.Value++;
        }

        OnClickHandler();
    }

    protected virtual void OnClickHandler()
    {
        var screen = Show();
    }

    private BaseScreen Show()
    {
        if (showInters && _visitCount.Value > visitCondition)
        {
            if (!SessionData.skipIntersOnce)
                Kernel.Resolve<AdsManager>()
                    .ShowInterstitial(showIntersLog.CreateDefaultLogInterstitial(), false,conditionLevel);
        }
        SessionData.skipIntersOnce = false;
        
        if (clearGame)
            RootView.rootView.gameController.Clear();
        
        if (setPreShowPopup == SetPreShowPopup.ByName)
        {
            if (key == (int) ScreenEnum.ByName)
                ScreenRoot.Resolve(screenClassName).SetPreShowPopup(popupName);
            else
                ScreenRoot.Resolve(key).SetPreShowPopup(popupName);
        }
        if (setPreShowPopup == SetPreShowPopup.ByEnum)
        {
            if (key == (int) ScreenEnum.ByName)
                ScreenRoot.Resolve(screenClassName).SetPreShowPopup((int)popupEnum);
            else
                ScreenRoot.Resolve(key).SetPreShowPopup((int)popupEnum);
        }
        
        if (key == (int) ScreenEnum.ByName)
            return ScreenRoot.ShowScreen(screenClassName, transition);
        return ScreenRoot.ShowScreen(key, transition);
    }

    protected override string GetName()
    {
        if((ScreenEnum)key == ScreenEnum.ByName)
            return "btn_ChangeScreen_" + screenClassName;
        return "btn_ChangeScreen_" + (ScreenEnum)key;
    }

    protected override string IconShortCutName => "ChangeScreen"+(ScreenEnum) key;
}