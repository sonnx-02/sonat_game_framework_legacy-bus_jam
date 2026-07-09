using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndexBindingObservableMove : BaseIndexBindingObservable
{
    [SerializeField] private Transform target;
    [SerializeField] private Vector3[] startPos;
    [SerializeField] private Vector3[] toPos;
    [SerializeField] private float timeMove = 1;
    [SerializeField] private float delayActive;
    [SerializeField] private float delayIn;
    [SerializeField] private float delayOut;
    [SerializeField] private float delayAfterOut;
    [SerializeField] private bool active;
    [SerializeField] private AnimationCurve curve;
    [SerializeField] private bool loop;

    protected override void OnValueChanged(int computedValue)
    {
        gameObject.SetActive(computedValue >= 0);
        target.gameObject.SetActive(computedValue >= 0);
        base.OnValueChanged(computedValue);
    }

    public override IEnumerator IeOnChanged(int value)
    {
        var tf = target == null ? transform : target;
        if (value >= 0)
            do
            {
                if (active && delayIn > 0)
                {
                    tf.gameObject.SetActive(false);
                    yield return new WaitForSeconds(delayActive);
                }


                tf.localPosition = startPos[value];
                tf.gameObject.SetActive(true);

                yield return new WaitForSeconds(delayIn);
                float t = 0;
                while (t < timeMove)
                {
                    t += Time.deltaTime;
                    tf.localPosition = Vector3.Lerp(startPos[value], toPos[value], curve.Evaluate(t / timeMove));
                    yield return null;
                }

                yield return new WaitForSeconds(delayOut);

                if (active)
                    tf.gameObject.SetActive(false);

                yield return new WaitForSeconds(delayAfterOut);
            } while (loop);
    }
}