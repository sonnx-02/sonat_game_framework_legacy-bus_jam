using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

[Serializable]
public abstract class BaseCustomGameSave
{
//    private static string FilePath => $"{Application.persistentDataPath}/CustomGameSave.json";
//
//    public static bool ignoreSave;
//
//    public void Save()
//    {
//        if (ignoreSave)
//            return;
//
//        Debug.Log("Save" + nameof(BaseCustomGameSave));
//        File.WriteAllText(FilePath, JsonUtility.ToJson(this));
//    }
//
//    public static T Load<T>(T whenNew) where T : BaseCustomGameSave
//    {
//        if (File.Exists(FilePath))
//        {
//            string json = File.ReadAllText(FilePath);
//            T data = JsonUtility.FromJson<T>(json);
//            if (data != null)
//            {
//                data.Validate();
//                return data;
//            }
//        }
//
//        var newData = whenNew;
//        newData.Validate();
//        return newData;
//    }
//
//    public static void DeleteFile()
//    {
//        File.Delete(FilePath);
//    }
    public abstract void Validate();
}


[Serializable]
public abstract class BaseBoardState
{
    public abstract BaseBoardState Clone();
}


[Serializable]
public class GameSave<T> where T : BaseBoardState // : IGameSave where T : IBoardState
{
    // need to validate if change
    public int[] remap;
    public int[] properties;
    public int continued;
    public bool won;
    public int maxState = 5;

    public int this[int key]
    {
        get
        {
            for (var i = 0; i < remap.Length; i++)
                if (key == i)
                    return properties[remap[key]];

            var newRemap = new int[remap.Length + 1];
            var newProperties = new int[remap.Length + 1];
            newRemap[remap.Length] = key;
            Array.Copy(remap, newRemap, remap.Length);
            Array.Copy(properties, newProperties, remap.Length);
            Debug.LogError("not valid,dirty,only for safe production");
            return 0;
        }
        set
        {
            for (var i = 0; i < remap.Length; i++)
                if (key == i)
                {
                    properties[remap[key]] = value;
                    return;
                }

            var newRemap = new int[remap.Length + 1];
            var newProperties = new int[remap.Length + 1];
            newRemap[remap.Length] = key;
            Array.Copy(remap, newRemap, remap.Length);
            Array.Copy(properties, newProperties, remap.Length);
            newProperties[remap.Length] = value;
            Debug.LogError("not valid,dirty,only for safe production");
        }
    }

    public bool GetBool(int key) => this[key] == 1;
    public void SetBool(int key, bool value) => this[key] = value ? 1 : 0;

    [SerializeField] private List<T> states = new List<T>();

    public void ClearMoves()
    {
        for (var i = states.Count - 1; i >= 1; i--)
            states.RemoveAt(i);
    }
    
    public void ClearMoves(T startState)
    {
        states.Clear();
        states.Add(startState);
    }

    public int Count => states.Count;

    public GameSave<T> Clone()
    {
        return new GameSave<T>(remap, properties)
        {
            states = states.Select(x => (T) x.Clone()).ToList(),
            remap = remap.ToArray(),
            continued = continued,
            properties = properties.ToArray(),
            maxState = maxState,
            won =  won,
        };
    }

    public GameSave(int[] remap, int[] properties)
    {
        this.remap = remap;
        this.properties = properties;
    }

    public GameSave(Type enumType, int maxState = 5)
    {
        var array = Enum.GetValues(enumType);
        remap = array.Cast<int>().ToArray();
        properties = new int[array.Length];
        this.maxState = maxState;
    }

    public void AddState(T add)
    {
        states.Add(add);
        if (Count > 0 && Count > maxState)
            states.RemoveAt(0);
    }

//    public int targetValue;

    public T TakeOutLastState()
    {
        if (states.Count > 1)
            states.RemoveAt(states.Count - 1);

        return GetLastState();
    }

    public T GetLastState()
    {
        if (states.Count > 0)
            return states[states.Count - 1];
        return null;
    }
    
    public T FirstState()
    {
        if (states.Count > 0)
            return states[0];
        return null;
    }
}