using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

public class HitView3D : PoolItemPoint
{
    [SerializeField] private BaseFrameworkView parentView;
    [SerializeField] private Collider hitCollider;
    [SerializeField] private IntBindingReactiveProperty value;
    [SerializeField] private IntBool valueOnRent;
    [SerializeField] private BaseBaseActionScript[] onCollide;
    public Collider HitCollider => hitCollider;
    [SerializeField] private int takes;
    [SerializeField] private IndexBindingScriptBase[] indexBindings;


    public BaseFrameworkView ParentView => parentView;

    private int _takes;
    protected override void Register()
    {
        base.Register();
        value.Init();
    }

    public override void OnRent()
    {
        base.OnRent();
        if(valueOnRent.value2)
            value.Value = valueOnRent.value;
    
    }

    private void OnEnable()
    {
        _takes = takes;
        if (takes > 0 && onCollide.Length > 0)
        {
            _disposable?.Dispose();

            0.5f.Timer(() =>
            {
                _disposable = this.OnTriggerEnterAsObservable()
                    .TakeWhile(x => _takes > 0)
                    .TakeUntilDisable(this)
                    .Subscribe(data =>
                    {
                        foreach (var baseActionScript in onCollide)
                            baseActionScript.StartAction();
                        _takes--;
                    });
            });
        }
    }

    private void OnDisable()
    {
        _disposable?.Dispose();
    }

    private IDisposable _disposable;

    public override void Return()
    {
        base.Return();
     
    }

    public IObservable<Collider> StartListenTriggerEnter()
    {
        return this.OnTriggerEnterAsObservable()
            .TakeUntilDisable(this);
    }

    public bool BoolValue
    {
        set => this.value.BoolValue = value;
    }
    
    public int Value
    {
        set => this.value.Value = value;
    }

    public void SetIndex(int characterKey)
    {
        indexBindings.OnChanged(characterKey);
    }
}
