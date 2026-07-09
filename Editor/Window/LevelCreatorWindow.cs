using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

// ReSharper disable Unity.InefficientPropertyAccess

public class LevelCreatorWindow : BaseManagerWindow
{
    [MenuItem("SonatFramework/" + nameof(LevelCreatorWindow))]
    public static void ShowWindow()
    {
        GetWindow(typeof(LevelCreatorWindow));
    }

    private EditorPrefInt _flagHide = new EditorPrefInt(nameof(LevelCreatorWindow) + "Selection");

    protected override void Prepare()
    {
        base.Prepare();
        if (EnumValues == null)
            EnumValues = (int[]) Enum.GetValues(typeof(HideFlag));
    }

    protected override void OnGUIHidable()
    {
        StartHorizon(() =>
        {
            for (var index = 0; index < EnumValues.Length; index++)
            {
                var value = EnumValues[index];
                GUI.enabled = (value != _flagHide.Value);
                if (GUILayout.Button(((HideFlag) value).ToString(), value == _flagHide.Value ? Style : Style2))
                {
                    _flagHide.Value = value;
                    Repaint();
                }

                GUI.enabled = true;
            }
        });
    }

    public override void OnGUI()
    {
        base.OnGUI();
//            StartHorizon(() =>
//            {
//                if (GUILayout.Button("Focus"))
//                {
//                }
//            });
//            EditorGUILayout.Space(10);
    }


//    private BuilderRoot _builderRoot;

    private enum HideFlag
    {
        //        BuilderRoot ,
        Flag0,
        Flag1,
        Flag2,
        Flag3,
    }

    private bool IsEnable(HideFlag flag)
    {
        return ((int) flag & _flagHide.Value) == 0;
    }

    private HideFlag GetCurrent()
    {
        foreach (var enumValue in EnumValues)
            if (_flagHide.Value == enumValue)
                return (HideFlag) enumValue;

        return (HideFlag) 0;
    }
}