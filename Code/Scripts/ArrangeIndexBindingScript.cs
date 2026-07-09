using System.Collections;
 using System.Collections.Generic;
 using UnityEngine;
 
 
 public class ArrangeIndexBindingScript : IndexBindingScript
 {
     public Transform[] targets;
 
     [SerializeField] private float gap;
 
     [MyButton(nameof(Arrange))] public int abc123;
 
     protected override void OnValueChanged(int computedValue)
     {
         base.OnValueChanged(computedValue);
         for (var i = 0; i < computedValue; i++)
         {
             targets[i].localPosition = new Vector3(DuongExtensions.GetArrangePos(computedValue, i, gap), 0, 0);
         }
     }
 
     public void Arrange()
     {
         for (var i = 0; i < targets.Length; i++)
         {
             targets[i].localPosition = new Vector3(DuongExtensions.GetArrangePos(targets.Length, i, gap), 0, 0);
         }
     }
 }