using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuilderItem : PoolItem
{
   [DisplayAsDisable]
   [SerializeField] private int current;

   [SerializeField] private BoolBindingReactiveProperty bindFinish;
   [SerializeField] private ProgressScript[] bindProgress;
   [SerializeField] private BoolBindingReactiveProperty isInProgress; // is current building
   [SerializeField] private ProgressScriptInt[] progressScriptInt;
   [SerializeField] private BaseBaseActionScript[] actionOnFinish;
   [SerializeField] private BaseBaseActionScript[] actionOnFinishWhole;

   protected override void Register()
   {
      base.Register();
      bindFinish.Init();
      isInProgress.Init();
   }

   public void OnAllFinish()
   {
      foreach (var baseActionScript in actionOnFinishWhole)
         baseActionScript.StartAction();
   }

   private int _target;
   public void SetTarget(int target,int currentValue,bool isCurrent)
   {
      _target = target;
      foreach (var scriptInt in progressScriptInt)
         scriptInt.SetTarget(target);
      SetCurrent(currentValue,false);
      SetInprogress(isCurrent);
      bindFinish.Value = IsFinish();
      _once = IsFinish();
   }
   
   public void SetInprogress(bool value)
   {
      isInProgress.Value = value;
   }

   private bool _once;
   public void SetCurrent(int value, bool check = true)
   {
      current = value;
      UpdateProgress();
      if (check && !_once && IsFinish())
      {
         _once = true;
         foreach (var baseActionScript in actionOnFinish)
            baseActionScript.StartAction();
         bindFinish.Value = true;
      }
   }
   
   private void UpdateProgress()
   {
      float progress = current / Mathf.Max(_target,1f); // for divide by zero
      foreach (var progressScript in bindProgress)
         progressScript.OnChanged(progress);
      foreach (var scriptInt in progressScriptInt)
         scriptInt.OnChanged(current);
   }


//   public void Add(int add)
//   {
//      current += add;
//      if (IsFinish())
//      {
//         foreach (var baseActionScript in actionOnFinish)
//            baseActionScript.StartAction();
//         bindFinish.Value = true;
//      }
//   }

   public bool IsFinish() => current >= _target;
}
