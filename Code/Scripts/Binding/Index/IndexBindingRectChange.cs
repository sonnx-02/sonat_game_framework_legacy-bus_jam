using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndexBindingRectChange : IndexBindingScript
{
    public RectTransform rectTransform;
    public RectCopy[] rectCopies;


    [SerializeField] [MyButtonRange(0,5,nameof(Copy),null,"Copy")] private int testCopy;
    [SerializeField] [MyButtonRange(0,5,nameof(OnValueChanged),null,"Apply")] private int testApply;

    private void Copy(int i)
    {
        rectCopies[i] = RectCopy.Copy(rectTransform);
    } 
    

    protected override void OnValueChanged(int i)
    {
        base.OnValueChanged(i);
        rectCopies[i].Apply(rectTransform);
    }

    protected override void LoadComponent()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    protected override bool IsNoComponent()
    {
        return rectTransform == null;
    }
}
