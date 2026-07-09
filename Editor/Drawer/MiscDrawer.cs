
using System;
using System.Linq;
using UnityEditor;
#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CustomPropertyDrawer(typeof(SingleUnityLayer))]
public class SingleUnityLayerPropertyDrawer : PropertyDrawer 
{
    public override void OnGUI(Rect _position, SerializedProperty _property, GUIContent _label)
    {
        EditorGUI.BeginProperty(_position, GUIContent.none, _property);
        SerializedProperty layerIndex = _property.FindPropertyRelative("layerIndex");
        _position = EditorGUI.PrefixLabel(_position, GUIUtility.GetControlID(FocusType.Passive), _label);
        if (layerIndex != null)
        {
            layerIndex.intValue = EditorGUI.LayerField(_position, layerIndex.intValue);
        }
        EditorGUI.EndProperty( );
    }
}

[CustomPropertyDrawer(typeof(DisablePropertyAttribute))]
public class DisablePropertyAttributeDrawer : PropertyDrawer
{
    private DisablePropertyAttribute Attribute => _attribute ?? (_attribute = attribute as DisablePropertyAttribute);

    private string PropertyToCheck => Attribute != null ? _attribute.PropertyToCheck : null;

    private object[] CompareValues => Attribute != null ? _attribute.CompareValues : null;

    private DisablePropertyAttribute _attribute;

    private int index = 0;
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        DisablePropertyAttribute popUpAttribute = attribute as DisablePropertyAttribute;
        bool show = true;
        if (!string.IsNullOrEmpty(PropertyToCheck))
        {
            var conditionProperty = FindPropertyRelative(property, PropertyToCheck);
            if (conditionProperty != null)
            {
                bool isBoolMatch = conditionProperty.propertyType == SerializedPropertyType.Boolean && conditionProperty.boolValue;

                string[] compareStringValues = null;
                if (CompareValues != null && CompareValues.Length > 0)
                {
                    compareStringValues = new string[CompareValues.Length];
                    for (int i = 0; i < compareStringValues.Length; i++)
                        compareStringValues[i] = CompareValues[i] != null ? CompareValues[i].ToString().ToUpper() : "NULL";
                }

                string conditionPropertyStringValue = AsStringValue(conditionProperty).ToUpper();
                bool objectMatch = compareStringValues != null && compareStringValues.Any(x => x == conditionPropertyStringValue);

                if (!isBoolMatch && !objectMatch)
                    show = false;
            }
        }

        GUI.enabled = show;
        EditorGUI.PropertyField(position, property, label, true);
        GUI.enabled = true;
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
                index = Convert.ToInt32(element.Substring(element.IndexOf("[", StringComparison.Ordinal)).Replace("[", "").Replace("]", ""));
                element = element.Substring(0, element.IndexOf("[", StringComparison.Ordinal));
            }

            parent = i == 0 ?
                property.serializedObject.FindProperty(element) :
                parent.FindPropertyRelative(element);

            if (index >= 0) parent = parent.GetArrayElementAtIndex(index);
        }


        return parent.FindPropertyRelative(toGet);
    }

    public string AsStringValue( SerializedProperty property)
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

[CustomPropertyDrawer(typeof(ConditionalEnumPopUpAttribute))]
public class ConditionalEnumPopUpPropertyDrawer : PropertyDrawer
{
    private ConditionalEnumPopUpAttribute Attribute => _attribute ?? (_attribute = attribute as ConditionalEnumPopUpAttribute);

    private string PropertyToCheck => Attribute != null ? _attribute.PropertyToCheck : null;

    private object[] CompareValues => Attribute != null ? _attribute.CompareValues : null;

    private ConditionalEnumPopUpAttribute _attribute;

    private int index = 0;
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        ConditionalEnumPopUpAttribute popUpAttribute = attribute as ConditionalEnumPopUpAttribute;
        bool show = true;
        if (!string.IsNullOrEmpty(PropertyToCheck))
        {
            var conditionProperty = FindPropertyRelative(property, PropertyToCheck);
            if (conditionProperty != null)
            {
                bool isBoolMatch = conditionProperty.propertyType == SerializedPropertyType.Boolean && conditionProperty.boolValue;

                string[] compareStringValues = null;
                if (CompareValues != null && CompareValues.Length > 0)
                {
                    compareStringValues = new string[CompareValues.Length];
                    for (int i = 0; i < compareStringValues.Length; i++)
                        compareStringValues[i] = CompareValues[i] != null ? CompareValues[i].ToString().ToUpper() : "NULL";
                }

                string conditionPropertyStringValue = AsStringValue(conditionProperty).ToUpper();
                bool objectMatch = compareStringValues != null && compareStringValues.Any(x => x == conditionPropertyStringValue);

                if (!isBoolMatch && !objectMatch)
                    show = false;
            }
        }


//        GUI.enabled = show;
        // Now draw the property as a Slider or an IntSlider based on whether it's a float or integer.
        if (property.propertyType == SerializedPropertyType.Integer)
        {
            if (show)
            {
                index = popUpAttribute.Values.ToList().IndexOf(property.intValue);
                var prevIndex = index;
               
                if (popUpAttribute.Values.Contains(property.intValue))
                {
                    index = EditorGUI.Popup(position, property.displayName, index, popUpAttribute.Names);
                    if (prevIndex != index)
                        property.intValue = popUpAttribute.Values[index];
                }
                else
                {
                    index = EditorGUI.Popup(position, property.displayName, 0, popUpAttribute.Names);
                    if (prevIndex != index)
                        property.intValue = popUpAttribute.Values[index];
                }
            }
            else
            {
                EditorGUI.PropertyField(position, property, label, true);
            }
        
        }
        else
            EditorGUI.LabelField(position, label.text, "Use PopUp for Integer.");
        
        GUI.enabled = true;
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
                index = Convert.ToInt32(element.Substring(element.IndexOf("[", StringComparison.Ordinal)).Replace("[", "").Replace("]", ""));
                element = element.Substring(0, element.IndexOf("[", StringComparison.Ordinal));
            }

            parent = i == 0 ?
                property.serializedObject.FindProperty(element) :
                parent.FindPropertyRelative(element);

            if (index >= 0) parent = parent.GetArrayElementAtIndex(index);
        }


        return parent.FindPropertyRelative(toGet);
    }

    public string AsStringValue( SerializedProperty property)
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


[CustomPropertyDrawer(typeof(MyCustomFontCharacter))]
public class MyCustomFontCharacterDrawer : DuongClassDrawer2
{
    public override string[] Displays => new[]
    {
        nameof(MyCustomFontCharacter.index),
        nameof(MyCustomFontCharacter.sprite),
        nameof(MyCustomFontCharacter.adjustSpacing),
    };

}

[CustomPropertyDrawer(typeof(EditorDisplayNameAttribute))]
public class EditorDisplayNameAttributeDrawer : DuongClassDrawer
{
    public override IList<DivideData> DivideData(SerializedProperty property) => Attribute.Data;
    protected override bool HideLabel => false;
    private EditorDisplayNameAttribute Attribute =>  attribute as EditorDisplayNameAttribute;
}


[CustomPropertyDrawer(typeof(InLineAttribute))]
public class InLineAttributeDrawer : ReloadEnumDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var att = ((InLineAttribute) attribute);

        var posWithoutWeight = position;
        posWithoutWeight.width -= EditorGUIUtility.labelWidth;
        posWithoutWeight.x += EditorGUIUtility.labelWidth;

        var rects = posWithoutWeight.GetSplitRect(att.RelativePropertyNames.Length + 1, 5);
        rects[0].x -= EditorGUIUtility.labelWidth;
        rects[0].width += EditorGUIUtility.labelWidth;

        if (att.FirstElementWidth > 0)
        {
            rects[0].width = att.FirstElementWidth;
            posWithoutWeight.x += att.FirstElementWidth + 5;
            posWithoutWeight.width -= att.FirstElementWidth;
            Rect[]  rects2 = posWithoutWeight.GetSplitRect(att.RelativePropertyNames.Length , 5);
            for (var i = 0; i < rects2.Length; i++)
                rects[i + 1] = rects2[i];
            rects[rects.Length - 1].width -= 5;
        }
        
        var label2 = new GUIContent()
        {
            text = label.text
        };
        EditorGUI.PropertyField(rects[0], property, label2);
        
        for (var i = 0; i < att.RelativePropertyNames.Length; i++)
        {
            var property2 = property.serializedObject.FindProperty(att.RelativePropertyNames[i]);
            EditorGUI.PropertyField(rects[i+1],property2,GUIContent.none);
        }
    }
}

[CustomPropertyDrawer(typeof(DisplayStringAsLabelAttribute))]
public class DisplayStringAsLabelAttributeDrawer : ReloadEnumDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (property.serializedObject.isEditingMultipleObjects)
        {
            EditorGUI.LabelField(position, new GUIContent(label.text + " : Cannot Display Multiple Object"));
            return;
        }

        EditorGUI.LabelField(position, label.text, property.stringValue);
    }
}

[CustomPropertyDrawer(typeof(DisplayAsDisableAttribute))]
public class DisplayAsDisableAttributeDrawer : ReloadEnumDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (property.serializedObject.isEditingMultipleObjects)
        {
            EditorGUI.LabelField(position, new GUIContent(label.text + " : Cannot Edit Multiple Object"));
            return;
        }

        GUI.enabled = false;
        EditorGUI.PropertyField(position, property, label, true);
        GUI.enabled = true;
    }
}

[CustomPropertyDrawer(typeof(SplitHeaderAttribute))]
public class SplitHeaderAttributeDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var att = ((SplitHeaderAttribute) this.attribute);
        position.height -= 10;
        position.y += 5;
        EditorGUI.DrawTextureAlpha(position,StaticTexture.SquareTexture);
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return 12;
    }
}

[CustomPropertyDrawer(typeof(LabelHeaderAttribute))]
public class LabelHeaderAttributeDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var att = ((LabelHeaderAttribute) this.attribute);

        Color original = EditorStyles.label.normal.textColor;
        Color newColor = new Color(att.R, att.G, att.B);

        SetColor(newColor);
        EditorGUI.LabelField(position, new GUIContent(att.text));
        SetColor(original);
    }

    private void SetColor(Color color)
    {
        SetStyleColor(EditorStyles.label, color);
        SetStyleColor(EditorStyles.foldout, color);
        SetStyleColor(EditorStyles.foldoutPreDrop, color);
    }

    private void SetStyleColor(GUIStyle style, Color color)
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
}

[CustomPropertyDrawer(typeof(LabelColorAttribute))]
public class LabelColorDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var att = (LabelColorAttribute) this.attribute;

        Color original = EditorStyles.label.normal.textColor;
        Color newColor = new Color(att.R, att.G, att.B);

        SetColor(newColor);
        if (att.AddName != null)
            label.text = att.AddName;
        EditorGUI.PropertyField(position, property, label, true);
        SetColor(original);
    }

    private void SetColor(Color color)
    {
        SetStyleColor(EditorStyles.label, color);
        SetStyleColor(EditorStyles.foldout, color);
        SetStyleColor(EditorStyles.foldoutPreDrop, color);
    }

    private void SetStyleColor(GUIStyle style, Color color)
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
}


#endif