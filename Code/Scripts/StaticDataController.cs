using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using HC.AI;
using UnityEngine;

public static class StaticDataController
{
    public static readonly EventDayPlayerData[] EventDays = new EventDayPlayerData[10];

    public static void LoadEventPlayerData(int eventType, int level, Action<int> onLoadedEvent)
    {
        if (level < RemoteConfigKey.start_level_have_event.GetValueInt())
        {
            onLoadedEvent.Invoke(-1);
            return;
        }

        var gs = Kernel.GetDatabase<GameSetting>();
        if (eventType == (int) EventDayType.Default)
            EventDays[0] = Load(EventDays[0], level, onLoadedEvent, gs.existEvents, gs.events, "Current");
        else if (eventType == (int) EventDayType.Special)
            EventDays[1] = Load(EventDays[1], level, onLoadedEvent, gs.existEvents2, gs.events2, "Special");
        else
            Debug.LogError("not handler for " + eventType);
    }

    private static EventDayPlayerData Load(EventDayPlayerData current, int level, Action<int> onLoadedEvent,
        int[] exists, IEnumerable<EventDescriptor> events, string prefix)
    {
        if (level < RemoteConfigKey.start_level_have_event.GetValueInt())
        {
            onLoadedEvent.Invoke(-1);
            return null;
        }

        if (current != null && !current.Validate())
        {
            current.DeleteFile();
            current = null;
        }

        if (current == null)
            current = EventDayPlayerData.Load(prefix + nameof(EventDays)) ?? IfHavingSpecialEvent();
        else
            Debug.LogError("Is On SpecialEvent " +
                           BuiltInEnumType.EventEnum.GetName(current.EventKey));


        onLoadedEvent.Invoke(current?.eventKey ?? -1);
        return current;

        EventDayPlayerData IfHavingSpecialEvent()
        {
            var epoch = DuongExtensions.GetEpochDayOfYear();
            foreach (var descriptor in events)
            {
#if UNITY_EDITOR
                if (DuongExtensions.IsInDate(epoch, descriptor.startEpochDate, descriptor.activeDays) &&
                    !exists.Contains(descriptor.eventKey))
                    Debug.LogError("exist event doesn't contains " + descriptor.eventKey);
#endif

                if (DuongExtensions.IsInDate(epoch, descriptor.startEpochDate, descriptor.activeDays) &&
                    exists.Contains(descriptor.eventKey))
                {
                    var data = new EventDayPlayerData(descriptor)
                    {
                        startEpochDate = descriptor.startEpochDate
                    };
                    data.Validate();
                    return data;
                }
            }

            return null;
        }
    }
}