using System;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;
using System.IO;
using System.Threading;
using UnityEngine;

public class NameRefactorWindow : EditorWindow
{
    private string _text = "{0}";
    
    [MenuItem("Assets/DuongPham/NameRefactor")]
    private static void ShowWindow()
    {
        NameRefactorWindow window = CreateInstance<NameRefactorWindow>();
        window.position = new Rect(Screen.width, Screen.height, 250, 150);
        window.CenterOnMainWin();
        window.ShowPopup();
    }
    private static string Path
    {
        get => PlayerPrefs.GetString("TempPath");
        set => PlayerPrefs.SetString("TempPath", value);
    }
    void OnGUI()
    {
        _text = EditorGUILayout.TextField( _text);
        if (GUILayout.Button("Format"))
        {
            foreach (var o in Selection.objects)
            {
                string assetPath = AssetDatabase.GetAssetPath(o);
                AssetDatabase.RenameAsset(assetPath, string.Format(_text, o.name));
                AssetDatabase.Refresh();
            }
        }

        
        if (GUILayout.Button("MoveToTemp"))
        {
            foreach (var o in Selection.objects)
            {
                var des = $"{Path}/{System.IO.Path.GetFileName(AssetDatabase.GetAssetPath(o))}";
                if (!File.Exists(des))
                {
                    AssetDatabase.MoveAsset(AssetDatabase.GetAssetPath(o), des);
                }
            }

            AssetDatabase.Refresh();
        }
        if (GUILayout.Button("focus"))
        {
            UnityEngine.Object obj = AssetDatabase.LoadAssetAtPath(Path, typeof(UnityEngine.Object));
            Selection.activeObject = obj;
            EditorGUIUtility.PingObject(obj);
        }
        if (GUILayout.Button("Close"))
        {
            this.Close();
        }
    }
}