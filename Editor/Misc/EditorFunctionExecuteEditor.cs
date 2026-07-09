using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;
using UnityEngine.UI;

[CustomEditor(typeof(EditorFunctionExecute))]
public class EditorFunctionExecuteEditor : UnityEditor.Editor
{
    public override void OnInspectorGUI()
    {
        EditorFunctionExecute myScript = (EditorFunctionExecute)target;
        DrawDefaultInspector();

        if (GUILayout.Button("Execute"))
        {
            myScript.Execute();
        }

        if (GUILayout.Button("Test"))
        {
         
        }
    }
}
