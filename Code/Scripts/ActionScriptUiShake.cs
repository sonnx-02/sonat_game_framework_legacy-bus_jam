using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class ActionScriptUiShake : BaseBaseActionScript
{
   [SerializeField] private AnimationCurve curve;
   [SerializeField] private Vector3 rotateTo;
   [SerializeField] private float duration = 0.3f;
   [SerializeField] private float delay = 0;
   [SerializeField] private Transform[] targets;
   [SerializeField] private CurveType curveType;
   public override void StartAction()
   {
      base.StartAction();
      Observable.FromCoroutine(IeAction).TakeUntilDisable(this).Subscribe();
   }

   public enum CurveType
   {
      Lerp,
      Multi
   }

   private IEnumerator IeAction()
   {
      float t = 0;
      yield return new WaitForSeconds(delay);
      while (t < duration)
      {
         Vector3 value = Vector3.zero;

         switch (curveType)
         {
            case CurveType.Lerp:
               value = Vector3.Lerp(Vector3.zero, rotateTo,
                  curve.Evaluate(t / duration));
               break;
            case CurveType.Multi:
               value = rotateTo * curve.Evaluate(t / duration);
               break;
            default:
               throw new ArgumentOutOfRangeException();
         }
       
         
         
         foreach (var target in targets)
            target.localEulerAngles = value;
         yield return null;
         t += Time.deltaTime;
      }
   }
}
