using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;


[ExecuteInEditMode]
public class RxBezierMoveToTarget : BaseRxAnimator
{
    [SerializeField] private GetPosScript getStart;

    [SerializeField] [ConditionalField(nameof(getStart), new object[] {null})]
    private Transform defaultStartWorld;

    [SerializeField] private Vector2 startPosOffset;

    [FormerlySerializedAs("getPosScript")] [SerializeField]
    private GetPosScript getTarget;

    [SerializeField] [ConditionalField(nameof(getTarget), new object[] {null})]
    private RectTransform defaultTarget;

    [SerializeField] private bool linear;

    // for berzier
    [ConditionalField(nameof(linear), new object[] {true}, true)] [SerializeField]
    private BezierTargetTransform bezierTarget; // will override main position (not anchor)

    [SerializeField] private AnimationCurve curveTween = AnimationCurve.Linear(0, 0, 1, 1);
    [SerializeField] private float time = 0.5f;
    [SerializeField] private AnimationData scaleIn; // optional
    [SerializeField] private AnimationData scaleOut; // optional

    [FormerlySerializedAs("updateWhenAnimIn")] [SerializeField]
    private bool overrideStartPointWhenAnimIn; // 

    [SerializeField] [ConditionalField(nameof(overrideStartPointWhenAnimIn))]
    private Vector2 adjustPosOverride;

    [SerializeField] private bool fromCurrentPos;

    private RectTransform Target => getTarget != null ? getTarget.GetRectTransformTarget() : defaultTarget;
    private RectTransform ParentTransform => transform.parent as RectTransform;
    private RectTransform RectTransform => transform as RectTransform;

    private BezierPathController _pathOriginal;
    private BezierPathController _pathOverride;
    private BezierPathController Path => _pathOverride ?? _pathOriginal;

    private bool _setupLinear;
    private Vector3 _startPos;
    private Vector3 _endPos;
    private Vector3 _startScale;

    private List<Vector3> _gizmoPoints;
    [SerializeField] private bool updateMoveTest;
    [SerializeField] private bool gizmo;
    [SerializeField] [Range(0, 1)] private float progress;
    [SerializeField] private RectTransform inputHandler; // to adjust anchor point

    protected override void Register()
    {
#if UNITY_EDITOR
        if (!Application.isPlaying)
            return;
#endif
        _startScale = transform.localScale;
        base.Register();
    }

    public override void Animator_In()
    {
        base.Animator_In();
        Setup();
    }

    private Vector2 GetStartPos()
    {
        if (getStart != null)
            return getStart.GetAnchorPos() + startPosOffset;
        if (defaultStartWorld != null)
            return FrameworkExtensions.WorldToPositionInCanvas(defaultStartWorld.position) + startPosOffset;
        return RectTransform.anchoredPosition + startPosOffset;
    }

    private void Setup()
    {
        _startPos = GetStartPos();
        _endPos = TargetPos();
        if (linear)
        {
            _setupLinear = true;
            _pathOriginal = null;
            _pathOverride = null;
            return;
        }

        _setupLinear = false;

        bezierTarget.parent = ParentTransform;
        bezierTarget.target = Target;
        _pathOriginal = bezierTarget.GetPath();
        _pathOriginal.SetUp(10, false, 20);
        _pathOverride = null;
#if UNITY_EDITOR
        OnDrawGizmos();
#endif
    }


    private void OverrideStartPoint(Vector3 pos)
    {
        _startPos = RectTransform.anchoredPosition + adjustPosOverride;
        _endPos = TargetPos();
        if (linear)
        {
            _setupLinear = true;
            _pathOriginal = null;
            _pathOverride = null;
            return;
        }

        _setupLinear = false;

        _pathOverride = bezierTarget.GetPath(ParentTransform.InverseTransformPoint(pos));
        _pathOverride.SetUp(10, false, 20);
    }

    private void OverrideStartPoint(Transform fromTarget)
    {
        OverrideStartPoint(fromTarget.position);
    }

    protected override IEnumerator ThisDoAnimate_In()
    {
        CheckRegister();
        //if (Path == null)
        Setup();
        
        SetAtT(0);
        if(settings.deactiveAfterOut)
            gameObject.SetActive(false);
        if (settings.DelayBeforeIn > 0)
            yield return CoroutineUtilities.WaitForRealTime(settings.DelayBeforeIn);
        gameObject.SetActive(true);
        
        if (scaleIn.active)
        {
            transform.localScale = scaleIn.vec;
            yield return transform.DOScale(_startScale, scaleIn.time).SetEase(scaleIn.ease).WaitForCompletion();
        }
        
        if (overrideStartPointWhenAnimIn)
            OverrideStartPoint(transform);

        StartInSound();
        if (settings.deactiveAfterOut)
            gameObject.SetActive(true);

    

        float t = 0;
        TriggerAfterInParticle(time - settings.particleTimeAdjust);
        while (t < time)
        {
            var d = t / time;
            SetAtT(d);
            t += Time.unscaledDeltaTime;
            yield return null;
        }
        SetAtT(1);
        yield return null;
        
        if (scaleOut.active)
        {
            yield return transform.DOScale( scaleOut.vec, scaleOut.time).SetEase(scaleOut.ease).WaitForCompletion();
        }

        if (settings.deactiveAfterOut)
            gameObject.SetActive(false);
        InvokeCallBack();
        StartOutSound();
    }

    private void SetAtT(float t)
    {
        if (t >= 1)
            RectTransform.anchoredPosition = TargetPos();
        else
            RectTransform.anchoredPosition = (linear) ? GetPosAtT(t) : Path.GetAtPosAtT(curveTween.Evaluate(t));
    }

    private Vector3 TargetPos()
    {
        return ParentTransform.InverseTransformPoint(Target.position);
    }

    // for drawer gizmo
    private Vector3 GetPosAtT(float t)
    {
        if (linear)
            return Vector2.Lerp(_startPos, _endPos, curveTween.Evaluate(t));
        return Path.GetAtPosAtT(curveTween.Evaluate(t));
    }

    protected override IEnumerator ThisDoAnimate_Out()
    {
        yield return null;
    }

    protected override IEnumerator ThisDoAnimate_InThenOut() => ThisDoAnimate_In();

    public override bool HasOut()
    {
        return false;
    }

    public override bool HasIn()
    {
        return true;
    }


    [System.Serializable]
    private class ScaleData
    {
        public bool active;
        public Vector3 tweenScaleValueFrom = Vector3.one;
        public Vector3 tweenScaleValueTo = Vector3.one;
        public AnimationCurve curveScale = AnimationCurve.Linear(0, 1, 1, 1);

        public Vector3 GetScale(float t)
        {
            return Vector3.Lerp(tweenScaleValueFrom, tweenScaleValueTo, curveScale.Evaluate(t));
        }
    }

#if UNITY_EDITOR
    [SplitHeader] public int split;

    [MyButton(nameof(Setup))] [SerializeField]
    private int test;

    [MyButton(
        new[]
        {
            nameof(Next0),
            nameof(Pre1),
            nameof(Pos1),
            nameof(Next1),
            nameof(Pre2),
        }
    )]
    [SerializeField]
    private int testInput;

    [MyButtonInt(nameof(SetAtT))] [SerializeField]
    private int testInput2;


    private void Update()
    {
        if (!Application.isPlaying)
        {
            if (!updateMoveTest)
                return;
            if (linear && !_setupLinear)
                return;
            if (!linear && Path == null)
                return;
            SetAtT(progress);
        }
    }

    private void OnDrawGizmos()
    {
        if (!gizmo)
            return;
        if (Path == null && !linear)
            Setup();
        DrawGizmo();
    }


    private void CreateHandler()
    {
        if (inputHandler == null)
        {
            var ng = new GameObject();
            inputHandler = ng.AddComponent<RectTransform>();
            inputHandler.SetParent(transform);
            inputHandler.anchoredPosition = Vector2.zero;
        }
    }

    private void Next0()
    {
        CreateHandler();
        bezierTarget.relativeNext0 = inputHandler.anchoredPosition;
        Setup();
    }

    private void Pre1()
    {
        CreateHandler();
        bezierTarget.relativePre1 = (Vector3) inputHandler.anchoredPosition - bezierTarget.pos1;
        Setup();
    }

    private void Pos1()
    {
        CreateHandler();
        bezierTarget.pos1 = (Vector3) inputHandler.anchoredPosition;
        Setup();
    }

    private void Next1()
    {
        CreateHandler();
        bezierTarget.relativeNext1 = (Vector3) inputHandler.anchoredPosition - bezierTarget.pos1;
        Setup();
    }

    private void Pre2()
    {
        CreateHandler();
        bezierTarget.relativePre2 = (Vector3) inputHandler.anchoredPosition - bezierTarget.TargetPos;
        Setup();
    }


    private void DrawGizmo()
    {
        Gizmos.matrix = ParentTransform.localToWorldMatrix;
        if (!linear)
        {
            _gizmoPoints = Path.GizmoLine();
            for (var i = 0; i < _gizmoPoints.Count / 2; i++)
            {
                Gizmos.color = i == 0 ? Color.yellow : Color.red;
                Gizmos.DrawLine(_gizmoPoints[i * 2], _gizmoPoints[i * 2 + 1]);
            }

            for (int i = 0; i < 100; i++)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawLine(GetPosAtT(i / 100f), GetPosAtT((i + 1) / 100f));
            }
        }
        else
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(GetPosAtT(0), GetPosAtT(1));
        }
    }
#endif
}