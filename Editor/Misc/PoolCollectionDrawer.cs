using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


[CustomPropertyDrawer(typeof(RentButton))]
public class RentButtonDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        Rect[] rects = new Rect[5];

        for (var i = 0; i < rects.Length; i++)
        {
            rects[i] = position;
            rects[i].width = position.width / rects.Length;
            rects[i].x += i * rects[i].width;
            
            if (GUI.Button(rects[i], $"{i}"))
            {
                Debug.Log(property.serializedObject.GetType());
                Debug.Log(property.serializedObject.targetObject);
            }
        }
    }
}
