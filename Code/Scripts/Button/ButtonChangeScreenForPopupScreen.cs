using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonChangeScreenForPopupScreen : ButtonChangeScreen
{
   protected override void OnClickHandler()
   {
      if(ScreenRoot.popupManager.GetCurrentPopup() != null && ScreenRoot.popupManager.GetCurrentPopup() is PopupScreen)
         ScreenRoot.popupManager.GetCurrentPopup() .Close();
      else
         base.OnClickHandler();
   }
}