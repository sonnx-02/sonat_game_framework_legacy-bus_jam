using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformActivator : MonoBehaviour
{
    [SerializeField] private bool window = true;
    [SerializeField] private bool editor = true;
    [SerializeField] private GameObject[] targets;

    private void OnEnable()
    {
#if UNITY_STANDALONE_WIN
       if(window)
           targets.SetActiveAll(true);
#endif
#if UNITY_EDITOR
       if(editor)
           targets.SetActiveAll(true);
       #endif
    }
}
