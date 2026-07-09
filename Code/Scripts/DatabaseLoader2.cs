//using System;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//#if UNITY_EDITOR
//using UnityEditor;
//#endif
//public static class DatabaseLoader2
//{
//    private static readonly Dictionary<Type, ScriptableObject> Settings = new Dictionary<Type, ScriptableObject>();
//
//
//    public static void Clear()
//    {
//        Settings.Clear();
//    }
//    public static T Resolve<T>() where T : ScriptableObject
//    {
//        if (!Settings.ContainsKey(typeof(T)))
//        {
//            T setting = Load<T>(typeof(T).Name);
//            if(setting != null)
//                Settings.Add(typeof(T),setting);
//        }
//     
//        return (T) Settings[typeof(T)];
//    }
//
//    public static void Add<T>(T setting) where T : ScriptableObject
//    {
//        if(setting != null)
//            Settings.Add(setting.GetType(),setting);
//    }
//    
//    static T Load<T>(string name) where T : ScriptableObject
//    {
//        #if UNITY_EDITOR
//        var projectSetting = ProjectSettings.LoadProjectSettings();
//        var data = AssetDatabase.LoadAssetAtPath<T>($"Assets/{projectSetting.projectSku}/Resources/{GetDatabaseFolder}/{name}.asset");
//    #else
//        var data = Resources.Load($"{GetDatabaseFolder}/{name}",typeof(T));
//    #endif
//        if(data == null)
//            Debug.LogError($"{typeof(T)} not exist on {GetDatabaseFolder}/{name}");
//        return data as T;
//    }
//    
//    private const string GetDatabaseFolder = "Database";
//}
