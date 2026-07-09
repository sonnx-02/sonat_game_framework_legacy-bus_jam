using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BasePoolItemGameView<T1, T2> : PoolItem where T1 : BaseGameController where T2 : ScriptableObject
{
    protected static T1 GameController;
    protected static T2 CurrentGameSetting;
    [SerializeField] private BaseBaseActionScript[] onRentActions;
    [SerializeField] private BaseBaseActionScript[] onReturnActions;
    [SerializeField] private BaseBaseActionScript[] onDestroyActions;

    public static void Set(T1 gc, T2 setting)
    {
        BaseCurrentGameView<T1,T2>.Set(gc,setting);
        GameController = gc;
        CurrentGameSetting = setting;
    }

    public override void Return()
    {
        base.Return();
        onReturnActions.StartAction();
    }

    public override void OnRent()
    {
        base.OnRent();
        onRentActions.StartAction();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        onDestroyActions.StartAction();
    }
}

public abstract class BaseCurrentGameView<T1, T2> : BaseFrameworkView where T1 : BaseGameController where T2 : ScriptableObject
{
    protected static T1 GameController;
    protected static T2 CurrentGameSetting;

    public static void Set(T1 gc, T2 setting)
    {
        GameController = gc;
        CurrentGameSetting = setting;
    }
}