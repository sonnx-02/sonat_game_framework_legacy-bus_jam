using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ToggleScriptEnableComponent : ToggleScript
{
    [SerializeField] private Image img;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private TMP_Text text;

    protected override void Trigger(bool val)
    {
        if (img != null)
            img.enabled = val;
        if (spriteRenderer != null)
            spriteRenderer.enabled = val;
        if (text != null)
            text.enabled = val;
    }

    protected override void LoadComponent()
    {
    }
}