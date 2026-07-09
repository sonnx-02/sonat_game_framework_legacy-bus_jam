using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;


public class IndexBindingToToggle : IndexBindingMathCompareBase
{
    [SerializeField] private ToggleScript[] toggles; // toggles to react
    [SerializeField] private GameObject[] setActiveOn; // toggles to react
    [SerializeField] private GameObject[] setActiveOff; // toggles to react
    [SerializeField] private bool setActiveThis = true; // toggle set active this gameobject
    [SerializeField] private bool setInteractable; // set interactable if is button of something

    protected override void LoadComponent()
    {
        toggles = GetComponentsInChildren<ToggleScript>();
    }

    protected override void OnValueChanged(bool val)
    {
        toggles.OnChanged(val);
        foreach (var o in setActiveOn)
            o.SetActive(val);
        foreach (var o in setActiveOff)
            o.SetActive(!val);
        if (setActiveThis)
            gameObject.SetActive(val);
        if (setInteractable)
        {
            if (GetComponent<CanvasGroup>() != null)
                GetComponent<CanvasGroup>().interactable = val;
            if (GetComponent<Button>() != null)
                GetComponent<Button>().interactable = val;
        }

        if (val)
            foreach (var sharedSettingSound in sharedSetting.sounds)
                sharedSettingSound.PlaySound();
    }
}