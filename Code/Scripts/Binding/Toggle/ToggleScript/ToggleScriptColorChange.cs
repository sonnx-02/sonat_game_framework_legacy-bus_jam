using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ToggleScriptColorChange : ToggleScript
{
    [SerializeField] private Image target;
    [SerializeField] private TMP_Text target2;
    [SerializeField] private SpriteRenderer[] target3 = new SpriteRenderer[0];
    [SerializeField] private Color whenOn = Color.white;
    [SerializeField] private Color whenOff = Color.gray;
    [SerializeField] private Image[] othersImages;
    [SerializeField] private TMP_Text[] otherTexts;
    [SerializeField] private bool turnRaycast;
    protected override void LoadComponent()
    {
        target = GetComponent<Image>();
        target2 = GetComponent<TMP_Text>();
    }

    protected override void Trigger(bool val)
    {
        var col = val ? whenOn : whenOff;
        if (target != null)
        {
            target.color = col;
            if (turnRaycast)
                target.raycastTarget = val;
        }
        if(target2 != null)
            target2.color = col;
        foreach (var spriteRenderer in target3)
            spriteRenderer.color = col;
        foreach (var item in othersImages)
            item.color = col;
        foreach (var item in otherTexts)
            item.color = col;
    }
}
