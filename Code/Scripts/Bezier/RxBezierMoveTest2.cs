//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//
//[ExecuteInEditMode]
//[RequireComponent(typeof(RxBezierMoveToTarget))]
//public class RxBezierMoveTest2 : MonoBehaviour
//{
//    [Range(0, 1)] [SerializeField] private float progress;
//    [SerializeField] private Canvas canvas;
//    [SerializeField] private bool move;
//
//    [MyButton(nameof(CalculatePath), nameof(SaveNextAnchor), nameof(SaveNextPreAnchor))] [SerializeField]  private int test;
//    [MyButton(nameof(SetAtTarget))] [SerializeField]  private int test2;
//
//    private List<Vector3> _gizmoPoints;
//    private RxBezierMoveToTarget _move;
//
//    public void SetAtTarget()
//    {
//        _move = GetComponent<RxBezierMoveToTarget>();
//        transform.localPosition =_move.TargetPos();
//    }
//
//    public void CalculatePath()
//    {
//        _move = GetComponent<RxBezierMoveToTarget>();
//        _move.CalculatePath();
//        _move.transform.localPosition = _move.GetPosAtT(0);
//    }
//
//    public void SaveNextAnchor()
//    {
//        _move = GetComponent<RxBezierMoveToTarget>();
//        _move.listPoints[0].nextAnchor = transform.localPosition;
//        _move.Init();
//    }
//
//    public void SaveNextPreAnchor()
//    {
//        _move = GetComponent<RxBezierMoveToTarget>();
//     //   _move.listPoints[1].preAnchor = transform.localPosition - _move.listPoints[1].point - _move.localPoint;
//
//     _move.listPoints[1].preAnchor =
//         (_move.TargetTransform as RectTransform).InverseTransformPoint(transform.position)
//                                      - _move.listPoints[_move.listPoints.Length-1].point;
//
//        _move.Init();
//    }
//
//    
//    
//    // Update is called once per frame
//    void Update()
//    {
//        if (move && _move != null && _move.Initialized)
//        {
//            _move.SetAtT(progress);
//        }
//    }
//
//    private void OnDrawGizmos()
//    {
//        if (_move != null && _move.Initialized)
//        {
//            Gizmos.matrix = (transform.parent as RectTransform).localToWorldMatrix;
//
//            _gizmoPoints = _move.GetGizmoVerticesLines();
//            for (var i = 0; i < _gizmoPoints.Count / 2; i++)
//            {
//                Gizmos.color = i == 0 ? Color.yellow :  Color.red;
//                Gizmos.DrawLine(_gizmoPoints[i * 2], _gizmoPoints[i * 2 + 1]);
//            }
//
//            for (int i = 0; i < 100; i++)
//            {
//                Gizmos.color = Color.green;
//                Gizmos.DrawLine(
//                    _move.GetPosAtT(i / 100f),
//                    _move.GetPosAtT((i + 1) / 100f)
//                );
//            }
//        }
//    }
//}