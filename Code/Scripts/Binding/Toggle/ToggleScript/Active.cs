using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public abstract class BaseActive
{
    public bool active = true;
    public bool twoWays = false;
    public abstract void Activate();
    public abstract void ActivateReverse();
}

[Serializable]
public struct Activator
{
    [SerializeField] private ActiveToggleScript[] activeToggle; // anim In,Out when screen show/close
    [SerializeField] private ActiveGameObject[] activeGo; // anim In,Out when screen show/close
    [SerializeField] private ActiveEnableComponent[] activeComponent; // anim In,Out when screen show/close
    [SerializeField] private ActiveIndexScript[] activeIndex; // anim In,Out when screen show/close
    [SerializeField] private ActiveResetRx[] rsAnims;
    [SerializeField] private ActiveSetAtInRx[] setAtInAnims;
    
    public void OnActivate()
    {
        foreach (var activeToggleScript in activeToggle)
            activeToggleScript.Activate();
        foreach (var activeToggleScript in activeGo)
            activeToggleScript.Activate();
        foreach (var activeToggleScript in activeComponent)
            activeToggleScript.Activate();
        foreach (var activeToggleScript in activeIndex)
            activeToggleScript.Activate();
        foreach (var activeToggleScript in rsAnims)
            activeToggleScript.Activate();
        foreach (var activeToggleScript in setAtInAnims)
            activeToggleScript.Activate();
    }
    public void OnDeactivate()
    {
        foreach (var activeToggleScript in activeToggle)
            activeToggleScript.ActivateReverse();
        foreach (var activeToggleScript in activeGo)
            activeToggleScript.ActivateReverse();
        foreach (var activeToggleScript in activeComponent)
            activeToggleScript.ActivateReverse();
        foreach (var activeToggleScript in rsAnims)
            activeToggleScript.ActivateReverse();
        foreach (var activeToggleScript in setAtInAnims)
            activeToggleScript.ActivateReverse();
    }
}

[Serializable]
public class ActiveToggleScript : BaseActive
{
    public ToggleScript script;

    public override void Activate() => script.OnChanged(active);
    public override void ActivateReverse()
    {
        if(twoWays)
            script.OnChanged(!active);
    }
}

[Serializable]
public class IntActiveGameObject : BaseActive
{
    public ToggleScript script;

    public override void Activate() => script.OnChanged(active);
    public override void ActivateReverse()
    {
        if(twoWays)
            script.OnChanged(!active);
    }
}


[Serializable]
public class ActiveGameObject : BaseActive
{
    public GameObject go;
    
    public override void Activate() => go.SetActive(active);
    public override void ActivateReverse()
    {
        if(twoWays)
            go.SetActive(!active);
    }
}

[Serializable]
public class ActiveResetRx : BaseActive
{
    public BaseRxAnimator go;

    public override void Activate()
    {
        if (active)
            go.ResetAnim();
    }

    public override void ActivateReverse()
    {
        if (twoWays && !active)
            go.SetAtIn();
    }
}

[Serializable]
public class ActiveSetAtInRx : BaseActive
{
    public BaseRxAnimator go;

    public override void Activate()
    {
        if (active)
            go.SetAtIn();
    }

    public override void ActivateReverse()
    {
        if (twoWays && !active)
            go.ResetAnim();
    }
}




[Serializable]
public class ActiveEnableComponent : BaseActive
{
    public MonoBehaviour mono;

    public override void Activate() => mono.enabled = active;
    public override void ActivateReverse()
    {
        if(twoWays)
            mono.enabled = !active;
    }
}

[Serializable]
public class ActiveIndexScript
{
    public int active;
    public IndexBindingScriptBase script;
    public void Activate() => script.OnChanged(active);
}