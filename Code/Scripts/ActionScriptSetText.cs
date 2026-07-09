using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionScriptSetText : BaseBaseActionScript
{
   [SerializeField] private BindingToStringTextChange textChange;
   [SerializeField] private bool deactivateIfEmpty = true;
   public override void StartAction(string value)
   {
      base.StartAction(value);
      if(deactivateIfEmpty)
         textChange.gameObject.SetActive(!string.IsNullOrEmpty(value));
      textChange.OnChanged(value);
   }
}
