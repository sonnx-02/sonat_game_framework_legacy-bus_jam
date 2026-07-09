using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UniRx;
using UnityEngine;

public class PopMessageParent : MonoBehaviour
{
   [SerializeField] private PopMessage[] popMessages;

   public void Register()
   {
      gameObject.SetActive(false);
      foreach (var popMessage in popMessages)
      {
         popMessage.gameObject.SetActive(false);
         popMessage.PopMessageParent = this;
      }
   }

   [MyButtonInt(nameof(Test1),nameof(Test2))] public int test;

   public void Test1(int a)
   {
      Show(0,"hello there");
   }
   
   public void Test2(int a)
   {
      Show(1,"hello there");
   }
   
   public void CheckDeactive()
   {
      foreach (var popMessage in popMessages)
         if(popMessage.gameObject.activeInHierarchy)
            return;
      gameObject.SetActive(false);
   }

   public void Clear()
   {
      gameObject.SetActive(false);
   }
   
   public void Show(int  index,string message)
   {
      popMessages[index].Show(message);
   }
}
