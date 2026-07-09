using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleScriptAnimator : ToggleScript
{
   [SerializeField] private BaseRxAnimator[] animators;
   
   
   protected override void LoadComponent()
   {
      animators = GetComponentsInChildren<BaseRxAnimator>(true);
   }

   public bool triggerLastOnEnable1 = false;
   public bool useLast = true;
   private bool _last;


   protected override void OnEnable()
   {
      base.OnEnable();
      if(triggerLastOnEnable1)
         Trigger(_last);
   }

   protected override void Trigger(bool val)
   {
      if (_last != val || !useLast)
      {
         _last = val;
         if(val)
            foreach (var baseRxAnimator in animators)
               baseRxAnimator.Animator_In();
         else
            foreach (var baseRxAnimator in animators)
               baseRxAnimator.Animator_Out();
      }
   }
}
