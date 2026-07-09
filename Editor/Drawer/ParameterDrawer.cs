
using System;
using UnityEngine;
#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;

[CustomPropertyDrawer(typeof(IndexValue))]
public class IndexValueDrawer : DuongClassDrawer
{
    protected override bool HideLabel => true;

    public override IList<DivideData> DivideData(SerializedProperty property) => new List<DivideData>
    {
        new DivideData(1, nameof(IndexValue.index), nameof(IndexValue.index), 40),
        new DivideData(1, nameof(IndexValue.value), nameof(IndexValue.value), 40)
    };
}


[CustomPropertyDrawer(typeof(CustomParameter))]
public class CustomParameterDrawer : DuongClassDrawer
{
    public override IList<DivideData> DivideData(SerializedProperty property) => new List<DivideData>
    {
        new DivideData(1, nameof(CustomParameter.intValue)),
        new DivideData(1, nameof(CustomParameter.intValue2)),
        new DivideData(1, nameof(CustomParameter.intValue3)),
        new DivideData(1, nameof(CustomParameter.boolValue)),
        new DivideData(1, nameof(CustomParameter.boolValue2))
    };
    protected override bool HideLabel => false;

}


[CustomPropertyDrawer(typeof(IndexParam))]
public class IndexParamDrawer : ReloadEnumDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (property.serializedObject.isEditingMultipleObjects)
        {
            EditorGUI.LabelField(position, new GUIContent(label.text + " : Cannot Edit Multiple Object"));
            return;
        }

        var att = ((IndexParam) attribute);
        SetColor(att.LabelColor);
//            EditorGUI.PropertyField(position, property, label, true);
        var rect = new Rect(position.x, position.y, position.width - 60, position.height);
        EditorGUI.PropertyField(rect, property, label, true);
        var enumType = FindPropertyRelative2(property, att.IndexPropertyName);
        rect = new Rect(position.x + position.width - 60, position.y, 60, position.height);
        enumType.intValue = EditorGUI.IntField(rect, "", enumType.intValue);
        ReleaseColor();
    }

    private SerializedProperty FindPropertyRelative2(SerializedProperty property, string toGet)
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

        return parent.FindPropertyRelative(toGet);
    }
}

[CustomPropertyDrawer(typeof(StringOnOff))]
[CustomPropertyDrawer(typeof(IntOnOff))]
[CustomPropertyDrawer(typeof(FloatOnOff))]
public class OnOffDrawer : DuongClassDrawer
{
    public override IList<DivideData> DivideData(SerializedProperty property) => new List<DivideData>
    {
        new DivideData(1, nameof(StringOnOff.@on), nameof(StringOnOff.@on), 20),
        new DivideData(1, nameof(StringOnOff.off), nameof(StringOnOff.off), 20)
    };
    protected override bool HideLabel => false;
}

#endif