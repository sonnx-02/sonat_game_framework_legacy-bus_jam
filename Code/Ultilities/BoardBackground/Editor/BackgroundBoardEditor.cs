#if UNITY_EDITOR

using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;
using DG.Tweening;
using UnityEngine.UI;

[CustomEditor(typeof(BackgroundBoard2D))]
public class BackgroundBoard2DEditor : UnityEditor.Editor
{
    int Size
    {
        get
        {
            return EditorPrefs.GetInt("Size",5);
        }
        set
        {
            EditorPrefs.SetInt("Size", value);
        }
    }

    public override void OnInspectorGUI()
    {
        BackgroundBoard2D myScript = (BackgroundBoard2D)target;

        if (GUILayout.Button("Test2"))
        {
            myScript.Test2();
        }

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Test"))
        {
            myScript.InitBoard(new bool[Size,Size]);
        }
        Size = EditorGUILayout.IntField(Size, new GUILayoutOption[] { GUILayout.Width(50f) });
        GUILayout.EndHorizontal();

        if (GUILayout.Button("Clear"))
        {
            myScript.Clear();
        }

        if (GUILayout.Button("Delete"))
        {
            myScript.Delete();
        }


        if (GUILayout.Button("Load"))
        {
            myScript.Load();
        }

        GUILayout.Space(10f);
        DrawDefaultInspector();
    }
}

#endif