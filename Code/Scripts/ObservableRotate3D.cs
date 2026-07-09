using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class ObservableRotate3D : BaseFrameworkView
{
    [SerializeField] private float rotateAdd;
    [SerializeField] private float rotateSpeed = 1;

    [SerializeField] [MyButtonInt(new[] {nameof(Set), nameof(Rotate)}, nameof(PointDirectionBase))]
    private int baseDirection;

    public AnimationCurve curve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1));

    public void Set(int directionBase)
    {
        Debug.Log((PointDirectionBase) directionBase);
        transform.localEulerAngles = new Vector3(0, directionBase * 90 + rotateAdd, 0);
    }

    public void Set(PointDirectionBase directionBase)
    {
        transform.localEulerAngles = new Vector3(0, (int) directionBase * 90 + rotateAdd, 0);
    }


    private IDisposable _disposable;

    public void Rotate(int directionBase)
    {
        _disposable?.Dispose();
        _disposable = ObservableRotate(directionBase).Subscribe();
    }

    public IObservable<Unit> ObservableRotate(int directionBase)
    {
        return Observable.FromCoroutine(() => IeRotate(directionBase));
    }

    public IEnumerator IeRotate(int directionBase)
    {
        var startRotate = transform.rotation;
        var toRotate = Quaternion.Euler(0, directionBase * 90 + rotateAdd, 0);
        float angle = Quaternion.Angle(startRotate, toRotate);
        var time = angle / rotateSpeed;
        float t = 0f;
        while (t < time)
        {
            transform.rotation = Quaternion.Lerp(startRotate, toRotate, curve.Evaluate(t / time));
            t += Time.deltaTime;
            yield return null;
        }

        transform.rotation = toRotate;
    }


    [SerializeField] [MyButtonVector3(nameof(SetTarget))]
    private Vector3 vec;


    public void SetTarget(Vector3 worldPos)
    {
        transform.rotation = DuongExtensions.GetRotateQuaternionY(transform.position, worldPos, rotateAdd);
    }

    public void RotateToTarget(Vector3 position)
    {
        _disposable?.Dispose();
        _disposable = ObservableRotate(position).Subscribe();
    }

    public IObservable<Unit> ObservableRotate(Vector3 localPos)
    {
        return Observable.FromCoroutine(() => IeRotate(localPos));
    }

    public IEnumerator IeRotate(Vector3 localPos)
    {
        var transform1 = transform;
        var startRotate = transform1.rotation;
        var toRotate = DuongExtensions.GetRotateQuaternionY(transform1.localPosition, localPos, rotateAdd);
        float angle = Quaternion.Angle(startRotate, toRotate);
        var time = angle / rotateSpeed;
        float t = 0f;
        while (t < time)
        {
            transform1.rotation = Quaternion.Lerp(startRotate, toRotate, curve.Evaluate(t / time));
            t += Time.deltaTime;
            yield return null;
        }

        transform.rotation = toRotate;
    }
}