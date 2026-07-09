using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class LevelMakerEditorBase : EditorWindow
{
    private int _saveTo;
    public virtual void DrawForLevelMaker<T>(BaseLevelMaker<T> levelMaker) where T : BaseLevel
    {
        if(levelMaker.LevelDatabase == null)
            return;
        
        StartHorizon(() =>
        {
            
            if (GUILayout.Button("Load From Json"))
            {
                levelMaker.LoadFromFile();
            }
            if (GUILayout.Button("Force Save Json"))
            {
                levelMaker.WriteJson();
            }
        });
        EditorGUILayout.Space(5);
        StartHorizon(() =>
        {
            if (GUILayout.Button("<<"))
            {
                levelMaker.PreLevel();
            }
            levelMaker.currentLevel.Value = EditorGUILayout.IntField( levelMaker.currentLevel.Value);
            GUI.enabled = false;
            EditorGUILayout.TextField("/"+levelMaker.TotalLevel);
            GUI.enabled = true;
            if (GUILayout.Button(">>"))
            {
                levelMaker.NextLevel();
            }
            if (GUILayout.Button("Save"))
            {
                levelMaker.Save();
            }
            if (GUILayout.Button("Load"))
            {
                levelMaker.LoadLevel();
            }
            if (GUILayout.Button("SaveN"))
            {
                levelMaker.SaveAsNew();
            }
            if (GUILayout.Button("Delete"))
            {
                levelMaker.Delete();
            }
        });

        StartHorizon(() =>
        {
            if (GUILayout.Button("SaveTo"))
            {
                levelMaker.Save(_saveTo);
            }

            _saveTo = EditorGUILayout.IntField(_saveTo);
        });
        StartHorizon(() =>
        {
            
            if (GUILayout.Button("Create"))
            {
                levelMaker.Create();
            }
            if (GUILayout.Button("Init"))
            {
                levelMaker.Init();
            }
            if (GUILayout.Button("Load FromScene"))
            {
                levelMaker.LoadFromScene();
            }
            if (GUILayout.Button("Play"))
            {
                levelMaker.PlayTemp();
            }
        });
        EditorGUILayout.Space(5);
        StartHorizon(() =>
        {
            
            if (GUILayout.Button("Random"))
            {
                levelMaker.Random();
            }
            if (GUILayout.Button("Random2"))
            {
                levelMaker.Random2();
            }
        });
        EditorGUILayout.Space(20);

        void StartHorizon(Action action)
        {
            //*
            EditorGUILayout.BeginHorizontal();
            action.Invoke();
            EditorGUILayout.EndHorizontal();
            //*
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

    public virtual void OnGUI()
    {
        
    }
}