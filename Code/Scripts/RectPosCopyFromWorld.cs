using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RectPosCopyFromWorld : ToggleScript
{
    [SerializeField] private Transform tfOn;
    [SerializeField] private Transform tfOff;
    [SerializeField] private float delay;
    [SerializeField] private bool keepX;
    [SerializeField] private bool keepY;
    [SerializeField] private RectTransform[] targets;
    
    [SerializeField] private bool setOnStart;
    
    private void Start()
    {
        if(setOnStart)
            Trigger(true);
    }

    private void StartAction(Transform tf)
    {
        if (tf == null) return;

        foreach (var rt in targets)
        {
            var anchorPos = rt.anchoredPosition;
            rt.position = tf.position;
            if (keepX)
                rt.anchoredPosition = new Vector2(anchorPos.x, rt.anchoredPosition.y);
            if (keepY)
                rt.anchoredPosition = new Vector2(rt.anchoredPosition.x, anchorPos.y);
            rt.localPosition = rt.localPosition.RemoveZ();
        }
    }

    protected override void LoadComponent()
    {
    }

    protected override void Trigger(bool val)
    {
        if (delay <= 0)
            StartAction(val ? tfOn : tfOff);
        else
            delay.Timer(() => StartAction(val ? tfOn : tfOff));
    }
}
