#define dummy
//#define use_facebook
//#define use_playfab

using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


#if !(dummy || global_dummy && !use_facebook && !use_playfab)
using Facebook.Unity;
using PlayFab;
using PlayFab.ClientModels;
using LoginResult = PlayFab.ClientModels.LoginResult;
#endif


public class LoginFacebookButton : BaseView
{
    [SerializeField] private Button btn;
    [SerializeField] private bool logInOut;
    [SerializeField] private ToggleScript[] loggedToggles;

    
#if !(dummy || global_dummy && !use_facebook && !use_playfab)
    protected override void OnKernelLoaded()
    {
        Kernel.Resolve<FacebookController>().loggedEvent += data => loggedToggles.OnChanged(FB.IsLoggedIn);
        btn.onClick.AddListener(() =>
            {
                if (FB.IsLoggedIn)
                    Kernel.Resolve<FacebookController>().LogOut();
                else
                    Kernel.Resolve<FacebookController>().LoginWithFacebook();
            });
    }
   

    private void OnEnable()
    {
        loggedToggles.OnChanged(FB.IsLoggedIn);
    }

#endif
}