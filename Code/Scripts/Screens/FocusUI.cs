using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class BaseFocusLocation : BaseFrameworkView
{
    public Vector2 positionAdjust;
    public float size = 1;
    public Button button;
    public BasePointerScript pointerScript;
    [SerializeField] private ToggleScript[] onActives;
    [SerializeField] private BaseBaseActionScript[] onActiveScripts;

    public float autoClose;

    public string message;
    
    private bool _last;
    

    public void Activate(bool b)
    {
        if (b)
            ScreenRoot.focusUi.StartAction(this);
        if (_last != b)
        {
            onActives.OnChanged(b);
            foreach (var baseBaseActionScript in onActiveScripts)
                baseBaseActionScript.StartAction(b);
        }
        _last = b;
    }
}

public class FocusUI : BaseBaseActionScript
{
    [SerializeField] private BaseFocusLocation location;

    [SerializeField] private Button fakeButton;
    // Start is called before the first frame update

    [SerializeField] private BaseRxAnimator[] animators;
    [SerializeField] private BaseBaseActionScript[] onIn;
    [SerializeField] private BaseBaseActionScript[] onOut;
    [SerializeField] private float delayClose = 0.5f;
    [SerializeField] private GameObject container;
    [SerializeField] private Transform holeSize;

    public override void StartAction()
    {
        base.StartAction();
        StartAction(location);
    }

    protected override void Register()
    {
        base.Register();
        container.SetActive(false);
    }

    private Button _current;
    private BasePointerScript _current2;
    [SerializeField] private BaseBaseActionScript[] textChanges;

    private IDisposable _autoDisposse;

    public void StartAction(BaseFocusLocation baseFocusLocation)
    {
        location = baseFocusLocation;
        if (location.button != null && location.pointerScript != null)
            Debug.LogError("invalid", gameObject);
        if (location.button != null)
            StartAction(location.button, location.positionAdjust, location.size);
        else if (location.pointerScript != null)
            StartAction(location.pointerScript, location.positionAdjust, location.size);
        textChanges.StartAction(baseFocusLocation.message);
        // DTODO describe text by BaseLocation
        if (baseFocusLocation.autoClose >= 1)
            _autoDisposse = Observable.Timer(TimeSpan.FromSeconds(baseFocusLocation.autoClose)).Subscribe(data =>
            {
                ScreenRoot.LocationDispose(baseFocusLocation);
                Close();
            });
    }


    private void StartAction(Button button, Vector2 adjust, float size = 1,float delay = 0)
    {
        if (delay > 0)
        {
            ScreenRoot.TurnRayCast(false);
            delay.Timer(() =>
            {
                ScreenRoot.TurnRayCast(true);
                Focus(button, adjust, size);
            });
        }
        else
            Focus(button, adjust, size);
    }
    
    private void StartAction(BasePointerScript button, Vector2 adjust, float size = 1,float delay = 0)
    {
        if (delay > 0)
        {
            ScreenRoot.TurnRayCast(false);
            delay.Timer(() =>
            {
                ScreenRoot.TurnRayCast(true);
                Focus(button, adjust, size);
            });
        }
        else
            Focus(button, adjust, size);
    }
    

    private void Focus(BasePointerScript button, Vector2 adjust, float size)
    {
        if (size <= 0.01f)
            size = 1;
        holeSize.transform.localScale = new Vector3(size, size, size);
        container.SetActive(true);
        transform.position = location.transform.position;
        (transform as RectTransform).anchoredPosition += adjust;
        button.OnClick += () =>
        {
            Close();
            button.ClearOnClick();
            ScreenRoot.LocationClick(location);
            _autoDisposse?.Dispose();
        };
        fakeButton.gameObject.SetActive(false);
        foreach (var baseRxAnimator in animators)
            baseRxAnimator.Animator_In();
        onIn.StartAction();
    }


    private void Focus(Button button, Vector2 adjust, float size)
    {
        if (size <= 0.01f)
            size = 1;
        holeSize.transform.localScale = new Vector3(size, size, size);
        container.SetActive(true);
        transform.position = location.transform.position;
        (transform as RectTransform).anchoredPosition += adjust;
        _current = button;
        fakeButton.gameObject.SetActive(true);
        fakeButton.onClick.RemoveAllListeners();
        fakeButton.onClick.AddListener(() =>
        {
            _current.onClick.Invoke();
            Close();
            ScreenRoot.LocationClick(location);
            _autoDisposse?.Dispose();
        });

        foreach (var baseRxAnimator in animators)
            baseRxAnimator.Animator_In();
        onIn.StartAction();
    }

    private void Close()
    {
        foreach (var baseRxAnimator in animators)
            baseRxAnimator.Animator_Out();
        onOut.StartAction();
        var parallel = Observable.WhenAll(animators.ToList().Select(x => x.IO_AnimatorOut()).ToArray())
            .Subscribe(l => { delayClose.Timer(() => container.SetActive(false)); });
        textChanges.StartAction(string.Empty);
    }

    public override void LogCustom()
    {
        base.LogCustom();
        transform.position = location.transform.position;
    }
}