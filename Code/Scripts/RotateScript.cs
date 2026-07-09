using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateScript : MonoBehaviour
{
    private Transform _transform;

    [MyButton(nameof(Start))] [SerializeField]
    private int abc;

    public Vector3 rotateVector = new Vector3(0, 0,120);
    public float delayStartTime = 0f;

    private float _t;

    public void Start()
    {
        _transform = transform;
        transform.localRotation = Quaternion.identity;
    }

    private void OnEnable()
    {
        _transform = transform;
        _t = delayStartTime;
        //   _transform.rotation = Quaternion.identity;
    }

    // Update is called once per frame
    void Update()
    {
        if (_t > 0)
            _t -= Time.deltaTime;
        else
            _transform.localEulerAngles += Time.deltaTime * rotateVector;
    }
}