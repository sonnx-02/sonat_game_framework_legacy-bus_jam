using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ActionScriptUnityEvent : BaseBaseActionScript
{
   [SerializeField] private UnityEvent _unityEvent;

   protected override void DoAction()
   {
      base.DoAction();
      _unityEvent.Invoke();
   }
}
