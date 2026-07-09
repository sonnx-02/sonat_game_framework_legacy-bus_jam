using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RxIndexByRetrieverPlayRepeatSound : BaseRxAnimator
{
    [SerializeField] private BaseIndexPropertySourceRetriever retriever;
    [SerializeField] private float startDelay;

    [SerializeField] [IndexAsEnum(BuiltInEnumType.SoundEnum)]
    private IntFloat[] sound;

    [SerializeField] private int add;

    public override bool HasOut()
    {
        return false;
    }

    public override bool HasIn()
    {
        return true;
    }

    protected override IEnumerator ThisDoAnimate_In()
    {
        if (startDelay > 0)
            yield return new WaitForSecondsRealtime(startDelay);
        var i = retriever.GetProperty().Value + add;
        for (int j = 0; j < i; j++)
        {
            sound[j].PlaySound();
            yield return new WaitForSecondsRealtime(sound[j].value2);
        }
    }

    protected override IEnumerator ThisDoAnimate_Out()
    {
        yield return null;
    }

    protected override IEnumerator ThisDoAnimate_InThenOut() => ThisDoAnimate_In();
}