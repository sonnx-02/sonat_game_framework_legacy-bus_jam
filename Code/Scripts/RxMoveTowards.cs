using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class RxMoveTowards : BaseRxAnimator
{
    [SerializeField] private LocationEnum location;
    [SerializeField] private Transform targetTransforms;
    [SerializeField] private float speed = 50;

    public override bool HasOut()
    {
        return false;
    }

    public override bool HasIn()
    {
        return true;
    }

    private Transform GetTarget()
    {
        if (targetTransforms != null)
            return targetTransforms;
        return RootView.rootView.screenRoot.GetLocation(location);
    }

    protected override IEnumerator ThisDoAnimate_In()
    {
        if (settings.DelayBeforeIn > 0)
            yield return new WaitForSeconds(settings.DelayBeforeIn);
        var target = GetTarget();
        while (Mathf.Abs((transform.position - target.position)
                   .magnitude) > 0.1f)
        {
            var d = Time.deltaTime * speed;
            transform.position = Vector3.MoveTowards(transform.position, target.position, d);
            yield return null;
        }
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
