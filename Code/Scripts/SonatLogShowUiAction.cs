using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sonat;
using UnityEngine;
using UnityEngine.UI;


public class SonatLogShowUiAction : BaseFrameworkView
{
    [SerializeField] private ShowUiAction[] actions;
    [SerializeField] private bool onEnable;

    public override void Start()
    {
        base.Start();
        foreach (var action in actions)
            action.Register();
    }


    [Serializable]
    public class ShowUiAction
    {
        [SerializeField] private string action;
        [SerializeField] private Button buttonHandler;
        [SerializeField] private BaseGetStringScript showUiName;
        [SerializeField] private BaseGetStringScript getOpenBy;
        [SerializeField] private string defaultName = "sample";
        [SerializeField] private string defaultOpenBy = "auto";
        public void Register()
        {
           buttonHandler.onClick.AddListener(() =>
           {
               BaseGameController.LogShowUi(
                   showUiName != null ? showUiName.GetString().ToSnakeCase() : defaultName ,
                   getOpenBy != null ?  getOpenBy.GetString() : defaultOpenBy
                   ,action);
           });
        }
    }
}