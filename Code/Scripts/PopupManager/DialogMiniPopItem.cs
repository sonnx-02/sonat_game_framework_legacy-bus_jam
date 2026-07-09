using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogMiniPopItem : BaseFrameworkView
{
   [SerializeField] [IndexAsEnum(BuiltInEnumType.MiniPopEnum)]
   private int key;

   public bool showBlockUI = true;
   [SerializeField] private BaseRxAnimator[] animators;
   [SerializeField] private ToggleScript[] toggles;
   [SerializeField] private IndexBindingScriptBase[] indexBindings;
   [SerializeField] private BindingToStringTextChange textChange;
   [SerializeField] private float deactiveAfter = 1;
   [SerializeField] private bool setToggleTrueOnShow;
   public int Key => key;

   public DialogMiniPopItem SetToggle(bool toggle)
   {
      toggles.OnChanged(toggle);
      return this;
   }
   
   public DialogMiniPopItem SetIndex(int index)
   {
      indexBindings.OnChanged(index);

      return this;
   }
   
   public DialogMiniPopItem SetString(string mess)
   {
      textChange.OnChanged(mess);
      return this;
   }

   public void Show()
   {
      foreach (var baseRxAnimator in animators)
         baseRxAnimator.Animator_In();

      if(deactiveAfter > 0)
         deactiveAfter.Timer(() => RootView.rootView.screenRoot.dialogController.CloseAll());

      if (setToggleTrueOnShow)
         SetToggle(true);
   }

   protected override string GetName()
   {
      return base.GetName() +"_"+ProjectSettings.GetKey(BuiltInEnumType.MiniPopEnum.ToString(),key);
   }
}
