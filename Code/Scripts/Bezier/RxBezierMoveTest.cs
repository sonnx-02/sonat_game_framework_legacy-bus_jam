using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(RxBezierMove))]
public class RxBezierMoveTest : MonoBehaviour
{
    [Range(0, 1)] [SerializeField] private float progress;
    [SerializeField] private Canvas canvas;
    [SerializeField] private bool move;

    [MyButton(nameof(Init), nameof(NextAnchor), nameof(NextPreAnchor))] [SerializeField]  private int test;
    [MyButton(nameof(ResetPreAnchor),nameof(SetLocalPoint),nameof(SetScale))] [SerializeField]  private int test1;
    [MyButton(nameof(TargetPosition))] [SerializeField]  private int test2;

    private List<Vector3> _gizmoPoints;
    private RxBezierMove _move;

    public void TargetPosition()
    {
        _move = GetComponent<RxBezierMove>();
        transform.localPosition =_move.TargetPos();
    }

    public void Init()
    {
        _move = GetComponent<RxBezierMove>();
        _move.Init();
        transform.localPosition = _move.localAppear.localPoint;
    }

    public void NextAnchor()
    {
        _move = GetComponent<RxBezierMove>();
        _move.listPoints[0].nextAnchor = transform.localPosition - _move.localAppear.localPoint;
        _move.Init();
    }

    public void NextPreAnchor()
    {
        _move = GetComponent<RxBezierMove>();
     //   _move.listPoints[1].preAnchor = transform.localPosition - _move.listPoints[1].point - _move.localPoint;

     _move.listPoints[1].preAnchor =
         (_move.TargetTransform as RectTransform).InverseTransformPoint(transform.position)
                                      - _move.listPoints[_move.listPoints.Length-1].point;

        _move.Init();
    }

    public void ResetPreAnchor()
    {
        _move = GetComponent<RxBezierMove>();
        _move.listPoints[1].preAnchor = Vector3.zero;
        _move.Init();
    }
    
    public void SetLocalPoint()
    {
        _move = GetComponent<RxBezierMove>();
        _move.localAppear.localPoint = transform.localPosition;
        _move.Init();
    }
    
    public void SetScale()
    {
        _move = GetComponent<RxBezierMove>();
        _move.SetCurrentScale();
    }


    // Update is called once per frame
    void Update()
    {
        if (move && _move != null && _move.Initialized)
        {
            _move.SetAtT(progress);
        }
    }

    private void OnDrawGizmos()
    {
        if (_move != null && _move.Initialized)
        {
            Gizmos.matrix = (transform.parent as RectTransform).localToWorldMatrix;

            _gizmoPoints = _move.GetGizmoVerticeLines();
            for (var i = 0; i < _gizmoPoints.Count / 2; i++)
            {
                Gizmos.color = i == 0 ? Color.yellow :  Color.red;
                Gizmos.DrawLine(_gizmoPoints[i * 2], _gizmoPoints[i * 2 + 1]);
            }

            for (int i = 0; i < 100; i++)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawLine(
                    _move.GetPosAtT(i / 100f),
                    _move.GetPosAtT((i + 1) / 100f)
                );
            }
        }
    }
}