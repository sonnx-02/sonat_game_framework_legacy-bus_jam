using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UniRx;
using UnityEngine;



public class CurveTweenScript : BaseTweenScript
{
    public Transform from;
    public Transform to;
    public Vector3 anchor;
    public float time = 1;
    public AnimationCurve curve;
    public float delay;

    public bool deactive = true;

    private IDisposable _disposable;
    public override void Tween()
    {
        base.Tween();
        if(deactive)
            gameObject.SetActive(true);
        _disposable?.Dispose();
        _disposable = Observable.FromMicroCoroutine(IeTween).TakeUntilDisable(this).Subscribe(_ =>
        {
            if(deactive)
                gameObject.SetActive(false);
        });
    }
    
#if UNITY_EDITOR
    public float size = 1;
    public void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + anchor,size);
    }
#endif
   

    public IEnumerator IeTween()
    {
        if(delay > 0)
        yield return new WaitForSeconds(delay);
        var fromPos = @from.transform.position;
        var toPos = to.transform.position;
        var ac = fromPos + this.anchor;
        
        float t = 0;
        while ( t < time)
        {
            t += Time.deltaTime;
            transform.position = DuongExtensions.Bezier2(fromPos,ac ,toPos,Mathf.Min(t/time,1));
            yield return null;
        }

        transform.position = toPos;
    }

    public override void ResetTween()
    {
        base.ResetTween();
        transform.position = @from.position;
        if(deactive)
            gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        transform.DOKill();
    }
}
