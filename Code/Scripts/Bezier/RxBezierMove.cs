using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class RxBezierMove : BaseRxAnimator
{
    public LocalAppear localAppear;

    // for berzier
    [SerializeField] private float time = 0.5f;
    [SerializeField] private Vector3 tweenScaleValue = Vector3.one;
    [SerializeField] private AnimationCurve curve = AnimationCurve.Linear(0, 0, 1, 1);
    [SerializeField] private AnimationCurve curveScale = AnimationCurve.Linear(0, 1, 1, 1);
    [SerializeField] public BezierPointVector3[] listPoints;
    [SerializeField] private Image fadeTarget;
    [SerializeField] private float fadeTime;
    
    [SerializeField] private Transform[] targetTransforms;
    [SerializeField] [DisplayAsDisable] private int targetIndex;

    public Transform TargetTransform => targetTransforms.Length == 0 ? null : targetTransforms[targetIndex];
    
    public override void SetIndex(int i)
    {
        targetIndex = i;
        Init();
    }

    public bool Initialized => _path != null;
    private BezierPathController _path;

    [System.Serializable]
    public class LocalAppear
    {
        public Vector3 localPoint;
        public float timeLocal = 0.5f;
        public float delayBeforeLocalRandom = 0f;
        public float delayAfterLocal = 0.5f;
        public AnimationCurve curveLocal = AnimationCurve.Linear(0, 0, 1, 1);
        public AnimationCurve curveScaleLocal = AnimationCurve.Linear(0, 0, 1, 1);
        public Vector3 startScaleValue = Vector3.one;
        public Vector3 localScaleValue = Vector3.one;
    }

    [SerializeField] private bool initOnRegister = true;

    public override void CheckRegister()
    {
        base.CheckRegister();
        if (initOnRegister)
            Init();
    }
    

    private bool _pathInitialized;

    public void Init()
    {
        _pathInitialized = true;
        if (listPoints.Length >= 2)
        {
            var points = listPoints.ToList();

            if (TargetTransform != null)
                points[points.Count - 1] = new BezierPointVector3()
                {
                    point = points[points.Count - 1].point +
                            (transform.parent as RectTransform).InverseTransformPoint(TargetTransform.position),
                    preAnchor = points[points.Count - 1].preAnchor,
                    nextAnchor = points[points.Count - 1].nextAnchor,
                };

            points[0] = points[0].AddPos(localAppear.localPoint);
            _path = new BezierPathController(points);
            _path.SetUp(10, false);
        }
        else
        {
            Debug.LogError("path must be greater than 2");
        }
    }

    public List<Vector3> GetGizmoVerticeLines()
    {
        var lines = _path.GizmoPoints();
        lines.Insert(0, localAppear.localPoint);
        lines.Insert(0, Vector3.zero);
        return lines;
    }


    public override bool HasOut()
    {
        return false;
    }

    public override bool HasIn()
    {
        return true;
    }

    protected override IEnumerator ThisDoAnimate_In()
    {
        if (!_pathInitialized)
            Init();
        float t = 0;
        transform.localPosition = Vector3.zero;
        transform.localScale = localAppear.startScaleValue;

        if (localAppear.delayBeforeLocalRandom > 0)
            yield return new WaitForSeconds(UnityEngine.Random.Range(0, localAppear.delayBeforeLocalRandom));
        if (settings.DelayBeforeIn > 0)
            yield return CoroutineUtilities.WaitForRealTime(settings.DelayBeforeIn);
        StartInSound();
        if (settings.deactiveAfterOut)
            gameObject.SetActive(true);
        if (fadeTarget != null)
            fadeTarget.color = Color.white;

        while (t < localAppear.timeLocal)
        {
            yield return null;
            var d = t / localAppear.timeLocal;
            transform.localPosition =
                Vector3.Lerp(Vector3.zero, localAppear.localPoint, localAppear.curveLocal.Evaluate(d));
            transform.localScale = Vector3.Lerp(localAppear.startScaleValue, localAppear.localScaleValue,
                localAppear.curveScaleLocal.Evaluate(d));
            t += Time.deltaTime;
        }

        if (localAppear.delayAfterLocal > 0)
            yield return new WaitForSeconds(localAppear.delayAfterLocal);

        t = 0;
        
        TriggerAfterInParticle(time - settings.particleTimeAdjust);

        while (t < time)
        {
            yield return null;
            var d = t / time;
            SetAtT(d);
            t += Time.deltaTime;
        }

        InvokeCallBack();
        
        StartOutSound();
        if (fadeTarget != null)
            yield return fadeTarget.DOFade(0, fadeTime).OnComplete(() => gameObject.SetActive(false))
                .WaitForCompletion();
    }

    public void SetAtT(float t)
    {
        if (t >= 1)
        {
            transform.localPosition = TargetPos();
            transform.localScale = tweenScaleValue;
        }
        else
        {
            transform.localPosition = _path.GetAtPosAtT(curve.Evaluate(t));
            transform.localScale = Vector3.Lerp(localAppear.localScaleValue, tweenScaleValue, curveScale.Evaluate(t));
        }
    }

    public Vector3 TargetPos()
    {
        return (transform.parent as RectTransform).InverseTransformPoint(TargetTransform.position) +
               listPoints[listPoints.Length - 1].point;
    }

    public Vector3 GetPosAtT(float t)
    {
        return _path.GetAtPosAtT(curve.Evaluate(t));
    }


    protected override IEnumerator ThisDoAnimate_Out()
    {
        yield return null;
    }

    protected override IEnumerator ThisDoAnimate_InThenOut()
    {
        yield return ThisDoAnimate_In();
    }

    public void SetTarget(Transform target,int i)
    {
        targetTransforms = new[] {target};
        targetIndex = i;
//        Init();
    }
    
    public void SetLastPoint(Vector3 local)
    {
        listPoints[listPoints.Length-1].point = local;
    }

    public void SetCurrentScale()
    {
        var localScale = transform.localScale;
        localAppear.startScaleValue = localScale;
        localAppear.localScaleValue = localScale;
        tweenScaleValue = localScale;
    }

  
}