using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class IndexBindingScriptPaging : IndexBindingScript
{
    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private int defaultSet;
    [SerializeField] private BoolFloat size;
    [SerializeField] private float duration = 0.35f;
    private void OnDisable()
    {
        rectTransform.SetAnchorX(0);
        _last = 0;
    }

    private void Reset()
    {
        if (rectTransform == null)
            rectTransform = GetComponent<RectTransform>();
    }


    private int _last = 0;

    [IndexAsEnum(BuiltInEnumType.SoundEnum)]
    [SerializeField] private int soundOnChanged;
    protected override void OnValueChanged(int computedValue)
    {
        base.OnValueChanged(computedValue);
        if (_last != computedValue)
        {
            var s = !size.value ? ScreenRoot.CanvasSize.x : size.value2;
            _last = computedValue;
            if (gameObject.activeInHierarchy)
            {
                rectTransform.DOAnchorPosX(-computedValue * s, duration);
                if(gameObject.activeInHierarchy)
                    soundOnChanged.PlaySound();
            }
            else
            {
                rectTransform.SetAnchorX(-computedValue * s);
                if(gameObject.activeInHierarchy)
                    soundOnChanged.PlaySound();
            }
        }
    }
}
