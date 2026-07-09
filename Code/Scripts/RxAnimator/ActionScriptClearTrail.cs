using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionScriptClearTrail : BaseBaseActionScript
{
    [SerializeField] private TrailRenderer trailRenderer;
    [SerializeField] private bool playOnEnable;
    [SerializeField] private bool playOnDisable;
    [SerializeField] private bool allComponent;
    public override void StartAction()
    {
        base.StartAction();
        if(trailRenderer != null)
            trailRenderer.Clear();
        if (allComponent)
            foreach (var trail in GetComponentsInChildren<TrailRenderer>())
                trailRenderer.Clear();
    }

    private void OnEnable()
    {
        if(playOnEnable)
            StartAction();
    }

    private void OnDisable()
    {
        if(playOnDisable)
            StartAction();
    }

    private void Reset()
    {
        trailRenderer = GetComponentInChildren<TrailRenderer>();
    }
}
