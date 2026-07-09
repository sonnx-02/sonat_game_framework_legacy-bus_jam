using System.Collections;
using System.Collections.Generic;
using System.IO;
using UniRx;
using UnityEngine;




[System.Serializable]
public class BuilderPlayerData
{
    public int[] stages;
    public IntegerSerializableDictionary stocks = new IntegerSerializableDictionary();
    private static string FilePath => $"{Application.persistentDataPath}/BuilderPlayerData.json";

    public void Save()
    {
        Debug.Log("Save" + nameof(BuilderPlayerData));
        File.WriteAllText(FilePath, JsonUtility.ToJson(this));
    }

   
    public static void DeleteFile()
    {
        File.Delete(FilePath);
    }


    public BuilderPlayerData()
    {
    }

    public BuilderPlayerData(ArrayInt[] targets)
    {
        stages = new int[targets.Length];
    }

    public virtual void Validate()
    {
        if(stocks == null)
            stocks = new IntegerSerializableDictionary();
    }

    public static T Load<T>(T whenNew) where T : BuilderPlayerData
    {
        if (File.Exists(FilePath))
        {
            string json = File.ReadAllText(FilePath);
            T data = JsonUtility.FromJson<T>(json);
            if (data != null)
            {
                data.Validate();
                return data;
            }
        }

        var newData = whenNew;
        newData.Validate();
        return newData;
    }

   
}