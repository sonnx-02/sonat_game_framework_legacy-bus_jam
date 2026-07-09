using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RegisterActionBindingScript : BaseFrameworkView
{
    [SerializeField] private IndexBindingScript[] scripts;
    
    [IndexAsEnum(BuiltInEnumType.RegisterAction)]
    [SerializeField] private IntInt[] keyValueActions;

    protected override void Register()
    {
        base.Register();
        foreach (var keyValueAction in keyValueActions)
        {
            SubjectController.RegisterAction(keyValueAction.value, () =>
            {
                scripts.OnChanged(keyValueAction.value2);
            });
        }
    }
}
