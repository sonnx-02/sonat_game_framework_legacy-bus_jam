using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DependencyGameOverScoreTween : BasePopupDependency
{
    [SerializeField] private ScriptPropertyFloat delayTweenScore;
    [SerializeField] private TweenDisplayText tweenScore;
    [SerializeField] private TweenDisplayText tweenBest;

    public override void OnShow()
    {
        base.OnShow();
        tweenScore.SetTo(0);
        if (tweenBest != null)
            tweenBest.SetTo(PlayerData.bestScore.Value);
        var score = WaveData.currentScore.Value;
        delayTweenScore.Value.Timer(() =>
        {
            var time = tweenScore.TweenToWithAction(score, data =>
            {
                if (tweenBest != null)
                    tweenBest.SetMax(data);
            });
            time.Timer(() => { PlayerData.bestScore.SetMax(WaveData.currentScore.Value); });
        });

        if (WaveData.currentScore.Value > PlayerData.bestScore.Value)
            Kernel.Resolve<PlayFabManager2>().UpdateBestScore(WaveData.currentScore.Value, true);
    }

    [MyButtonVector2Int(nameof(TestGameOver))] [SerializeField]
    private Vector2Int testValue;

    public void TestGameOver(Vector2Int vec)
    {
        PlayerData.bestScore.Value = vec.x;
        WaveData.currentScore.Value = vec.y;
        ScreenRoot.popupManager.ShowPopup<PopupGameOver>();
    }
}