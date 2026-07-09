using System;
using System.Collections;
using System.Collections.Generic;
using Sonat;
using UnityEngine;
using UniRx;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class BuilderRoot : BaseFrameworkView
{
    [IndexAsEnum(BuiltInEnumType.UnlockEquipDataEnum)]
    [SerializeField] private int unlockEquipDataEnum;
    [SerializeField] private BuilderDatabase database;
    [SerializeField] private bool finishFirst = true;
    [SerializeField] private BuilderStage[] stages;
    [FormerlySerializedAs("bindStock0")] 
    [SerializeField] private IndexBindingScriptBase[] bindStock;
    [SerializeField] private IntBindingReactiveProperty currentStage;
    [SerializeField] private BoolBindingReactiveProperty fillable;
    [SerializeField] private BuilderUI builderUi;
    [SerializeField] private SpeedCurve speedCurve;
    [SerializeField] private Paginator3DScript paginator;

    [FormerlySerializedAs("syncStock0")]
    [IndexAsEnum(BuiltInEnumType.CustomSessionDataProperty)]
    [SerializeField] private IntBool syncStock;
    private BuilderPlayerData _builderPlayerData;
    public BuilderPlayerData BuilderPlayerData => _builderPlayerData;
    [SerializeField] private IndexBindingScript[] bindCurrentStage;

    [SerializeField]
    private int stockKey = 0;
//    [IndexAsEnum(BuiltInEnumType.CongratulationEnum)]
//    [SerializeField] private IntFloat congratulation;

    public override void LogCustom()
    {
        base.LogCustom();
        Debug.LogError(_builderPlayerData.stocks[stockKey]);
        BuilderPlayerData.DeleteFile();
    }

    public override void LogCustomInt(int value)
    {
        base.LogCustomInt(value);
        SubjectController.BuilderAddStock.OnNext(new Vector2Int(0, value));
    }

    protected override void Register()
    {
        base.Register();
        _builderPlayerData = BuilderPlayerData.Load(new BuilderPlayerData(database.stages));
        if (syncStock.value2)
            _property = syncStock.value.GetSessionDataCustomProperty();
        
        SubjectController.OnEquipDataUpdate.Subscribe(data =>
        {
            if (unlockEquipDataEnum == data.key && data.updateType == EquipDataUpdateType.Switch)
            {
                var value = PlayerData.equipData[unlockEquipDataEnum].GetSwitchValue();
                builderUi.CurrentChange(value);
            }
        });
    }

    private void OnDisable()
    {
//        Debug.LogError("disable");
    }
    
    
    public enum FocusEnum
    {
        CurrentSelectStageSwitch = 0,
        LastFinishStage = 1,
        FirstUnfinishedStage =2,
        Zero = 3,
        
        
        None = 99
    }

    [SerializeField] private FocusEnum focusEnum = FocusEnum.CurrentSelectStageSwitch;

    public void Focus(FocusEnum focus,bool instant)
    {
        paginator.Instant = instant;
        switch (focus)
        {
            case FocusEnum.CurrentSelectStageSwitch:
                currentStage.Value = PlayerData.equipData[unlockEquipDataEnum].GetSwitchValue(0);
                break;
            case FocusEnum.LastFinishStage:
                currentStage.Value = GetLastFinished();
                break;
            case FocusEnum.FirstUnfinishedStage:
                currentStage.Value = GetFirstUnfinished();
                break;
            case FocusEnum.None:
                break;
            case FocusEnum.Zero:
                currentStage.Value = 0;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
    
    protected override void OnKernelLoaded()
    {
        base.OnKernelLoaded();
//        #if UNITY_EDITOR
//        _builderPlayerData.stocks[0] = 30000;
//        #endif
        if (builderUi == null)
            builderUi = ScreenRoot.Resolve<HomeScreen>().ResolveDependency<BuilderUI>();
        
        if (finishFirst && database.stages.Length > 0)
            _builderPlayerData.stages[0] = database.stages[0].Sum();
        
        if (database == null)
            database = Kernel.GetDatabase<BuilderDatabase>();
        stages.SetActiveAll(false);

        if (database.stages.Length > _builderPlayerData.stages.Length)
        {
            List<int> list = new List<int>();
            list.AddRange(_builderPlayerData.stages);
            for (int i = 0; i < database.stages.Length - _builderPlayerData.stages.Length; i++)
                list.Add(0);
            _builderPlayerData.stages = list.ToArray();
        }
        for (var i = 0; i < Mathf.Min(database.stages.Length,stages.Length); i++)
        {
            stages[i].gameObject.SetActive(true);
            stages[i].SetTarget(database.stages[i],i, _builderPlayerData);
        }
        
        
        SubjectController.BuilderAddStock.Subscribe(data =>
        {
            _builderPlayerData.stocks[data.x] += data.y;
            StockChange(_builderPlayerData.stocks[data.x]);
            OnCurrentStageChanged();
        });

        Focus(focusEnum,true);
     
     
        
        if (builderUi != null)
        {
            currentStage.Init(builderUi.CurrentChange);
            fillable.Init(x =>
            {
                builderUi.OnFillable(x);
            });
            builderUi.build += TryToBuild;
            paginator.SetButton(currentStage,builderUi.PreButton,builderUi.NextButton,builderUi.BindPre,builderUi.BindNext,builderUi.SwiftButtons);
        }
        else
        {
            Debug.LogError("builder ui null");
        }
  
        StockChange(CurrentStock());
        currentStage.Subscribe(data => OnCurrentStageChanged());
        UpdateState();
        OnCurrentStageChanged();
    }
    
    public enum ClampType
    {
        NoRestrict,
        OnlyFinish,
        None,
    }

    public void Bind(ClampType clampType,Button[] preBtn, Button[] nextBtn, ToggleScript[] pre = null,
        ToggleScript[] next = null)
    {
        foreach (var button in preBtn)
            button.onClick.AddListener(PreHandler);
        foreach (var button in nextBtn)
            button.onClick.AddListener(NextHandler);

        currentStage.Subscribe(val =>
        {
            switch (clampType)
            {
                case ClampType.NoRestrict:
                    pre?.OnChanged(val > 0);
                    next?.OnChanged(val < database.stages.Length - 1);
                    break;
                case ClampType.OnlyFinish:
                    pre?.OnChanged(val > 0);
                    next?.OnChanged(val < GetLastFinished());
                    break;
                case ClampType.None:
                    pre?.OnChanged(false);
                    next?.OnChanged(false);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(clampType), clampType, null);
            }
       
        });

        void PreHandler()
        {
            currentStage.Value = Mathf.Max(0, currentStage.Value - 1);
        }

        void NextHandler()
        {
            currentStage.Value = Mathf.Min(database.stages.Length - 1, currentStage.Value + 1);
        }
    }
    
    
    private float _remain;
    private float _delta;


    public int CurrentStage => currentStage.Value;
    [SerializeField]
    private BaseBaseActionScript[] onBuilds;


    private int _count;
    private void TryToBuild(bool onHold)
    {
        if (!onHold)
        {
            _delta = 0;
            if (_count > 0)
            {
                _count = 0;
                var product = new Product(Quantity.EquipData,0,unlockEquipDataEnum,CurrentStage,0);
                var price = new Product(Quantity.Stock,0,stockKey);
                var trade = new Trade<Product,Product>(product, price,true);
                BaseGameController.CheckLogVirtualCurrency(trade,true);
            }
            return;
        }

        _delta += Time.deltaTime;

        if (IsCurrentFinish())
        {
            if(fillable.Value)
                UpdateFillable();
            return;
        }
        
        var total = _remain + Time.deltaTime;
        var toAdd = (int) (total * speedCurve.GetSpeed(_delta));
        var take = Build(currentStage.Value, Mathf.Min(toAdd, CurrentStock()));
        if (take > 0)
        {
            _count += take;
            _remain = total - toAdd * 1f / speedCurve.GetSpeed(_delta);
            DuongExtensions.ActionDelay(() =>
            {
                _builderPlayerData.Save();
                UpdateFillable();
            });
            foreach (var baseActionScript in onBuilds)
                baseActionScript.StartAction();
            if (IsCurrentFinish())
            {
                builderUi.OnFinishedMoment();
                stages[currentStage.Value].OnAllFinish();
                foreach (var baseActionScript in onAStageFinish)
                    baseActionScript.StartAction(currentStage.Value);
                
//                congratulation.value2.Timer(() =>
//                {
//                    ScreenRoot.TurnRayCast(true);
//                    var popup = ScreenRoot.popupManager.ShowPopup<PopupCongratulation>();
//                    popup.Congratulate(congratulation.value,currentStage.Value);
//                });
                
                UpdateFillable();
            }
        }
    }

    [SerializeField] private BaseBaseActionScript[] onAStageFinish;

    [SerializeField] private Vector2 delayAdd = new Vector2(1,0.5f);
    private int Build(int stage, int addAmount)
    {
        if (!stages[stage].IsUnlocked())
            return 0;
        if (addAmount <= 0)
            return 0;
        
        var add = delayAdd.y > 0 ? 
            stages[stage].DelayAdd(addAmount,delayAdd):
            stages[stage].Add(addAmount);
        
        if (add >= 0)
        {
            _builderPlayerData.stocks[stockKey] -= add;
            StockChange(CurrentStock());
            UpdateState();
            builderUi.TweenTo(stages[currentStage.Value].Current);
//            OnCurrentStageChanged();
            return add;
        }
        return 0;
    }
    
    public void AddStock( int i,  int value)
    {
        _builderPlayerData.stocks[i] += value;
        StockChange(CurrentStock());
        _builderPlayerData.Save();
        Debug.Log("add stock");
        UpdateFillable();
    }

    private IntReactiveProperty _property;
    private void StockChange(int value)
    {
        bindStock.OnChanged(value);
        builderUi.ChangeStock(value);
        if (syncStock.value2)
            _property.Value = value;
    }

    public int CurrentStock() => _builderPlayerData.stocks[stockKey];
    private bool IsCurrentFinish() => IsStageFinish(currentStage.Value);

    public void FocusUnfinishedStage()
    {
        var target = GetFirstUnfinished();
        if (target < database.stages.Length - 1)
            currentStage.Value = target;
        else
            currentStage.Value = 0;
    }

    private int GetFirstUnfinished()
    {
        for (var i = 0; i < database.stages.Length; i++)
        {
            if (!IsStageFinish(i))
            {
                return i;
            }
        }

        return 0;
    }
    
    private int GetLastFinished()
    {
        for (var i = database.stages.Length - 1; i >= 0; i--)
        {
            if (IsStageFinish(i))
            {
                return i;
            }
        }

        return 0;
    }

    private bool IsStageFinish(int index)
    {
        CheckRegister();
        if (index < 0 || index >= database.stages.Length)
        {
            Debug.LogError("out of range");
            return false;
        }
        return database.stages[index].values.Length > 0 && database.stages[index].Sum() <=
               _builderPlayerData.stages[index];
    }
    
    private void UpdateState()
    {
        for (var i = 0; i < database.stages.Length; i++)
            stages[i].SetState(GetState(i));
    }

    private void OnCurrentStageChanged()
    {
        builderUi.SetFinish(stages[currentStage.Value].IsFinish());
        builderUi.SetTarget(stages[currentStage.Value].Total,stages[currentStage.Value].Current);
        UpdateFillable();
        builderUi.Availables.OnChanged(database.stages[currentStage.Value].values.Sum() > 10);
        bindCurrentStage.OnChanged(currentStage.Value);
    }

    private void UpdateFillable()
    {
        DuongExtensions.ExecuteInNextFrame(() =>
        {
            fillable.Value = CurrentStock() > 0 &&  database.stages[currentStage.Value].values.Length > 0 &&  stages[currentStage.Value].IsUnlocked() &&  !stages[currentStage.Value].IsFinish();
        });
    }

    private LevelSelectState GetState(int index)
    {
        if (_builderPlayerData.stages[index] > database.stages[index].Sum())
            return LevelSelectState.Passed | LevelSelectState.Unlocked;

        if (index == 0)
            return LevelSelectState.Unlocked;
        
        if (index > 0)
        {
            if (_builderPlayerData.stages[index - 1] >= database.stages[index - 1].Sum())
                return LevelSelectState.Unlocked;
        }

        return LevelSelectState.Locked;
    }

    public void OnNext()
    {
        builderUi.OnNext();
    }

    public UnlockEquipState IsFinish(int itemIndex)
    {
        if (IsStageFinish(itemIndex))
            return UnlockEquipState.Unlocked;
        return UnlockEquipState.Locked;
    }
}