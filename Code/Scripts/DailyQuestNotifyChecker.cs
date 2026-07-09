using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class DailyQuestNotifyChecker : BaseFrameworkView
{
    [SerializeField] private ToggleScript[] bindingAnyFinishes;
    protected override void OnKernelLoaded()
    {
        base.OnKernelLoaded();
        Check();
        SubjectController.CheckDailyQuestCompleteEvent.Subscribe(data => { Check();});
    }

    private void Check()
    {
        bindingAnyFinishes.OnChanged(GameSetting.AnyDailyQuestFinish());
    }
}
