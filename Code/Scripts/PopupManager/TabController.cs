using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TabController : BaseView
{
    [SerializeField] private bool disable;
    [SerializeField] private BaseIndexPropertySourceRetriever source;
    [SerializeField] private IndexPropertyChangeScriptBase[] tabs;
    
    protected override void Register()
    {
        base.Register();
        if (!disable)
        {
            var p = source.GetProperty();
            foreach (var tab in tabs)
                tab.SetProperty(p);
        }
    }
}
