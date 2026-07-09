using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameRootButton : BaseButton
{
    [SerializeField] [IndexAsEnum(BuiltInEnumType.GameRootButtonEnum)] private int key;

    [SerializeField]
    private PointParameter parameter;

    protected override void OnClick()
    {
        base.OnClick();
        RootView.rootView.gameController.HandlerButton(key,parameter);
    }

    // Update is called once per frame

    protected override string GetName()
    {
        return  "btnGameRoot_"+ ProjectSettings.GetKey(BuiltInEnumType.ShopItemKey.ToString(),key);
    }
}