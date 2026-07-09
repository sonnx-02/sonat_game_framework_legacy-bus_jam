using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.Serialization;

public class CountDownView : BaseFrameworkView
{
    public TextMeshProUGUI txt;
    private Action _action;
    private IDisposable _disposable;
    private DateTime _countTo;
   
    [SerializeField] private CountDownType countDownType;
    [SerializeField] private string format = "{0:D2}:{1:D2}:{2:D2}";
    [SerializeField] private string formatWithOutDays = "{0:D2}:{1:D2}:{2:D2}";

    public enum CountDownType
    {
        hhmmss,
        ddhhmm,
        ddhhmmss,
    }
    public void StartCountDown(DateTime countTo, Action onCompleteAction)
    {
        _countTo = countTo;
        _action = onCompleteAction;
        _disposable?.Dispose();
        _disposable = null;
        
        if (!SetText())
            _disposable = Observable.Interval(TimeSpan.FromSeconds(0.3333f))
                .TakeUntilDisable(this)
                . Subscribe(data =>
                {
                    SetText();
                })
                .DisposeWith(this);
    }

    public void StartCountDownToNextDay(Action onCompleteAction)
    {
        _countTo = DateTime.Today.AddDays(1);
        _action = onCompleteAction;
        _disposable?.Dispose();
        _disposable = Observable.Interval(TimeSpan.FromSeconds(1)).Subscribe(data =>
            {
                SetText();
            })
            .DisposeWith(this);
    }

    private bool SetText()
    {
        DateTime now = DateTime.Now;
        TimeSpan difference = _countTo.Subtract(now);
        switch (countDownType)
        {
            case CountDownType.hhmmss:
                txt.text = string.Format(format, difference.Hours, difference.Minutes, difference.Seconds);
                break;
            case CountDownType.ddhhmm:
                txt.text = string.Format(format, difference.Days, difference.Hours, difference.Minutes);
                break;
            case CountDownType.ddhhmmss:
                if(difference.Days > 0)
                    txt.text = string.Format(format, difference.Days, difference.Hours, difference.Minutes, difference.Seconds);
                else
                    txt.text = string.Format(formatWithOutDays,  difference.Hours, difference.Minutes, difference.Seconds); 
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        bool finish = difference.TotalSeconds <= 0;
        if (finish)
        {
            _action?.Invoke();
            _disposable?.Dispose();
            _disposable = null;
        }

        return finish;
    }
}
