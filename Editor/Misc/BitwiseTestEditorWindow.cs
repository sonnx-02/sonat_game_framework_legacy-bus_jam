using System;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;
using System.IO;
using TMPro;

public class BitwiseTestEditorWindow : EditorWindow
{
    [MenuItem("Tools/DuongPham/"+nameof(BitwiseTestEditorWindow))]
    private static void ShowEditor()
    {
        GetWindow(typeof(BitwiseTestEditorWindow), false, nameof(BitwiseTestEditorWindow));
    }


    private int a;
    private int b;
    private int t;
    private bool[] c = new bool[10];
    void OnGUI()
    {
        GUI.color = Color.cyan;

        EditorGUIUtility.labelWidth = 50;
        StartHorizon(() =>
        {
            a = EditorGUILayout.IntField("a", a);
            b = EditorGUILayout.IntField("b", b);
//            if (GUILayout.Button("Paste"))
//            {
//                
//            }
        });
        t = EditorGUILayout.IntField("t", t);

        EditorGUILayout.IntField("a&b", a& b);
        EditorGUILayout.IntField("a|b", a| b);
        EditorGUILayout.IntField("a&~b", a& ~b);
       
        EditorGUIUtility.labelWidth = 10;
        StartHorizon(() =>
        {
            for (int i = 0; i < c.Length; i++)
            {
                c[i] = EditorGUILayout.Toggle( i.ToString(),(c[i]));
            }

        });
        
        EditorGUIUtility.labelWidth = 150;
        int union = 0;
        int intersect = 0;
        for (int i = 0; i < c.Length; i++)
        {
            int value =  (c[i] ? 1 : 0) << i;
            union |= value;
            if (intersect == 0 && value != 0)
                intersect = value;
            else if (value != 0)
                intersect &= value;
        }
        EditorGUILayout.IntField("union", union);
        EditorGUILayout.IntField("intersect", intersect);
        
        StartHorizon(() =>
        {
          
            if (GUILayout.Button("Repeat Distance a-b on t"))
            {
                Debug.LogError(a.RepeatDistance(b,t));
            }
        });
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