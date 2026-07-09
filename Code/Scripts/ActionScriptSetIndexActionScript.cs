using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionScriptSetIndexActionScript : BaseActionScriptInput
{
   [SerializeField] private BaseBaseActionScript[] setIndexes;

   protected override void DoAction()
   {
      base.DoAction();
      foreach (var baseBaseActionScript in setIndexes)
         baseBaseActionScript.StartAction(Value);
   }
}
