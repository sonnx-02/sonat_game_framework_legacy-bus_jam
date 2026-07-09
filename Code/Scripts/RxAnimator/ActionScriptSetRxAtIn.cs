using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionScriptSetRxAtIn : BaseBaseActionScript
{
   [SerializeField] private BaseRxAnimator[] rxAnimators;
   protected override void DoAction()
   {
      base.DoAction();
      foreach (var baseRxAnimator in rxAnimators)
      {
         baseRxAnimator.SetAtIn();
      }
   }
}
