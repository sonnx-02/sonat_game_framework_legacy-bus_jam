using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndexBindingListActiveToggle : IndexBindingScript
{
   [SerializeField] private IndexActiveToggle[] items;
   protected override void OnValueChanged(int computedValue)
   {
      base.OnValueChanged(computedValue);
      for (var i = 0; i < items.Length; i++)
      {
         items[i].OnChanged(computedValue);
      }
   }
}
