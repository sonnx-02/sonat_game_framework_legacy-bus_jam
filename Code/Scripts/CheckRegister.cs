using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckRegister : BaseFrameworkView
{
   [SerializeField] private BaseFrameworkView[] onRegisters;
   [SerializeField] private BaseFrameworkView[] onKernelLoaded;

   protected override void Register()
   {
      base.Register();
      foreach (var baseFrameworkView in onRegisters)
         baseFrameworkView.CheckRegister();
   }

   protected override void OnKernelLoaded()
   {
      base.OnKernelLoaded();
      foreach (var baseFrameworkView in onKernelLoaded)
         baseFrameworkView.CheckKernelLoaded();
   }
}
