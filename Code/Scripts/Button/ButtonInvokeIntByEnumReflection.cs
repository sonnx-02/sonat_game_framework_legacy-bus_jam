using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using UnityEngine;

public class ButtonInvokeIntByEnumReflection : BaseButton
{
    [IntStringAsEnumKeyAttribute]
    [SerializeField] private IntString keyEnum;
    [SerializeField] private string method;
    [SerializeField] private BaseFrameworkView view;
    [SerializeField] private TMP_Text buttonText;
    [SerializeField] private bool renameOnPlay = true;
    protected override void Register()
    {
        base.Register();
        if(renameOnPlay)
            RenameButton();
    }

    protected override void OnClick()
    {
        base.OnClick();
        var m = view.GetType().GetMethod(method,BindingFlags.Instance | BindingFlags.Public| BindingFlags.NonPublic,
            Type.DefaultBinder,new[]{typeof(int)} ,null);
        if(m != null)
            m.Invoke(view,new object[] {keyEnum.value});
    }

    public override void LogCustom()
    {
        base.LogCustom();
        RenameButton();
        var m = view.GetType().GetMethod(method,BindingFlags.Instance | BindingFlags.Public| BindingFlags.NonPublic,
            Type.DefaultBinder,new[]{typeof(int)} ,null);
    }

    private void RenameButton()
    {
        if(buttonText == null) return;
        var enumList = new EnumList(keyEnum.value2);
        if (enumList.Found)
        {
            buttonText.text = enumList.GetNameString(keyEnum.value);
//            buttonText.text = method+":"+enumList.GetNameString(keyEnum.value);
        }
    }
}
