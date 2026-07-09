using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class RxRandomAnimationIdle : BaseRxAnimationIdle
{
    [SerializeField] private Animator animator;
    [SerializeField] private StringIntFloatFloat[] triggers;
    [SerializeField] private RandomType randomType;

    public enum RandomType
    {
        RandomIndex,
        Sequence,
        RandomByFlag
    }

    protected override IEnumerator ThisDoAnimate_In()
    {
        animator.ResetAllAnimatorTriggers();
        int i = 0;
        while (triggers.Length > 0)
        {
            switch (randomType)
            {
                case RandomType.RandomIndex:
                    i = Random.Range(0, triggers.Length);
                    break;
                case RandomType.Sequence:
                    i++;
                    i = i % triggers.Length;
                    break;
                case RandomType.RandomByFlag:
                    var flag = triggers[i].valueInt.GetListFromFlag();
                    i = flag.TakeRandom();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            yield return new WaitForSeconds(triggers[i].GetRandom());
            animator.SetTrigger(triggers[i].valueStr);
        }
    }
}