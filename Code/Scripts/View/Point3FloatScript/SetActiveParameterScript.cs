using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class SetActiveParameterScript : Point3FloatScript
{
    public override void SetParam(Point3Float para)
    {
        if (para.Value2 < 0)
        {
            if (para.Value1 > 0)
                Observable.Timer(TimeSpan.FromSeconds(para.Value1))
                    .Subscribe(data =>
                    {
                        gameObject.SetActive(false);
                    });
            else
            {
                gameObject.SetActive(false);
            }
        }
    }
}
