using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateZScript : MonoBehaviour
{
    public Vector3 localPos;
    [MyButtonInt(nameof(Rotate))] [SerializeField]
    private int test;

    [SerializeField] private bool reverse;

    public void Rotate(int progress100)
    {
        Rotate(progress100 / 100f);
    }
    
    public void Rotate(float progress)
    {
        transform.localPosition = Quaternion.Euler(0, 0, (reverse ? 360 : -360) * progress) * localPos;
    }
}
