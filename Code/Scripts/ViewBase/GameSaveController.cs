using System;
using System.IO;
using System.Linq;
using UnityEngine;
using UniRx;



public abstract class GameSaveController<T, T2, T3> : BaseGameControllerNavi where T : BaseBoardState
    where T2 : BaseLevel
    where T3 : BaseCustomGameSave
{
    [SerializeField] private bool disableSaveGame;
    protected T2 CurrentLevel { get; set; }
    protected GameSave<T> CurrentGameSave { get; set; }
    protected GameSave<T> DeletedCurrentGameSave { get; set; }

    public abstract BaseLevelDatabase<T2> BaseLevelDatabase { get; }
    private  RemoteLevelDatabase<T2> _remoteLevelDatabase;

    public override BaseLevelDatabaseClass BaseLevelDatabaseClass => BaseLevelDatabase;
    
    protected T3 CustomGameSave { get; set; }

    protected virtual string CustomGameSaveFileName => "CurrentGameData.json";
    protected virtual string CustomGameSaveFilePath => $"{Application.persistentDataPath}/{CustomGameSaveFileName}";


    public override int GetDifficulty(int i)
    {
        if (i < 0)
            return 0;
        return BaseLevelDatabase.GetLevel(i).difficulty;
    }

    protected virtual void LoadCustomGameSave(T3 whenNew)
    {
        if (File.Exists(CustomGameSaveFilePath))
        {
            string json = File.ReadAllText(CustomGameSaveFilePath);
            T3 data = JsonUtility.FromJson<T3>(json);
            if (data != null)
            {
                data.Validate();
                CustomGameSave = data;
                return;
            }
        }

        var newData = whenNew;
        newData.Validate();
        CustomGameSave = newData;
    }

    public override int GetGameSaveValue(int key)
    {
        if (CurrentGameSave != null)
            return CurrentGameSave[key];

        return 0;
    }

    protected abstract T3 CreateNewGameSave();

    public virtual void DeleteCustomGameSave()
    {
        File.Delete(CustomGameSaveFilePath);
    }

    public virtual void SaveCustomGameSave(bool reload)
    {
        Debug.Log("Save " + CustomGameSaveFileName);
        File.WriteAllText(CustomGameSaveFilePath, JsonUtility.ToJson(CustomGameSave));
    }


    [SerializeField] private bool ignoreRemoteLevel;
    protected virtual T2 GetLevel(int i)
    {
        if (!ignoreRemoteLevel && _remoteLevelDatabase != null)
        {
            var level = _remoteLevelDatabase.GetReplaceLevel(i);
            if (level != null)
                return level;
        }
        return BaseLevelDatabase.GetLevel(i);
    }

#if UNITY_EDITOR
    [SerializeField] [MyButtonInt(nameof(LoadAndStartCurrentGameSave))]
    private int testPlay1;

#endif

    protected override void Register()
    {
        base.Register();
        RegisterBoard();
        LoadCustomGameSave(CreateNewGameSave());
        #if UNITY_EDITOR
        var db = BaseLevelDatabase;
        for (var index = 0; index < db.levels.Count; index++)
        {
            var baseLevel = db.levels[index];
            if (baseLevel.IsNotValid())
            {
                Debug.LogError($"Level {index } is not valid ",db);
                Debug.Break();
            }
        }
#endif
    }

    protected override void OnKernelLoaded()
    {
        base.OnKernelLoaded();
        OnKernelLoadedBoard();
    }

    public override void OnRemoteConfigFetched(bool complete)
    {
        base.OnRemoteConfigFetched(complete);
        _remoteLevelDatabase = new RemoteLevelDatabase<T2>();
        _remoteLevelDatabase.LoadRemote();
    }

    protected abstract void OnKernelLoadedBoard();

    protected virtual void StartLevel(T2 input,int playMode = 0)
    {
        CurrentLevel = input;
        CurrentGameSave = CreateGameSaveFromCurrentLevel();
        SaveLevel();
        SaveCurrentGameSave();
        StartCurrentGameSave(playMode);
        OnStartANewGame();
    }

    public override void SetCurrentLevelDirty(BaseLevel level)
    {
        base.SetCurrentLevelDirty(level);
        CurrentLevel = level as T2;
        CurrentGameSave = CreateGameSaveFromLevel(level as T2);
    }

    protected T LastSaveState;

    protected override void AddSave()
    {
        AddSave(GetState());
    }

    public void AddSave(T state)
    {
        if (disableSaveGame) return;
        Debug.Log(nameof(AddSave));
        if (!ReadyToAddSave()) return;
        LastSaveState = state;
        CurrentGameSave.AddState(LastSaveState);
        GameActionEvent.SaveGame.OnNext();
        SubjectController.NotifyCustomGameCondition.OnNext(true);
    }

    protected virtual bool ReadyToAddSave() => !IsInTutorial();

    public override void SaveTemp()
    {
        PlayerPrefs.SetString("game_save_temp", JsonUtility.ToJson(CurrentGameSave));
        Debug.Log(JsonUtility.ToJson(CurrentGameSave));
    }

    public override void LoadTemp()
    {
#if UNITY_EDITOR
        var save = JsonUtility.FromJson<GameSave<T>>(PlayerPrefs.GetString("game_save_temp"));
        CurrentGameSave = save;
        StartCurrentGameSave();
        Debug.Log(save);
#endif
    }

    protected override void HandlerContinue()
    {
        base.HandlerContinue();
        CurrentGameSave.continued++;
    }

    protected override void HandlerWin()
    {
        base.HandlerWin();
        CurrentGameSave.won = true;
        SaveCurrentGameSave();
    }

    protected override void SetBoardFinish(bool winLose)
    {
        if (winLose)
        {
            CurrentGameSave.won = true;
            SaveCurrentGameSave();
        }
    }
   

    private Type _gameSaveKeyType;

    private Type GameSaveKeyType
    {
        get
        {
            if (_gameSaveKeyType == null)
            {
                _gameSaveKeyType = ReflectionExtension.FindTypeInDomain(BuiltInEnumType.GameSaveKey, NameSpace);
                if (_gameSaveKeyType == null)
                    Debug.LogError("not found Type " + NameSpace + "." + BuiltInEnumType.GameSaveKey);
            }

            return _gameSaveKeyType;
        }
    }

    protected override int[] GetGameSaveDisplayPropertyRemap()
    {
        var type = GameSaveKeyType;
        var values = Enum.GetValues(type).Cast<int>().ToArray();
        return values;
    }

    protected virtual void SetDefaultGameSaveKey(GameSave<T> save)
    {
//        save[(int) GameSaveKey.StartBestScore] = PlayerData.bestScore.Value;
    }

    protected virtual GameSave<T> CreateGameSaveFromCurrentLevel()
    {
        return CreateGameSaveFromLevel(CurrentLevel);
    }

    protected virtual GameSave<T> CreateGameSaveFromLevel(T2 level)
    {
        var gameSave = new GameSave<T>(GameSaveKeyType,setting.maxSaveState);
        SetDefaultGameSaveKey(gameSave);
//            gameSave[(int) GameSaveKey.StartBestScore] = PlayerData.bestScore.Value;
        gameSave.AddState(CreateStateFromLevel(level));
        return gameSave;
    }

    protected virtual GameSave<T> CreateGameSaveFromInputDirty(object input)
    {
        return CreateGameSaveFromLevel((T2) input);
    }

    // ReSharper disable once UnusedMemberInSuper.Global
    public abstract T CreateStateFromLevel(T2 level);

    protected override void StartPlayerDataLevel()
    {
        StartLevel(GetLevel(PlayerData.currentLevel.Value));

        5f.Timer(() =>
        {
            if (PlayerData.currentLevel.Value == 0 && PlayerPrefs.GetInt(FireBaseDefaultEvent.my_first_game_play.ToString()) == 0)
            {
                Kernel.Resolve<FireBaseController>().LogEvent(FireBaseDefaultEvent.my_first_game_play.ToString());
                PlayerPrefs.SetInt(FireBaseDefaultEvent.my_first_game_play.ToString(), 1);
            }
        });
    }

    public override void StartLevelDirty(object input)
    {
        CurrentLevel = (T2) input;
        var gameSave = CreateGameSaveFromInputDirty(input);
        if (gameSave != null)
            OverrideGameSave(gameSave);
        else
            Debug.LogError("input not valid");
    }

    public void OverrideGameSave(GameSave<T> gameSave, bool start = true)
    {
        CurrentGameSave = gameSave;
        if (start)
            StartCurrentGameSave();
    }

    protected virtual int[] GameSaveKeyClear => new int[0];

    public override void ClearMovesThenRestartGame()
    {
        PlayerData.logEachGame.ClearDict();
        ClearMoves();
        foreach (var i in GameSaveKeyClear)
            CurrentGameSave[i] = 0;

        if (ScreenRoot.popupManager.GetCurrentPopup() != null)
            ScreenRoot.popupManager.CloseCurrentPopupAsObservable().Subscribe(data =>
            {
                SetupState(CurrentGameSave.GetLastState());
            });
        else
            SetupState(CurrentGameSave.GetLastState());
    }

    protected override void ClearMoves()
    {
        if(CurrentLevel != null)
            CurrentGameSave.ClearMoves(CreateStateFromLevel(CurrentLevel));
        else
            CurrentGameSave.ClearMoves();
    }

    public virtual void SetupState(T state)
    {
    }

    protected override void InitGame()
    {
        LoadLevelConfigOnly(CurrentLevel);
        base.InitGame();
    }
    public virtual void LoadLevelConfigOnly(T2 level)
    {
        
    }
    
    /// <summary>
    /// to load state only, to help lever creator create level though game controller
    /// </summary>
    /// <param name="level"></param>
    /// <param name="state"></param>
    public virtual void DoLoadStateOnly(T2 level,T state)
    {
        
    }

    private static string LevelKey => "level_save";
    private static string GameKey => "game_save";

    protected override void SaveLevel()
    {
        PlayerPrefs.SetString(LevelKey, JsonUtility.ToJson(CurrentLevel));
    }

    protected override void SaveCurrentGameSave()
    {
        PlayerPrefs.SetString(GameKey, JsonUtility.ToJson(CurrentGameSave));
    }

    public abstract T GetState();

    protected override void StartCurrentGameSave(int playMode = 0)
    {
        base.StartCurrentGameSave(playMode);

//            TweenCoin(TextTweenType.Set, WaveData.coin);
//            TweenGem(TextTweenType.Set, PlayerData.gem.Value);
        Kernel.GetDatabase<GameSetting>().time.delayStartGame.Timer(StartGame);
        void StartGame()
        {
            InitGame();
            var state = CurrentGameSave.GetLastState();
            if (!InitTut())
                LoadState(state);
            AfterLoadState();

            if (Kernel.GetDatabase<GameSetting>().time.delayStartInput >= 0.05f)
                StartInput(Kernel.GetDatabase<GameSetting>().time.delayStartInput);
            else
            {
                UIDebugLog.LogError("Input will not start automatically");
            }
            AfterInitGame();
        }
    }

    protected virtual void AfterLoadState()
    {
        SubjectController.NotifyCustomGameCondition.OnNext(true);
        DuongQualityChecking.CheckDuplicateCall(nameof(AfterLoadState));
        0.1f.Timer(InvokeEquip);
    }

    public virtual void LoadState(T state)
    {
        SubjectController.NotifyCustomGameCondition.OnNext(true);
    }

    protected override void DeleteGameSave()
    {
        CurrentLevel = null;
        if (CurrentGameSave != null)
            DeletedCurrentGameSave = CurrentGameSave;
        CurrentGameSave = null;
        PlayerPrefs.DeleteKey(LevelKey);
        PlayerPrefs.DeleteKey(GameKey);
    }

    protected override bool LoadGameSave()
    {
        if (!disableSaveGame)
        {
            if (CheckGameSaveWon())
                return false;

            CurrentLevel = JsonUtility.FromJson<T2>(PlayerPrefs.GetString(LevelKey));
            if (CurrentLevel != null && !CurrentLevel.IsNotValid())
            {
                CurrentGameSave = JsonUtility.FromJson<GameSave<T>>(PlayerPrefs.GetString(GameKey)) ??
                                  CreateGameSaveFromCurrentLevel();
                return true;
            }
        }

        DeleteGameSave();
        return false;
    }

    // for level creator
 

    protected virtual bool CheckGameSaveWon()
    {
        CurrentGameSave = JsonUtility.FromJson<GameSave<T>>(PlayerPrefs.GetString(GameKey));
        if (CurrentGameSave != null && CurrentGameSave.won)
        {
            Debug.Log("Last Game Save is win, play next game");
            ValidateGameWon();
            PlayerData.currentLevel.Value++;
            PlayerData.Save();
            DeleteGameSave();
            return true;
        }

        return false;
    }

    protected virtual void ValidateGameWon()
    {
        
    }
    
    

    protected override void UpdateGameSaveDisplay()
    {
        if(CurrentGameSave == null)
            return;
        for (var i = 0; i < GameSaveDisplayProperty.Length; i++)
            GameSaveDisplayProperty[i].Value = CurrentGameSave[GameSaveDisplayRemap[i]];
    }
}