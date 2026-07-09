using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TestOverrideProductScript : MonoBehaviour
{
    [SerializeField] private int add;
    [SerializeField] private ClaimProductView[] claims;

    [MyButton(nameof(UpdateData))] [SerializeField]
    protected int logAndRename;

    public bool index;
    public bool paramter;
    public bool paramter2;
    
    private void UpdateData()
    {
        for (var i = 0; i < claims.Length; i++)
        {
            if(paramter)
                claims[i].Products[0].parameter = i + add;
            if(paramter2)
                claims[i].Products[0].parameter2 = i + add;
            if (claims[i].GetComponentInChildren<TMP_Text>() != null)
                claims[i].GetComponentInChildren<TMP_Text>().text = (i + add).ToString();
        }
    }
}
