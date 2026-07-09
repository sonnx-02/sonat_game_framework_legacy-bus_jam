using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HollowScreenByPosition : MonoBehaviour
{
    public Point Point;
    public int AddCol;
    public int AddRow;

    public Transform Top;
    public Transform Bottom;
    public Transform Left;
    public Transform Right;

    public float Outer;
    public int SpriteSize;

    [ContextMenu("show")]
    public void Show()
    {
        transform.localPosition = Point.GetPos();
//
        Bottom.localPosition = new Vector3( Mathf.Max( AddCol - 0.5f,0) * Point.PointSize,( - 0.5f) * Point.PointSize + Outer);
        Top.localPosition = new Vector3(Mathf.Max(AddCol - 0.5f,0) * Point.PointSize,(AddRow + 0.5f) * Point.PointSize - Outer);
        
        
        Bottom.localScale = new Vector3(100f/SpriteSize * Point.PointSize*(1+AddCol),Bottom.localScale.y);
        Top.localScale = new Vector3(100f/SpriteSize * Point.PointSize *(1+AddCol),Bottom.localScale.y);

        Left.localPosition = new Vector3(( - 0.5f) * Point.PointSize + Outer, Mathf.Max( AddRow - 0.5f,0) * Point.PointSize);
        Right.localPosition = new Vector3((AddCol + 0.5f) * Point.PointSize - Outer,
            Mathf.Max(AddRow - 0.5f,0) * Point.PointSize);
        
//        Top.localPosition = new Vector3(( - 0.5f) * Point.PointSize + Outer, -0.5f * Point.PointSize);
//        Bottom.localPosition = new Vector3((AddRow + 0.5f) * Point.PointSize - Outer,
//            Mathf.Max(AddRow - 0.5f) * Point.PointSize);
    }
    
    public void Black()
    {
        transform.localPosition = Vector3.zero;
        
//
//        Bottom.localPosition = new Vector3( Mathf.Max( AddCol - 0.5f,0) * Point.PointSize,( - 0.5f) * Point.PointSize + Outer);
//        Top.localPosition = new Vector3(Mathf.Max(AddCol - 0.5f,0) * Point.PointSize,(AddRow + 0.5f) * Point.PointSize - Outer);
//        
        
        Bottom.localScale = new Vector3(0,Bottom.localScale.y);
        Top.localScale = new Vector3(0,Bottom.localScale.y);

        Left.localPosition = Vector3.zero;
        Right.localPosition =  Vector3.zero;
        
//        Top.localPosition = new Vector3(( - 0.5f) * Point.PointSize + Outer, -0.5f * Point.PointSize);
//        Bottom.localPosition = new Vector3((AddRow + 0.5f) * Point.PointSize - Outer,
//            Mathf.Max(AddRow - 0.5f) * Point.PointSize);
    }
}
