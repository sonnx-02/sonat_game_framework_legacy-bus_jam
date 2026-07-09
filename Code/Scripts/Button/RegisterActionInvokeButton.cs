using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RegisterActionInvokeButton : BaseFrameworkView
{
    [IndexAsEnum(BuiltInEnumType.RegisterAction)]
    [SerializeField] private int action;

    [SerializeField] private Button invokeButton;

    protected override void Register()
    {
        base.Register();
        SubjectController.RegisterAction(action, () =>
        {
            invokeButton.onClick.Invoke();
        });
    }

    public override void LogCustom()
    {
        base.LogCustom();
        invokeButton.onClick.Invoke();
    }
}
