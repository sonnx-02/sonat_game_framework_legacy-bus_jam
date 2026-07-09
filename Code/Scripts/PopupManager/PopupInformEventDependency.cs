using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupInformEventDependency : BasePopupDependency
{
    [IndexAsEnum(BuiltInEnumType.EventEnum)]
    [SerializeField] private int[] eventChecks;
    
    [SerializeField] private IndexBindingScript[] enumBinds;
    [IndexAsEnum(BuiltInEnumType.InformEnum)]
    [SerializeField] private IntBoolGameObject[] setActives;  

    [IndexAsEnum(nameof(EventDayType))]
    [SerializeField] private int eventType;
    private EventDayPlayerData Data => StaticDataController.EventDays[eventType];
    
    public void Set(int navigateEnum)
    {
        enumBinds.OnChanged(navigateEnum);
        foreach (var item in setActives)
            item.value3.SetActive(item.value2 ? item.value != navigateEnum : item.value == navigateEnum);
    }

    public override bool ReadyToShow()
    {
        if (Data != null && !Data.inform)
            foreach (var eventCheck in eventChecks)
            {
                if (eventCheck == Data.eventKey)
                {
                    Set(Data.eventKey);
                    Data.inform = true;
                    Data.Save();
                    return true;
                }
            }
        return base.ReadyToShow();
    }
    
    public override void OnShow()
    {
        base.OnShow();
    }
}
