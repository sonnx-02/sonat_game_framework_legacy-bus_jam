using System;
using System.Collections;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
 
public static class EditorHelper
{
    public static T GetSerializedValue<T>(this SerializedProperty property)
    {
        object @object = property.serializedObject.targetObject;
        string[] propertyNames = property.propertyPath.Split('.');
 
        List<string> propertyNamesClean = new List<String>();
 
        for (int i = 0; i < propertyNames.Count(); i++)
        {
            if (propertyNames[i] == "Array")
            {
                if (i != (propertyNames.Count() -1) && propertyNames[i + 1].StartsWith("data"))
                {
                    int pos = int.Parse(propertyNames[i + 1].Split('[', ']')[1]);
                    propertyNamesClean.Add($"-GetArray_{pos}");
                    i++;
                }
                else
                    propertyNamesClean.Add(propertyNames[i]);
            }
            else
                propertyNamesClean.Add(propertyNames[i]);
        }
        // Get the last object of the property path.
        foreach (string path in propertyNamesClean)
        {
            if (path.StartsWith("-GetArray"))
            {
                string[] split = path.Split('_');
                int index = int.Parse(split[split.Count() - 1]);
                IList l = (IList)@object;
                @object = l[index];
            }
            else
            {
                @object = @object.GetType()
                    .GetField(path, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance)
                    .GetValue(@object);
            }
        }
        return (T)@object;
    }
    
    /// <summary>
    /// Gets the object the property represents.
    /// </summary>
    /// <param name="prop"></param>
    /// <returns></returns>
    public static object GetTargetObjectOfProperty(SerializedProperty prop)
    {
        var path = prop.propertyPath.Replace(".Array.data[", "[");
        object obj = prop.serializedObject.targetObject;
        var elements = path.Split('.');
        foreach (var element in elements)
        {
            if (element.Contains("["))
            {
                var elementName = element.Substring(0, element.IndexOf("["));
                var index = System.Convert.ToInt32(element.Substring(element.IndexOf("[")).Replace("[", "").Replace("]", ""));
                obj = GetValue_Imp(obj, elementName, index);
            }
            else
            {
                obj = GetValue_Imp(obj, element);
            }
        }
        return obj;
    }
 
    private static object GetValue_Imp(object source, string name)
    {
        if (source == null)
            return null;
        var type = source.GetType();
 
        while (type != null)
        {
            var f = type.GetField(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            if (f != null)
                return f.GetValue(source);
 
            var p = type.GetProperty(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
            if (p != null)
                return p.GetValue(source, null);
 
            type = type.BaseType;
        }
        return null;
    }
 
    private static object GetValue_Imp(object source, string name, int index)
    {
        var enumerable = GetValue_Imp(source, name) as System.Collections.IEnumerable;
        if (enumerable == null) return null;
        var enm = enumerable.GetEnumerator();
        //while (index-- >= 0)
        //    enm.MoveNext();
        //return enm.Current;
 
        for (int i = 0; i <= index; i++)
        {
            if (!enm.MoveNext()) return null;
        }
        return enm.Current;
    }
 
}