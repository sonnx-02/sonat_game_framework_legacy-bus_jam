using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndexPropertyChangeActionScript : BaseIndexPropertySourceRetriever
{
   [SerializeField] private int value;
   [SerializeField] private IntBool startOnEnable;
   [SerializeField] private IntBool startOnDisable;

   protected virtual void OnEnable()
   {
      if(startOnEnable.value2)
         Property.Value = startOnEnable.value;
   }

   protected virtual  void OnDisable()
   {
      if(startOnDisable.value2)
         Property.Value = startOnDisable.value;
   }

   public virtual void Invoke()
   {
      Property.Value = value;
   }
}
