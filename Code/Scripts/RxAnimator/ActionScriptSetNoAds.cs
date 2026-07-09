using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class ActionScriptSetNoAds : BaseActionScript
{
    protected override void DoAction()
    {
        base.DoAction();
        Observable.FromCoroutine(WaitForKernelLoaded);
    }

    IEnumerator WaitForKernelLoaded()
    {
        while (!Kernel.IsReady())
        {
            yield return null;
        }
        Kernel.Resolve<DuongAdsManager>().EnableNoAds();
    }
}
