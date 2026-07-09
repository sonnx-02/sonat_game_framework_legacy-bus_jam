using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UniRx;
using UnityEngine;

[Serializable] public class ListScriptAnimationData
{
    [ArrayElementIndexTitleAttribute2(BuiltInEnumType.AnimationSpriteSheetEnum)]
    [SerializeField] private ScriptAnimationData[] animations;

    public ScriptAnimationData[] Animations => animations;
}

public class RxAnimatorSpriteSheet : BaseRxAnimator
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [ArrayElementIndexTitleAttribute2(BuiltInEnumType.AnimationSpriteSheetEnum)]
    [SerializeField] private ScriptAnimationData[] animations;
    [SerializeField] private bool loop = true;

    [SerializeField] private ListScriptAnimationData[] listRandoms;
    public ScriptAnimationData[] Animations
    {
        get
        {
            if (listRandoms.Length > 0)
            {
                return listRandoms.TakeRandom().Animations;
            }
            else
            {
                return animations;
            }
        }
    }

    public override void LogCustom()
    {
        base.LogCustom();
        spriteRenderer.sprite = animations[index].sprites[0];
    }

    public override bool HasOut()
    {
        return false;
    }

    public override void ResetAnimation(bool setActive = false)
    {
        base.ResetAnimation(setActive);
        spriteRenderer.color = spriteRenderer.color.Fade(1);
    }

    public override bool HasIn()
    {
        return true;
    }
    protected override bool MircoCoroutine => true;

    protected override IEnumerator ThisDoAnimate_In()
    {
        ScriptAnimationData currentAnimation = Animations[index];
        if (currentAnimation.sprites.Length > 0)
        {
            float t = 0f;
            int i = 0;

            spriteRenderer.sprite = currentAnimation.sprites[i];
            while (t < currentAnimation.time)
            {
                t += Time.deltaTime;
                var eT = currentAnimation.Evaluate(t / currentAnimation.time);
                var newIndex = Mathf.RoundToInt(eT * (currentAnimation.sprites.Length - 1));
                if (newIndex != i)
                {
                    i = newIndex;
                    spriteRenderer.sprite = currentAnimation.sprites[i];
                }

                if (t >= currentAnimation.time && loop)
                    t %= currentAnimation.time;
                yield return null;
            }
        }
    }

    protected override IEnumerator ThisDoAnimate_Out()
    {
        yield return null;
    }

    protected override IEnumerator ThisDoAnimate_InThenOut() => ThisDoAnimate_In();
}

[Serializable]
public class ScriptAnimationData
{
    public Sprite[] sprites;
    public bool useCurve;
    [SerializeField] AnimationCurve curve = AnimationCurve.Linear(0, 0, 1, 1);
    public float time = 0.5f;

    public float Evaluate(float t)
    {
        if (t > 1)
            return 1;
        if (!useCurve)
            return t;
        return curve.Evaluate(t);
    }
//
//    public IEnumerator AnimEnumerator(Action<Sprite> action)
//    {
//        float t = 0f;
//        int index = 0;
//        action(sprites[index]);
//        while (t < time)
//        {
//            t += Time.deltaTime;
//            var eT = curve.Evaluate(t / time);
//            var newIndex = Mathf.RoundToInt(eT * (sprites.Length - 1));
//            if (newIndex != index)
//            {
//                index = newIndex;
//                action(sprites[index]);
//            }
//
//            yield return null;
//        }
//    }

    public int GetSpriteIndex(float t)
    {
        if(useCurve)
            return Mathf.RoundToInt(curve.Evaluate(t) * (sprites.Length - 1));
        return Mathf.RoundToInt(t * (sprites.Length - 1));
    }

    public int GetSpriteIndex(float t, AnimationCurve animationCurve)
    {
        if(useCurve)
            return Mathf.RoundToInt(curve.Evaluate(t) * (sprites.Length - 1));
        return Mathf.RoundToInt(t * (sprites.Length - 1));
    }
}