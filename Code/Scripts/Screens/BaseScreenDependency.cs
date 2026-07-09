using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseScreenDependency : BaseFrameworkView
{
    [SerializeField] private BaseBaseActionScript[] onShows;
    [SerializeField] private BaseBaseActionScript[] onHides;

    public virtual void OnShow()
    {
        onShows.StartAction();
    }

    public virtual void OnHide()
    {
        onHides.StartAction();
    }
}
