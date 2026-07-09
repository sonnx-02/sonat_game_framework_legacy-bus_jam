using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UniRx;

/// <summary>
/// to many integer properties from one source
/// </summary>
public abstract class GetPointScriptBase : BaseFrameworkView
{
   [SerializeField] private BuiltInEnumType enumType;
   [SerializeField] private int enumValue;

   public virtual PointParameter GetPoint()
   {
      var point = new PointParameter(enumType,enumValue); 
      return point;
   }
   
   public abstract PointParameter GetPoint(BuiltInEnumType type,int afterInCallBack); // value is afterInCallBack
}
