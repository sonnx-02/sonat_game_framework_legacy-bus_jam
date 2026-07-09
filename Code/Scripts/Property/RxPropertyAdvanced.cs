using System;
using UnityEngine;
using UniRx;

[Serializable]
public class BoolBindingReactiveProperty : BoolReactiveProperty
{
    [SerializeField] private ToggleScript[] bindings;
    [SerializeField] private IndexBindingScript[] iBindings;
    [SerializeField] private GameObject[] actives;
    [SerializeField] private GameObject[] inactives;
    public void SetBindings(ToggleScript[] toggleScripts)
    {
        bindings = toggleScripts;
    }
    public void Init()
    {
        this.Subscribe(OnChanged);
    }
    
    public void Init(Action<bool> action)
    {
        this.Subscribe(data =>
        {
            OnChanged(data);
            action.Invoke(data);
        });
    }

    private ToggleButton[] _toggleButtons;
    public void Init(Action<bool> action,ToggleButton[] toggleButtons)
    {
        SetToggleButtons(toggleButtons);
        this.Subscribe(data =>
        {
            OnChanged(data);
            action.Invoke(data);
        });
    }

    public void SetToggleButtons(ToggleButton[] toggleButtons)
    {
        _toggleButtons = toggleButtons;
        OnChanged(Value);
    }
    public void SetToggleButtons(ToggleButton toggleButtons)
    {
        SetToggleButtons(new[] {toggleButtons});
    }

    private void OnChanged(bool data)
    {
        bindings.OnChanged(data);
        foreach (var gameObject in actives)
            gameObject.SetActive(data);
        foreach (var gameObject in inactives)
            gameObject.SetActive(!data);
        if (_toggleButtons != null)
            foreach (var toggleButton in _toggleButtons)
                toggleButton.SetProperty(this);
    }
}


[Serializable]
public class IntBindingReactiveProperty : IntReactiveProperty
{
    [SerializeField] private ToggleScript[] toggles;
    [SerializeField] private IndexBindingScriptBase[] bindings;
    [SerializeField] private Compare compare;
    [SerializeField] private GameObject[] actives;
    [SerializeField] private GameObject[] inactives;

    [SerializeField] private enum Compare
    {
        NotEqualZero,
        GreaterThanZero,
        GreaterOrEqualZero,
        EqualMinusOne
    }

    public IDisposable Init()
    {
        return this.Subscribe(data =>
        {
            foreach (var toggleScript in toggles)
                toggleScript.OnChanged(BoolValue);
            bindings.OnChanged(data);

            var toggle = GetToggle(data);
            foreach (var go in actives)
                go.SetActive(toggle);
            foreach (var go in inactives)
                go.SetActive(!toggle);
        });
    }
    
    public IDisposable Init(Action<int> action)
    {
        return this.Subscribe(data =>
        {
            foreach (var toggleScript in toggles)
                toggleScript.OnChanged(BoolValue);
            bindings.OnChanged(data);

            var toggle = GetToggle(data);
            foreach (var go in actives)
                go.SetActive(toggle);
            foreach (var go in inactives)
                go.SetActive(!toggle);
            action.Invoke(data);
        });
    }

    private bool GetToggle(int val)
    {
        switch (compare)
        {
            case Compare.NotEqualZero:
                return val != 0;
            case Compare.GreaterThanZero:
                return val > 0;
            case Compare.EqualMinusOne:
                return val == -1;
            case Compare.GreaterOrEqualZero:
                return val >= 0;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}


[Serializable]
public class FloatBindingReactiveProperty : FloatReactiveProperty
{
    [SerializeField] private ProgressScript[] progresses;


    public IDisposable Init()
    {
        if(progresses != null && progresses.Length > 0)
            return this.Subscribe(data =>
            {
                foreach (var progressScript in progresses)
                    progressScript.OnChanged(data);
            });
        return null;
    }
    
    public IDisposable Init(Action<float> action)
    {
        return this.Subscribe(data =>
        {
            foreach (var progressScript in progresses)
                progressScript.OnChanged(data);
        });
    }

}