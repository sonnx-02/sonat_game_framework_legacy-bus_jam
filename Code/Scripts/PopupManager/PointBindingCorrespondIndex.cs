using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointBindingCorrespondIndex : PointBindingScript
{
   public BuiltInEnumType builtInEnumType;
   [SerializeField] private IndexBindingScriptBase[] indexBindingScripts;

//   
   [IndexByBuiltInEnum(nameof(builtInEnumType))]
   public IntInt[] values;

   public override void OnChanged(Point point)
   {
      base.OnChanged(point);
      for (var i = 0; i < values.Length; i++)
      {
         if (values[i].value == point.col && values[i].value2 == point.row)
         {
            indexBindingScripts.OnChanged(i);
            break;
         }
      }
   }
}
