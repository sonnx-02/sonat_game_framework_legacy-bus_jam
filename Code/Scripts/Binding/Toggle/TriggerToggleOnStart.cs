using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerToggleOnStart : MonoBehaviour
{
    [SerializeField] private bool onStart;
    [SerializeField] private bool reverse;

    [SerializeField] private ToggleScript[] toggles;

    public void Start()
    {
        if (onStart)
        {
            toggles.OnChanged(!reverse);
        }
    }
}
