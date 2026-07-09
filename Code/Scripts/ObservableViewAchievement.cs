using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UniRx;
using UnityEngine;

public class ObservableViewAchievement : ObservableView
{
    [SerializeField] private AchievementItemView achievementView;
    [SerializeField] private BaseBaseActionScript[] actionScripts;
    [SerializeField] private float timeTween = 3;
    [SerializeField] private float delayStartAction = 1;
    [SerializeField] private float delayAfterAction = .5f;
    [SerializeField] private float delayGetReward = .2f;

    public override IEnumerator Ie(int[] parameter)
    {
        yield return new WaitForSecondsRealtime(delayStartAction);
        
        foreach (var baseActionScript in actionScripts)
            baseActionScript.StartAction();
        achievementView.CheckKernelLoaded();

        yield return new WaitForSecondsRealtime(delayAfterAction);
        
        int add = parameter[0];
        PlayerData.achievementData[achievementView.Key] += add;
        achievementView.UpdateStatus();
        
        yield return new WaitForSecondsRealtime(timeTween);
        
        if ((achievementView.QuestState & QuestState.Complete) == QuestState.Complete)
        {
            var parallel = UniRx.Observable.WhenAll(anims.Select(x => x.IO_AnimatorOut()).ToArray()).ToYieldInstruction();
            yield return parallel;
            yield return new WaitForSecondsRealtime(delayGetReward);
            ScreenRoot.TurnRayCast(true);
            achievementView.GetReward();
            PlayerData.Save();
            while (ScreenRoot.popupManager.AnyPopupShow)
                yield return null;
        }
        else
        {
            var parallel = UniRx.Observable.WhenAll(anims.Select(x => x.IO_AnimatorOut()).ToArray()).ToYieldInstruction();
            yield return parallel;
        }
    //    yield return new WaitForSecondsRealtime(waitAnimsOut);
    }

  
}
