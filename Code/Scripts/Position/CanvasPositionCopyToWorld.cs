using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class CanvasPositionCopyToWorld : ByCameraAdjustScript
{
   [SerializeField] private bool autoStart = true;
   [SerializeField] private Camera persispectiveCam;
   [SerializeField] private float keepUpdateOnStartUntil;
   [SerializeField] private bool startOnEnable;
   [SerializeField] private RectTransform rectTransform;
   [SerializeField] private Transform copyTo;
   [SerializeField] private Bool3 copy = new Bool3(true,true,true);

   [MyButton(nameof(SetPos))] [SerializeField]
   private int myFunction;

   void Start()
   {
      if(GetComponent<Image>() != null)
         Destroy(GetComponent<Image>());
      if (autoStart)
      {
         Bind();
      }
   }

   public override void Bind()
   {
      if (keepUpdateOnStartUntil > 0.1f)
         Observable.FromMicroCoroutine(() => KeepUpdate(keepUpdateOnStartUntil)).Subscribe();
   }

   private IEnumerator KeepUpdate(float time)
   {
     var t = 0f;
     while (t < time)
     {
        SetPos();
        t += Time.deltaTime;
        yield return null;
     }
   }
   
   private void OnEnable()
   {
      if(startOnEnable)
         .5f.Timer(SetPos);
   }

   public void SetPos()
   {
      if (persispectiveCam == null)
      {
         var position = rectTransform.position;
         var position1 = copyTo.position;
         copyTo.position  
            = new Vector3(
               copy.value1 ? position.x : position1.x,
               copy.value2 ? position.y : position1.y,
               copy.value3 ? position.z : position1.z
            );
      }
      else
      {
         Debug.LogError("not handler");
      }
   }

   private void Reset()
   {
      rectTransform.GetComponent<RectTransform>();
   }
}
