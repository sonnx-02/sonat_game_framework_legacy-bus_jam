using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class AchievementItemView_StupidUpdateEnable : AchievementItemView
{
   [SerializeField] private BaseRxAnimator[] anims;
   [SerializeField] private IndexBindingScriptBase[] addBinds;
   [SerializeField] private float delayTween = 1f;
   [SerializeField] private BaseRxAnimator[] animsOnTween;

   private PlayerPrefInt _incrementValue;
   private float _currentTempValue = 0f;
   // step 1 : load value on start to _currentTempValue
   // every time enable update new value

   protected override void Register()
   {
      base.Register();
      _incrementValue = new PlayerPrefInt("achievement_stupid_"+key);
      _incrementValue.Value = BaseFrameworkView.PlayerData.achievementData[key];
   }
   
   void OnEnable()
   {
      SubjectController.PopupChanged
         .TakeUntilDisable(this)
         .Subscribe(data => { UpdateStatus(); });
   }

   public override void UpdateStatus()
   {
      CheckRegister();
      if (_data == null)
         return;
      BaseUpdateStatus();
      
      var progress = Data.GetFragmentProgress();
      
      var incrementProgressValue = Data.IncrementProgressValue;
      if (_incrementValue.Value < incrementProgressValue)
      {
         foreach (var baseRxAnimator in anims)
            baseRxAnimator.Animator_In();
         addBinds.OnChanged(incrementProgressValue - _incrementValue.Value);

         delayTween.Timer(() =>
         {
            Tween(progress);
            foreach (var baseRxAnimator in animsOnTween)
               baseRxAnimator.Animator_In();
         });
      }
      else
      {
         Tween(progress);
      }

      _incrementValue.Value = Data.IncrementProgressValue;
   }

   
   private void Tween(float progress)
   {
      foreach (var progressScript in fragmentProgress)
         if (tween)
            progressScript.OnChangedTween(progress,timeTween);
         else
            progressScript.OnChanged(progress);
      foreach (var bindingToStringTextChange in fragmentProgressText)
         bindingToStringTextChange.OnChanged(Data.GetFragmentProgressString());
   }
}
