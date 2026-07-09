using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogWifi : BaseDialog
{
   public override void Show()
   {
      base.Show();
      ScreenRoot.TurnRayCast(false);
      BaseGameController.Stop = true;
   }

   protected override void Close()
   {
      base.Close();
      BaseGameController.Stop = false;
   }

   private void OnDisable()
   {
      ScreenRoot.TurnRayCast(true);
      BaseGameController.Stop = false;
   }
}
