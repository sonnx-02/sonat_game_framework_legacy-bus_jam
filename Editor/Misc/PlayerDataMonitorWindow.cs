using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static UnityEditor.EditorPrefs;

public class PlayerDataMonitorWindow : EditorWindow
{
    [MenuItem("Tools/DuongPham/" + nameof(PlayerDataMonitorWindow))]
    public static void ShowWindow()
    {
        GetWindow(typeof(PlayerDataMonitorWindow));
    }


    private readonly int[] _session = new int[20];
    private readonly int[] _shared = new int[20];
    public void OnGUI()
    {
        var style = new GUIStyle(GUI.skin.button);
        style.normal.textColor = Color.green;
        
        EditorGUILayout.LabelField("SessionData",style);
        EnumList enumList = new EnumList(BuiltInEnumType.CustomSessionDataProperty.ToString());
        if (enumList.Found && enumList.Values.Length > 0)
        {
            IList<int> values = enumList.Values.ToList();
            IList<GUIContent> names = enumList.Names.ToList();
            for (var i = 0; i < names.Count; i++)
            {
             
                if (BaseFrameworkView.SessionData != null)
                {
                    var i1 = i;
                    StartHorizon(() =>
                    {
                        EditorGUILayout.IntField(names[i1], BaseFrameworkView.SessionData.customPropertyList[values[i1]].Value);
                        _session[i1] = EditorGUILayout.IntField(_session[i1]);
                        if (GUILayout.Button("Set"))
                        {
                            BaseFrameworkView.SessionData.customPropertyList[values[i1]].Value = _session[i1];
                            Repaint();
                        }
                    });
                }
            }
        }
        
        EditorGUILayout.LabelField("SharedData",style);
         enumList = new EnumList(BuiltInEnumType.FrameworkSharedData.ToString());
        if (enumList.Found && enumList.Values.Length > 0)
        {
            IList<int> values = enumList.Values.ToList();
            IList<GUIContent> names = enumList.Names.ToList();
            for (var i = 0; i < names.Count; i++)
            {
                if (BaseFrameworkView.SharedData != null)
                {
                    var i1 = i;
                    StartHorizon(() =>
                    {
                        EditorGUILayout.IntField(names[i1], BaseFrameworkView.SharedData[values[i1]]);
                        if (i1 == 1)
                        {
                            _shared[i1] = (int)((Quantity)EditorGUILayout.EnumPopup((Quantity)_shared[i1]));
                        }
                        else
                        {
                            _shared[i1] = EditorGUILayout.IntField(_shared[i1]);
                        }
                        if (GUILayout.Button("Set"))
                        {
                            BaseFrameworkView.SharedData[values[i1]] = _shared[i1];
                            Repaint();
                        }
                    });
                }
            }
        }
    }


    void StartHorizon(Action action)
    {
        //*
        EditorGUILayout.BeginHorizontal();
        action.Invoke();
        EditorGUILayout.EndHorizontal();
        //*
    }


}