using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;

[Serializable]
public class TimeTickBind
{
   [IndexAsEnum(BuiltInEnumType.CustomTimeTick)]
   public int timeTickKey;
   public IndexBindingScriptBase[] bindings;
   public BaseBaseActionScript[] onCountDownFinish;
}

public class CountDownPlayerDataTimeTick : BaseFrameworkView
{
   [SerializeField] private bool countDownOnEnable = true;
   [ArrayElementIndexTitle(nameof(TimeTickBind.timeTickKey), BuiltInEnumType.CustomTimeTick)]
   [SerializeField] private TimeTickBind[] timeTicks;
   
   private void OnEnable()
   {
      if(countDownOnEnable)
         StartCountDown();
   }

   private IDisposable _disposable;
   private void StartCountDown()
   {
      _disposable?.Dispose();
      _disposable = null;
      OnWinds(0);
      _disposable = Observable.Interval(TimeSpan.FromSeconds(1))
         .TakeUntilDisable(this)
         .Subscribe(OnWinds);
   }

   private void OnWinds(long l)
   {
      var now = DateTime.Now;
      for (var index = 0; index < timeTicks.Length; index++)
      {
         var timeTickBind = timeTicks[index];
         TimeSpan difference = PlayerData.CustomDateTime[index].Subtract(now);
         difference = difference.Add(TimeSpan.FromSeconds(1));
         var remain = (int)difference.TotalSeconds;
         if (remain < 0)
            remain = 0;
         timeTickBind.bindings.OnChanged(remain);
         if (remain <= 0)
            foreach (var baseActionScript in timeTickBind.onCountDownFinish)
               baseActionScript.StartAction();
      }
   }
}
