using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

[RequireComponent(typeof(PoolItem))]
public class ReturnViewScript : MonoBehaviour
{
    [SerializeField] private float timeReturn = 1f;
    [SerializeField] private bool destroy;
    [SerializeField] private BaseRxAnimator[] animators;
    
    [SerializeField] private BaseRxAnimator[] checkAnimator;

    private void OnEnable()
    {
        foreach (var baseRxAnimator in animators)
            baseRxAnimator.Animator_In();
        timeReturn.Timer(Return);
    }

    private void Return()
    {
        foreach (var baseRxAnimator in checkAnimator)
            if (baseRxAnimator.Busy)
            {
                Debug.LogError("shouldn't' return if animator not finish");
                break;
            }

        if (destroy)
            GetComponent<PoolItem>().Destroy();
        else
            GetComponent<PoolItem>().Return();
    }
}
