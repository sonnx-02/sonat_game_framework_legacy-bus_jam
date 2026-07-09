using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UniRx;
using UnityEngine.Serialization;
using Object = System.Object;

public enum EquipType
{
    Switch,
    MultipleEquipped,
    Custom,
}

    
[Flags]
public enum SelectType 
{
    None = 0,
    SetCurrent = 1,
    Equip = 2,
}

public class UnlockEquipManagerDependency : BaseScreenDependency
{

    [SerializeField] private SelectType selectType = SelectType.SetCurrent | SelectType.Equip;
    [IndexAsEnum(BuiltInEnumType.UnlockEquipDataEnum)] [SerializeField]
    private int unlockEquipDataEnum;
    [SerializeField] protected ScrollRectUnlockEquipInfinitive scrollRect;
    [SerializeField] private EquipType equipType;
    [ConditionalField(nameof(equipType),new object[]{EquipType.Custom})]
    [SerializeField] private BaseEquipHandlerScript baseEquipHandlerScript;
    [SerializeField] private ProductUnlock[] unlockConditions;
    [SerializeField] private ProductUnlock[] extraConditions;

    [IndexAsEnum(nameof(UnlockEquipConditionEnum))]
    [SerializeField] private IntInt[] overrideRules;


    public int UnlockEquipDataEnum => unlockEquipDataEnum;

    public IEnumerable<ProductUnlock> Conditions
    {
        get
        {
            foreach (var unlockCondition in unlockConditions)
            {
                yield return unlockCondition;
            }
//            foreach (var unlockCondition in extraConditions)
//            {
//                yield return unlockCondition;
//            }
        }
    }


    [SerializeField] public IntReactiveProperty currentSelectIndex  = new IntReactiveProperty();
    public UnlockEquipItem GetCurrentEquip()
    {
        var find = scrollRect.activeItems.ToList().Find(x => x.Index == currentSelectIndex.Value);
        if (find == null)
        {
            scrollRect.IndicateIndex(currentSelectIndex.Value);
            find = scrollRect.activeItems.ToList().Find(x => x.Index == currentSelectIndex.Value);
        }

        return find;
    }

    public IntReactiveProperty CurrentSelectIndex => currentSelectIndex;

    protected override void Register()
    {
        base.Register();
        _playerDataEquipData = PlayerData.equipData[unlockEquipDataEnum];

        if (equipType == EquipType.Switch && _playerDataEquipData.equipped.Count == 0)
        {
            _playerDataEquipData.equipped.Add(0);
            _playerDataEquipData.availables.Add(0);
        }
        SubjectController.OnEquipDataUpdate.Subscribe(data =>
        {
            if(unlockEquipDataEnum != data.key)
                return;
            if (unlockEquipDataEnum == data.key || data.updateType == EquipDataUpdateType.Undefined)
                scrollRect.InvokeItemRentAll();

            if (unlockEquipDataEnum == data.key && data.updateType == EquipDataUpdateType.Switch)
            {
                foreach (var scrollRectActiveItem in scrollRect.activeItems)
                {
                    if (currentSelectIndex.Value != data.index && scrollRectActiveItem.Index == data.index)
                    {
                        HandlerClick(scrollRectActiveItem);
                        break;
                    }
                }
            }
        });
        
//        SubjectController.OnEquipDataUpdate.Subscribe(data =>
//        {
//            if (unlockEquipDataEnum == data.x && data.z > 0)
//            {
//                var value = PlayerData.equipData[unlockEquipDataEnum].GetSwitchValue(defaultValue);
//                scripts.OnChanged(value);
//                foreach (var baseActionScript in scriptIndex)
//                    baseActionScript.StartAction(value);
//            }
//        });

        if (overrideRules.Length > 0)
        {
            if(scrollRect == null)
                Debug.LogError("scrollRect null");
            scrollRect.OverrideRules(GetRules().ToArray());
        }
    }

    public override void LogCustomInt(int value)
    {
        base.LogCustomInt(value);
        if (value == -1)
        {
            if (overrideRules.Length > 0)
                scrollRect.OverrideRules(GetRules().ToArray());
        }
    }

    IEnumerable<Vector2Int> GetRules()
    {
        for (var i = 0; i < unlockConditions.Length; i++)
        {
            for (int j = 0; j < overrideRules.Length; j++)
            {
                if((int)(unlockConditions[i].condition) == overrideRules[j].value && overrideRules[j].value2 > 0)
                    yield return new Vector2Int(i,overrideRules[j].value2);
            }
        }
    }

    private PlayerDataEquipData _playerDataEquipData;

    protected override void Initialize()
    {
        base.Initialize();
        scrollRect.CheckRegister();
        scrollRect.onItemClick += HandlerClick;
        scrollRect.onItemRent += HandlerItemChanged;
        scrollRect.InvokeItemRentAll();
        currentSelectIndex.Value = _playerDataEquipData.GetCurrent();
    }

    
    
    private void HandlerItemChanged(UnlockEquipItem scrollItem)
    {
        var state =  GetUnlockState(scrollItem);
        if (_playerDataEquipData.CheckEquipped(scrollItem.Index))
            state |=  UnlockEquipState.Equipped;
        if(scrollItem.Index == currentSelectIndex.Value)
            state |=  UnlockEquipState.Current;
        
        scrollItem.SetState((int)state);
        scrollItem.SetCondition(unlockConditions[scrollItem.Index],unlockEquipDataEnum);
    }

    private void HandlerClick(UnlockEquipItem scrollItem)
    {
        var last = GetCurrentEquip();
      
        if((selectType & SelectType.SetCurrent) != SelectType.None)
            currentSelectIndex.Value = scrollItem.Index;

        HandlerItemChanged(last);
        if ((selectType & SelectType.Equip) != SelectType.None)
        {
            if ((scrollItem.State & UnlockEquipState.Unlocked) != UnlockEquipState.None)
            {
                switch (equipType)
                {
                    case EquipType.Switch:
                        _playerDataEquipData.Switch(scrollItem.Index);
                        scrollRect.InvokeItemRentAll();
                        SubjectController.OnEquipDataUpdate.OnNext(new EquipDataUpdate(unlockEquipDataEnum,scrollItem.Index,EquipDataUpdateType.Switch));
                        break;
                    case EquipType.MultipleEquipped:
                        _playerDataEquipData.ToggleEquipped(scrollItem.Index);
                        SubjectController.OnEquipDataUpdate.OnNext(new EquipDataUpdate(unlockEquipDataEnum,scrollItem.Index,EquipDataUpdateType.Switch));
                        break;
                    case EquipType.Custom:
                        baseEquipHandlerScript.Handler(scrollItem,unlockEquipDataEnum);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

        }
       
        HandlerItemChanged(scrollItem);
    }

    public override void OnShow()
    {
        base.OnShow();
        CheckInitialize();
        scrollRect.InvokeItemRentAll();
    }

    protected virtual UnlockEquipState GetUnlockState(UnlockEquipItem item)
    {
        if (_playerDataEquipData.CheckEquipped(item.Index))
            return UnlockEquipState.Unlocked | UnlockEquipState.Equipped;
        if (_playerDataEquipData.CheckAvailable(item.Index))
            return UnlockEquipState.Unlocked;

        if ((GetUnlockState(unlockConditions[item.Index], item.Index) & UnlockEquipState.Unlocked) == UnlockEquipState.Unlocked)
            return UnlockEquipState.Unlocked;
        if (extraConditions.Length > item.Index && (GetUnlockState(extraConditions[item.Index], item.Index) & UnlockEquipState.Unlocked) == UnlockEquipState.Unlocked)
            return UnlockEquipState.Unlocked;
        
        return UnlockEquipState.Locked;
    }

    UnlockEquipState GetUnlockState(ProductUnlock condition, int index)
    {
        switch (condition.condition)
        {
            case UnlockEquipConditionEnum.GameControllerCalculate:
                return (UnlockEquipState) BaseGameController.GetUnlockEquipUnlockState(unlockEquipDataEnum, index);
            case UnlockEquipConditionEnum.FinishLevel:
                return (PlayerData.levelProgress.Max >= condition.amount)  ? UnlockEquipState.Unlocked  : UnlockEquipState.Locked;
            case UnlockEquipConditionEnum.Shopping:
                return UnlockEquipState.Locked;
            case UnlockEquipConditionEnum.AlwaysUnlocked:
                return UnlockEquipState.Unlocked;
            case UnlockEquipConditionEnum.BuilderFinish:
                if (BaseGameController.BuilderRoot == null)
                {
                    Debug.LogError("builder root null");
                    return UnlockEquipState.Locked;
                }
                return BaseGameController.BuilderRoot.IsFinish(condition.index);
            case UnlockEquipConditionEnum.None:
                return UnlockEquipState.Locked;
            default:
                return UnlockEquipState.Locked;
        }
    }
    
}