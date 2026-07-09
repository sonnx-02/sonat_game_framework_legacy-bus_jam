using System.Collections;
using System.Collections.Generic;
using Sonat;
using UnityEngine;
using UnityEngine.UI;
using UniRx;


//[RequireComponent(typeof(Button))]
public abstract class BaseButton : BaseActionScript
{
    [SerializeField] [IndexAsEnum(BuiltInEnumType.SoundEnum)] private int clickSound = 1;
    [SerializeField] protected bool handlerAtTransition;
    [SerializeField] private IndexPropertyChangeActionScript[] actionBefore;
    [SerializeField] private BaseBaseActionScript[] actionBefore2;
    #if UNITY_EDITOR
    [MyButton(nameof(MoveUp))]
    [SerializeField] private int forTest;
#endif
    public override void Rename()
    {
        base.Rename();
        MoveUp();
    }

    public Button Button { get; protected set; }

    protected override void Register()
    {
        base.Register();
        Button = GetComponent<Button>();
        if (!disableAction && Button != null)
            Button.onClick.AddListener(OnClick);
    }

    protected override void DoAction()
    {
        base.DoAction();
        OnClick();
    }

    protected virtual string IconShortCutName => GetName();
    [SerializeField] protected string customShortcutName;

    
    protected virtual void OnClick()
    {
        ShortHand.LightFeedBack();
        string shortCut = string.IsNullOrEmpty(customShortcutName) ? IconShortCutName : customShortcutName;
        BaseGameController.LogShortcut(shortCut.ToSnakeCase());
        foreach (var indexPropertyChangeActionScript in actionBefore)
            indexPropertyChangeActionScript.Invoke();
        foreach (var indexPropertyChangeActionScript in actionBefore2)
            indexPropertyChangeActionScript.StartAction();
        clickSound.PlaySound();
        if(!handlerAtTransition)
            Handler();
        else
            ScreenRoot.IoUnloadScreen(Handler).Subscribe();
    }

    protected virtual void Handler()
    {
        
    }

    public void TriggerOnClick()
    {
        OnClick();
    }

    // ReSharper disable once MemberCanBePrivate.Global
    protected void MoveUp()
    {
#if UNITY_EDITOR
        UnityEditorInternal.ComponentUtility.MoveComponentUp(this);
        UnityEditorInternal.ComponentUtility.MoveComponentUp(this);
        UnityEditorInternal.ComponentUtility.MoveComponentUp(this);
        UnityEditorInternal.ComponentUtility.MoveComponentUp(this);
        UnityEditorInternal.ComponentUtility.MoveComponentUp(this);
        if(GetComponent<CanvasRenderer>() != null)
            UnityEditorInternal.ComponentUtility.MoveComponentDown(this);
#endif
    }
}
