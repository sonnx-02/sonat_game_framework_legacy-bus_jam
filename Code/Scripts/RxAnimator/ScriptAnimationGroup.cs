using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

public class ScriptAnimationGroup : MonoBehaviour
{
   public ScriptAnimationItem[] Items;

   public bool IdleOnEnable = true;

   private float t = 0;
   private IDisposable idle;
   void OnEnable()
   {
      if (IdleOnEnable)
      {
         var maxTime = Items.Max(x => x.GetLargestTime());
         idle = this.UpdateAsObservable()
            .Subscribe(_ =>
            {
               t += Time.deltaTime;
               while (t > maxTime && maxTime > 0)
                  t -= maxTime;
               
               foreach (var item in Items)
               {
                  item.OnChanged(t);
               }
            });
      }
   }

   private void OnDisable()
   {
      idle?.Dispose();
      t = 0;
   }
}
