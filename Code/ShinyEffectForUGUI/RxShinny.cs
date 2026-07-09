using System.Collections;
using System.Collections.Generic;
using Coffee.UIExtensions;
using UnityEngine;

public class RxShinny : BaseRxAnimator
{
    [SerializeField] private float time = 1;
    [SerializeField] AnimationCurve curve =  AnimationCurve.Linear(0, 0, 1, 1);
    [SerializeField] private float delay;
    [SerializeField] private ShinyEffectForUGUI shinyEffect;
    [SerializeField] private BaseBaseActionScript[] actions;
    public override bool HasOut()
    {
        return false;
    }

    public override bool HasIn()
    {
        return true;
    }

    private void Reset()
    {
        if(shinyEffect == null)
            shinyEffect = GetComponent<ShinyEffectForUGUI>();
    }
    
    protected override IEnumerator ThisDoAnimate_In()
    {
        yield return new WaitForSeconds(delay);
        actions.StartAction();
        float t = 0;
        while (true)
        {
             t += Time.deltaTime;
             yield return null;
             if (t > time)
             {
                 t = t % time;
                 yield return new WaitForSeconds(delay);
                 actions.StartAction();
             }
             shinyEffect.location = curve.Evaluate(t / time);
        }
    }

    protected override IEnumerator ThisDoAnimate_Out() => ThisDoAnimate_In();

    protected override IEnumerator ThisDoAnimate_InThenOut() => ThisDoAnimate_In();
}
