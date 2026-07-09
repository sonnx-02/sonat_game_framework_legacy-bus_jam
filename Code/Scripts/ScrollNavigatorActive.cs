using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class ScrollNavigatorActive : MonoBehaviour
{
   [SerializeField] private ScrollRect scrollView;
   [SerializeField] private ToggleScript before;
   [SerializeField] private ToggleScript after;
   
   [SerializeField] private float beforeValue;
   [SerializeField] private float afterValue;
   
   private void Start()
   {
      if (scrollView == null)
         scrollView = GetComponent<ScrollRect>();
      scrollView.onValueChanged.AddListener(WhenValueChanged);
   }

   private void OnEnable()
   {
   }

   private void WhenValueChanged(Vector2 arg0)
   {
      if (scrollView.horizontal)
      {
         if (before != null) before.OnChanged(arg0.x > beforeValue);
         if (after != null) after.OnChanged(arg0.x < afterValue);
      }
      else
      {
         if (after != null) after.OnChanged(arg0.y < beforeValue);
         if (before != null) before.OnChanged(arg0.y > afterValue);
      }
   }
}
