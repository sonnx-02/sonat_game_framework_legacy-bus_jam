using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndexBindingScriptMultiplePosition : IndexBindingScript
{
   [SerializeField] private Vector3 multi;
   [SerializeField] private Vector3 adjust;
   [SerializeField] private bool anchor;
   protected override void OnValueChanged(int computedValue)
   {
      base.OnValueChanged(computedValue);
      if(anchor)
         (transform as RectTransform).anchoredPosition = computedValue * multi + adjust;
      else
         transform.localPosition = computedValue * multi + adjust;
   }
}
