using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;


[Serializable]
public class StringOnOff
{
    public string on;
    public string off;
}

[Serializable]
public class IntOnOff
{
    public int on;
    public int off;
}

[Serializable]
public class FloatOnOff
{
    public float on;
    public float off;
}


[Serializable]
public class Point2Float
{
    public float x;
    public float y;

    public Point2Float(float a, float b)
    {
        x = a;
        y = b;
    }

    public float GetRandom() => UnityEngine.Random.Range(x, y);

    public Point2Float Clone() => new Point2Float(x, y);
}


[Serializable]
public class Point3Float
{
    public float x;
    public float y;
    public float z;

    public float Value1 => x;
    public float Value2 => y;
    public float Value3 => z;

    public Point3Float()
    {
        
    }

    public Point3Float(float x, float y, float z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }
    
    public Point3Float Clone() => new Point3Float()
    {
        x = x,y =y,z=z
    };
}

[System.Serializable]
public abstract class BaseLevel
{
    public int id;
    public int difficulty;
    public abstract BaseLevel Clone();

    public abstract bool IsNotValid();

    public virtual void Log()
    {
        
    }

    public virtual void Random()
    {
        
    }
    
    public virtual void Random2()
    {
        
    }

    public virtual void RandomByKey(int key)
    {
        
    }
    
    public virtual bool IsEqual(BaseLevel other)
    {
        return false;
    }
}


[System.Serializable]
public class ListLevel<T> where T : BaseLevel
{
    public  T[] levels;
    public ListLevel(T[] list)
    {
        levels = list;
    }
}


[System.Serializable]
public class BaseLevelDatabaseClass : ScriptableObject
{

}

[System.Serializable]
public class BaseLevelDatabase<T> : BaseLevelDatabaseClass where T : BaseLevel
{ 

    [SerializeField] protected int startLoopRandomFrom = 5;
    public  T GetLevelRandom(int i)
    {
        var rng = new System.Random(i);
        var from = Mathf.Min(startLoopRandomFrom, levels.Count-1);
        return levels[rng.Next(from,levels.Count)];
    }
    
    public void SetDirtyThis()
    {
#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(this);
        UnityEditor.AssetDatabase.Refresh();
#endif
        
    }


    
   // #if UNITY_EDITOR
    [SerializeField] protected TextAsset textAsset;
    [SerializeField] protected List<TextAsset> listLevel;
    [SerializeField] [MyButton(nameof(LoadLevel),nameof(SaveLevel))] private int loadLevel;
    [SerializeField] [MyButton(nameof(RandomAllLevel))] private int testRandomAll;

    [SerializeField] [TextArea] private string remapParseText;
       
    [MyButton(nameof(Parse))] [SerializeField] private int parse;
    public void Parse()
    {
       IList<int> arr = new List<int>();
       foreach (var s in remapParseText.Split('\n'))
       {
           Debug.Log(s);
       }
       var a = remapParseText.Split('\n').Select(x => int.Parse(x)).ToArray();
       remap = a;
       
       this.SetDirty();
    }

    protected virtual void RandomAllLevel()
    {
        foreach (var baseLevel in levels)
        {
            baseLevel.Random();
        }

        SetDirtyThis();
    }

    public virtual void LoadLevelFromJson(IEnumerable<string> levelData)
    {
        
    }

    public virtual void LoadLevel()
    {
        JsonUtility.FromJsonOverwrite(textAsset.text,this);
        for (int i = 0; i < levels.Count; i++)
        { 
            BaseLevel level = levels[i];
            level.id = i + 1;
        }
    }
    
    public virtual void SaveLevel()
    {
        string path = "Assets/level.txt";
        System.IO.File.WriteAllText(path, JsonUtility.ToJson(new ListLevel<T>(levels.ToArray())));
        
#if UNITY_EDITOR
        UnityEditor.AssetDatabase.Refresh();
            .5f.Timer(() =>
        {
            Debug.Log(path,UnityEditor.AssetDatabase.LoadAssetAtPath<TextAsset>(path));
        });
#endif
    }

   
    
    [Tooltip("copyFrom/number/insert")]
    [SerializeField] protected ScriptableObject other;
    [LabelHeader("TakeFrom,AmountTake,InsertAt(last is length)",0,1,1)] [SerializeField]  private int test_kjdfjgkldjf;
    [SerializeField] private PointParameter copy;
    [MyButton(nameof(CopY),nameof(Shuffle))] [SerializeField] private int copyTest;
    public void CopY()
    {
        var copyFrom = other as BaseLevelDatabase<T>;
        if (copyFrom != null)
        {
            var take = copyFrom.levels.Take(copy.row).Skip(copy.col);
            levels.InsertRange(copy.value,take);
        }
        Debug.Log(copyFrom);
    }
    
    public void Shuffle()
    {
        var cut = levels.Take(copy.col);
        var take = levels.Take(copy.row).Skip(copy.col).ToList();
        take.Shuffle(new System.Random(UnityEngine.Random.Range(0, 1000)));
        levels = cut.ToList();
        levels.AddRange(take);
       #if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(this);
        #endif
    }

    [MyButton(nameof(Log))] [SerializeField] private int testLog;
    public void Log()
    {
        for (int i = 0; i < levels.Count; i++)
        {
            levels[i].Log();
        }
    }
    
    [MyButton(nameof(FillId))] [SerializeField] private int fillId;
    public void FillId()
    {
        for (var i = 0; i < levels.Count; i++)
        {
            levels[i].id = i;
        }
    }
    
    [MyButtonVector2Int(nameof(Cut))] [SerializeField] private Vector2Int v;
    public void Cut(Vector2Int vec)
    {
        List<T> cp = new List<T>();
        for (int i = vec.x; i < vec.y; i++)
        {
            cp.Add(levels[i]);
        }

        levels = cp;
    }
    
    [MyButton(nameof(ExportData))] [SerializeField] private int texport;
    [MyButton(nameof(CreateRemap),nameof(RemapLevel),nameof(UndoRemap))] [SerializeField] private int tremap;
   
    public int[] remap;

    protected virtual string exportPath => $"Assets/export/{name}.csv";

    protected char seprateChar = ',';
    protected virtual string GetExportData()
    {
//        string title = $"Level{seprateChar}Time{seprateChar}Car{seprateChar}CObstacle{seprateChar}C" 
////                           + string.Join("\t", colors) + "\t"
////                           + string.Join("\t", shape) + "\t"
////                           + string.Join("\t", sizes) + "\t"
//                       + "\n";
//        _count = 0;
//        return title + string.Join("\n", levels.Select(GetLevel));
        Debug.LogError("not handler yet");
        return "empty";
    }

    protected int _count;
    public void Export(string filePath)
    {
        System.IO.File.WriteAllText(filePath, GetExportData());
#if UNITY_EDITOR
        UnityEditor.AssetDatabase.Refresh();
        Debug.Log(this,this);
        1f.Timer(() =>
        {
            Debug.Log(filePath,UnityEditor.AssetDatabase.LoadAssetAtPath<TextAsset>(filePath));
        });
#endif
    }
    
   
    protected void ExportData()
    {
#if UNITY_EDITOR
        if (!UnityEditor.AssetDatabase.IsValidFolder("Assets/export"))
            UnityEditor. AssetDatabase.CreateFolder("Assets", "export");
#endif
        Export(exportPath);
    }

    protected void CreateRemap()
    {
        remap = new int[levels.Count];
        for (var i = 0; i < remap.Length; i++)
            remap[i] = i;
    }
    
    protected void UndoRemap()
    {
        levels = levels.OrderBy(x => x.id).ToList();
        this.SetDirty();
    }

    protected void RemapLevel()
    {
        bool notValid = false;
        for (var i = 0; i < remap.Length; i++)
        {
            var temp = remap[i];
            for (var j = i + 1; j < remap.Length; j++)
                if (temp == remap[j])
                {
                    Debug.LogError("item duplicate at "+i+" and "+j);
                    notValid = true;
                }
        }
        
        for (var i = 0; i < levels.Count; i++)
        {
            if (!remap.Contains(i))
            {
                Debug.LogError("item "+i+" not remap");
                notValid = true; 
            }
        }

        
        for (var i = 0; i < levels.Count; i++)
            if(levels[i].id != i)
            {
                Debug.LogError("id "+i+" is not arranged");
                notValid = true;
            }    

        if(notValid)
            return;
        var temp2 = new T[levels.Count];
        for (var i = 0; i < remap.Length; i++)
            temp2[i] = levels[remap[i]];

        levels = temp2.ToList();
        this.SetDirty();
    }
    
//#endif

    [ArrayElementTitle(nameof(BaseLevel.id))]
    public List<T> levels = new List<T>();
    public virtual T GetLevel(int i)
    {
        if (i < levels.Count)
            return levels[i];

        if (startLoopRandomFrom == 0)
        {
            var rng = new System.Random(i);
            return levels[rng.Next(0,levels.Count)];
        }

        return GetLevelRandom(i);
    }
    
    public virtual T GetLevelClone(int i)
    {
       return (T) GetLevel(i).Clone();
    }

    public virtual string GetParseData()
    {
        return "level=" + levels.Count;
    }
}

public static class LevelHelper
{
    
}
