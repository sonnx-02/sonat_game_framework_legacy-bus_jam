using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogNavigate : BaseDialog
{
    [SerializeField] private IndexBindingScriptBase[] bindings;
    
    protected override void Register()
    {
        base.Register();
    }
    
    public DialogNavigate ShowNavigate(int keyEnum)
    {
        base.Show();
        bindings.OnChanged(keyEnum);
        return this;
    }
}
