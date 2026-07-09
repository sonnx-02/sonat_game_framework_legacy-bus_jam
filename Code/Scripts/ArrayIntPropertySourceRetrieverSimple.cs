
using UniRx;
using UnityEngine;

public class ArrayIntPropertySourceRetrieverSimple : BaseArrayIntPropertySourceRetriever
{
    [SerializeField] private IntIntSetActive[] binds;
    [SerializeField] private ArrayIntPropertySourceRetrieverModelBase model;
    protected override ArrayIntReactiveProperty Property => model.Property;

    public override void LogCustom()
    {
        base.LogCustom();
        Debug.Log(Property.GetString());
    }

    public override void LogCustomInt(int value)
    {
        base.LogCustomInt(value);
        Property.Value = new ArrayInt(new []{value,1,2});
        LogCustom();
    }

    [SerializeField] private bool subscribeNextFrame;
    protected override void OnKernelLoaded()
    {
        base.OnKernelLoaded();
        if (subscribeNextFrame)
            DuongExtensions.ExecuteInNextFrame(Bind);
        else
            Bind();

        void Bind()
        {
            Property.Subscribe(data =>
            {
                foreach (var bind in binds)
                    bind.OnChanged(data);
            });
        }
    }
    
}