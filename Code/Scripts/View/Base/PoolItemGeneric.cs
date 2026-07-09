using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolItemGeneric : PoolItem
{
   [SerializeField] private BaseBaseActionScript[] actionScripts;
   [SerializeField] private ToggleScript[] toggles;
   [SerializeField] private IndexBindingScriptBase[] indexBindings;
   [SerializeField] private ToggleScript[] togglesIsLast;

   public ToggleScript[] Toggles => toggles;
   public IndexBindingScriptBase[] IndexBindings => indexBindings;

   public ToggleScript[] TogglesIsLast => togglesIsLast;

   public void StartAction()
   {
      foreach (var baseActionScript in actionScripts)
      {
         baseActionScript.StartAction();
      }
   }
}
