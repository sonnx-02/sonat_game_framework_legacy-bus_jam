using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CopyScript : MonoBehaviour
{
    public Transform source;

    public Transform[] targets;


    [MyButton(nameof(Test))] [SerializeField] private  int testTest;

    public void Test()
    {
        int c = 0;
        for (int i = source.childCount - 1; i >= 0; i--)
        {
            var go = source.GetChild(i);
            go.SetParent(targets[c]);
            c++;
        }
    }
}
