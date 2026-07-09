using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class UnLockEquipDataUpdater : BaseFrameworkView
{
    [IndexAsEnum(BuiltInEnumType.UnlockEquipDataEnum)]
    [SerializeField] private int unlockEquipDataEnum;
    [SerializeField] private int defaultValue;
    [SerializeField] private IndexBindingScriptBase[] switchValueBindings;
    [SerializeField] private ActionByUpdateType[] byUpdateActions;
    [SerializeField] private BaseBaseActionScript[] onUpdateIntActions;
    [SerializeField] private BaseBaseActionScript[] onStartIntActions;
    
    

    [Serializable]
    protected class ActionByUpdateType
    {
        [SerializeField] private EquipDataUpdateType updateType;
        [SerializeField] private BaseBaseActionScript[] intActions;

        public void StartAction(EquipDataUpdateType type, int switchValue)
        {
            if(updateType == type)
                intActions.StartAction(switchValue);
        }
    }

    protected override void OnKernelLoaded()
    {
        base.OnKernelLoaded();
        var switchValue = PlayerData.equipData[unlockEquipDataEnum].GetSwitchValue(defaultValue);
        switchValueBindings.OnChanged(switchValue);
        onStartIntActions.StartAction(switchValue);
        SubjectController.OnEquipDataUpdate.Subscribe(data =>
        {
            if (unlockEquipDataEnum == data.key)
            {
                var value = PlayerData.equipData[unlockEquipDataEnum].GetSwitchValue(defaultValue);
                onUpdateIntActions.StartAction(value);
                switchValueBindings.OnChanged(value);
                
                foreach (var actionByUpdateType in byUpdateActions)
                    actionByUpdateType.StartAction(data.updateType ,value);
            }
        });
    }
}
