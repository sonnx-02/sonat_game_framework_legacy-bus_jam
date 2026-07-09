using System;
using UnityEditor;

public static class DuongEditorHelper
{
    public static SerializedProperty FindPropertyRelative(SerializedProperty property, string propertyPath)
    {
        if (property.depth == 0) return property.serializedObject.FindProperty(propertyPath);

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


        return parent.FindPropertyRelative(propertyPath);
    }
    
    public static Type GetType(string enumTypeName)
    {
        Type type = Type.GetType(enumTypeName);
        if (type != null && type.IsEnum)
        {
            return type;
        }
#if UNITY_EDITOR
        else
        {
            var projectSettings = ProjectSettings.LoadProjectSettings();
            type = GetTypeClass.GetType(projectSettings.nameSpace, enumTypeName);
            if (type != null && type.IsEnum)
                return type;
        }
        return null;
#endif
    }
}
