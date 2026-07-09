using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class OnEventTextUpdater : BindingToStringTextChange
{
    [SerializeField] [IndexAsEnum(BuiltInEnumType.TextUpdateKey)]
    private int key;

    protected override void Register()
    {
        base.Register();
        SubjectController.TextUpdater.Subscribe(data => OnChanged(data.message));
    }

//    private void OnEnable()
//    {
//        
//    }
//
//    private void OnDisable()
//    {
//        
//    }
}