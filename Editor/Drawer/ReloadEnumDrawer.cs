using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
public class ReloadEnumDrawer : DuongButtonDrawer
{
    protected Color original;

    private static readonly Color[] _colors = new Color[]
    {
        new Color(1, 0, 0),
        new Color(0, 1, 0),
        new Color(0, 0, 1),
    };

    protected void ReleaseColor()
    {
        SetColor(original);
    }

    protected void SetColor(int color)
    {
        original = EditorStyles.label.normal.textColor;
        if (color >= 0)
        {
            SetStyleColor(EditorStyles.label, color);
            SetStyleColor(EditorStyles.foldout, color);
            SetStyleColor(EditorStyles.foldoutPreDrop, color);
        }
    }

    protected void SetStyleColor(GUIStyle style, int color)
    {
        style.normal.textColor =
            style.onNormal.textColor =
                style.active.textColor =
                    style.onActive.textColor =
                        style.focused.textColor =
                            style.onFocused.textColor =
                                style.hover.textColor =
                                    style.onHover.textColor = _colors[color];
    }

    protected void SetColor(Color color)
    {
        original = EditorStyles.label.normal.textColor;
        SetStyleColor(EditorStyles.label, color);
        SetStyleColor(EditorStyles.foldout, color);
        SetStyleColor(EditorStyles.foldoutPreDrop, color);
    }

    protected void SetStyleColor(GUIStyle style, Color color)
    {
        style.normal.textColor =
            style.onNormal.textColor =
                style.active.textColor =
                    style.onActive.textColor =
                        style.focused.textColor =
                            style.onFocused.textColor =
                                style.hover.textColor =
                                    style.onHover.textColor = color;
    }


    protected static Type GetType(string enumTypeName)
    {
        return DuongEditorHelper.GetType(enumTypeName);
    }

    protected static SerializedProperty FindPropertyRelative(SerializedProperty property, string toGet)
    {
        if (property.depth == 0) return property.serializedObject.FindProperty(toGet);
        var path = property.propertyPath.Replace(".Array.data[", "[");
        var elements = path.Split('.');
        SerializedProperty parent = null;
        for (int i = 0; i < elements.Length - 1; i++)
        {
            var element = elements[i];
            int index = -1;
            if (element.Contains("["))
            {
                index = Convert.ToInt32(element.Substring(element.IndexOf("[", StringComparison.Ordinal))
                    .Replace("[", "").Replace("]", ""));
                element = element.Substring(0, element.IndexOf("[", StringComparison.Ordinal));
            }

            parent = i == 0
                ? property.serializedObject.FindProperty(element)
                : parent.FindPropertyRelative(element);

            if (index >= 0) parent = parent.GetArrayElementAtIndex(index);
        }

        if (parent == null)
            return property.serializedObject.FindProperty(toGet);
        return parent.FindPropertyRelative(toGet);
    }

    protected static EnumList FindEnumList(SerializedProperty property, string attPropertyToCheck,
        EnumFindBy attEnumFindBy)
    {
        if (attEnumFindBy == EnumFindBy.BuiltInEnumType)
        {
            var enumType =
                ((BuiltInEnumType) FindPropertyRelative(property, attPropertyToCheck).intValue).ToString();

            var el = new EnumList(enumType);
            if (!el.Found)
                el.Reload(GetType(el.TypeName));

            return el;
        }

        if (attEnumFindBy == EnumFindBy.Name)
        {
            var el = new EnumList(FindPropertyRelative(property, attPropertyToCheck).stringValue);
            if (!el.Found)
                el.Reload(GetType(el.TypeName));

            return el;
        }

        return null;
    }

    protected int _index = -1;

    protected virtual List<SerializedProperty> GetListProperty(SerializedProperty property,out int t)
    {
        var propertyValue = property.serializedObject.FindProperty(property.propertyPath + ".value");
        t = 1;
        if (propertyValue != null)
        {
            List<SerializedProperty> listProperties = new List<SerializedProperty>() {propertyValue};
            for (int i = 2; i < 10; i++)
            {
                var propertyValue2 = property.serializedObject.FindProperty(property.propertyPath + ".value" + i);
                if (propertyValue2 != null)
                {
                    listProperties.Add(propertyValue2);
                    t++;
                }
                else
                    break;
            }

            return listProperties;
        }

        t = 0;
        return null;
    }

    protected void DrawForIntInt(SerializedProperty property, GUIContent label, Rect position, EnumList[] enumLists,
        int labelColor, int indexForCondition = -1)
    {
        List<SerializedProperty> listProperties = GetListProperty(property, out int t);
        if (t > 0)
        {
            SetColor(labelColor);
            for (var i = 0; i < enumLists.Length; i++)
                if (!enumLists[i].Found)
                    enumLists[i].Reload(GetType(enumLists[i].TypeName));

            for (var i = 0; i < enumLists.Length; i++)
                if (!enumLists[i].Found)
                    enumLists[i] = new EnumList("Dummy" + enumLists[i].TypeName, 10);

            bool isArrayElement = property.IsArrayElement();
            if (!isArrayElement)
            {
                position.x += EditorGUIUtility.labelWidth;
                position.width -= EditorGUIUtility.labelWidth;
            }

            var splits = position.GetSplitRect2(listProperties.Count, 1).ToArray();
            if (!isArrayElement)
            {
                splits[0].x -= EditorGUIUtility.labelWidth;
                splits[0].width += EditorGUIUtility.labelWidth;
            }

            var cache = EditorGUIUtility.labelWidth;
            if (isArrayElement)
                EditorGUIUtility.labelWidth = 20;
            for (int k = 0; k < t; k++)
            {
                var currentProperty = listProperties[k];
                _index = -1;
                if (k < enumLists.Length)
                    for (var i = 0; i < enumLists[k].Values.Length; i++)
                        if (currentProperty.intValue == enumLists[k].Values[i])
                        {
                            _index = i;
                            break;
                        }

                if (k < enumLists.Length && indexForCondition == -1) // for [IndexAsEnum] multiple enum list
                {
                    IList<int> values = enumLists[k].Values.ToList();
                    IList<GUIContent> names = enumLists[k].Names.ToList();
                    if (_index == -1)
                    {
                        _index = values.Count;
                        values = values.ArrayAdd(property.intValue);
                        names = names.ArrayAdd(
                            new GUIContent($"NotFound {enumLists[0].TypeName}_{currentProperty.intValue}"));
                    }

                    currentProperty.intValue = EditorGUI.IntPopup(splits[k], k == 0 ? (isArrayElement 
                            ? new GUIContent(Int32.Parse(Regex.Match(label.text, @"\d+").Value).ToString()) : label) : new GUIContent(),
                        currentProperty.intValue, names.ToArray(), values.ToArray());
                }
                else if (k == 0 && indexForCondition >= 0) // for [IndexAsEnumMultiple] condition for first
                {
                    IList<int> values = enumLists[indexForCondition].Values.ToList();
                    IList<GUIContent> names = enumLists[indexForCondition].Names.ToList();
                    if (_index == -1)
                    {
                        _index = values.Count;
                        values = values.ArrayAdd(property.intValue);
                        names = names.ArrayAdd(
                            new GUIContent($"NotFound {enumLists[0].TypeName}_{currentProperty.intValue}"));
                    }

                    currentProperty.intValue = EditorGUI.IntPopup(splits[k], k == 0 ? (isArrayElement ? new GUIContent(Int32.Parse(Regex.Match(label.text, @"\d+").Value).ToString()) : label) : new GUIContent(),
                        currentProperty.intValue, names.ToArray(), values.ToArray());
                }
                else
                {
                    if (currentProperty.propertyType == SerializedPropertyType.Integer)
                        currentProperty.intValue = EditorGUI.IntField(splits[k], "", currentProperty.intValue);
                    else if (currentProperty.propertyType == SerializedPropertyType.Float)
                        currentProperty.floatValue = EditorGUI.FloatField(splits[k], currentProperty.floatValue);
                    else if (currentProperty.propertyType == SerializedPropertyType.String)
                        currentProperty.stringValue = EditorGUI.TextField(splits[k], currentProperty.stringValue);
                    else if (currentProperty.propertyType == SerializedPropertyType.Boolean)
                        currentProperty.boolValue = EditorGUI.Toggle(splits[k], currentProperty.boolValue);
                    else
                        EditorGUI.PropertyField(splits[k], currentProperty, new GUIContent(), true);
                }
            }

            EditorGUIUtility.labelWidth = cache;
            ReleaseColor();
        }
        else
            EditorGUI.LabelField(position, label.text, "Use PopUp for Integer,Or IntIntValue ");
    }

    protected void DrawForInt(SerializedProperty property, GUIContent label, Rect position, EnumList enumList,
        int labelColor, bool disable)
    {
        SetColor(labelColor);
        if (!enumList.Found)
            enumList.Reload(GetType(enumList.TypeName));

        if (enumList.Found && enumList.Values.Length > 0)
        {
            GUI.enabled = !disable;
            IndexAsEnumDrawerHelper.DrawPropertyAsEnum(enumList, property, position, label);
            GUI.enabled = true;
        }
        else
        {
            label.text = label.text + $"({enumList.TypeName})";
            EditorGUI.PropertyField(position, property, label, true);
        }

        ReleaseColor();
    }
    
    protected void DrawForInt(SerializedProperty property,  Rect position, EnumList enumList,
        int labelColor, bool disable)
    {
        SetColor(labelColor);
        if (!enumList.Found)
            enumList.Reload(GetType(enumList.TypeName));

        if (enumList.Found && enumList.Values.Length > 0)
        {
            GUI.enabled = !disable;
            IndexAsEnumDrawerHelper.DrawPropertyAsEnum(enumList, property, position);
            GUI.enabled = true;
        }
        else
        {
            EditorGUI.PropertyField(position, property, true);
        }

        ReleaseColor();
    }


    protected string AsStringValue(SerializedProperty property)
    {
        switch (property.propertyType)
        {
            case SerializedPropertyType.String:
                return property.stringValue;
            case SerializedPropertyType.Character:
            case SerializedPropertyType.Integer:
                if (property.type == "char") return System.Convert.ToChar(property.intValue).ToString();
                return property.intValue.ToString();
            case SerializedPropertyType.ObjectReference:
                return property.objectReferenceValue != null ? property.objectReferenceValue.ToString() : "null";
            case SerializedPropertyType.Boolean:
                return property.boolValue.ToString();
            case SerializedPropertyType.Enum:
                return property.enumNames[property.enumValueIndex];
            default:
                return string.Empty;
        }
    }
}

#endif