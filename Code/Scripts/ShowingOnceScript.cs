using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowingOnceScript : MonoBehaviour
{
    [SerializeField] private string key = "test";
    [SerializeField] private BaseRxAnimator[] anims;
    [SerializeField] private ToggleScript[] binds;

    private void OnEnable()
    {
        if (PlayerPrefs.GetInt(key) != 0)
        {
            gameObject.SetActive(false);
            binds.OnChanged(false);
        }
        else
        {
            binds.OnChanged(true);
            foreach (var baseRxAnimator in anims)
            {
                baseRxAnimator.Animator_In();
            }
        }
        PlayerPrefs.SetInt(key,1);
    }
}
