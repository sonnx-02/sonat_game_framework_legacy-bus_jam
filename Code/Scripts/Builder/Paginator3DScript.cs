using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class Paginator3DScript : IndexBindingScriptBase
{
    [IndexAsEnum(BuiltInEnumType.SoundEnum)]
    [SerializeField] private IntFloat soundOnChange;
    [SerializeField] private Button previousButton;
    [SerializeField] private Button nextButton;
    [SerializeField] private Transform container;
    [SerializeField] private Transform[] items;
    [SerializeField] private ToggleScript[] bindPre;
    [SerializeField] private ToggleScript[] bindNext;
    [SerializeField] private UiBackgroundPaging uiBg;
    [SerializeField] private IndexBindingScript[] bindings;
    private ToggleScript[] _pre;
    private ToggleScript[] _next;
    private IntReactiveProperty _source;
    private int _currentSceneIndex;

    public void SetButton(IntReactiveProperty source, Button[] preBtn, Button[] nextBtn, ToggleScript[] pre = null,
        ToggleScript[] next = null, SwiftButton[] swiftButton = null)
    {
        _source = source;
        _pre = pre;
        _next = next;
        _currentSceneIndex = source.Value;
        container.localPosition = items[_currentSceneIndex].localPosition.FlipX();
        if (uiBg != null)
            uiBg.Set(_currentSceneIndex);
        source.Subscribe(OnValueChanged);

        foreach (var button in preBtn)
            button.onClick.AddListener(PreHandler);
        foreach (var button in nextBtn)
            button.onClick.AddListener(NextHandler);
        if (previousButton != null)
            previousButton.onClick.AddListener(PreHandler);
        if (nextButton != null)
            nextButton.onClick.AddListener(NextHandler);

        if (bindings != null && bindings.Length > 0)
            _source.Subscribe(data => bindings.OnChanged(data));

        if(swiftButton != null)
            foreach (var button in swiftButton)
            {
                button.ActionOnClick += OnClick;
                button.ActionOnSwiftLeft += PreHandler;
                button.ActionOnSwiftRight += NextHandler;
            }
    }

    private void OnClick()
    {
    }

    public override void OnChanged(int value)
    {
        _source.Value = value;
    }


    private void PreHandler()
    {
        _source.Value = Mathf.Max(0, _source.Value - 1);
    }

    private void NextHandler()
    {
        _source.Value = Mathf.Min(items.Length - 1, _source.Value + 1);
    }

    [SerializeField] private float timeMove = 0.5f;
    [SerializeField] private Ease ease = Ease.InSine;
    [SerializeField] private ProgressScript[] positionUpdate;

    public bool Instant { get; set; }
    private void OnValueChanged(int val)
    {
        bindPre.OnChanged(val > 0);
        bindNext?.OnChanged(val < items.Length - 1);
        _pre?.OnChanged(val > 0);
        _next?.OnChanged(val < items.Length - 1);

        if (_currentSceneIndex != val)
        {
            if (Instant)
            {
                OnChanged2(val);
                _currentSceneIndex = val;
            }
            else
            {
                if (positionUpdate.Length != 0)
                {
                    int total = items.Length;
                    DOVirtual.Float(_currentSceneIndex * 1f / total, val * 1f / total, timeMove, f =>
                    {
                        foreach (var progressScript in positionUpdate)
                            progressScript.OnChanged(f);
                    });
                }

                if(gameObject.activeInHierarchy)
                    soundOnChange.PlaySound();
                _currentSceneIndex = val;
                container.DOKill();
                container.DOLocalMoveX(-items[_currentSceneIndex].localPosition.x, timeMove).SetEase(ease);
                if(uiBg != null)
                    uiBg.Move(_currentSceneIndex, timeMove, ease);
            }
        }

        Instant = false;
    }

    public override void OnChanged2(int val)
    {
        bindPre.OnChanged(val > 0);
        bindNext?.OnChanged(val < items.Length - 1);
        _pre?.OnChanged(val > 0);
        _next?.OnChanged(val < items.Length - 1);

        if (_currentSceneIndex != val)
        {
            if (positionUpdate.Length != 0)
            {
                int total = items.Length;
                foreach (var progressScript in positionUpdate)
                    progressScript.OnChanged(val * 1f / total);
            }
            
            _currentSceneIndex = val;
            container.DOKill();
            container.SetLocalX(-items[_currentSceneIndex].localPosition.x);
            if(uiBg != null)
                uiBg.Move(_currentSceneIndex, timeMove, ease);
        }
        Instant = false;
    }
}