//using UnityEngine;
//using UnityEditor;
//using System.Reflection;
//using System;
//using UnityEngine.UI;
//using static UnityEditor.EditorPrefs;
//
//[CanEditMultipleObjects]
//[CustomEditor(typeof(RectTransform), true)]
//public class RectTransformEditor : Editor
//{
//    private Editor _editorInstance;
//
//    private void OnEnable()
//    {
//        Assembly ass = Assembly.GetAssembly(typeof(UnityEditor.Editor));
//        Type rtEditor = ass.GetType("UnityEditor.RectTransformEditor");
//        _editorInstance = CreateEditor(target, rtEditor);
//    }
//
//    private bool Wave
//    {
//        get => GetBool("ShowRT");
//        set => SetBool("ShowRT", value);
//    }
//
////    private string _json;
////    void CopyValues<T>(T from) {
////        _json = JsonUtility.ToJson(from);
////    }
////    
////    void PasteValue<T>(T to) {
////        JsonUtility.FromJsonOverwrite(_json, to);
////    }
//
//    private static Sprite _jsonImg;
//
//    void CopyValuesImg(Image from)
//    {
//        _jsonImg = from.sprite;
//    }
//
//    void PasteValueImg(Image to)
//    {
//        to.sprite = _jsonImg;
//    }
//
//    private static Vector2[] _rtData;
//
//    void CopyValues(RectTransform from)
//    {
//        _rtData = new[]
//        {
//            from.anchoredPosition,
//            from.anchorMin,
//            from.anchorMax,
//            from.sizeDelta,
//            from.pivot,
//        };
//    }
//
//    void PasteValue(RectTransform to)
//    {
//        to.anchoredPosition = _rtData[0];
//        to.anchorMin = _rtData[1];
//        to.anchorMax = _rtData[2];
//        to.sizeDelta = _rtData[3];
//        to.pivot = _rtData[4];
//    }
//
//    public override void OnInspectorGUI()
//    {
//        _editorInstance.OnInspectorGUI();
//
//        if (!TestingSettings.CopyPaste)
//            return;
//
//
//        GUI.color = Color.green;
//        if (GUILayout.Button("Expand"))
//        {
//            foreach (var temp in targets)
//            {
//                RectTransform myScript = (RectTransform) temp;
//                myScript.anchorMax = Vector2.one;
//                myScript.anchorMin = Vector2.zero;
//                myScript.sizeDelta = Vector2.zero;
//            }
//        }
//
//        GUI.color = Color.cyan;
//        StartHorizon(() =>
//        {
//            if (GUILayout.Button("Copy"))
//            {
//                CopyValues((RectTransform) targets[0]);
//            }
//
//            if (GUILayout.Button("Paste"))
//            {
//                foreach (var o in targets)
//                {
//                    PasteValue((RectTransform) o);
//                    EditorUtility.SetDirty(this);
//                }
//            }
//        });
//        StartHorizon(() =>
//        {
//            if (GUILayout.Button("CopyImage"))
//            {
//                CopyValuesImg(((RectTransform) targets[0]).gameObject.GetComponent<Image>());
//            }
//
//            if (GUILayout.Button("PasteImage"))
//            {
//                foreach (var o in targets)
//                {
//                    PasteValueImg(((RectTransform) o).gameObject.GetComponent<Image>());
//                    EditorUtility.SetDirty(this);
//                }
//            }
//        });
//        StartHorizon(() =>
//        {
//            if (GUILayout.Button("Top Left"))
//            {
//                foreach (var temp in targets)
//                {
//                    RectTransform myScript = (RectTransform) temp;
//                    var size = myScript.rect.size;
//                    myScript.anchorMin = new Vector2(0, 1);
//                    myScript.anchorMax = new Vector2(0, 1);
//                    myScript.sizeDelta = size;
//                }
//            }
//
//            if (GUILayout.Button("Top"))
//            {
//                foreach (var temp in targets)
//                {
//                    RectTransform myScript = (RectTransform) temp;
//                    var size = myScript.rect.size;
//                    myScript.anchorMin = new Vector2(0.5f, 1);
//                    myScript.anchorMax = new Vector2(0.5f, 1);
//                    myScript.sizeDelta = size;
//                }
//            }
//
//            if (GUILayout.Button("Top Right"))
//            {
//                foreach (var temp in targets)
//                {
//                    RectTransform myScript = (RectTransform) temp;
//                    var size = myScript.rect.size;
//                    myScript.anchorMin = new Vector2(1, 1);
//                    myScript.anchorMax = new Vector2(1, 1);
//                    myScript.sizeDelta = size;
//                }
//            }
//        });
//
//        StartHorizon(() =>
//        {
//            if (GUILayout.Button("Left"))
//            {
//                foreach (var temp in targets)
//                {
//                    RectTransform myScript = (RectTransform) temp;
//                    var size = myScript.rect.size;
//                    myScript.anchorMin = new Vector2(0, .5f);
//                    myScript.anchorMax = new Vector2(0, .5f);
//                    myScript.sizeDelta = size;
//                }
//            }
//
//            if (GUILayout.Button("Center"))
//            {
//                foreach (var temp in targets)
//                {
//                    RectTransform myScript = (RectTransform) temp;
//                    var size = myScript.rect.size;
//                    myScript.anchorMin = new Vector2(0.5f, .5f);
//                    myScript.anchorMax = new Vector2(0.5f, .5f);
//                    myScript.sizeDelta = size;
//                }
//            }
//
//            if (GUILayout.Button("Right"))
//            {
//                foreach (var temp in targets)
//                {
//                    RectTransform myScript = (RectTransform) temp;
//                    var size = myScript.rect.size;
//                    myScript.anchorMin = new Vector2(1, .5f);
//                    myScript.anchorMax = new Vector2(1, .5f);
//                    myScript.sizeDelta = size;
//                }
//            }
//        });
//        StartHorizon(() =>
//        {
//            if (GUILayout.Button("Bottom Left"))
//            {
//                foreach (var temp in targets)
//                {
//                    RectTransform myScript = (RectTransform) temp;
//                    var size = myScript.rect.size;
//                    myScript.anchorMin = new Vector2(0, 0);
//                    myScript.anchorMax = new Vector2(0, 0);
//                    myScript.sizeDelta = size;
//                }
//            }
//
//            if (GUILayout.Button("Bottom"))
//            {
//                foreach (var temp in targets)
//                {
//                    RectTransform myScript = (RectTransform) temp;
//                    var size = myScript.rect.size;
//                    myScript.anchorMin = new Vector2(0.5f, 0);
//                    myScript.anchorMax = new Vector2(0.5f, 0);
//                    myScript.sizeDelta = size;
//                }
//            }
//
//            if (GUILayout.Button("Bottom Right"))
//            {
//                foreach (var temp in targets)
//                {
//                    RectTransform myScript = (RectTransform) temp;
//                    var size = myScript.rect.size;
//                    myScript.anchorMin = new Vector2(1, 0);
//                    myScript.anchorMax = new Vector2(1, 0);
//                    myScript.sizeDelta = size;
//                }
//            }
//        });
//    }
//
//    void StartHorizon(Action action)
//    {
//        //*
//        EditorGUILayout.BeginHorizontal();
//        action.Invoke();
//        EditorGUILayout.EndHorizontal();
//        //*
//    }
//
//    private void OnSceneGUI()
//    {
//        MethodInfo onSceneGUI_Method = _editorInstance.GetType()
//            .GetMethod("OnSceneGUI", BindingFlags.NonPublic | BindingFlags.Instance);
//        onSceneGUI_Method.Invoke(_editorInstance, null);
//    }
//}