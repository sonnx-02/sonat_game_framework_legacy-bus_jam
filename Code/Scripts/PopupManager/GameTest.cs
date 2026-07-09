using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameTest : BaseFrameworkView
{
    public int score;
    public int bestScore;

    public IndexPropertySourceSetter[] setters;

    [EditorDisplayName(
        new []{15,10,15,10}, 
        new []{nameof(PointParameter.col),nameof(PointParameter.row),nameof(PointParameter.value),nameof(PointParameter.value2)}, 
        new []{"Point","","Size","Test"}, 
        new []{30,30,30,30} )]
    public PointParameter pointParameter;
    
    [MyButtonInt(nameof(TestGameLose))] public int test;

    public void TestGameLose()
    {
        WaveData.currentScore.Value = score;
        PlayerData.bestScore.Value = bestScore;
        foreach (var indexPropertySourceSetter in setters)
            indexPropertySourceSetter.SetValue();
        1f.Timer(() => SubjectController.GameActionEvent.OnNext(GameActionEvent.ForceBoardLose));
    }
}