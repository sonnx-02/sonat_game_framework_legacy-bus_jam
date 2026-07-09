using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleScriptAnimRotate : ToggleScriptAnimation
{
    [SerializeField] private float delay;
    [SerializeField] private float time = 0.5f;
    [SerializeField] private Vector3 from;
    [SerializeField] private Vector3 to = new Vector3(0,360,0);
    [SerializeField] private bool useCurve;
    [SerializeField] private AnimationCurve curve;
    protected override void LoadComponent()
    {
    }

    public override void CopyFrom()
    {
    }

    public override void CopyTo()
    {
    }

    protected override IEnumerator ThisDoAnimate_In() => ThisDoAnimate(true);

    protected override IEnumerator ThisDoAnimate_Out() => ThisDoAnimate(false);

    private IEnumerator ThisDoAnimate(bool value)
    {
        var rFrom = value ? @from : to;
        var rTo = value ? @to : @from;
        transform.rotation = Quaternion.Euler(@rFrom);
        if (delay > 0)
            yield return new WaitForSeconds(delay);
        float t = 0;

        if (useCurve)
            while (t < time)
            {
                transform.rotation = Quaternion.Euler(Vector3.Lerp(@rFrom, rTo, curve.Evaluate(t / time)));
                yield return null;
                t += Time.deltaTime;
            }
        else
            while (t < time)
            {
                transform.rotation = Quaternion.Euler(Vector3.Lerp(@rFrom, rTo, t / time));
                yield return null;
                t += Time.deltaTime;
            }

        transform.rotation = Quaternion.Euler(@rTo);
    }
}