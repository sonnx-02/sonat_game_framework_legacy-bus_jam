using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using UnityEngine.Serialization;

[RequireComponent(typeof(Image))]
public class RxImageFillAnimator : BaseRxAnimator
{
    [SerializeField] private float fillTime = 3;
    [SerializeField] private float delayBefore;
    [SerializeField] private float delayAfter;
    [SerializeField] private AnimationCurve animationCurve = new AnimationCurve(new Keyframe(0,0),new Keyframe(1,1));
    private Image _image;

    protected override void Register()
    {
        base.Register();
    }

    public override bool HasOut()
    {
        return false;
    }

    public override bool HasIn()
    {
        return true;
    }

    public override void ResetAnimation(bool setActive = false)
    {
        base.ResetAnimation(setActive);
        if (_image == null)
            _image = GetComponent<Image>();
        _image.fillAmount = settings.reverse ? 1 : 0;
        state = RxAnimatorState.Reset;
    }

    protected override IEnumerator ThisDoAnimate_In()
    {
        yield return new WaitForSeconds(delayBefore);

        float from = settings.reverse ? 1 : 0;
        float to = settings.reverse ? 0 : 1;
        float t = 0f;
        while (t < fillTime)
        {
            t += Time.deltaTime;
            _image.fillAmount =  Mathf.Lerp(from, to, animationCurve.Evaluate(t / fillTime));
            yield return new WaitForEndOfFrame();
        }

        _image.fillAmount = Mathf.Lerp(from, to, t / fillTime);

        yield return new WaitForSeconds(delayAfter);
    }

    protected override IEnumerator ThisDoAnimate_Out()
    {
        yield return null;
    }

    protected override IEnumerator ThisDoAnimate_InThenOut()
    {
        yield return ThisDoAnimate_In();
    }
}