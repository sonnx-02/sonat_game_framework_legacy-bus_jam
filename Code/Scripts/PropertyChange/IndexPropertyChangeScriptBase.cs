using UniRx;
using UnityEngine;
using UnityEngine.UI;

public abstract class IndexPropertyChangeScriptBase : BaseActionScriptIndexPropertyChange
{
    [SerializeField] private IndexBindingScriptBase[] bindings;
    [SerializeField] protected ToggleScript[] toggles;
    [SerializeField] protected Button btn;

    [SerializeField] [IndexAsEnum(BuiltInEnumType.SoundEnum)]
    protected int sound;


    protected IntReactiveProperty source;
    
    protected abstract int Value { get; set; }

    /// <summary>
    ///  rarely use, special case only
    /// </summary>
    /// <param name="val"></param>
    public virtual void SetValue(int val)
    {
        Value = val;
        bindings.OnChanged(val);
        OnChanged();
    }

    public virtual void SetProperty(IntReactiveProperty p)
    {
        source = p;
        if(btn != null)
            btn.onClick.AddListener(OnClick);
        source.Subscribe(data => OnChanged());
    }

    protected virtual void OnClick()
    {
        sound.PlaySound();
        source.Value = Value;
    }

    protected override void DoAction()
    {
        base.DoAction();
        source.Value = Value;
    }

    protected virtual void OnChanged()
    {
        toggles.OnChanged(source.Value == Value);
    }

    protected override string GetName()
    {
        return "btn_property_change_to_" + Value;
    }
}
