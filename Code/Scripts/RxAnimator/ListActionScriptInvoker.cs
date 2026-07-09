using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.Serialization;

public class ListActionScriptInvoker : BaseActionScript
{
    [FormerlySerializedAs("InitAnimator")] public BaseRxAnimator[] initAnimator;
    [FormerlySerializedAs("AddedAnimatorParent")] public Transform addedAnimatorParent;
    [FormerlySerializedAs("Gap")] public float gap;
   
    protected override void DoAction()
    {
        base.DoAction();
        Observable.FromCoroutine(IeStartAction).Subscribe();
    }

    public override void Prepare()
    {
        var anims = new List<BaseRxAnimator>();
        anims.AddRange(initAnimator);
        if (addedAnimatorParent != null)
            anims.AddRange(addedAnimatorParent.GetComponentsInFirstBornChildren<BaseRxAnimator>(true));
        foreach (var baseRxAnimator in anims)
        {
            baseRxAnimator.ResetAnimation();
        }
    }

    public IEnumerator IeStartAction()
    {
        var _anims = new List<BaseRxAnimator>();
        _anims.AddRange(initAnimator);
        if(addedAnimatorParent != null)
            _anims.AddRange(addedAnimatorParent.GetComponentsInFirstBornChildren<BaseRxAnimator>(true));

        _anims.SetActive(false);
        yield return Delay;
        for (var i = 0; i < _anims.Count; i++)
        {
            _anims[i].gameObject.SetActive(true);
            _anims[i].Animator_In();
            yield return gap;
        }
    }
}
