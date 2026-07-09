using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class BuilderUI : BaseScreenDependency
{
   [SerializeField] private TweenDisplayScript[] progressScripts;
   [SerializeField] private IndexBindingScriptBase[] currentStageBindings;
   [SerializeField] private IndexBindingScriptBase[] stockBindings;
   [SerializeField] private ToggleScript[] fillables;
   [SerializeField] private ToggleScript[] onFinish;
   [SerializeField] private OnImageHoldScript onImageHoldScript;
   [SerializeField] private Button[] preButtons;
   [SerializeField] private Button[] nextButtons;
   [SerializeField] private ToggleScript[] bindPre;
   [SerializeField] private ToggleScript[] bindNext;
   [SerializeField] private ToggleScript[] onMomentFinish;
   [SerializeField] private ToggleScript[] onNext;
   [SerializeField] private ToggleScript[] availables;
   [SerializeField] private SwiftButton[] swiftButtons;

   public SwiftButton[] SwiftButtons => swiftButtons;
   public Button[] PreButton => preButtons;
   public Button[] NextButton => nextButtons;
   public ToggleScript[] BindPre => bindPre;
   public ToggleScript[] BindNext => bindNext;

   public ToggleScript[] Availables => availables;
   
   public Action<bool> build; 

   public void ChangeStock(int stock)
   {
      stockBindings.OnChanged(stock);
   }

   private IDisposable _disposable;
   [SerializeField] private bool setUnFinishedOnShow;
   public override void OnShow()
   {
      base.OnShow();
      if(setUnFinishedOnShow)
         RootView.rootView.gameController.BuilderRoot.Focus(BuilderRoot.FocusEnum.FirstUnfinishedStage,true);
      _disposable =  Observable
         .FromCoroutine(IeBuild).Subscribe();
      
   }

   public override void OnHide()
   {
      base.OnHide();
      _disposable?.Dispose();
   }



   public void SetTarget(int target,int current)
   {
      foreach (var tweenDisplayScript in progressScripts)
      {
         tweenDisplayScript.SetTarget(target);
         tweenDisplayScript.SetTo(current);
      }
   }



   public void TweenTo(int current)
   {
      foreach (var tweenDisplayScript in progressScripts)
         tweenDisplayScript.TweenTo(current);
   }

   private IEnumerator IeBuild()
   {
      while (true)
      {
         yield return null;
         build?.Invoke(onImageHoldScript.onHold.Value);
      }
   }

   public void CurrentChange(int current)
   {
      currentStageBindings.OnChanged(current);
   }

   public void OnFillable(bool b)
   {
      fillables.OnChanged(b);
      onImageHoldScript.SetValid(b);
   }
   
   public void SetFinish(bool b)
   {
      onFinish.OnChanged(b);
   }

   public void OnFinishedMoment()
   {
      onMomentFinish.OnChanged(true);
   }
   
   public void OnNext()
   {
      onNext.OnChanged(true);
   }
}
