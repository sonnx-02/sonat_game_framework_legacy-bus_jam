using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestItemView : GenericQuestItemView
{
   [IndexAsEnum(BuiltInEnumType.QuestEnum)]
   [SerializeField] private int key;

   protected override void OnKernelLoaded()
   {
      base.OnKernelLoaded();
      var gameSetting = Kernel.GetDatabase<GameSetting>();
       foreach (var item in gameSetting.dailyQuestItems)
           if (item.Key == key)
           {
               Setup(item);
               break;
           }
   }
}
