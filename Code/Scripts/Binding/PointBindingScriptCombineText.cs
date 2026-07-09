using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PointBindingScriptCombineText : PointBindingScript
{
    [SerializeField] private TMP_Text textMesh;
    [SerializeField] private string separated;
    [SerializeField] private string[] textBindCols;
    [SerializeField] private string[] textBindRows;

    public override void OnChanged(Point point)
    {
        base.OnChanged(point);
        textMesh.text = textBindCols[Mathf.Min(textBindCols.Length - 1, point.col)] + separated +
                        textBindRows[Mathf.Min(textBindRows.Length - 1, point.row)];
        test = new Vector2Int(point.col,point.row);
    }

    [SerializeField]
    [MyButtonVector2Int(nameof(Test))]
    private Vector2Int test;

    public void Test(Vector2Int vec)
    {
        OnChanged(new Point(vec.x,vec.y));
    }
}
