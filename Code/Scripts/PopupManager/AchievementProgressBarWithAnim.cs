using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AchievementProgressBarWithAnim : AchievementProgressBar
{
    private int LastValue
    {
        get => PlayerPrefs.GetInt("achievement_" + key);
        set => PlayerPrefs.SetInt("achievement_" + key, value);
    }

    public override void TestAdd(int value)
    {
        PlayerData.achievementData[key] += value;
        if (gameObject.activeInHierarchy)
            UpdateCurrentProgress();
    }

    protected override void UpdateCurrentProgress()
    {
        if (PlayerData.achievementChecker[achievementItem.key] >= achievementItem.rewards.Length)
        {
            PlayerData.achievementChecker[achievementItem.key] = 0;
            PlayerData.achievementData[key] = 0;
        }

        int currentProgress = PlayerData.achievementData[key];
        foreach (var tweenDisplayScript in progressFill)
        {
            tweenDisplayScript.SetTo(LastValue);
            var states = GetStates(LastValue);
            for (var i = 0; i < finishStateBindings.Length; i++)
                finishStateBindings[i].OnChanged(states[i]);

            delay.Timer(() =>
            {
                float tween = tweenDisplayScript.TweenToWithAction(currentProgress, value =>
                {
                    var states2 = GetStates(value);
                    for (var i = 0; i < finishStateBindings.Length; i++)
                        finishStateBindings[i].OnChanged(states2[i]);
                });


                tween.Timer(() =>
                {
                    if (gameObject.activeInHierarchy)
                    {
                        var states3 = GetStates(currentProgress);
                        foreach (var i in states3)
                        {
                            if (i == (int) RewardState.Ready)
                            {
                                OpenMultiple();
                                break;
                            }
                        }
                    }
                });
            });

            LastValue = currentProgress;
        }
    }
}