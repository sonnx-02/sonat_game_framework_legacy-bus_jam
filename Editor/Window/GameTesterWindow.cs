using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

// ReSharper disable Unity.InefficientPropertyAccess

public class GameTesterWindow : BaseManagerWindow
{
    [MenuItem("SonatFramework/" + nameof(GameTesterWindow))]
    public static void ShowWindow()
    {
        GetWindow(typeof(GameTesterWindow));
    }

    private EditorPrefInt _flagHide = new EditorPrefInt(nameof(GameTesterWindow) + "Selection");

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
                if (GUILayout.Button(((HideFlag) value).ToString(), value == _flagHide.Value ? Style2 : Style))
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

     
        EditorGUILayout.Space(5);
        switch (GetCurrent())
        {
            case HideFlag.Framework:
                OnFramework();
                break;
            case HideFlag.GameController:
                OnGameController();
                break;
            case HideFlag.Flag2:
                break;
            case HideFlag.Flag3:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private string CustomMethod
    {
        get => EditorPrefs.GetString("CustomMethod");
        set =>  EditorPrefs.SetString("CustomMethod", value);
    }
    private int Level
    {
        get => EditorPrefs.GetInt("Level");
        set => EditorPrefs.SetInt("Level", value);
    }
    private void OnGameController()
    {
        EditorGUIUtility.labelWidth = 100;

        StartHorizon(() =>
        {
            Level = EditorGUILayout.IntField("Level", Level);
            if (GUILayout.Button("Play Level"))
            {
                RootView.rootView.gameController.StartLevel(Level);
                RootView.rootView.gameController.CalculateUpdateQuest(-1);
            }
            if (GUILayout.Button("<<"))
            {
                Level--;
                RootView.rootView.gameController.StartLevel(Level);
                RootView.rootView.gameController.CalculateUpdateQuest(-1);
            }
            if (GUILayout.Button(">>"))
            {
                Level++;
                RootView.rootView.gameController.StartLevel(Level);
                RootView.rootView.gameController.CalculateUpdateQuest(-1);
            }
        });
        StartHorizon(() =>
        {
            CustomMethod = EditorGUILayout.TextField("Method Name",CustomMethod);
            if (GUILayout.Button("Invoke"))
            {
                var type = RootView.rootView.gameController.GetType();
                var method =
                    type.GetMethod(
                        CustomMethod,
                        BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public,
                        Type.DefaultBinder,
                        new Type[] { },
                        null);

                if (method == null)
                {
                    Debug.LogError("function null");
                    return;
                }
                method.Invoke(RootView.rootView.gameController, null);
            }
        });
        
        StartHorizon(() =>
        {
            if (GUILayout.Button("gameController.SaveTemp()"))
            {
                RootView.rootView.gameController.SaveTemp();
            }

            if (GUILayout.Button("gameController.LoadTemp()"))
            {
                RootView.rootView.gameController.LoadTemp();
            }
        });
    }

    private readonly EnumIntPlayerPref[] _enumInt = new EnumIntPlayerPref[10];
    public NotifyEnum NotifyEnum;
    public GameActionEvent GameActionEvent;

    private void OnFramework()
    {
        StartHorizon(() =>
        {
            GameActionEvent =
                (GameActionEvent) EditorGUILayout.EnumPopup(this.GameActionEvent);
            if (GUILayout.Button("GameActionEvent"))
            {
                SubjectController.GameActionEvent.OnNext(GameActionEvent);
            }
        });
        StartHorizon(() =>
        {
            int i = 0;
            var type = BuiltInEnumType.GlobalCallBackEnum;
            if (_enumInt[i] == null)
                _enumInt[i] = new EnumIntPlayerPref(type);
            _enumInt[i].Value = EditorGUILayout.IntPopup(_enumInt[i].Value, _enumInt[i].EnumNames.ToArray(),
                _enumInt[i].EnumValues.ToArray());
            if (GUILayout.Button(type.ToString()))
            {
                SubjectController.OnPointParameterCallBack.OnNext(
                    new PointParameter(BuiltInEnumType.GlobalCallBackEnum, _enumInt[i].Value));
            }
        });
        StartHorizon(() =>
        {
            int i = 1;
            var type = BuiltInEnumType.ProductGameAction;
            if (_enumInt[i] == null)
                _enumInt[i] = new EnumIntPlayerPref(type);
            _enumInt[i].Value = EditorGUILayout.IntPopup(_enumInt[i].Value, _enumInt[i].EnumNames.ToArray(),
                _enumInt[i].EnumValues.ToArray());
            if (GUILayout.Button(type.ToString()))
            {
                SubjectController.ProductGameActionEvent.OnNext(new ProductAction()
                {
                    index = _enumInt[i].Value,
                    amount = 1
                });
            }
        });
        StartHorizon(() =>
        {
            NotifyEnum = (NotifyEnum) EditorGUILayout.EnumPopup(this.NotifyEnum);
            if (GUILayout.Button("NotifyEnum"))
            {
                SubjectController.NotifyEvent.OnNext(NotifyEnum);
            }
        });
    }


//    private BuilderRoot _builderRoot;

    private enum HideFlag
    {
        //        BuilderRoot ,
        GameController,
        Framework,
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