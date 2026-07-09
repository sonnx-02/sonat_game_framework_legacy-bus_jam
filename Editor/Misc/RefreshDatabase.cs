using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class RefreshDatabase : EditorWindow
{
    [MenuItem("Tools/Refresh Database")]
    public static void ShowWindow()
    {
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    } 
}
