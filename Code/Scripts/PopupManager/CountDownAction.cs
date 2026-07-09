using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class CountDownAction : BaseFrameworkView
{
    [SerializeField] private bool startOnEnable;
    [SerializeField] private int startValue = 10;
    [SerializeField] private float delayStartCountDown = 1;
    [SerializeField] [IndexAsEnum(BuiltInEnumType.SoundEnum)] private int soundTick;
    [SerializeField] private ProgressScript[] progresses;
    [SerializeField] private ToggleScript[] enableOnCountingDown;
    [SerializeField] private Product onFinishProduct = Product.Free;
    [SerializeField] private float speed = 1;
    [SerializeField] private float delayEach = .3f;
    [SerializeField] private float delayClaim = 1;
    
    private IDisposable _countDownDisposable;
    public void OnEnable()
    {
        if(startOnEnable)
            StartCountDown();
    }

    public void StopCountDown()
    {
        _countDownDisposable.Dispose();
        foreach (var progressScript in progresses)
            progressScript.OnChanged(0);
    }

    public void StartCountDown()
    {
        foreach (var progressScript in progresses)
            progressScript.OnChanged(startValue);
        _countDownDisposable?.Dispose();
        _countDownDisposable = Observable.FromCoroutine(IeCountDown).TakeUntilDisable(this).Subscribe();
    }

    private IEnumerator IeCountDown()
    {
        enableOnCountingDown.OnChanged(true);
        int lastValue = startValue;
        float value = startValue;
        
        if(delayStartCountDown > 0)
            yield return new WaitForSeconds(delayStartCountDown);

        while (value > 0)
        {
            value -= Time.deltaTime * speed;
            var intValue = Mathf.FloorToInt(value);
            if (intValue != lastValue)
            {
                lastValue = intValue;
                if(delayEach > 0)
                    yield return new WaitForSeconds(delayEach);
                if(intValue >= 0)
                    soundTick.PlaySound();
            }
            
            foreach (var progressScript in progresses)
                progressScript.OnChanged(value);
            yield return null;
        }
        
        enableOnCountingDown.OnChanged(false);
        if(delayClaim > 0)
            yield return new WaitForSeconds(delayClaim);
        PlayerData.Claim(onFinishProduct);
    }
}
