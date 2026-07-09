using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public  abstract class BaseManagerWindow : EditorWindow
{
    protected Vector2 ScrollPosition;
    protected GUIStyle Style;
    protected GUIStyle Style2;
    protected GUIStyle StyleRed;
    protected int[] EnumValues;

    protected virtual void StartHorizon(Action action)
    {
        //*
        EditorGUILayout.BeginHorizontal();
        if (action != null)
            action.Invoke();
        EditorGUILayout.EndHorizontal();
        //*
    }

    protected ProjectSettings _projectSettings;
    protected virtual void Prepare()
    {
        if (Style == null)
        {
            Style = new GUIStyle(GUI.skin.button);
            Style.normal.textColor = new Color(.8f, 0.8f, 0.8f);
        }

        if (Style2 == null)
        {
            Style2 = new GUIStyle(GUI.skin.button);
            Style2.normal.textColor = Color.green;
        }
        if (StyleRed == null)
        {
            StyleRed = new GUIStyle(GUI.skin.button);
            StyleRed.normal.textColor = new Color(1f, 0.3f, 0.3f);
        }
        
        if (_projectSettings == null || _projectSettings.IsNotCurrent)
            _projectSettings = ProjectSettings.LoadProjectSettings();
    }

    protected virtual void Clear()
    {
        EnumValues = null;
        Style = null;
        Style2 = null;
        StyleRed = null;
    }

    protected bool Hide;

    protected abstract void OnGUIHidable();
    
    
    
    public virtual void OnGUI()
    {
        Prepare();
        var oldColor = GUI.color;
        GUI.color = Hide ? Color.green : Color.red;
        if (GUILayout.Button(Hide ? "Show" :"Hide"))
        {
            Hide = !Hide;
        }
        EditorGUILayout.Space(10);
        GUI.color = oldColor;

        if (!Hide)
        {
            StartHorizon(() =>
            {
                GUI.enabled = !(this is FocusManagerWindow);
                if (GUILayout.Button("Focus Manager", GUI.enabled ? Style : Style2))
                {
                    FocusManagerWindow.ShowWindow();
                }


                GUI.enabled = !(this is GameTesterWindow);
                if (GUILayout.Button("Game Tester", GUI.enabled ? Style : Style2))
                {
                    GameTesterWindow.ShowWindow();
                }

                GUI.enabled = !(this is SdkToolWindow);
                if (GUILayout.Button("Sdk Tool", GUI.enabled ? Style : Style2))
                {
                    SdkToolWindow.ShowWindow();
                }

                GUI.enabled = !(this is LevelCreatorWindow);
                if (GUILayout.Button("Level Creator", GUI.enabled ? Style : Style2))
                {
                    LevelCreatorWindow.ShowWindow();
                }

                GUI.enabled = !(this is EditorToolWindow);
                if (GUILayout.Button("Editor Tool", GUI.enabled ? Style : Style2))
                {
                    EditorToolWindow.ShowWindow();
                }

                GUI.enabled = true;
            });

        
            StartHorizon(() =>
            {
                if (GUILayout.Button("Clear          "))
                {
                    Clear();
                }

                GUI.color = Color.red;
                if (GUILayout.Button("Delete All Data"))
                {
                    DuongExtensions.DeleteData();
                }

                GUI.color = oldColor;
            });

            EditorGUILayout.Space(5);

            OnGUIHidable();
        }
    }
    
    public class EnumIntPlayerPref
    {
        private IList<int> values;
        private IList<GUIContent> names;

        public int Value
        {
            get => EditorPrefs.GetInt(type + "editor", 0);
            set => EditorPrefs.SetInt(type + "editor", value);
        }

        public IList<int> EnumValues => values;
        public IList<GUIContent> EnumNames => names;
        private BuiltInEnumType type;

        public EnumIntPlayerPref(BuiltInEnumType builtInEnumType)
        {
            type = builtInEnumType;
            EnumList enumList = new EnumList(builtInEnumType.ToString());

            int _index = -1;
            if (enumList.Found && enumList.Values.Length > 0)
            {
                _index = -1;
                for (var i = 0; i < enumList.Values.Length; i++)
                    if (Value == enumList.Values[i])
                    {
                        _index = i;
                        break;
                    }

                values = enumList.Values;
                names = enumList.Names;
                if (_index == -1)
                {
                    _index = values.Count;
                    values = values.ArrayAdd(Value);
                    names = names.ArrayAdd(new GUIContent($"NotFound {enumList.TypeName}_{Value}"));
                }
            }
            else
            {
                values = new[] {Value};
                names = new[] {new GUIContent($"NotFound {enumList.TypeName}_{Value}")};
            }
        }
    }
}