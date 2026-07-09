using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogMiniPop : BaseDialog
{
   [SerializeField] private DialogMiniPopItem[] items;

   protected override void Register()
   {
      base.Register();
      items.SetActive(false);
   }

   [MyButtonInt(nameof(TestShowPopup),BuiltInEnumType.MiniPopEnum)] [SerializeField]
   private int testShow;

   public void TestShowPopup(int a)
   {
      ShowMiniPop(a);
   }  
   
   public DialogMiniPopItem ShowMiniPop(int key)
   {
      items.SetActive(false);

      DialogMiniPopItem popup = null;
      foreach (var dialogMiniPopItem in items)
      {
         if (dialogMiniPopItem.Key == key)
         {
            dialogMiniPopItem.gameObject.SetActive(true);
            dialogMiniPopItem.Show();
            showBlockUi = dialogMiniPopItem.showBlockUI;
            popup =  dialogMiniPopItem;
            break;
         }
      }

      Show();
      if(popup == null)
         Debug.LogError("err not found popup");
      return popup;
   }
}
