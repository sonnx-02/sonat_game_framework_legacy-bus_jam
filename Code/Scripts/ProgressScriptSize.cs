using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressScriptSize : ProgressScript
{
    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private Vector2 from = new Vector3(0, 100);
    [SerializeField] private Vector2 to = new Vector3(1000, 100);
    
    protected override void Trigger(float val)
    {
        base.Trigger(val);
        rectTransform.sizeDelta = Vector2.Lerp(@from, to, val);
    }
}
