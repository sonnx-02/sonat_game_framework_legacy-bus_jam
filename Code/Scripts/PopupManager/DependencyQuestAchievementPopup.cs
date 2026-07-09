using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;


public class DependencyQuestAchievementPopup : BasePopupDependency
{
    [SerializeField] private IndexBindingScriptBase[] questTabBindings;
    [SerializeField] private GenericQuestItemView[] items;

    #if UNITY_EDITOR
    [LabelHeader("Turn on debug to edit",1,.5f,0)] [SerializeField]
    private int header;
    #endif
    [IndexAsEnum(BuiltInEnumType.CustomSessionDataProperty,true)]
    [SerializeField] private int sessionQuestTab = (int) BaseCustomSessionDataProperty.QuestTab;
    
    
    [IndexAsEnum(BuiltInEnumType.QuestTabEnum,true)]
    [SerializeField] private int dailyQuestTab = (int) BaseQuestTabEnum.DailyQuest;
    
    [IndexAsEnum(BuiltInEnumType.QuestTabEnum,true)]
    [SerializeField] private int achievementTab = (int) BaseQuestTabEnum.Achievement;

    private int CurrentSessionQuestTab => SessionData.customPropertyList[sessionQuestTab].Value;
    public override void OnShow()
    {
        base.OnShow();
        questTabBindings.OnChanged(CurrentSessionQuestTab);

        var quests = GetQuest();
        for (var i = 0; i < items.Length; i++)
        {
           items[i].gameObject.SetActive(i < quests.Length);
           if (i < quests.Length)
               items[i].Setup(quests[i]);
        }

        for (var i = 0; i < items.Length; i++)
            items[i].UpdateStatus();
        
        SubjectController.CustomUpdatedEvent
            .TakeUntilDisable(this).Where(data => data == CustomUpdateEvent)
            .Subscribe(data =>
            {
                for (var i = 0; i < items.Length; i++)
                    items[i].UpdateStatus();
            });
    }

    private CustomUpdateEvent CustomUpdateEvent
    {
        get
        {
            if (CurrentSessionQuestTab == dailyQuestTab)
                return CustomUpdateEvent.DailyQuest;
            if (CurrentSessionQuestTab == achievementTab)
                return CustomUpdateEvent.Achievement;

            return CustomUpdateEvent.None;
        }
    }

    private  BaseQuestItem[] GetQuest()
    {
        var gameSetting = Kernel.GetDatabase<GameSetting>();
        if (CurrentSessionQuestTab == dailyQuestTab)
            return gameSetting.dailyQuestItems.Cast<BaseQuestItem>().ToArray();
        if (CurrentSessionQuestTab == achievementTab)
            return gameSetting.achievementItems.Cast<BaseQuestItem>().ToArray();

        return null;
    }


    [MyButton(nameof(LoadItem))] [SerializeField]
    private int intLoad;

    public void LoadItem()
    {
        items = GetComponentsInChildren<GenericQuestItemView>().ToArray();
    }
}