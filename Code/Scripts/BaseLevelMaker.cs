using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public abstract class BaseLevelMaker<T> : BaseFrameworkView where T : BaseLevel
{
    public static BaseLevelMaker<T> Instance;
    public IntBindingReactiveProperty currentLevel;
    public virtual string savePath => "D:/levels.json";
    
    [SerializeField] private BaseLevelDatabase<T> source;
    private BaseLevelDatabase<T> _jsonLevelDatabase;

    public virtual BaseLevelDatabase<T> LevelDatabase
    {
        get
        {
            if (source != null)
                return source;
            if (_jsonLevelDatabase == null)
            {
                _jsonLevelDatabase = ScriptableObject.CreateInstance<BaseLevelDatabase<T>>();
                if (File.Exists(savePath))
                {
                    string json = File.ReadAllText(savePath);
                    JsonUtility.FromJsonOverwrite(json, _jsonLevelDatabase);
                }
            }

            return _jsonLevelDatabase;
        }
    }


    [SerializeField] public T temp;
    [SerializeField] protected Button btnPre, btnNext, btnSave, btnLoad, btnSaveAsNew, btnDelete, btnCreate;
    [SerializeField] protected Button btnInit, btnLoadFromScene, btnPlay;
    [SerializeField] protected TMP_InputField inputLevel;
    [SerializeField] protected TMP_Text textTotal;
    
    public virtual int CurrentLevel
    {
        get => currentLevel.Value;
        set => currentLevel.Value = value;
    }

    public virtual int TotalLevel => LevelDatabase.levels.Count;
    

    protected override void Register()
    {
        base.Register();
        Instance = this;
        currentLevel.Init(level => { inputLevel.text = level.ToString(); });
        RegisterButton();
    }

    public virtual void LoadLevel(int level)
    {
        if (LevelDatabase.levels.InRange(level))
        {
            CurrentLevel = int.Parse(inputLevel.text);
            temp = (T)LevelDatabase.levels[CurrentLevel].Clone();
            LoadLevel();
        }
    }
    
    private void RegisterButton()
    {
        if (btnPre != null) btnPre.onClick.AddListener(PreLevel);
        if (btnNext != null) btnNext.onClick.AddListener(NextLevel);
        if (btnSave != null) btnSave.onClick.AddListener(Save);
        if (btnLoad != null)
            btnLoad.onClick.AddListener(() =>
            {
                CurrentLevel = int.Parse(inputLevel.text);
                LoadLevel(int.Parse(inputLevel.text));
            });
        if (btnSaveAsNew != null) btnSaveAsNew.onClick.AddListener(SaveAsNew);
        if (btnDelete != null)
            btnDelete.onClick.AddListener(() => { });
        if (btnCreate != null) btnCreate.onClick.AddListener(Create);
        if (btnInit != null) btnInit.onClick.AddListener(Init);
        if (btnLoadFromScene != null) btnLoadFromScene.onClick.AddListener(LoadFromScene);
        if (btnPlay != null) btnPlay.onClick.AddListener(PlayTemp);
        LoadLevel();
    }

    public virtual void PreLevel()
    {
        if (CurrentLevel > 0)
            CurrentLevel--;
        LoadLevel();
    }

    public virtual void NextLevel()
    {
        if (CurrentLevel <= TotalLevel - 1)
            CurrentLevel++;
        LoadLevel();
    }
    
    public void LoadFromFile()
    {
        source = null;
        _jsonLevelDatabase = null;
    }
    
    public void WriteJson()
    {
        File.WriteAllText(savePath,JsonUtility.ToJson(LevelDatabase));
    }

    public virtual void Create()
    {
        
    }
    
    public virtual void Random()
    {
        temp.Random();   
    }
    public virtual void Random2()
    {
        temp.Random2();   
    }

    public virtual void SaveAsNew()
    {
        LevelDatabase.levels.Add((T)temp.Clone());
        CurrentLevel = LevelDatabase.levels.Count - 1;
        UpdateView();
        File.WriteAllText(savePath, JsonUtility.ToJson(LevelDatabase));
    }

    protected virtual void AfterLoad()
    {
//        Init();
//        UpdateView();
//        UpdateStatus();
    }

    public virtual void UpdateView()
    {
        textTotal.text = "/" + LevelDatabase.levels.Count;
    }

    public virtual void Delete()
    {
        LevelDatabase.levels.RemoveAt(CurrentLevel);
        CurrentLevel = Mathf.Clamp(CurrentLevel, 0, LevelDatabase.levels.Count);
        File.WriteAllText(savePath, JsonUtility.ToJson(LevelDatabase));
    }

    public virtual void LoadLevel()
    {
        if (CurrentLevel < LevelDatabase.levels.Count)
            temp = (T) LevelDatabase.levels[CurrentLevel].Clone();
        else
            Create();
        AfterLoad();
    }

    public virtual void Save()
    {
        if (CurrentLevel < LevelDatabase.levels.Count)
            LevelDatabase.levels[CurrentLevel] = (T)temp.Clone();
        else
            LevelDatabase.levels.Add( (T)temp.Clone());
        UpdateView();
        File.WriteAllText(savePath, JsonUtility.ToJson(LevelDatabase));
    }

    public virtual void Save(int i)
    {
        if (i < LevelDatabase.levels.Count)
            LevelDatabase.levels[i] = (T)temp.Clone();
        else
            LevelDatabase.levels.Add( (T)temp.Clone());
        UpdateView();
        File.WriteAllText(savePath, JsonUtility.ToJson(LevelDatabase));
        CurrentLevel = i;
    }
    
    public virtual void PlayTemp()
    {
        BaseGameController.CreatingLevel = false;
        BaseGameController.StartLevelDirty(temp);
    }
    
    public virtual void Init()
    {
        BaseGameController.CreatingLevel = true;
        BaseGameController.StartLevelDirty(temp);
    }
    
    public void LoadFromScene()
    {
        temp = GetLevelFromScene();
    }

    protected abstract T GetLevelFromScene();
}