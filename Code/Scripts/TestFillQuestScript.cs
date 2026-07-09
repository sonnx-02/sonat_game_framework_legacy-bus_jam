using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestFillQuestScript : BaseFrameworkView
{
   [IndexAsEnum(BuiltInEnumType.QuestEnum)]
   [SerializeField] private IntInt quest;

   public override void LogCustom()
   {
      base.LogCustom();
      FillQuest(quest.value,quest.value2);
      var items = FindObjectsByType<GenericQuestItemView>(FindObjectsSortMode.InstanceID);
      foreach (var genericQuestItemView in items)
      {
         genericQuestItemView.UpdateStatus();
      }
   }
}
