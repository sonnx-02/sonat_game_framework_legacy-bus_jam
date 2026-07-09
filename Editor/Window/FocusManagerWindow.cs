using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

// ReSharper disable Unity.InefficientPropertyAccess

public class FocusManagerWindow : BaseManagerWindow
{
    [MenuItem("SonatFramework/" + nameof(FocusManagerWindow))]
    public static void ShowWindow()
    {
        GetWindow(typeof(FocusManagerWindow));
    }

    private EditorPrefInt _flagHide = new EditorPrefInt(nameof(FocusManagerWindow) + "Selection");


    private RootView _rootView;
    private BaseBaseLevelCreator _levelCreator;

    private AudioManager _audioManager;
//    private BuilderRoot _builderRoot;

    private enum HideFlag
    {
        //        BuilderRoot ,
        GameController,
        RootView,
        ScreenRoot,
        PoolManager,
        AudioManager,
        ShopDatabase,
        GameSetting,
        CurrentGameSetting,
        LevelDatabase,
        LevelCreator,
    }

    private bool IsEnable(HideFlag flag)
    {
        return ((int) flag & _flagHide.Value) == 0;
    }

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
            if (GUILayout.Button("Start Scene"))
            {
                EditorSceneManager.OpenScene(
                    $"Assets/{_projectSettings.projectSku}/Scenes/{_projectSettings.startSceneName}.unity");
            }

            if (GUILayout.Button("Level Editor"))
            {
                EditorSceneManager.OpenScene(
                    $"Assets/{_projectSettings.projectSku}/Scenes/{_projectSettings.levelEditorSceneName}.unity");
            }

            if (GUILayout.Button("Test"))
            {
                EditorSceneManager.OpenScene(
                    $"Assets/{_projectSettings.projectSku}/Scenes/Test.unity");
            }
        });
        EditorGUILayout.Space(10);

        StartHorizon(() =>
        {
            if (GUILayout.Button("Focus", StyleRed))
            {
                FocusTarget();
            }
        });
        StartHorizon(() =>
        {
            for (var index = 0; index < EnumValues.Length / 2; index++)
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
        StartHorizon(() =>
        {
            if (EnumValues != null)
                for (var index = EnumValues.Length / 2; index < EnumValues.Length; index++)
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


        if (_rootView == null) _rootView = ((RootView) FindObjectOfType(typeof(RootView), true));


        EditorGUILayout.Space(10);
        UnityEditor.Editor editor = GetEditor();
        if (editor != null)
        {
            ScrollPosition = EditorGUILayout.BeginScrollView(ScrollPosition);
            editor.OnInspectorGUI();
            EditorGUILayout.EndScrollView();
        }
    }

    private UnityEditor.Editor GetEditor()
    {
        switch (GetCurrent())
        {
            case HideFlag.GameController:
                if (_rootView != null) return UnityEditor.Editor.CreateEditor(_rootView.gameController);
                break;
            case HideFlag.RootView:
                if (_rootView != null) return UnityEditor.Editor.CreateEditor(_rootView);
                break;
            case HideFlag.ScreenRoot:
                if (_rootView != null) return UnityEditor.Editor.CreateEditor(_rootView.screenRoot);
                break;
            case HideFlag.PoolManager:
                if (_rootView != null) return UnityEditor.Editor.CreateEditor(_rootView.poolManager);
                break;
            case HideFlag.AudioManager:
                if (_rootView != null) return UnityEditor.Editor.CreateEditor(_rootView.audioManager);
                break;
            case HideFlag.ShopDatabase:
                if (_rootView != null) return UnityEditor.Editor.CreateEditor(_rootView.FindDatabase<ShopDatabase>());
                break;
            case HideFlag.GameSetting:
                if (_rootView != null) return UnityEditor.Editor.CreateEditor(_rootView.FindDatabase<GameSetting>());
                break;
            case HideFlag.CurrentGameSetting:
                if (_rootView != null) return UnityEditor.Editor.CreateEditor(_rootView.gameController.BaseCurrentGameSetting);
                break;
            case HideFlag.LevelDatabase:
                if (_rootView != null) return UnityEditor.Editor.CreateEditor(_rootView.gameController.BaseLevelDatabaseClass);
                break;
            case HideFlag.LevelCreator:
                if (_levelCreator == null)
                    _levelCreator = ((BaseBaseLevelCreator) FindObjectOfType(typeof(BaseBaseLevelCreator), true));
                if (_levelCreator != null) return UnityEditor.Editor.CreateEditor(_levelCreator);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        return null;
    }

    private void FocusTarget()
    {
        switch (GetCurrent())
        {
            case HideFlag.GameController:
                Selection.activeObject = _rootView.gameController.gameObject;
                EditorGUIUtility.PingObject(_rootView.gameController.gameObject);
                break;
            case HideFlag.RootView:
                Selection.activeObject = _rootView.gameObject;
                EditorGUIUtility.PingObject(_rootView.gameObject);
                break;
            case HideFlag.ScreenRoot:
                Selection.activeObject = _rootView.screenRoot.gameObject;
                EditorGUIUtility.PingObject(_rootView.screenRoot.gameObject);
                break;
            case HideFlag.PoolManager:
                Selection.activeObject = _rootView.poolManager.gameObject;
                EditorGUIUtility.PingObject(_rootView.poolManager.gameObject);
                break;
            case HideFlag.AudioManager:
                Selection.activeObject = _rootView.audioManager.gameObject;
                EditorGUIUtility.PingObject(_rootView.audioManager.gameObject);
                break;
            case HideFlag.ShopDatabase:
                Selection.activeObject = _rootView.FindDatabase<ShopDatabase>();
                EditorGUIUtility.PingObject(_rootView.FindDatabase<ShopDatabase>());
                break;
            case HideFlag.GameSetting:
                Selection.activeObject = _rootView.FindDatabase<GameSetting>();
                EditorGUIUtility.PingObject(_rootView.FindDatabase<GameSetting>());
                break;
            case HideFlag.CurrentGameSetting:
                Selection.activeObject = _rootView.gameController.BaseCurrentGameSetting;
                EditorGUIUtility.PingObject(_rootView.gameController.BaseCurrentGameSetting);
                break;
            case HideFlag.LevelDatabase:
                Selection.activeObject = _rootView.gameController.BaseLevelDatabaseClass;
                EditorGUIUtility.PingObject(_rootView.gameController.BaseLevelDatabaseClass);
                break;
            case HideFlag.LevelCreator:
                Selection.activeObject = _levelCreator;
                EditorGUIUtility.PingObject(_levelCreator);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private HideFlag GetCurrent()
    {
        foreach (var enumValue in EnumValues)
            if (_flagHide.Value == enumValue)
                return (HideFlag) enumValue;

        return (HideFlag) 0;
    }
}