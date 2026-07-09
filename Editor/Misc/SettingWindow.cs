using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static UnityEditor.EditorPrefs;

public class SettingWindow : EditorWindow
{
    [MenuItem("Tools/DuongPham/"+nameof(SettingWindow))]
    public static void ShowWindow()
    {
        GetWindow(typeof(SettingWindow));
    } 


    Vector2 scrollPosition;
    private string SceneFolder
    {
        get => GetString("SceneFolder", "Assets/Scenes");
        set => SetString("SceneFolder", value);
    }
 


    public void OnGUI()
    {
        var oldColor = GUI.backgroundColor;
        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

        TestingSettings.UiLogEnable = EditorGUILayout.Toggle(nameof(TestingSettings.UiLogEnable), TestingSettings.UiLogEnable);
        TestingSettings.TurnOffBanner = EditorGUILayout.Toggle(nameof(TestingSettings.TurnOffBanner), TestingSettings.TurnOffBanner);
        TestingSettings.TurnOffOnFocusAds = EditorGUILayout.Toggle(nameof(TestingSettings.TurnOffOnFocusAds), TestingSettings.TurnOffOnFocusAds);
        TestingSettings.SkipRewarded = EditorGUILayout.Toggle(nameof(TestingSettings.SkipRewarded), TestingSettings.SkipRewarded);
        TestingSettings.CopyPaste = EditorGUILayout.Toggle(nameof(TestingSettings.CopyPaste), TestingSettings.CopyPaste);
        TestingSettings.InternetConnected = EditorGUILayout.Toggle(nameof(TestingSettings.InternetConnected), TestingSettings.InternetConnected);
        TestingSettings.EpochDayAdd = EditorGUILayout.IntField(nameof(TestingSettings.EpochDayAdd), TestingSettings.EpochDayAdd);
        TestingSettings.DebugLevel = EditorGUILayout.IntField(nameof(TestingSettings.DebugLevel), TestingSettings.DebugLevel);
        TestingSettings.DrawPointMap  = EditorGUILayout.Toggle(nameof(TestingSettings.DrawPointMap),TestingSettings.DrawPointMap);
        TestingSettings.DebugBreak  = EditorGUILayout.Toggle(nameof(TestingSettings.DebugBreak),TestingSettings.DebugBreak);
        TestingSettings.DebugBreak1  = EditorGUILayout.Toggle(nameof(TestingSettings.DebugBreak1),TestingSettings.DebugBreak1);
        EditorGUILayout.EndScrollView();
        TestingSettings.Param1 = EditorGUILayout.IntField(nameof(TestingSettings.Param1), TestingSettings.Param1);

    }

    void StartHorizon(Action action)
    {
        //*
        EditorGUILayout.BeginHorizontal();
        action.Invoke();
        EditorGUILayout.EndHorizontal();
        //*
    }

    private static string FilePath => string.Format("{0}/{1}", Application.persistentDataPath, "PlayerData.json");
}