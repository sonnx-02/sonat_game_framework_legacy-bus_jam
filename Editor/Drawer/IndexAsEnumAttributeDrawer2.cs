using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
public abstract class ValueValueDefaultDrawer : DuongClassDrawer
{
    protected virtual void DrawDefault(Rect position, SerializedProperty property, GUIContent label)
    {
        var list = GetListProperty(property, out int t);
        if(t == 0)
            EditorGUI.PropertyField(position, property, label, true);
        else
            base.OnGUI(position,property,label);
    }
    
    public override IList<DivideData> DivideData(SerializedProperty property)
    {
        var list = GetListProperty(property, out int t);
        var data = new List<DivideData>();
        for (int i = 0; i < t; i++)
            data.Add(new DivideData(20, "value"+(i > 0 ? (i+1).ToString() : "")));

        return data;
    }
}

public class IndexAsEnumConditionBaseAttributeDrawer : ValueValueDefaultDrawer
{
    private EnumList enumList;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (property.serializedObject.isEditingMultipleObjects)
        {
            EditorGUI.LabelField(position, new GUIContent(label.text + " : Cannot Edit Multiple Object"));
            return;
        }

        //  if (property.propertyType == SerializedPropertyType.Integer)
        {
            var att = ((IndexAsEnumConditionBaseAttribute) attribute);
            SetColor(att.LabelColor);
            int index = -1;
            var conditionProperty = FindPropertyRelative(property, att.PropertyToCheck);
            for (int i = 0; i < att.CompareValues.Length; i++)
            {
                var toCompare = att.CompareValues[i].ToString().ToUpper();
                string conditionPropertyStringValue = AsStringValue(conditionProperty).ToUpper();
                bool objectMatch = toCompare == conditionPropertyStringValue;
                if (objectMatch)
                {
                    index = i;
                    break;
                }
            }

            EnumList enumList2 = null;
            if (index >= 0)
            {
                enumList2 = att.EnumList[index];
                if (!enumList2.Found)
                    enumList2.Reload(GetType(enumList2.TypeName));
            }

            if (index >= 0)
            {
                if (enumList2 != null && enumList2.Found && enumList2.Values.Length > 0)
                {
//                    _index = -1;
//                    for (var i = 0; i < enumList2.Values.Length; i++)
//                        if (property.intValue == enumList2.Values[i])
//                        {
//                            _index = i;
//                            break;
//                        }

                    if (property.propertyType == SerializedPropertyType.Integer)
                        DrawForInt(property, label, position, att.EnumList[index], att.LabelColor, false);
                    // for IntInt
                    else
                        DrawForIntInt(property, label, position, att.EnumList, att.LabelColor, index);

//                    property.intValue = EditorGUI.IntPopup(position, label, property.intValue,enumList2.Names.Select(x => new GUIContent(x)).ToArray(), enumList2.Values.ToArray());
                }
                else
                {
                    var old = GUI.color;
                    GUI.color = Color.red;
                    label.text = $"!{att.EnumList[index].TypeName}";
                    EditorGUI.LabelField(position, label);
//                    EditorGUI.PropertyField(position, property, label, true);
                    GUI.color = old;
                }
            }
            else
            {
                DrawDefault(position, property, label);
            }

            ReleaseColor();
        }
//        else
//            EditorGUI.LabelField(position, label.text, "Use PopUp for Integer.");
    }
    
 
}

#endif