using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class TiltObjectScript : DirectionHitScriptBase
{
    [SerializeField] private Vector2 size = new Vector2(0.5f, 1f);
    [SerializeField] private Vector2 rotateAngle = new Vector2(20, 20);
    [SerializeField] private float time = 1;
    [SerializeField] private AnimationCurve curve = AnimationCurve.Linear(0, 0, 1, 1);

    public override void LogCustom()
    {
        base.LogCustom();
        var transform1 = transform;
        transform1.position = Vector3.zero;
        transform1.rotation = Quaternion.identity;
        transform.RotateAround(transform1.position + new Vector3(size.x, 0, 0), Vector3.forward, -rotateAngle.x);
    }

    public override void LogCustomInt(int value)
    {
        base.LogCustomInt(value);
        var transform1 = transform;
//        transform1.position = Vector3.zero;
//        transform1.rotation = Quaternion.identity;
        switch (value)
        {
            case 0:
            {
                TiltForward();
                break;
            }
            case 1:
            {
                TiltRight();
                break;
            }
            case 2:
            {
                TiltBackward();
                break;
            }
            case 3:
            {
                TiltLeft();
                break;
            }
        }
    }



    private void OnEnable()
    {
        Busy = false;
    }

    protected override IEnumerator OnHit(int direction)
    {
        if (!Busy)
        {
            Busy = true;

            float t = 0f;
            var transform1 = transform;
            var startRotation = transform1.rotation;

            //  transform1.rotation = Quaternion.identity;
            var pos = transform1.position;
            Vector3 rotatePoint;
            float angle;

            switch ((PointDirectionBase) direction)
            {
                case PointDirectionBase.Top:
                    rotatePoint = new Vector3(0, 0, -size.y);
                    angle = rotateAngle.y;
                    break;
                case PointDirectionBase.Right:
                    rotatePoint = new Vector3(-size.x, 0, 0);
                    angle = rotateAngle.x;
                    break;
                case PointDirectionBase.Bottom:
                    rotatePoint = new Vector3(0, 0, size.y);
                    angle = rotateAngle.y;
                    break;
                case PointDirectionBase.Left:
                    rotatePoint = new Vector3(size.x, 0, 0);
                    angle = rotateAngle.x;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
            }

            while (t < time)
            {
                t += Time.deltaTime;
                float d = Mathf.Lerp(0, angle, curve.Evaluate(t / time));
                Quaternion rotation;
                switch ((PointDirectionBase) direction)
                {
                    case PointDirectionBase.Top:
                        rotation = Quaternion.Euler(d, 0, 0);
                        break;
                    case PointDirectionBase.Right:
                        rotation = Quaternion.Euler(0, 0, -d);
                        break;
                    case PointDirectionBase.Bottom:
                        rotation = Quaternion.Euler(-d, 0, 0);
                        break;
                    case PointDirectionBase.Left:
                        rotation = Quaternion.Euler(0, 0, d);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
                }

                transform1.position = rotation * rotatePoint - rotatePoint + pos;
                transform1.rotation = rotation * startRotation;
                yield return null;
            }

            Busy = false;
        }
    }

    private void TiltLeft()
    {
        var transform1 = transform;
        var pos = transform1.position;
        var rotatePoint = new Vector3(size.x, 0, 0);
        var rotation = Quaternion.Euler(0, 0, rotateAngle.x);
        transform1.position = rotation * rotatePoint - rotatePoint + pos;
        transform1.rotation = rotation;
        Debug.Log(rotation * rotatePoint);
    }

    private void TiltRight()
    {
        var transform1 = transform;
        var pos = transform1.position;
        var rotatePoint = new Vector3(-size.x, 0, 0);
        var rotation = Quaternion.Euler(0, 0, -rotateAngle.x);
        transform1.position = rotation * rotatePoint - rotatePoint + pos;
        transform1.rotation = rotation;
        Debug.Log(rotation * rotatePoint);
    }


    private void TiltForward()
    {
        var transform1 = transform;
        var pos = transform1.position;
        var startRotation = transform1.rotation;


        var rotatePoint = new Vector3(0, 0, -size.y);
        var rotation = Quaternion.Euler(rotateAngle.y, 0, 0);
        transform1.position = rotation * rotatePoint - rotatePoint + pos;
        transform1.rotation = rotation * startRotation;
    }

    private void TiltBackward()
    {
        var transform1 = transform;
        var pos = transform1.position;
        var rotatePoint = new Vector3(0, 0, size.y);
        var rotation = Quaternion.Euler(-rotateAngle.y, 0, 0);
        transform1.position = rotation * rotatePoint - rotatePoint + pos;
        transform1.rotation = rotation;
        Debug.Log(rotation * rotatePoint);
    }
}