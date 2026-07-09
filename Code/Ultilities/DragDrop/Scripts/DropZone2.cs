using System;
using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class DropZone2 : DropZone
{
   public float minDistance = 0;
   public void Calculate()
   {
      if(transform.childCount > 1)
         minDistance = Vector3.Distance(transform.GetChild(0).position, transform.GetChild(1).position);
   }
}
