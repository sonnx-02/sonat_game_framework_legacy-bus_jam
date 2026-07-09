using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class TweenScaleScript : BaseTweenScript
 {
     [SerializeField] private Vector3 from = new Vector3(.5f, .5f, .5f);
     [SerializeField] private Vector3 to = new Vector3(1, 1, 1);
     [SerializeField] private float time = 1;
     [SerializeField] private float delay = 0;
     [SerializeField] private Ease ease;
     [SerializeField] private bool yoyo;
 
     public override void Tween()
     {
         base.Tween();
         transform.localScale = @from;
         if(yoyo)
             transform.DOScale(to, time).SetEase(ease).SetDelay(delay).SetLoops(2, LoopType.Yoyo);
         else
             transform.DOScale(to, time).SetEase(ease).SetDelay(delay);
     }
 
     private void OnDisable()
     {
         transform.DOKill();
     }
 
     public override void ResetTween()
     {
         base.ResetTween();
         transform.localScale = @from;
     }
 }