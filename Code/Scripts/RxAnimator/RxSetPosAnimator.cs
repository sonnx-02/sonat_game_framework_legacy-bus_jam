using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RxSetPosAnimator : BaseRxAnimator
{
    [SerializeField] private Vector3 adjustment;

    public BaseRxAnimator SetPos(Vector3 position)
    {
        transform.position = position + adjustment;
        return this;
    }
    
    public BaseRxAnimator SetPosLocal(Vector3 position)
    {
        transform.localPosition = position + adjustment;
        return this;
    }
    
    
    public override bool HasOut()
    {
        return false;
    }

    public override bool HasIn()
    {
        return true;
    }

    protected override IEnumerator ThisDoAnimate_In()
    {
        gameObject.SetActive(true);
        yield return null;
    }

    protected override IEnumerator ThisDoAnimate_Out()
    {
        yield return null;
    }

    protected override IEnumerator ThisDoAnimate_InThenOut()
    {
        yield return null;
    }
}
