using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RxAnimatorCameraSize : BaseRxAnimator
{
    [SerializeField] private Camera cam;
    [SerializeField] private float from = 1;
    [SerializeField] private float to = 2;
    [SerializeField] private float timeIn = 1;
    [SerializeField] private float timeOut = 1;
    [SerializeField] private AnimationCurve curveIn  = AnimationCurve.Linear(0.0f, 0.0f, 1.0f, 1.0f);
    [SerializeField] private AnimationCurve curveOut  = AnimationCurve.Linear(0.0f, 0.0f, 1.0f, 1.0f);


    private void Reset()
    {
        if (cam == null)
        {
            cam = GetComponent<Camera>();
            if (cam != null)
            {
                if (cam.orthographic)
                    from = cam.orthographicSize;
                else
                    from = cam.fieldOfView ;
                
                to = from  + 1;
            }
        }
    }

    public override bool HasOut()
    {
        return true;
    }

    public override bool HasIn()
    {
        return true;
    }

    public override void SetAtAnimationIn()
    {
        base.SetAtAnimationIn();
        if (cam.orthographic)
            cam.orthographicSize = @to;
        else
            cam.fieldOfView = @to;
    }

    public override void ResetAnimation(bool setActive = false)
    {
        base.ResetAnimation(setActive);
        if (cam.orthographic)
            cam.orthographicSize = @from;
        else
            cam.fieldOfView = @from;
    }

    protected override IEnumerator ThisDoAnimate_In()
    {
       
            if (settings.DelayBeforeIn > 0)
                yield return new WaitForSeconds(settings.DelayBeforeIn);
            if (cam.orthographic)
                cam.orthographicSize = @from;
            else
                cam.fieldOfView = @from;
            float t = 0;
            settings.animInSound.PlaySound();
            while (t < timeIn)
            {
                t += Time.deltaTime;
                yield return null;
                if (cam.orthographic)
                    cam.orthographicSize = Mathf.Lerp(@from, to, curveIn.Evaluate(t / timeIn));
                else
                    cam.fieldOfView = Mathf.Lerp(@from, to, curveIn.Evaluate(t / timeIn));
            }
    }

    protected override IEnumerator ThisDoAnimate_Out()
    {

        float f = cam.orthographic ? cam.orthographicSize : cam.fieldOfView;
        float t = 0;
        settings.animOutSound.PlaySound();
        while (t < timeOut)
        {
            t += Time.deltaTime;
            yield return null;
            if (cam.orthographic)
                cam.orthographicSize = Mathf.Lerp(@f, @from, curveOut.Evaluate(t / timeOut));
            else
                cam.fieldOfView = Mathf.Lerp(@f, from, curveOut.Evaluate(t / timeOut));
        }
    }

    protected override IEnumerator ThisDoAnimate_InThenOut()
    {
        
        if (settings.DelayBeforeIn > 0)
            yield return new WaitForSeconds(settings.DelayBeforeIn);
        if (cam.orthographic)
            cam.orthographicSize = @from;
        else
            cam.fieldOfView = @from;
        float t = 0;
        while (t < timeIn)
        {
            t += Time.deltaTime;
            yield return null;
            if (cam.orthographic)
                cam.orthographicSize = Mathf.Lerp(@from, to, curveIn.Evaluate(t / timeIn));
            else
                cam.fieldOfView = Mathf.Lerp(@from, to, curveIn.Evaluate(t / timeIn));
        }
        
        float f = cam.orthographic ? cam.orthographicSize : cam.fieldOfView;
         t = 0;
        while (t < timeOut)
        {
            t += Time.deltaTime;
            yield return null;
            if (cam.orthographic)
                cam.orthographicSize = Mathf.Lerp(@f, @from, curveOut.Evaluate(t / timeOut));
            else
                cam.fieldOfView = Mathf.Lerp(@f, from, curveOut.Evaluate(t / timeOut));
        }
    }
}
