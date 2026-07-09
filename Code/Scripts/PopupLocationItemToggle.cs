using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UnityEngine.Serialization;

public class PopupLocationItemToggle : BaseFrameworkView
{
    [FormerlySerializedAs("turnOff")] [SerializeField]
    private bool turnOffForDuplicate;

    [SerializeField] private LocationEnum location;
    [SerializeField] private bool popupLevel2;
    [SerializeField] private BasePopup thisPopup;

    protected override void OnKernelLoaded()
    {
        base.OnKernelLoaded();
        if (turnOffForDuplicate)
        {
            SubjectController.PopupChanged.Subscribe(data =>
            {
                if (popupLevel2)
                {
                    var currentPopup = ScreenRoot.popupManager.GetCurrentPopup();
                    Debug.Log("currentPopup " + currentPopup);
                    if (currentPopup != null)
                    {
                        if (currentPopup != thisPopup && currentPopup.LocationManager != null &&
                            currentPopup.LocationManager.GetLocation(location) != null)
                        {
                            gameObject.SetActive(false);
                            return;
                        }

                        if (currentPopup == thisPopup)
                            gameObject.SetActive(true);
                    }
                    else
                    {
                        gameObject.SetActive(true);
                    }
                }
                else
                {
                    var locationManagers = ScreenRoot.GetCurrentLocationManagers();
                    foreach (var locationManager in locationManagers)
                        if (locationManager.GetLocation(location) != null)
                        {
                            gameObject.SetActive(false);
                            return;
                        }

                    gameObject.SetActive(true);
                }
            });
        }
    }
}