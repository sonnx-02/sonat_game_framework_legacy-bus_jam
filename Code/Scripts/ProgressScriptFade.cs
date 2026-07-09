using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressScriptFade : ProgressScript
{
    [SerializeField] private Image[] target1;
    [SerializeField] private SpriteRenderer[] target2;
    [SerializeField] private Color colorOn = Color.white;
    [SerializeField] private Color colorOff = new Color(1,1,1,0);
    
    protected override void Trigger(float val)
    {
        base.Trigger(val);
        var c = Color.Lerp(colorOff, colorOn, val);
        foreach (var image in target1)
            image.color = c;
        foreach (var image in target2)
            image.color = c;
    }
}
