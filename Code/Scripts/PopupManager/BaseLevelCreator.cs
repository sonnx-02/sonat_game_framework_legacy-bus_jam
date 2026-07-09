using System;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UniRx;


#if UNITY_EDITOR
// using UnityEditor;
#endif

[System.Serializable]
public class LevelCreatorUI
{
    public Button pre, next, loadToTemp, save, load, saveAsNew, delete, create, init, play, reload, undo, fileSelect;
    public Button loadJson, writeJson, fillDatabase, parse, exportData;
    public Button clearLog;
    public TMP_InputField inputLevel;
    public TMP_InputField setSpecialCustomer;
    public ButtonPoint saveAmountSpecial;
    public TMP_InputField filePath; // = "D:\\bus_jam.json";
    public IndexBindingScript textTotal;
    public ButtonPoint[] buttonParameters;
    public ButtonPoint[] bindCurrentPointButtons;
    public TMP_Text logText;

    public int InputLevelValue => int.Parse(inputLevel.text);
        
    private static readonly List<string> Logs = new List<string>();
    public void AddLog(string log)
    {
        Logs.Add(log);
        ShowLog();
    }

    public void ShowLog()
    {
        if (logText != null)
        {
            logText.text = string.Join(" \n", GetLog());
        }
    }
    
    private IEnumerable<string> GetLog()
    {
        for (var i = Logs.Count - 1; i >= 0; i--)
        {
            yield return Logs[i];
        }
    }

    public void ClearLog()
    {
        Logs.Clear();
        ShowLog();
    }

    public void Register()
    {
        if (clearLog != null)
            clearLog.onClick.AddListener(ClearLog);
        saveAmountSpecial.CheckRegister();
//#if UNITY_EDITOR
        inputLevel.text = PlayerPrefs.GetInt("input_level").ToString();
        inputLevel.onValueChanged.AddListener( data => PlayerPrefs.SetInt("input_level",int.Parse(data)));
//#endif
        ClearLog();
    }
}

public abstract class BaseBaseLevelCreator : ValidScriptBase
{
    
}

//public enum ButtonPointAction
//{
//    SetCurrent,
//}

public abstract class BaseLevelCreator<T, T2> : BaseBaseLevelCreator where T : BaseGameController where T2 : BaseLevel
{
    public virtual int Count => LevelDatabase == null ? 1 : LevelDatabase.levels.Count;
    public BaseLevelDatabaseClass source;
    public string defaultSave = "D:/level.json";
    public BoolBindingReactiveProperty isEditing;
    public int maxUndo;


    public List<T2> saveUndos = new List<T2>();

    public virtual void AddCurrentStateToUndo()
    {
        if(GameController.CreatingLevel)
            DuongExtensions.ExecuteInNextNFrame(() =>
            {
                var clone = temp.Clone() as T2;
                LoadFromPool(clone);
                AddUndo(clone);
            },1);
    }

    public virtual void AddUndo(T2 t2)
    {
        if (saveUndos.Count < maxUndo)
        {
            if (saveUndos.Count == 0 || !saveUndos[saveUndos.Count - 1].IsEqual(t2))
            {
                saveUndos.Add(t2.Clone() as T2);
              //  creatorUi.AddLog("new undo");
            }
            else
            {
              //  creatorUi.AddLog("cannot save, identical detected");
            }
        }
    }

    public virtual void Undo()
    {
        if (saveUndos.Count > 1)
        {
            saveUndos.RemoveAt(saveUndos.Count - 1);
            temp = saveUndos[saveUndos.Count - 1];
            InitTemp();
        }
        else
        {
            creatorUi.AddLog("There is no undo");
        }
    }

    public virtual string FilePath
    {
        get
        {
            return Application.streamingAssetsPath + "/LevelData";
            if (creatorUi != null)
                return creatorUi.filePath.text;
            return defaultSave;
        }
    }

    public virtual string FileExportPath
    {
        get
        {
            return defaultSave.Replace(".json", ".csv");
        }
    }

    public LevelCreatorUI creatorUi;
    [SerializeField] protected T2 temp;
    
    
    public abstract T GameController { get; }
    protected T2 _cached;
    protected BaseLevelDatabase<T2> JsonLevelDatabase;

    [SerializeField]
    public virtual BaseLevelDatabase<T2> LevelDatabase
    {
        get
        {
            if (source != null)
                return (BaseLevelDatabase<T2>) source;

            if (JsonLevelDatabase == null)
            {
                JsonLevelDatabase = CreateLevelDatabase();
                if (File.Exists(FilePath) && JsonLevelDatabase != null)
                {
                    var levelData = Directory.EnumerateFiles(FilePath, "*.json");
                    JsonLevelDatabase.LoadLevelFromJson(levelData);
                }
            }

            return JsonLevelDatabase;
        }
    }


    protected abstract BaseLevelDatabase<T2>  CreateLevelDatabase(); //=> ScriptableObject.CreateInstance<LevelDatabase>();

    protected override void Register()
    {
        base.Register();
        creatorUi.Register();
        isEditing.Init();
        RegisterUiBinding();
        UpdateView();
    }
    
    [MyButtonInt(new[] {nameof(RandomByKey)}, BuiltInEnumType.RandomLevelType)]
    public int randomKey;

    public abstract void RandomByKey(int key);


    [MyButton(nameof(LoadFromPool), nameof(LoadFromScene))] [SerializeField]
    private int load;

    [MyButtonIntPageSelect(new[]
            {nameof(SaveLevel), nameof(LoadLevel), nameof(Delete), nameof(SaveAsNew)},
        new[] {"Save", "Load", "Delete", "SaveN"}, nameof(Count))]
    public int currentLevel;

    [MyButton(nameof(Create), nameof(Clear))] 
    public int create2;
    [MyButton(nameof(InitTemp), nameof(PlayTemp))]
    public int create3;

    [MyButton(nameof(SetDirty))]
    public int create4; 
    [MyButton(nameof(SetEditing))]
    public int create5;
    [MyButton(nameof(WriteJson))]
    public int create6;

    public virtual int CurrentLevel
    {
        get => currentLevel;
        set => currentLevel = value;
    }

    public void SetEditing()
    {
        isEditing.Value = true;
    }

    protected override bool IsThisValid()
    {
        return !isEditing.Value;
    }


    protected virtual void RegisterUiBinding()
    {
        if (creatorUi.pre != null)
            creatorUi.pre.onClick.AddListener(() =>
            {
                CurrentLevel = Mathf.Clamp(CurrentLevel - 1, 0, Count);
                LoadCurrentLevel();
            });
        if (creatorUi.next != null)
            creatorUi.next.onClick.AddListener(() =>
            {
                CurrentLevel = Mathf.Clamp(CurrentLevel + 1, 0, Count);
                LoadCurrentLevel();
            });
        if (creatorUi.init != null)
            creatorUi.init.onClick.AddListener(InitTemp);
        if (creatorUi.create != null)
            creatorUi.create.onClick.AddListener(Create);
        if (creatorUi.init != null)
            creatorUi.init.onClick.AddListener(InitTemp);
        if (creatorUi.play != null)
            creatorUi.play.onClick.AddListener(PlayTemp);
        if (creatorUi.reload != null)
            creatorUi.reload.onClick.AddListener(ReloadJson);
        if (creatorUi.delete != null)
            creatorUi.delete.onClick.AddListener(() =>
            {
                CurrentLevel = int.Parse(creatorUi.inputLevel.text);
                Delete(CurrentLevel);
            });
        if (creatorUi.load != null)
            creatorUi.load.onClick.AddListener(() =>
            {
                CurrentLevel = int.Parse(creatorUi.inputLevel.text);
                LoadLevel(CurrentLevel);
            });
        if (creatorUi.inputLevel != null)
        {
            creatorUi.inputLevel.onEndEdit.AddListener(level =>
            {
                var levelNew = int.Parse(level);
                if (levelNew > LevelDatabase.levels.Count)
                {
                    levelNew = LevelDatabase.levels.Count + 1;
                }

                creatorUi.inputLevel.text = levelNew.ToString();
            });
        }
        if (creatorUi.save != null)
            creatorUi.save.onClick.AddListener(SaveCurrent);
        if (creatorUi.loadToTemp != null)
            creatorUi.loadToTemp.onClick.AddListener(() => LoadFromPool(temp));
        if (creatorUi.saveAsNew != null)
            creatorUi.saveAsNew.onClick.AddListener(SaveAsNew);
        if (creatorUi.loadJson != null)
            creatorUi.loadJson.onClick.AddListener(LoadJson);
        if (creatorUi.parse != null)
            creatorUi.parse.onClick.AddListener(Parse);       
        if (creatorUi.exportData != null)
            creatorUi.exportData.onClick.AddListener(Export);
        if (creatorUi.writeJson != null)
            creatorUi.writeJson.onClick.AddListener(WriteJson);
        if (creatorUi.fillDatabase != null)
            creatorUi.fillDatabase.onClick.AddListener(FillDatabase);
        if (creatorUi.undo != null)
            creatorUi.undo.onClick.AddListener(Undo);
        foreach (var creatorUiButtonPoint in creatorUi.buttonParameters)
            creatorUiButtonPoint.onClick.Subscribe(data => OnParameter(data.Action,data.PointParameter));
    }

    protected virtual void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z) && Input.GetKey(KeyCode.LeftShift))
        {
            Undo();
        }
    }

    private void ReloadJson()
    {
        // JsonLevelDatabase = null;
        // UpdateViewUI();
        // creatorUi.inputLevel.text = "0";
        // CurrentLevel = int.Parse(creatorUi.inputLevel.text);
        // LoadLevel(CurrentLevel);
        source = null;
        JsonLevelDatabase = CreateLevelDatabase();;
        if (Directory.Exists(FilePath) && JsonLevelDatabase != null)
        {
            var levelData = Directory.EnumerateFiles(FilePath, "*.json");
            JsonLevelDatabase.LoadLevelFromJson(levelData);
        }
    }

    protected virtual void SaveCurrent()
    {
        CurrentLevel = int.Parse(creatorUi.inputLevel.text);
        LoadFromPool(temp);
        SaveLevel(CurrentLevel);
    }
    
    private void Export()
    {
        LevelDatabase.Export(FileExportPath);
        creatorUi.AddLog("Exported : "+FileExportPath);
    }

    [SerializeField] protected PointParameterBase LastParameter;
    protected virtual void OnParameter(int action,PointParameterBase point)
    {
        if (action == 0)
        {
            LastParameter = point;
            foreach (var buttonPoint in creatorUi.bindCurrentPointButtons)
                buttonPoint.Change(point);
        }
    }


    public virtual void UpdateViewUI()
    {
        if (creatorUi.textTotal != null)
            creatorUi.textTotal.OnChanged(Count);
        if (creatorUi.inputLevel != null)
            creatorUi.inputLevel.text = CurrentLevel.ToString();
    }

    public virtual void LoadCurrentLevel()
    {
        LoadLevel(CurrentLevel);
        UpdateView();
    }

    public virtual void LoadJson()
    {
        source = null;
        JsonLevelDatabase = null;
    }

    public virtual void FillDatabase()
    {
    }

    public virtual void WriteJson()
    {
        //File.WriteAllText(FilePath, JsonUtility.ToJson(LevelDatabase));
    }
    
    public virtual void Parse()
    {
      //  File.WriteAllText(FileExportPath, LevelDatabase.GetParseData());
    }

    

    public virtual void Delete(int level)
    {
        if (level >= 0 && level < LevelDatabase.levels.Count)
        {
            LevelDatabase.levels.RemoveAt(level);
            var last = CurrentLevel;
            CurrentLevel = Mathf.Clamp(CurrentLevel, 0, LevelDatabase.levels.Count-1);
            if (source == null) WriteJson();
            if(last != CurrentLevel)
                LoadCurrentLevel();
            UpdateViewUI();
        }
    }

    public virtual void SaveAsNew()
    {
        LevelDatabase.levels.Add((T2) temp.Clone());
        CurrentLevel = LevelDatabase.levels.Count - 1;
        UpdateViewUI();
        if (source == null) WriteJson();
    }

    public virtual void Clear() => Create();
    public abstract void Create();

    public abstract void LoadFromPool(T2 level);
    public abstract void LoadFromScene();

    public virtual void LoadLevel(int levelID)
    {
        if (levelID <= Count)
        {
            temp = LevelDatabase.GetLevelClone(levelID - 1);
            saveUndos.Clear();
            AddUndo(temp);
            LoadTemp(temp);
            if(Registered)
                UpdateView();
        }
        else
        {
            creatorUi.AddLog("out of range");
        }
    }
    

    public virtual void LoadTemp(T2 level)
    {
       
    }

    protected override void OnKernelLoaded()
    {
        base.OnKernelLoaded();
        AddUndo(temp);
    }

    public virtual void InitTemp()
    {
        GameController.SetCurrentLevelDirty(temp);
        GameController.CreatingLevel = true;
        isEditing.Value = true;
    }

    public virtual void PlayTemp()
    {
        isEditing.Value = false;
        PlayerData.currentLevel.Value = creatorUi.InputLevelValue;
        GameController.CreatingLevel = false;
        GameController.StartLevelDirty(temp);
    }
    
    public void LoadCurrentToTemp() => LoadFromPool(temp);

    public virtual void SaveLevel(int index)
    {
        if (index > LevelDatabase.levels.Count)
            LevelDatabase.levels.Add((T2) temp.Clone());
        else
            LevelDatabase.levels[index - 1] = (T2) temp.Clone();

        UpdateViewUI();

#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(LevelDatabase);
        UnityEditor.AssetDatabase.SaveAssets();
        UnityEditor.AssetDatabase.Refresh();
#endif
    }

    public virtual void UpdateView()
    {
        if (creatorUi.inputLevel != null)
            creatorUi.inputLevel.text = CurrentLevel.ToString();
        UpdateViewUI();
    }
    

    protected override void OnEnable()
    {
        base.OnEnable();
    }

    public void SetDirty()
    {
        LevelDatabase.SetDirtyThis();
    }
}