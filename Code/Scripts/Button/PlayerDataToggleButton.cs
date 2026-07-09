using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UnityEngine.UI;

public class PlayerDataToggleButton : BaseButton
{
    [SerializeField] private PlayerToggleEnum playerToggle;
    public IntReactiveProperty Property
    {
        get
        {
            return playerToggle switch
            {
                PlayerToggleEnum.Music => PlayerData.isMusicEnable,
                PlayerToggleEnum.Sound => PlayerData.isSoundEnable,
                PlayerToggleEnum.Vibration => PlayerData.isVibrationEnable,
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }

    [SerializeField] private ToggleScript[] toggles;
    [SerializeField] private GameObject[] on;
    [SerializeField] private GameObject[] off;

    private enum PlayerToggleEnum
    {
        Music,
        Sound,
        Vibration,
    }

    protected override string IconShortCutName => $"{playerToggle.ToString().ToSnakeCase()}_{(Property.BoolValue ? "on" : "off")}";


    protected override void OnKernelLoaded()
    {
        base.OnKernelLoaded();
        
        GetComponent<Button>().onClick.AddListener(() => { Property.BoolValue = !Property.BoolValue; });
        Property.Subscribe(data =>
        {
            foreach (var o in @on)
                o.SetActive(Property.BoolValue);
            
            foreach (var o in @off)
                o.SetActive(!Property.BoolValue);
            
            toggles.OnChanged(Property.BoolValue);
        });
    }

    protected override string GetName()
    {
        return "toggle_"+playerToggle.ToString().ToLower();
    }
}
