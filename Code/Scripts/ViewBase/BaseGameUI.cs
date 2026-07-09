using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseGameUI : BaseScreenDependency
{
    [SerializeField] private GameControllerBinding gcBindingUI;
    [SerializeField] private IndexBindingScriptBase[] gameStateBindings;
    [SerializeField] private CooldownProgressScript comboProgress;
    [SerializeField] private ObservableView finishingObservableLose;
    [SerializeField] private ObservableView finishingObservableWin;

    public GameControllerBinding BindingUi => gcBindingUI;

    public ObservableView FinishingObservableLose => finishingObservableLose;
    public ObservableView FinishingObservableWin => finishingObservableWin;
    public CooldownProgressScript ComboProgress => comboProgress;

    public IndexBindingScriptBase[] GameStateBindings => gameStateBindings;

    public virtual void OnStartGame()
    {
        if (finishingObservableLose != null)
            finishingObservableLose.ResetView();
        if (finishingObservableLose != null)
            finishingObservableWin.ResetView();
    }

    protected override void Register()
    {
        base.Register();
    }
}