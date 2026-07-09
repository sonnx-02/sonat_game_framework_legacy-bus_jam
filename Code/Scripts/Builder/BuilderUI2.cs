using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class BuilderUI2 : BaseScreenDependency
{
   [SerializeField] private Button[] preButtons;
   [SerializeField] private Button[] nextButtons;
   [SerializeField] private ToggleScript[] bindPre;
   [SerializeField] private ToggleScript[] bindNext;
   [SerializeField] private BuilderRoot.ClampType clampType;
 
   protected override void OnKernelLoaded()
   {
      base.OnKernelLoaded();
      RootView.rootView.gameController.BuilderRoot.Bind(clampType,preButtons, nextButtons, bindPre, bindNext);
   }

   public override void OnShow()
   {
      base.OnShow();
   }
}
