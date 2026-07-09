using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;


[System.Serializable]
public struct DropDownItem
{
    public Dropdown dropdown;
    public InputField input;
    public Button button;
}

public enum DialogTestEnum
{
    AddScore,
    SpawnBlock,
}


public class DialogTest : BaseDialog
{
    public DropDownItem questDropDown;
    public DropDownItem achievementDropDown;
    public DropDownItem testDropDown;
    public Button btnShow;
    public Button btnNewDay;
    public Button btnClearData;
    public AdsInstanceAdmob admob;

    public Button btnDummyIap;
    public Text dummyIapText;
    public Button btnNoAds;

    
    public int count = 2;
    public int time = 1000;
    protected override void Register()
    {
        base.Register();

        if (btnShow != null && admob != null)
        {
            btnShow.gameObject.SetActive(admob.testAds);
//            btnShow.onClick.AddListener(Show);
            var clickStream = btnShow.OnPointerClickAsObservable();
//            clickStream.Subscribe(data => Debug.Log("click"));
            clickStream.Buffer(clickStream.Throttle(TimeSpan.FromMilliseconds(time)))
                .Where(xs => xs.Count >= count)
                .Subscribe(xs =>
                {
                    Show();
                    Debug.Log("DoubleClick Detected! Count:" + xs.Count);
                });
        }
        
        btnDummyIap.onClick.AddListener(() =>
        {
            var purchase = Kernel.ResolveDirty<BasePurchaser>();
            purchase.SetDummy(!purchase.Dummy);
            dummyIapText.text = "Dummy IAP :" + purchase.Dummy;
        });
    }

    public void ClearData()
    {
        PlayerData.dailyGiftChecked = false;
        DailyData.dailyGiftChecked = false;
        PlayerData.gem.Value = 0;
        PlayerData.bestScore.Value = 0;
        foreach (var intReactiveProperty in PlayerData.customPropertyList)
            intReactiveProperty.Value = 0;
        SubjectController.NotifyEvent.OnNext(NotifyEnum.All);
    }

    public void TestNewDay()
    {
        DailyData.Clear();
        PlayerData.dailyGiftChecked = false;
        PlayerData.dailyGiftProgress++;
        SubjectController.NotifyEvent.OnNext(NotifyEnum.All);
    }

    
    public void AddGem(int gem)
    {
        PlayerData.gem.Value += gem;
    }

  
    
    private void FillAchievement(int key,int amount)
    {
        PlayerData.achievementData[key] += amount;
        SubjectController.NotifyEvent.OnNext(NotifyEnum.All);
    }

  
    protected override void OnKernelLoaded()
    {
        base.OnKernelLoaded();
        questDropDown.button.onClick.AddListener(() =>
        {
            FillQuest(questValues[questDropDown.dropdown.value],int.Parse(questDropDown.input.text));
        });
        
        achievementDropDown.button.onClick.AddListener(() =>
        {
            FillAchievement(questValues[achievementDropDown.dropdown.value],int.Parse(achievementDropDown.input.text));
        });
        
        testDropDown.button.onClick.AddListener(() =>
        {
            RootView.rootView.gameController.DialogTest((int)questValues2[testDropDown.dropdown.value],int.Parse(testDropDown.input.text));
        });
        
     
        dummyIapText.text = "Dummy IAP :" + Kernel.ResolveDirty<BasePurchaser>().Dummy;
        btnNoAds.onClick.AddListener(() =>
        {
            Kernel.ResolveDirty<AdsManager>().EnableNoAds();
        });

        btnNewDay.onClick.AddListener(TestNewDay);
        btnClearData.onClick.AddListener(ClearData);
 
    }
    

    [SerializeField] private string[] questNames;
    [SerializeField] private int[] questValues;
    
    [SerializeField] private string[] achievementNames;
    [SerializeField] private int[] achievementValues;
    
    [SerializeField] private string[] questNames2;
    [SerializeField] private DialogTestEnum[] questValues2;
    public override void LogCustom()
    {
        base.LogCustom();
//        EnumList enumList = new EnumList(nameof(BuiltInEnumType.QuestEnum));
//        questNames = enumList.names;
//        questValues = enumList.values;
//        var cast = questNames.Select(x => new Dropdown.OptionData(x)).ToList();
//        questDropDown.dropdown.options = cast;
//        
//        enumList = new EnumList(nameof(BuiltInEnumType.QuestEnum));
//        achievementNames = enumList.names;
//        achievementValues = enumList.values;
//         cast = achievementNames.Select(x => new Dropdown.OptionData(x)).ToList();
//        achievementDropDown.dropdown.options = cast;
//        
//        EnumList enumList2 = new EnumList(nameof(DialogTestEnum));
//        questNames2 = enumList2.names;
//        questValues2 =  enumList2.values.Cast<DialogTestEnum>().ToArray();
//        var cast2 = questNames2.Select(x => new Dropdown.OptionData(x)).ToList();
//        testDropDown.dropdown.options = cast2;
    }
}
