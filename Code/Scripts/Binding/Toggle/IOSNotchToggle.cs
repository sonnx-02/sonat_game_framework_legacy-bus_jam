using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IOSNotchToggle : MonoBehaviour
{
    [SerializeField] private ToggleScript[] toggles;
    [SerializeField] private bool testNotch;
    private void Start()
    {
        #if UNITY_EDITOR
        if (testNotch)
        {
            toggles.OnChanged(true); 
            return;
        }
        #endif
#if UNITY_IOS
      var notch = DuongExtensions.HaveNotch();
      toggles.OnChanged(notch);
#endif
    }
}