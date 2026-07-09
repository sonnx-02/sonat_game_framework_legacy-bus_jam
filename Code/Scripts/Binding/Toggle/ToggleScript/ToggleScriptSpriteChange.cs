using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleScriptSpriteChange : ToggleScript
{
    public SpriteRenderer spriteRenderer;
    public Image img;
    public Sprite @on;
    public Sprite off;

    protected override void LoadComponent()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        img = GetComponent<Image>();
    }

    protected override void Reset()
    {
        base.Reset();
        LoadComponent();

    }

    protected override void LoadOn()
    {
        base.LoadOn();
        @on = img.sprite;
    }

    protected override void LoadOff()
    {
        base.LoadOn();
        @off = img.sprite;
    }

    protected override void Trigger(bool val)
    {
        if(img != null)
            img.sprite = val ? @on : off;
        if(spriteRenderer != null)
            spriteRenderer.sprite = val ? @on : off;
    }
}
