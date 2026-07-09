using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

[System.Serializable]
public class ScreenBinding
{
    [SerializeField] private BaseScreen screen;
    [ConditionalField(nameof(screen),new object[] {null})]
    [SerializeField] private string screenName;
    [SerializeField] private ToggleScript[] toggles;
    [SerializeField] private GameObject[] onActives;
    [SerializeField] private GameObject[] offActives;
    [SerializeField] private Activator activator;


    public void OnChanged(BaseScreen scr,bool showOrHide)
    {
        if (screen == scr || screen == null && screenName.ToLower() == scr.ScreenName.ToLower())
        {
            toggles.OnChanged(showOrHide);
            onActives.SetActiveAll(showOrHide);
            offActives.SetActiveAll(!showOrHide);
            if(showOrHide)
                activator.OnActivate();
            else
                activator.OnDeactivate();
        }
    }
}
public class ScreenBindingController : BaseFrameworkView
{
    [SerializeField] private ScreenBinding[] screenBindings;

    protected override void Register()
    {
        base.Register();
        SubjectController.ScreenShowed.Subscribe(data =>
        {
            var screenName = data.GetType().ToString().ToLower();
            foreach (var screenBinding in screenBindings)
                screenBinding.OnChanged(data,true);
        });
        SubjectController.ScreenClosed.Subscribe(data =>
        {
            var screenName = data.GetType().ToString().ToLower();
            foreach (var screenBinding in screenBindings)
                screenBinding.OnChanged(data,false);
        });
    }
}
