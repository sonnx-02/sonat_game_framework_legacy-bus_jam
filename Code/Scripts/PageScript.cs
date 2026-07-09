using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PageScript : BaseFrameworkView
{
   [SerializeField] private ToggleScript[] onCurrents;
   [SerializeField] private bool setActive;
   private RectTransform _rectTransform;
   public RectTransform RectTransform
   {
      get
      {
         if (_rectTransform == null)
            _rectTransform = GetComponent<RectTransform>();
         return _rectTransform;
      }
   }

   private bool _isCurrent;
   public void SetCurrent(bool isCurrent)
   {
      _isCurrent = isCurrent;
      onCurrents.OnChanged(_isCurrent);
      if(setActive)
         gameObject.SetActive(isCurrent);
   }
}
