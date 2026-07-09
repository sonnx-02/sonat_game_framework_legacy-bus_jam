using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateScriptAdvanced : MonoBehaviour
{
    [Range(-1.0f, 1.0f)]
    public float xForceDirection = 0.0f;
    [Range(-1.0f, 1.0f)]
    public float yForceDirection = 0.0f;
    [Range(-1.0f, 1.0f)]
    public float zForceDirection = 0.0f;

    public float speedMultiplier = 1;

    public bool worldPivote = false;

    private Space _spacePivot = Space.Self;

    public bool useCurve;
    public float timeCurve;

    public AnimationCurve curve = AnimationCurve.Linear(0,1,1,1);
    public bool scale;
    public AnimationCurve scaleCurve = AnimationCurve.Linear(0,1,1,1);

    void Start()
    {
        if (worldPivote) _spacePivot = Space.World;
        else
        {
            _spacePivot = Space.Self;
        }
    }

    void Update()
    {
        var multi = speedMultiplier * Time.deltaTime * 60;

        if (useCurve)
        {
            float t = (Time.time % timeCurve) / timeCurve;
            multi *= curve.Evaluate(t);
            if (scale)
            {
                var scale1 = scaleCurve.Evaluate(t);
                transform.localScale = new Vector3(scale1,scale1,scale1);
            }
        }
        
        this.transform.Rotate(xForceDirection * multi
            , yForceDirection * multi
            , zForceDirection * multi
            , _spacePivot);
    }

}
