using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class RxFadeMesh : BaseTweenScript
{
    public MeshRenderer meshRenderer;

    public float from = 0;
    public float to;
    public float time = 1;
    public float delay = 0;

    private Color startColor;
    public string colorName = "_BaseColor";

//    private Color fromColor;
//    private Color toColor;
    protected override void Register()
    {
        base.Register();
        startColor = meshRenderer.material.GetColor(colorName);
//        fromColor = new Color(toColor.r,toColor.g,toColor.b,@from);
//        toColor = new Color(toColor.r,toColor.g,toColor.b,to);
//        startColor = meshRenderer.material.SetColor("color",Color.green)
    }

    [ContextMenu("abc")]
    void SetColor()
    {
        meshRenderer.material.SetColor(colorName, Color.green);
    }

    public override void Tween()
    {
        base.Tween();
        DOTween.To(() => from, x =>
        {
            meshRenderer.material.SetColor(colorName, new Color(startColor.r,startColor.g,startColor.b,x));
        }, to, time);
//        meshRenderer.material.SetColor(colorName, fromColor);
//        transform.DOScale(to, time).SetEase(Ease.Linear).SetDelay(delay);
    }
    
    public override void ResetTween()
    {
        base.ResetTween();
        meshRenderer.material.SetColor(colorName, startColor);
    }
}
