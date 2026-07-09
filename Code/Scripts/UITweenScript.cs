using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public enum TweenTarget
{
    WorldPosition,
    LocalPosition,
    AddPosition,
    ScreenPosition,
    StayDoNothing,
}

public enum TweenFade
{
    NoFade,
    FadeIn,
    FadeOut,
}

[Serializable]
public class TweenData
{
    public TweenTarget tweenTarget;
    [ConditionalField(nameof(tweenTarget),new object[]{TweenTarget.ScreenPosition})]
    public ScreenPosition screenPosition;
    [ConditionalField(nameof(tweenTarget),new object[]{TweenTarget.AddPosition,TweenTarget.LocalPosition,TweenTarget.WorldPosition})]
    public Vector2 position;
    public float time;
    public float delayBefore;
    public float delayAfter;

    public TweenFade fade;
}

public abstract class InvokeScript : MonoBehaviour
{
    [MyButton(nameof(ResetScript),nameof(StartScript))]
    [SerializeField] private bool test;
    
    public abstract void ResetScript();

    public virtual void StartScript()
    {
        ResetScript();
    }
}

public class UITweenScript : InvokeScript
{
    [SerializeField] private TweenData[] data;

    [SerializeField] private Image img;
    public bool visibleOnReset;
    public override void ResetScript()
    {
        if (visibleOnReset && img != null)
            img.color = new Color(img.color.r,img.color.g,img.color.b,1);
    }

    public override void StartScript()
    {
        base.StartScript();
        Observable.FromCoroutine(Tween).TakeUntilDisable(this).Subscribe(_ =>
        {
            gameObject.SetActive(false);
        });
    }
  
    IEnumerator Tween()
    {
        foreach (var tweenData in data)
        {
            if (tweenData.delayBefore > 0)
                yield return new WaitForSeconds(tweenData.delayBefore );
            
            switch (tweenData.fade)
            {
                case TweenFade.NoFade:
                    break;
                case TweenFade.FadeIn:
                    if (img != null)
                        img.DOFade(1, tweenData.time);
                    break;
                case TweenFade.FadeOut:
                    if (img != null)
                        img.DOFade(0, tweenData.time);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            switch (tweenData.tweenTarget)
            {
                case TweenTarget.WorldPosition:
                    yield return transform.DOMove(tweenData.position,tweenData.time).WaitForCompletion();
                    break;
                case TweenTarget.LocalPosition:
                    yield return transform.DOLocalMove(tweenData.position,tweenData.time).WaitForCompletion();
                    break;
                case TweenTarget.AddPosition:
                    yield return transform.DOLocalMove(tweenData.position,tweenData.time).SetRelative().WaitForCompletion();
                    break;
                case TweenTarget.ScreenPosition:
                    yield return transform.DOMove(
                        RootView.rootView.screenRoot.GetPosition(tweenData.screenPosition).position,
                        tweenData.time).WaitForCompletion();
                    break;
                case TweenTarget.StayDoNothing:
                    yield return new WaitForSeconds(tweenData.time);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            if (tweenData.delayAfter > 0)
            {
                yield return new WaitForSeconds(tweenData.delayBefore );
            }
        }
    }
}
