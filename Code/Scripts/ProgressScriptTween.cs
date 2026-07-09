using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressScriptTween : ProgressScript
{
   [MyButtonInt(nameof(CopyFrom), nameof(CopyTo))]
   [SerializeField] private  int abc123;

   [SerializeField] private bool localPos = true;
   [SerializeField] private Vector3 from;
   [SerializeField] private Vector3 to;
   [SerializeField] private Vector3 toReverse;
   
   
   [SerializeField] private bool euler;
   [SerializeField] private Vector3 fromEuler;
   [SerializeField] private Vector3 toEuler;
   [SerializeField] private Vector3 toEulerReverse;
   
   [SerializeField] private bool scale;
   [SerializeField] private Vector3 fromScale;
   [SerializeField] private Vector3 toScale;
   [SerializeField] private Vector3 toScaleReverse;
   
   protected override void Trigger(float val)
   {
      base.Trigger(val);
      if(localPos)
         transform.localPosition = Vector3.Lerp(@from, to, val);
      if(euler)
         transform.localEulerAngles = Vector3.Lerp(@fromEuler, toEuler, val);
      if(scale)
         transform.localScale = Vector3.Lerp(@fromScale, toScale, val);
   }

   public override void TriggerToggle(float progress, bool reverse)
   {
      base.TriggerToggle(progress,reverse);
      if(!reverse)
         Trigger(progress);
      else
      {
         if(localPos)
            transform.localPosition = Vector3.Lerp(@from, toReverse, progress);
         if(euler)
            transform.localEulerAngles = Vector3.Lerp(@fromEuler, toEulerReverse, progress);
         if(scale)
            transform.localScale = Vector3.Lerp(@fromScale, toScaleReverse, progress);
      }
   }
   
   public void CopyFrom()
   {
      @from = transform.localPosition;
   }
   
   public void CopyTo()
   {
      @to = transform.localPosition;
   }
   
}
