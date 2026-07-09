using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class EditorSlotView : PoolItemPoint
{
   [SerializeField] private PointReactiveProperty point;
   [SerializeField] private ToggleScript[] pointEventBinds;

   protected override void Register()
   {
      base.Register();
      point.Subscribe(data =>
      {
         BindPointEvent(data.IsEven());
//         transform.localPosition = data.GetPos();
      });
   }

   public override void SetPoint(Point p)
   {
      base.SetPoint(p);
      point.Value = p;
   }

   public override void SetPointOnly(Point p)
   {
      base.SetPointOnly(p);
      point.Value = p;
   }

   private void BindPointEvent(bool isEven)
   {
      pointEventBinds.OnChanged(isEven);
   }
}
