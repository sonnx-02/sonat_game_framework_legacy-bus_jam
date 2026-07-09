using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndexBindingTweenProgressScript : IndexBindingScript
{
   [SerializeField] private ProgressScript[] progressScripts;
   [SerializeField] private float startValue;
   [SerializeField] private float[] values;

   protected override void Register()
   {
      base.Register();
      foreach (var progressScript in progressScripts)
         progressScript.OnChanged(startValue);
   }

   protected override void OnValueChanged(int computedValue)
   {
      base.OnValueChanged(computedValue);
      foreach (var progressScript in progressScripts)
         progressScript.OnChanged(values[Mathf.Clamp(computedValue,0,values.Length)]);
   }
}
