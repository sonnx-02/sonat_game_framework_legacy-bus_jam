using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class TestEnableBinding : KernelLoadedView
{
    protected override void Start()
    {
        base.Start();
        gameObject.SetActive(false);
        RootView.testEnable.Subscribe(data =>
        {
            gameObject.SetActive(data != 0);
        });
    }
}
