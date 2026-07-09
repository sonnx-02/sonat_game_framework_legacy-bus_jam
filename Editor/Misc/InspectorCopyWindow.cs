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

public class InspectorCopyWindow : EditorWindow
{
    [MenuItem("Tools/DuongPham/" + nameof(InspectorCopyWindow))]
    public static void ShowWindow()
    {
        GetWindow(typeof(InspectorCopyWindow));
    }


    public  BaseBaseActionScript[] actions = new BaseBaseActionScript[0];
    public  BaseRxAnimator[] anims = new BaseRxAnimator[0];
    public  BaseFrameworkView[] views = new BaseFrameworkView[0];
    public void OnGUI()
    {
        var style = new GUIStyle(GUI.skin.button);
        style.normal.textColor = Color.green;
        if (GUILayout.Button("Load",style))
        {
            if (Selection.gameObjects.Length > 0)
            {
                actions = Selection.gameObjects[0].GetComponents<BaseBaseActionScript>();
                anims = Selection.gameObjects[0].GetComponents<BaseRxAnimator>();
                views = Selection.gameObjects[0].GetComponents<BaseFrameworkView>();
            }
            Repaint();
        }
        
        SerializedObject serialisedObject = new SerializedObject(this);
        SerializedProperty myIngredientProp = serialisedObject.FindProperty("actions");
        EditorGUILayout.PropertyField(myIngredientProp, true);
        SerializedProperty myIngredientProp2 = serialisedObject.FindProperty("anims");
        EditorGUILayout.PropertyField(myIngredientProp2, true);
        SerializedProperty myIngredientProp3 = serialisedObject.FindProperty("views");
        EditorGUILayout.PropertyField(myIngredientProp3, true);
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