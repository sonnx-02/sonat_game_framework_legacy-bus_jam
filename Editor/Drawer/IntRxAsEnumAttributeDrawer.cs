using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using UniRx;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(IntRxAsEnumFlagAttribute))]
public class IntRxAsEnumFlagAttributeDrawer : BaseIntRxAsEnumAttributeDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (property.serializedObject.isEditingMultipleObjects)
        {
            EditorGUI.LabelField(position, new GUIContent(label.text + " : Cannot Edit Multiple Object"));
            return;
        }

        var att = ((IntRxAsEnumFlagAttribute) attribute);
        string fieldName;
        bool notifyPropertyChanged;
        {
            var attr = attribute as InspectorDisplayAttribute;
            fieldName = (attr == null) ? "value" : attr.FieldName;
            notifyPropertyChanged = (attr == null) ? true : attr.NotifyPropertyChanged;
        }

        if (notifyPropertyChanged)
        {
            EditorGUI.BeginChangeCheck();
        }

        var targetSerializedProperty = property.FindPropertyRelative(fieldName);
        if (targetSerializedProperty == null)
        {
            EditorGUI.LabelField(position, label,
                new GUIContent() {text = "InspectorDisplay can't find target:" + fieldName});
            if (notifyPropertyChanged)
                EditorGUI.EndChangeCheck();
        }
        else
        {
            // EmitPropertyField(position, targetSerializedProperty, label);

            var propertyValue = property.serializedObject.FindProperty(property.propertyPath + ".value");
            if (propertyValue != null)
            {
                SetColor(att.LabelColor);
                if (!att.EnumList.Found)
                    att.EnumList.Reload(GetType(att.EnumList.TypeName));

                if (att.EnumList.Found && att.EnumList.Values.Length > 0)
                {
                    _index = -1;
                    for (var i = 0; i < att.EnumList.Values.Length; i++)
                        if (propertyValue.intValue == att.EnumList.Values[i])
                        {
                            _index = i;
                            break;
                        }

                    IList<int> values = att.EnumList.Values.ToList();
                    IList<string> names = att.EnumList.Names.Select(x => x.text).ToList();
                    propertyValue.intValue = IndexAsEnumDrawerHelper.DrawBitMaskField(position, propertyValue.intValue, names.ToArray(),
                        values.ToArray(), label);
                }
                else
                {
                    label.text = label.text + $"({att.EnumList.TypeName})";
                    EmitPropertyField(position, targetSerializedProperty, label);
//                    EditorGUI.PropertyField(position, property, label, true);
                }

                ReleaseColor();
            }


            if (notifyPropertyChanged)
            {
                if (EditorGUI.EndChangeCheck())
                {
                    property.serializedObject.ApplyModifiedProperties(); // deserialize to field

                    var paths = property.propertyPath.Split('.'); // X.Y.Z...
                    var attachedComponent = property.serializedObject.targetObject;

                    var targetProp = (paths.Length == 1)
                        ? fieldInfo.GetValue(attachedComponent)
                        : GetValueRecursive(attachedComponent, 0, paths);
                    if (targetProp == null) return;
                    var propInfo = targetProp.GetType().GetProperty(fieldName,
                        BindingFlags.IgnoreCase | BindingFlags.GetProperty | BindingFlags.Instance |
                        BindingFlags.Public |
                        BindingFlags.NonPublic);
                    var modifiedValue = propInfo.GetValue(targetProp, null); // retrieve new value

                    var methodInfo = targetProp.GetType().GetMethod("SetValueAndForceNotify",
                        BindingFlags.IgnoreCase | BindingFlags.InvokeMethod | BindingFlags.Instance |
                        BindingFlags.Public |
                        BindingFlags.NonPublic);
                    if (methodInfo != null)
                    {
                        methodInfo.Invoke(targetProp, new object[] {modifiedValue});
                    }
                }
                else
                {
                    property.serializedObject.ApplyModifiedProperties();
                }
            }
        }
    }
}


[CustomPropertyDrawer(typeof(IntRxAsEnumAttribute))]
public class IntRxAsEnumAttributeDrawer : BaseIntRxAsEnumAttributeDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (property.serializedObject.isEditingMultipleObjects)
        {
            EditorGUI.LabelField(position, new GUIContent(label.text + " : Cannot Edit Multiple Object"));
            return;
        }

        var att = ((IntRxAsEnumAttribute) attribute);
        string fieldName;
        bool notifyPropertyChanged;
        {
            var attr = attribute as InspectorDisplayAttribute;
            fieldName = (attr == null) ? "value" : attr.FieldName;
            notifyPropertyChanged = (attr == null) ? true : attr.NotifyPropertyChanged;
        }

        if (notifyPropertyChanged)
        {
            EditorGUI.BeginChangeCheck();
        }

        var targetSerializedProperty = property.FindPropertyRelative(fieldName);
        if (targetSerializedProperty == null)
        {
            EditorGUI.LabelField(position, label,
                new GUIContent() {text = "InspectorDisplay can't find target:" + fieldName});
            if (notifyPropertyChanged)
                EditorGUI.EndChangeCheck();
        }
        else
        {
            // EmitPropertyField(position, targetSerializedProperty, label);

            var propertyValue = property.serializedObject.FindProperty(property.propertyPath + ".value");
            if (propertyValue != null)
            {
                SetColor(att.LabelColor);
                if (!att.EnumList.Found)
                    att.EnumList.Reload(GetType(att.EnumList.TypeName));

                if (att.EnumList.Found && att.EnumList.Values.Length > 0)
                {
                    _index = -1;
                    for (var i = 0; i < att.EnumList.Values.Length; i++)
                        if (propertyValue.intValue == att.EnumList.Values[i])
                        {
                            _index = i;
                            break;
                        }

                    IList<int> values = att.EnumList.Values.ToList();
                    IList<GUIContent> names = att.EnumList.Names.ToList();
                    if (_index == -1)
                    {
                        _index = values.Count;
                        values = values.ArrayAdd(property.intValue);
                        names = names.ArrayAdd(
                            new GUIContent($"NotFound {att.EnumList.TypeName}_{propertyValue.intValue}"));
                    }

                    propertyValue.intValue = EditorGUI.IntPopup(position, label, propertyValue.intValue,
                        names.ToArray(),
                        values.ToArray());
                }
                else
                {
                    label.text = label.text + $"({att.EnumList.TypeName})";
                    EmitPropertyField(position, targetSerializedProperty, label);
//                    EditorGUI.PropertyField(position, property, label, true);
                }

                ReleaseColor();
            }


            if (notifyPropertyChanged)
            {
                if (EditorGUI.EndChangeCheck())
                {
                    property.serializedObject.ApplyModifiedProperties(); // deserialize to field

                    var paths = property.propertyPath.Split('.'); // X.Y.Z...
                    var attachedComponent = property.serializedObject.targetObject;

                    var targetProp = (paths.Length == 1)
                        ? fieldInfo.GetValue(attachedComponent)
                        : GetValueRecursive(attachedComponent, 0, paths);
                    if (targetProp == null) return;
                    var propInfo = targetProp.GetType().GetProperty(fieldName,
                        BindingFlags.IgnoreCase | BindingFlags.GetProperty | BindingFlags.Instance |
                        BindingFlags.Public |
                        BindingFlags.NonPublic);
                    var modifiedValue = propInfo.GetValue(targetProp, null); // retrieve new value

                    var methodInfo = targetProp.GetType().GetMethod("SetValueAndForceNotify",
                        BindingFlags.IgnoreCase | BindingFlags.InvokeMethod | BindingFlags.Instance |
                        BindingFlags.Public |
                        BindingFlags.NonPublic);
                    if (methodInfo != null)
                    {
                        methodInfo.Invoke(targetProp, new object[] {modifiedValue});
                    }
                }
                else
                {
                    property.serializedObject.ApplyModifiedProperties();
                }
            }
        }
    }
}

public abstract class BaseIntRxAsEnumAttributeDrawer : ReloadEnumDrawer
{
    protected int _index = -1;

   
    protected virtual void EmitPropertyField(Rect position, UnityEditor.SerializedProperty targetSerializedProperty,
        GUIContent label)
    {
        var multiline = GetMultilineAttribute();
        if (multiline == null)
        {
            var range = GetRangeAttribute();
            if (range == null)
            {
                UnityEditor.EditorGUI.PropertyField(position, targetSerializedProperty, label, includeChildren: true);
            }
            else
            {
                if (targetSerializedProperty.propertyType == SerializedPropertyType.Float)
                {
                    EditorGUI.Slider(position, targetSerializedProperty, range.Min, range.Max, label);
                }
                else if (targetSerializedProperty.propertyType == SerializedPropertyType.Integer)
                {
                    EditorGUI.IntSlider(position, targetSerializedProperty, (int) range.Min, (int) range.Max, label);
                }
                else
                {
                    EditorGUI.LabelField(position, label.text, "Use Range with float or int.");
                }
            }
        }
        else
        {
            var property = targetSerializedProperty;

            label = EditorGUI.BeginProperty(position, label, property);
            var method = typeof(EditorGUI).GetMethod("MultiFieldPrefixLabel",
                BindingFlags.Static | BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.NonPublic);
            position = (Rect) method.Invoke(null, new object[] {position, 0, label, 1});

            EditorGUI.BeginChangeCheck();
            int indentLevel = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;
            var stringValue = EditorGUI.TextArea(position, property.stringValue);
            EditorGUI.indentLevel = indentLevel;
            if (EditorGUI.EndChangeCheck())
            {
                property.stringValue = stringValue;
            }

            EditorGUI.EndProperty();
        }
    }

    protected object GetValueRecursive(object obj, int index, string[] paths)
    {
        var path = paths[index];

        FieldInfo fldInfo = null;
        var type = obj.GetType();
        while (fldInfo == null)
        {
            // attempt to get information about the field
            fldInfo = type.GetField(path,
                BindingFlags.IgnoreCase | BindingFlags.GetField | BindingFlags.Instance | BindingFlags.Public |
                BindingFlags.NonPublic);

            if (fldInfo != null ||
                type.BaseType == null ||
                type.BaseType.IsSubclassOf(typeof(ReactiveProperty<>))) break;

            // if the field information is missing, it may be in the base class
            type = type.BaseType;
        }

        // If array, path = Array.data[index]
        if (fldInfo == null && path == "Array")
        {
            try
            {
                path = paths[++index];
                var m = Regex.Match(path, @"(.+)\[([0-9]+)*\]");
                var arrayIndex = int.Parse(m.Groups[2].Value);
                var arrayValue = (obj as System.Collections.IList)[arrayIndex];
                if (index < paths.Length - 1)
                {
                    return GetValueRecursive(arrayValue, ++index, paths);
                }
                else
                {
                    return arrayValue;
                }
            }
            catch
            {
                Debug.Log("InspectorDisplayDrawerBase Exception, objType:" + obj.GetType().Name + " path:" +
                          string.Join(", ", paths));
                throw;
            }
        }
        else if (fldInfo == null)
        {
            throw new Exception("Can't decode path, please report to UniRx's GitHub issues:" +
                                string.Join(", ", paths));
        }

        var v = fldInfo.GetValue(obj);
        if (index < paths.Length - 1)
        {
            return GetValueRecursive(v, ++index, paths);
        }

        return v;
    }

    protected MultilineReactivePropertyAttribute GetMultilineAttribute()
    {
        var fi = this.fieldInfo;
        if (fi == null) return null;
        return fi.GetCustomAttributes(false).OfType<MultilineReactivePropertyAttribute>().FirstOrDefault();
    }

    protected RangeReactivePropertyAttribute GetRangeAttribute()
    {
        var fi = this.fieldInfo;
        if (fi == null) return null;
        return fi.GetCustomAttributes(false).OfType<RangeReactivePropertyAttribute>().FirstOrDefault();
    }
}

