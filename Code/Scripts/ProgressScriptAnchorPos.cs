using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressScriptAnchorPos : ProgressScript
{
    [MyButtonInt(nameof(CopyFrom), nameof(CopyTo))]
    [SerializeField] private  int abc123;
   
    [SerializeField] private RectTransform rt;
    [SerializeField] private Vector3 from;
    [SerializeField] private Vector3 to;
    protected override void Trigger(float val)
    {
        base.Trigger(val);
        rt.anchoredPosition = Vector3.Lerp(@from, to, val);
    }
    
    public void CopyFrom()
    {
        @from = rt.anchoredPosition;
    }
   
    public void CopyTo()
    {
        @to = rt.anchoredPosition;
    }
   
}
