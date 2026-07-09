using System.Collections;
using System.Collections.Generic;
using Sonat;
using UniRx;
using UnityEngine;

public class DialogAdBreak : BaseDialog
{
    [SerializeField] private int waitTime = 3;
    [SerializeField] private RewardView rewardView;
    [SerializeField] private IndexBindingScriptBase[] countDown;

    protected override void OnKernelLoaded()
    {
        base.OnKernelLoaded();
        rewardView.SetData(GameSetting.adBreakReward);
    }

    public override void Show()
    {
        base.Show();
        Pause.Value = true;
        Observable.FromCoroutine(IeCountDown).TakeUntilDisable(this).Subscribe();
    }

    private IEnumerator IeCountDown()
    {
        var t = waitTime;

        while (t > 0)
        {
            countDown.OnChanged(t);
            yield return new WaitForSecondsRealtime(1);
            t--;
        }
        countDown.OnChanged(0);
        yield return new WaitForSecondsRealtime(1);
        Close();
    }

    protected override void Close()
    {
        base.Close();
        
        Kernel.Resolve<AdsManager>().ShowInterstitial(SonatLogCreator.CreateDefaultLogInterstitial(ParameterValue.ad_break),false,false);
        //Kernel.Resolve<FireBaseController>().LogEvent("ad_break");
        PlayerData.Claim(GameSetting.adBreakReward);
        Pause.Value = false;
    }
}