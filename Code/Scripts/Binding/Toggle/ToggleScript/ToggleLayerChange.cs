using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleLayerChange : ToggleScript
{
    [SerializeField] private string layerOn;
    [SerializeField] private string layerOff;
    protected override void LoadComponent()
    {
        
    }

    private bool _check;
    private int _layerOn;
    private int _layerOff;
    protected override void Trigger(bool val)
    {
        if (!_check)
        {
            _check = true;
            _layerOn= LayerMask.NameToLayer(layerOn);
            _layerOff = LayerMask.NameToLayer(layerOff);
        }

        gameObject.layer = val ? _layerOn : _layerOff;
    }
}
