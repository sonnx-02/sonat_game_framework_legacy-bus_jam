using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseActionScriptIndexPropertyChange : BaseActionScript
{
   [SerializeField] protected IndexPropertyChangeActionScript[] others;

   protected override void DoAction()
   {
      base.DoAction();
      foreach (var indexPropertyChangeActionScript in others)
         indexPropertyChangeActionScript.Invoke();
   }
}
