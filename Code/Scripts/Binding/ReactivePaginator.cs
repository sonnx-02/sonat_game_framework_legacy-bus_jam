using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class ReactivePaginator : IndexBindingScript
{
    [SerializeField] private bool tween = true;
    [SerializeField] private IntBool startIndex;
    [SerializeField] private BaseIndexPropertySource propertyRetriever;
    [SerializeField] private Button[] buttons;
    [SerializeField] private IndexBindingScriptBase[] pageBindings;
    [SerializeField] private IntReactiveProperty property;

    public IntReactiveProperty Property => property;

    protected override void OnEnable()
    {
        base.OnEnable();
        CheckKernelLoaded();
        if(!IsKernelLoaded())
            return;
        if (startIndex.value2)
        {
            Current = startIndex.value;
        }
    }

    private int Current
    {
        get => property.Value;
        set => property.Value = value;
    }

    protected override void Register()
    {
        base.Register();
        for (var i = 0; i < buttons.Length; i++)
        {
            var i1 = i;
            buttons[i].onClick.AddListener(() => { OnChanged(i1); });
        }
    }

    protected override void OnValueChanged(int computedValue)
    {
        base.OnValueChanged(computedValue);
        Current = computedValue;

    }

    protected override void OnKernelLoaded()
    {
        base.OnKernelLoaded();
        property = propertyRetriever != null ? propertyRetriever.GetProperty() : new IntReactiveProperty();
        property.Subscribe(data =>
        {
            pageBindings.OnChanged(data);
        });
        pageBindings.OnChanged(Current);
    }
}