using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ProgressScriptJelly : ProgressScript
{
    [SerializeField] private bool pos = true;
    [SerializeField] private Vector3 posFrom;
    [SerializeField] private Vector3 posTo;
    [SerializeField] private AnimationCurve curvePos;
    [SerializeField] private bool scale = true;
    [SerializeField] private AnimationCurve curveScaleX;
    [SerializeField] private AnimationCurve curveScaleY;
    [SerializeField] private AnimationCurve curveScaleZ;

    protected override void Trigger(float val)
    {
        base.Trigger(val);
        if(pos)
            transform.localPosition = Vector3.Lerp(posFrom, posTo, curvePos.Evaluate(val));
        if (scale)
            transform.localScale = new Vector3(curveScaleX.Evaluate(val), curveScaleY.Evaluate(val), curveScaleZ.Evaluate(val));
    }
}
