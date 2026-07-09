using System;
using System.Collections;
using System.Collections.Generic;
using EZCameraShake;
using UniRx;
using UnityEngine;
using Random = UnityEngine.Random;

public enum CameraShakeEnum
{
    NoneShake = 0,
    Shake1 = 1,
    Shake2 = 2,
    Shake3 = 3,
    ExplostionShake = 10,
    BossDieStartShake = 11,
    ShakeMiniBossDead = 12,
    NormalShake = 13,
}

[System.Serializable]
public class ShakeDescriptor
{
    public CameraShakeEnum ShakeEnum;
    public Vector3 PositionInfluence;
    public Vector3 RotationInfluence;
    public float Magnitude = 2;
    public float Roughnes = 4;
    public float FadeIn = 1;
    public float FadeOut = 1;
}

public class CameraShake : MonoBehaviour
{
    public static CameraShake Instance;

    public CameraShaker CameraShaker;
    protected CameraShakeInstance CameraShakeInstance;

    public float Amount = 5;
    public float Duration = 1;
    public float Smooth = 1;
    public float DeltaTime = 1;
    public bool Increase;

    public ShakeDescriptor[] ShakeDescriptors;
    private Dictionary<CameraShakeEnum, ShakeDescriptor> _shakeDictionary;

    void Start()
    {
        Instance = this;
        _shakeDictionary = new Dictionary<CameraShakeEnum, ShakeDescriptor>();
        foreach (var shakeDescriptor in ShakeDescriptors)
        {
            _shakeDictionary.Add(shakeDescriptor.ShakeEnum, shakeDescriptor);
        }
    }

    public void ShakeCamera(CameraShakeEnum enumShake)
    {
        if (_shakeDictionary.ContainsKey(enumShake))
        {
            var descriptor = _shakeDictionary[enumShake];
            CameraShakeInstance = CameraShaker.ShakeOnce(descriptor.Magnitude, descriptor.Roughnes, descriptor.FadeIn, descriptor.FadeOut);
            CameraShakeInstance.PositionInfluence = descriptor.PositionInfluence;
            CameraShakeInstance.RotationInfluence = descriptor.RotationInfluence;
        }
    }

    public CameraShakeEnum CameraShakeEnumTest;

    [MyButton(nameof(TEstShake))] public int abc123;
    public void TEstShake()
    {
        ShakeCamera(CameraShakeEnumTest);
    }

}
