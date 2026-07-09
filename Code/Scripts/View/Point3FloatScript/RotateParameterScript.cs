using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;


public class RotateParameterScript : Point3FloatScript
{
    public Vector3 rotateVector = new Vector3(0,4,0);
 
    public override void SetParam(Point3Float para)
    {
        Observable.FromMicroCoroutine(() => Ie(para.Value1))
            .TakeUntilDisable(this)
            .Subscribe();
    }

    private IEnumerator Ie(float delay)
    {
        var s = 0f;
        while ( s < delay)
        {
            s += Time.deltaTime;
            yield return null;
        }
        var tf = transform;
        var rotate = Quaternion.Euler(rotateVector.x,rotateVector.y,rotateVector.z);

        while (true)
        {
            tf.rotation *= rotate;
            yield return null;
        }
    }
}
