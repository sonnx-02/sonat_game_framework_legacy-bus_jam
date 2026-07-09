using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class LevelSelectorBase : BaseScreenDependency
{
    [SerializeField] protected PoolCollection<LevelSelect> selects;
//    [SerializeField] private Transform container;

    protected override void Register()
    {
        base.Register();
        selects.Init(-1);
        selects.ViewFirst(0);
    }
}
