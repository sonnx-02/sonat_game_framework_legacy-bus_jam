using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class RxFadeSpriteRenderer : BaseTweenScript
{
    public SpriteRenderer spriteRenderer;

    public float from = 1;
    public float to;
    public float time = 1;
    public float delay = 0;
    private Color _startColor;
    public Ease ease;

//    private Color fromColor;
//    private Color toColor;
    protected override void Register()
    {
        base.Register();
        _startColor = spriteRenderer.color;
    }

    public override void Tween()
    {
        base.Tween();
        DOTween.To(() => from,
                x => { spriteRenderer.color = new Color(_startColor.r, _startColor.g, _startColor.b, x); }, to, time)
            .SetDelay(delay)
            .SetEase(ease);
    }
    
    public override void ResetTween()
    {
        base.ResetTween();
        spriteRenderer.color = _startColor;
    }
}
