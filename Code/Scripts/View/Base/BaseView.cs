using System;
using UniRx;
using UnityEngine;



public class BaseView : MonoBehaviour, IDisposableContainer
{
    protected static AudioManager AudioManager => RootView.rootView.audioManager;
    
    public static readonly BoolReactiveProperty Pause = new BoolReactiveProperty();


    private CompositeDisposable _disposer;

    protected static IEventAggregator EventAggregator => RootView.EventAggregator;
    CompositeDisposable IDisposableContainer.Disposer
    {
        get => _disposer ??= new CompositeDisposable();
        set => _disposer = value;
    }
    
    protected bool IsKernelLoaded() => _kernelLoadedRegistered;


    protected virtual void OnDestroy()
    {
        _disposer?.Dispose();
    }

    protected virtual void Awake()
    {
        CheckRegister();
        if (Kernel.IsReady())
            CheckKernelLoaded();
        else
            DuongKernel.EventKernelLoaded.Take(1)
                .Subscribe(_ => CheckKernelLoaded())
                .DisposeWith(this);
    }
    

    private bool _registered;
    private bool _kernelLoadedRegistered;

    public bool Registered => _registered;

    public bool KernelLoadedRegistered => _kernelLoadedRegistered;

    public virtual void CheckRegister()
    {
        if (!_registered)
        {
            Register();
            _registered = true;
        }
    }
    
    private bool _initialized;
    protected void CheckInitialize()
    {
        if (_initialized) return;
        Initialize();
        _initialized = true;
    }

    protected virtual void Initialize()
    {
        CheckRegister();
    }
    
    public virtual void CheckKernelLoaded()
    {
        if (!_kernelLoadedRegistered)
        {
            OnKernelLoaded();
            _kernelLoadedRegistered = true;
        }
    }

    protected virtual void Register()
    {
        _registered = true;
    }

    public virtual void Start()
    {
    }

    protected virtual void OnKernelLoaded()
    {
        _kernelLoadedRegistered = true;
    }
}

public interface IDisposableContainer
{
    CompositeDisposable Disposer { get; set; }
}

public static class DisposableContainerExtensions
{
    public static IDisposable DisposeWith(this IDisposable disposable, IDisposableContainer container)
    {
        if (container.Disposer.IsDisposed)
        {
            throw new Exception(string.Format("DisposeWith on {0} object is already disposed", container.GetType().Name));
        }
        container.Disposer.Add(disposable);
        return disposable;
    }
}
