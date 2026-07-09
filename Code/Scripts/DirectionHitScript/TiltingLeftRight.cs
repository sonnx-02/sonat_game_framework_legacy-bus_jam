using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class TiltingLeftRight : BaseFrameworkView
{
   [SerializeField] private float sizeDistance = 0.5f;
   [SerializeField] private float rotateAngle = 20;
   [SerializeField] private Vector3 axisVector = Vector3.forward;
   [SerializeField] private float time = 1;
   [SerializeField] private AnimationCurve curve = AnimationCurve.Linear(0, 0, 1, 1);
   
   
   public override void LogCustom()
   {
      base.LogCustom();
      transform.position = Vector3.zero;
      transform.rotation = Quaternion.identity;
      transform.RotateAround(transform.position - new Vector3(sizeDistance,0,0), axisVector, rotateAngle);
   }
   
   public override void LogCustomInt(int value)
   {
      base.LogCustomInt(value);
      transform.position = Vector3.zero;
      transform.rotation = Quaternion.identity;
      transform.RotateAround(transform.position + new Vector3(sizeDistance,0,0), axisVector, -rotateAngle);
   }
   
   private IDisposable _disposable;

   [MyButtonToggle(nameof(Hit))] public int testHit;
   
   public void Hit(bool leftOrRight)
   {
      _disposable?.Dispose();
      _disposable = null;
      _disposable = Observable.FromCoroutine(() => OnHit(leftOrRight)).Subscribe();
   }

   private IEnumerator OnHit(bool leftOrRight)
   {
      float t = 0f;
      var transform1 = transform;
      transform1.rotation = Quaternion.identity;
      var position = transform1.position;
      var pos = leftOrRight ? position + new Vector3(sizeDistance,0,0) : position - new Vector3(sizeDistance,0,0);
      var lr = leftOrRight ? rotateAngle : -rotateAngle;
      
      while (t < time)
      {
         t += Time.deltaTime;
         transform.RotateAround(pos, axisVector, - lr * (Time.deltaTime/time));
         yield return null;
      }

      t = 0f;
      while (t < time)
      {
         t += Time.deltaTime;
         transform.RotateAround(pos, axisVector, + lr * (Time.deltaTime/time));
         yield return null;
      }
   }
}
