using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Splines;

[ExecuteInEditMode]
public class MySplineAnimateUI : BaseFrameworkView
{
    [SerializeField] private SplineContainer splineContainer;
    [SerializeField] private int splineIndex;
    [SerializeField] [Range(0f, 1f)] private float time;
    [SerializeField] private float duration = 1;
    [SerializeField] private bool animate;
    [SerializeField] private bool unScaleTime;
    [SerializeField] private float startT;
    [SerializeField] private bool rotate;


    private float3 _position;
    private float3 _tangent;
    private float3 _forward;
    private float3 _upVector;

    public override void LogCustom()
    {
        base.LogCustom();
        UpdatePosition();
    }

    [SerializeField]
    private Quaternion r = Quaternion.Euler(90,270,0);
    protected override void Register()
    {
        base.Register();
    }

    public override void LogCustomInt(int value)
    {
        base.LogCustomInt(value);
        r = Quaternion.LookRotation(Vector3.forward, -Vector3.up) * Quaternion.LookRotation(Vector3.up, Vector3.right);
    }


    private void UpdatePosition()
    {
        splineContainer.Evaluate(splineIndex, time, out _position, out _forward, out _upVector);
        transform.position = new Vector3(_position.x, _position.y);
        if (rotate)
        {
            transform.eulerAngles = new Vector3(0,0,-Quaternion.LookRotation(_forward, _upVector).eulerAngles.x);
            transform.rotation =  Quaternion.LookRotation(_forward, _upVector) * r;
        }
    }
    
    public static float ToEuler(quaternion quaternion) {
        float4 q = quaternion.value;
        double3 res;
 
        double sinr_cosp = +2.0 * (q.w * q.x + q.y * q.z);
        double cosr_cosp = +1.0 - 2.0 * (q.x * q.x + q.y * q.y);
        return (float)math.atan2(sinr_cosp, cosr_cosp);
    }

    private void OnEnable()
    {
        time = startT;
    }

    private void Update()
    {
        if (animate)
        {
            if (!unScaleTime)
                time += Time.unscaledDeltaTime * duration;
            else
                time += Time.deltaTime * duration;
            if (time > 1)
                time = time % 1;
            UpdatePosition();
        }
        else
        {
            UpdatePosition();
        }
    }
}