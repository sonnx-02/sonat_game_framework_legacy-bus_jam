using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class TutorialPopupButton : BaseButton
{
   [SerializeField] private UnityEvent action;
   [IndexAsEnum(BuiltInEnumType.TutorialPopupKey)]
   [SerializeField] private int tutorialPopup;
  
   protected override void OnClick()
   {
      base.OnClick();
      var resolve = ScreenRoot.popupManager.Resolve<PopupTutorial>();
      if (!resolve.CheckTut(tutorialPopup, Action))
      {
         Action();
      }
   }

   private void Action()
   {
      action.Invoke();
   }
}