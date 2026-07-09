using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR



//^^^^^^^^^^^^^^^^^

[CustomPropertyDrawer(typeof(IndexAsEnumAttribute))]
public class IndexAsEnumAttributeDrawer : ReloadEnumDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (property.serializedObject.isEditingMultipleObjects)
        {
            EditorGUI.LabelField(position, new GUIContent(label.text + " : Cannot Edit Multiple Object"));
            return;
        }

        var att = ((IndexAsEnumAttribute) attribute);
        // for integer
        if (property.propertyType == SerializedPropertyType.Integer)
            DrawForInt(property, label, position, att.EnumList[0], att.LabelColor, att.Disable);
        // for IntInt
        else
            DrawForIntInt(property, label, position,att.EnumList,att.LabelColor);
    }
}

//^^^^^^^^^^^^^^^^^^^

[CustomPropertyDrawer(typeof(IndexAsEnumFlagAttribute))]
public class IndexAsEnumFlagAttributeDrawer : ReloadEnumDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (property.serializedObject.isEditingMultipleObjects)
        {
            EditorGUI.LabelField(position, new GUIContent(label.text + " : Cannot Edit Multiple Object"));
            return;
        }

        if (property.propertyType == SerializedPropertyType.Integer)
        {
            var att = ((IndexAsEnumFlagAttribute) attribute);
            SetColor(att.LabelColor);
            if (!att.EnumList.Found)
                att.EnumList.Reload(GetType(att.EnumList.TypeName));

            if (att.EnumList.Found && att.EnumList.Values.Length > 0)
            {
                IndexAsEnumDrawerHelper.DrawPropertyAsFlagEnum(att.EnumList, property, position, label);
            }
            else
            {
                label.text = label.text + $"({att.EnumList.TypeName})";
                EditorGUI.PropertyField(position, property, label, true);
            }

            ReleaseColor();
        }
        else
            EditorGUI.LabelField(position, label.text, "Use PopUp for Integer.");
    }
}

[CustomPropertyDrawer(typeof(EditEnumPointAttribute))]
public class EditEnumPointDrawer : ReloadEnumDrawer
{
    
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (property.serializedObject.isEditingMultipleObjects)
        {
            EditorGUI.LabelField(position, new GUIContent(label.text + " : Cannot Edit Multiple Object"));
            return;
        }
        
        var splits = EditorGUI.PrefixLabel(position, label).GetSplitRect(2, 1).ToArray();
//        EditorGUI.PropertyField(splits[0], property, label, true);
        if (property.propertyType == SerializedPropertyType.String)
        {
            property.stringValue = EditorGUI.TextField(splits[0], property.stringValue);
            
            var att = (EditEnumPointAttribute) attribute;
            var pointProperty = FindPropertyRelative(property, att.PointPropertyName);
            string[] values = {nameof(PointParameterBase.value), nameof(PointParameterBase.value2),nameof(PointParameterBase.value3),nameof(PointParameterBase.value4)};
            var propertyValue = pointProperty.serializedObject.FindProperty(pointProperty.propertyPath + "."+values[att.ValueIndex-1]);
            if (string.IsNullOrEmpty(property.stringValue) || property.stringValue.Contains(" "))
            {
                propertyValue.intValue = EditorGUI.IntField(splits[1], propertyValue.intValue);
            }
            else
            {  
                var enumList = new EnumList(property.stringValue);
                if (enumList.Found)
                    DrawForInt(propertyValue, splits[1], enumList, att.LabelColor, false);
                else
                    propertyValue.intValue = EditorGUI.IntField(splits[1], propertyValue.intValue);
            }
        }
    }
}


[CustomPropertyDrawer(typeof(IntStringAsEnumKeyAttribute))]
public class IntStringAsEnumKeyAttributeDrawer : ReloadEnumDrawer
{
    
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (property.serializedObject.isEditingMultipleObjects)
        {
            EditorGUI.LabelField(position, new GUIContent(label.text + " : Cannot Edit Multiple Object"));
            return;
        }
        
        var splits = EditorGUI.PrefixLabel(position, label).GetSplitRect(2, 1).ToArray();
        var att = (IntStringAsEnumKeyAttribute) attribute;
        SerializedProperty propertyInt = property.serializedObject.FindProperty(property.propertyPath + ".value");
        SerializedProperty propertyString =  property.serializedObject.FindProperty(property.propertyPath + ".value2");
        if (propertyInt != null && propertyString != null)
        {
            propertyString.stringValue = EditorGUI.TextField(splits[0], propertyString.stringValue);
            var enumList = new EnumList(propertyString.stringValue);
            if (enumList.Found)
                DrawForInt(propertyInt, splits[1], enumList, att.LabelColor, false);
            else
                propertyInt.intValue = EditorGUI.IntField(splits[1], propertyInt.intValue);
        }
    }
}


[CustomPropertyDrawer(typeof(IndexByBuiltInEnumAttribute))]
public class IndexByBuiltInEnumAttributeDrawer : ReloadEnumDrawer
{
    
    private EnumList _enumList;
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (property.serializedObject.isEditingMultipleObjects)
        {
            EditorGUI.LabelField(position, new GUIContent(label.text + " : Cannot Edit Multiple Object"));
            return;
        }

        if (property.propertyType == SerializedPropertyType.Integer)
        {
            var att = (IndexByBuiltInEnumAttribute) attribute;
            SetColor(att.LabelColor);
            if (_enumList == null)
                _enumList = FindEnumList(property, att.PropertyToCheck, att.EnumFindBy);

            if (_enumList.Found && _enumList.Values.Length > 0)
            {
                _index = -1;
                for (var i = 0; i < _enumList.Values.Length; i++)
                    if (property.intValue == _enumList.Values[i])
                    {
                        _index = i;
                        break;
                    }

                property.intValue = EditorGUI.IntPopup(position, label, property.intValue,
                    _enumList.Names.ToArray(), _enumList.Values.ToArray());
            }
            else
            {
                label.text = label.text + $"({_enumList.TypeName})";
                EditorGUI.PropertyField(position, property, label, true);
            }

            ReleaseColor();
        }
        // for IntInt
        else
        {
            var propertyValue = property.serializedObject.FindProperty(property.propertyPath + ".value");
            if (propertyValue != null)
            {
                List<SerializedProperty> listProperties = new List<SerializedProperty>() {propertyValue};
                for (int i = 2; i < 10; i++)
                {
                    var propertyValue2 = property.serializedObject.FindProperty(property.propertyPath + ".value" + i);
                    if (propertyValue2 != null)
                        listProperties.Add(propertyValue2);
                    else
                        break;
                }

                var att = ((IndexByBuiltInEnumAttribute) attribute);
                SetColor(att.LabelColor);
                if (_enumList == null)
                    _enumList = FindEnumList(property, att.PropertyToCheck, att.EnumFindBy);

                if (_enumList.Found && _enumList.Values.Length > 0)
                {
                    _index = -1;
                    for (var i = 0; i < _enumList.Values.Length; i++)
                        if (propertyValue.intValue == _enumList.Values[i])
                        {
                            _index = i;
                            break;
                        }

                    IList<int> values = _enumList.Values.ToList();
                    IList<GUIContent> names = _enumList.Names.ToList();
                    if (_index == -1)
                    {
                        _index = values.Count;
                        values = values.ArrayAdd(property.intValue);
                        names = names.ArrayAdd(
                            new GUIContent($"NotFound {_enumList.TypeName}_{propertyValue.intValue}"));
                    }

                    bool isArrayElement = property.IsArrayElement();
                    if (!isArrayElement)
                    {
                        position.x += EditorGUIUtility.labelWidth;
                        position.width -= EditorGUIUtility.labelWidth;
                    }

                    var splits = position.GetSplitRect(listProperties.Count, 1).ToArray();
                    if (!isArrayElement)
                    {
                        splits[0].x -= EditorGUIUtility.labelWidth;
                        splits[0].width += EditorGUIUtility.labelWidth;
                    }

                    propertyValue.intValue = EditorGUI.IntPopup(splits[0], isArrayElement ? new GUIContent() : label,
                        propertyValue.intValue,
                        names.ToArray(),
                        values.ToArray());

                    // otherrrsss
                    for (var i = 1; i < splits.Length; i++)
                    {
                        var valueI = listProperties[i];
                        //       EditorGUI.DrawTextureAlpha(splits[i],StaticTexture.SquareTexture);
//                        if (att.enumList.Length > i && att.enumList[i] != null)
//                        {
//                            var enumList = att.enumList[i];
//                            _index = -1;
//                            for (var i1 = 0; i1 < enumList.values.Length; i1++)
//                                if (valueI.intValue == enumList.values[i1])
//                                {
//                                    _index = i1;
//                                    break;
//                                }
//
//                            IList<int> values2 = enumList.values.ToList();
//                            IList<GUIContent> names2 = enumList.names.ToList();
//                            if (_index == -1)
//                            {
//                                _index = values.Count;
//                                values2 = values2.ArrayAdd(valueI.intValue);
//                                names2 = names2.ArrayAdd(
//                                    new GUIContent($"NotFound {enumList.typeName}_{valueI.intValue}"));
//                            }
//                            
//                            valueI.intValue = EditorGUI.IntPopup(splits[i], isArrayElement ? new GUIContent() : label,
//                                valueI.intValue,
//                                names2.ToArray(),
//                                values2.ToArray());
//                        }
//                        else
                        {
                            if (valueI.propertyType == SerializedPropertyType.Integer)
                                valueI.intValue = EditorGUI.IntField(splits[i], "", valueI.intValue);
                            if (valueI.propertyType == SerializedPropertyType.Float)
                                valueI.floatValue = EditorGUI.FloatField(splits[i], valueI.floatValue);
                            if (valueI.propertyType == SerializedPropertyType.String)
                                valueI.stringValue = EditorGUI.TextField(splits[i], valueI.stringValue);
                        }
                    }
                }
                else
                {
                    label.text = label.text + $"({_enumList.TypeName})";
                    EditorGUI.PropertyField(position, property, label, true);
                }

                ReleaseColor();
            }
            else EditorGUI.LabelField(position, label.text, "Use PopUp for Integer,Or IntValue ");
        }
    }
}

[CustomPropertyDrawer(typeof(IndexAsEnumMultipleNameAttribute))]
public class IndexAsEnumMultipleNameAttributeDrawer : IndexAsEnumConditionBaseAttributeDrawer
{
}

[CustomPropertyDrawer(typeof(IndexAsEnumMultipleAttribute))]
public class IndexAsEnumMultipleAttributeDrawer : IndexAsEnumConditionBaseAttributeDrawer
{
}

[CustomPropertyDrawer(typeof(IndexAsEnumFlagConditionAttribute))]
public class IndexAsEnumFlagConditionAttributeDrawer : PropertyDrawer
{
    private IndexAsEnumFlagConditionAttribute Attribute =>
        _attribute ?? (_attribute = attribute as IndexAsEnumFlagConditionAttribute);

    private string PropertyToCheck => Attribute != null ? _attribute.PropertyToCheck : null;
    private object[] CompareValues => Attribute != null ? _attribute.CompareValues : null;


    private IndexAsEnumFlagConditionAttribute _attribute;
    private bool _toShow = true;

    
    private int _indexMatch;
    private int _index = -1;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        _toShow = true;
        if (!string.IsNullOrEmpty(PropertyToCheck))
        {
            var conditionProperty = FindPropertyRelative(property, PropertyToCheck);
            if (conditionProperty != null)
            {
                bool isBoolMatch = conditionProperty.propertyType == SerializedPropertyType.Boolean &&
                                   conditionProperty.boolValue;

                string[] compareStringValues = null;
                if (CompareValues != null && CompareValues.Length > 0)
                {
                    compareStringValues = new string[CompareValues.Length];
                    for (int i = 0; i < compareStringValues.Length; i++)
                        compareStringValues[i] =
                            CompareValues[i] != null ? CompareValues[i].ToString().ToUpper() : "NULL";
                }


                //if (isBoolMatch && compareStringValue == "FALSE")
                //    isBoolMatch = false;

                bool objectMatch = false;
                if ((compareStringValues != null))
                {
                    string conditionPropertyStringValue = conditionProperty.AsStringValue().ToUpper();
                    for (var i = 0; i < compareStringValues.Length; i++)
                    {
                        if (compareStringValues[i] == conditionPropertyStringValue)
                        {
                            objectMatch = true;
                            _indexMatch = i;
                            break;
                        }
                    }
                }


                if (!isBoolMatch && !objectMatch)
                {
                    _toShow = false;
                }
            }
        }

        if (_toShow)
        {
            if (property.propertyType == SerializedPropertyType.Integer)
            {
                var att = ((IndexAsEnumFlagConditionAttribute) attribute);
                var enumList = att.EnumList[_indexMatch];
                if (enumList.Found && enumList.Values.Length > 0)
                {
                    _index = -1;
                    for (var i = 0; i < enumList.Values.Length; i++)
                    {
                        if (property.intValue == enumList.Values[i])
                        {
                            _index = i;
                            break;
                        }
                    }

                    IList<int> values = enumList.Values.ToList();
                    IList<string> names = enumList.Names.Select(x => x.text).ToList();

                    property.intValue = IndexAsEnumDrawerHelper.DrawBitMaskField(position, property.intValue,
                        names.ToArray(),
                        values.ToArray(), label);
                }
                else
                {
                    label.text = label.text + $"({enumList.TypeName})";
                    EditorGUI.PropertyField(position, property, label, true);
                }
            }
            else
                EditorGUI.LabelField(position, label.text, "Use PopUp for Integer.");
        }
        else
        {
            GUI.enabled = _toShow;
            EditorGUI.PropertyField(position, property, label, true);
            GUI.enabled = true;
        }
    }

    private SerializedProperty FindPropertyRelative(SerializedProperty property, string toGet)
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

            parent = i == 0 ? property.serializedObject.FindProperty(element) : parent.FindPropertyRelative(element);

            if (index >= 0) parent = parent.GetArrayElementAtIndex(index);
        }

        if (parent == null)
            return property.serializedObject.FindProperty(toGet);
        return parent.FindPropertyRelative(toGet);
    }
}

#endif