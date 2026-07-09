using System;
using System.Collections.Generic;

public class ToggleButtonEnumSelect : BaseFrameworkView
{
    public IntBindingReactiveProperty property = new IntBindingReactiveProperty();
    public ToggleEnumButton[] buttons;

    public void Setup(Action<int> action = null)
    {
        Setup(property, buttons,action);
    }
    
    private static void Setup(IntBindingReactiveProperty property, IEnumerable<ToggleEnumButton> buttons,Action<int> action = null)
    {
        property.Init();
        foreach (var toggleEnumButton in buttons)
        {
            toggleEnumButton.SetProperty(property);
            if(action!= null)
                toggleEnumButton.Button.onClick.AddListener(() => action.Invoke(property.Value));
        }
    }

    public override void LogCustom()
    {
        base.LogCustom();
        buttons = GetComponentsInChildren<ToggleEnumButton>();
       
    }

    public override void LogCustomInt(int value)
    {
        base.LogCustomInt(value);
        foreach (var toggleEnumButton in buttons)
        {
            toggleEnumButton.SetSelectValue(toggleEnumButton.gameObject.name.GetNumberFromString());
        }
    }
}
