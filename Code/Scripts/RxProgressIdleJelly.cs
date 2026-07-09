using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class RxProgressIdleJelly : RxProgressIdle
{
   [SerializeField] private XyzCurveDatabase curveDatabase;

   private int _index;
   private CurveXyz curveXyz => curveDatabase.Curves[_index];
   protected override void Register()
   {
      base.Register();
      _index = UnityEngine.Random.Range(0,curveDatabase.Curves.Length);
   }

   protected override void OnProgress(float t)
   {
      base.OnProgress(t);
      transform.localScale = new Vector3(curveXyz.curveScaleX.Evaluate(t), curveXyz.curveScaleY.Evaluate(t), curveXyz.curveScaleZ.Evaluate(t));
   }

   protected override void OnCompleteLapse()
   {
      base.OnCompleteLapse();
      _index = UnityEngine.Random.Range(0,curveDatabase.Curves.Length);
   }
}
