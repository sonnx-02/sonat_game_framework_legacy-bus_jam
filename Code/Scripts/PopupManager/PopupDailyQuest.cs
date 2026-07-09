using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;

public class PopupDailyQuest : BasePopup
{
    public GenericQuestItemView[] items;

    protected override void Register()
    {
        base.Register();
        var gameSetting = Kernel.GetDatabase<GameSetting>();
        for (var i = 0; i < items.Length; i++)
            items[i].gameObject.SetActive(false);
        
        for (var i = 0; i < gameSetting.dailyQuestItems.Length; i++)
        {
            items[i].gameObject.SetActive(true);
            items[i].Setup(gameSetting.dailyQuestItems[i]);
        }

        SubjectController.CustomUpdatedEvent.Where(data => data == CustomUpdateEvent.DailyQuest)
            .Subscribe(data =>
            {
                for (var i = 0; i < items.Length; i++)
                    items[i].UpdateStatus();
            });
    }

    public override void OnShowHandler(WhenPopupClose whenPopupClosed)
    {
        base.OnShowHandler(whenPopupClosed);
        CheckRegister();
        for (var i = 0; i < items.Length; i++)
            items[i].UpdateStatus();
    }
    
    [MyButton(nameof(LoadItem))] [SerializeField] private int intLoad; 

    public void LoadItem()
    {
        items = GetComponentsInChildren<GenericQuestItemView>().ToArray();
    }
}
