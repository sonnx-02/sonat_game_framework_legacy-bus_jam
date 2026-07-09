using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class RotateAroundScript : DirectionHitScriptBase
{

    [SerializeField] private Vector2 size = new Vector2(0.5f,1f);
    [SerializeField] private Vector2 rotateAngle  = new Vector2(20,20);
    [SerializeField] private float time = 1;
    [SerializeField] private AnimationCurve curve = AnimationCurve.Linear(0, 0, 1, 1);


//    public Transform testTf;
//    [MyButton(nameof(Test),nameof(Test2))] [SerializeField]
//    private int test;
//    public void Test()
//    {
//        transform.position = Vector3.zero;
//        transform.rotation = Quaternion.identity;
//        transform.RotateAround(testTf.position, Vector3.right, rotateAngle.y);
//    }
//
//    [SerializeField] private Vector3 euler;
//    [SerializeField] private Vector3 pos;
//    [SerializeField] private Vector3 axis = Vector3.forward;
//    public void Test2()
//    {
//        transform.position = Quaternion.Euler(euler) * pos;
//    }

    public override void LogCustom()
    {
        base.LogCustom();
        var transform1 = transform;
        transform1.position = Vector3.zero;
        transform1.rotation = Quaternion.identity;
        transform.RotateAround(transform1.position + new Vector3(size.x,0,0), Vector3.forward,- rotateAngle.x);
    }

    public override void LogCustomInt(int value)
    {
        base.LogCustomInt(value);
        var transform1 = transform;
        transform1.position = Vector3.zero;
        transform1.rotation = Quaternion.identity;
        transform.RotateAround(transform1.position + new Vector3(0,0,size.y), Vector3.right, rotateAngle.y);
    }

    protected override IEnumerator OnHit(int direction)
    {
        float t = 0f;
        var transform1 = transform;
        transform1.rotation = Quaternion.identity;
        var position = transform1.position;
        float angle = 0;
        Vector3 axis = Vector3.forward;
        switch ((PointDirectionBase)direction)
        {
            case PointDirectionBase.Top:
                position += new Vector3(0, 0, size.y);
                angle = rotateAngle.y;
                axis = Vector3.right;
                break;
            case PointDirectionBase.Right:
                position += new Vector3(size.x, 0, 0);
                angle = -rotateAngle.x;
                break;
            case PointDirectionBase.Bottom:
                position -= new Vector3(0, 0, size.y);
                angle = -rotateAngle.y;
                axis = Vector3.right;
                break;
            case PointDirectionBase.Left:
                position -= new Vector3(size.x, 0, 0);
                angle = rotateAngle.x;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
        }

        while (t < time)
        {
            t += Time.deltaTime;
            transform.RotateAround(position, axis, angle * (Time.deltaTime/time));
            yield return null;
        }

        t = 0f;
        while (t < time)
        {
            t += Time.deltaTime;
            transform.RotateAround(position, axis, -angle * (Time.deltaTime/time));
            yield return null;
        }
    }
}