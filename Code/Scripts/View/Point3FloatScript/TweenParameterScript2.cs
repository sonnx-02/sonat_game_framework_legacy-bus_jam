using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;



public class TweenParameterScript2 : Point3FloatScript
{
    public bool resetStartPos;
    [SerializeField] Vector3 startPos;
    [SerializeField] Vector3 pos1;
    [SerializeField] Vector3 pos2;
    [SerializeField] TweenType type = TweenType.FromCurrentToPos2ThenPos1;
    [SerializeField] private float defaultSpeed = 10;

    [MyButton(nameof(LoadPos))] public int tabc;
    public virtual void LoadPos()
    {
        pos1 = transform.localPosition;
        pos2 = pos1;
        startPos = pos1;
    }


    public override void SetParam(Point3Float para)
    {
        if (resetStartPos)
            transform.localPosition = startPos;
        Observable.FromCoroutine(() => IeTween(para.Value1 , para.Value2 > 0 ? para.Value2 : defaultSpeed))
            .TakeUntilDisable(this)
            .Subscribe();
    }

    IEnumerator IeTween(float delay,float speed)
    {
        bool flip = false;

        switch (type)
        {
            case TweenType.FromCurrentToPos1ThenPos2:
                flip = true;
                break;
            case TweenType.FromCurrentToPos2ThenPos1:
                break;
            case TweenType.FromPos1ToPos2:
                transform.localPosition = pos1;
                break;
            case TweenType.FromPos2ToPos1:
                flip = true;
                transform.localPosition = pos2;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        
        if(delay > 0)
            yield return new WaitForSeconds(delay);

        var from = flip ? pos2 : pos1;
        var to = flip ? pos1 : pos2;

        while (true)
        {
            var d = Time.deltaTime * speed;
            var distance = Vector3.Distance(transform.localPosition, to);

            if (distance > d)
            {
                transform.localPosition = Vector3.MoveTowards(transform.localPosition, to, d);
            }
            else
            {
                transform.localPosition = Vector3.MoveTowards(to, from, d - distance);
                flip = !flip;
                from = flip ? pos2 : pos1;
                to = flip ? pos1 : pos2;
            }

            yield return null;
        }
    }
}
