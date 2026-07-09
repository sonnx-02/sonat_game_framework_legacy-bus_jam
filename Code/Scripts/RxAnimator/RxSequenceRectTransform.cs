using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;

public enum RxSequenceRtAction
{
    Move = 1,
    ScaleTo = 2,
    RotateTo = 3,
    MoveToTarget = 4,
    MoveToInject = 5,
    TriggerInjectScript = 6,
    StartAnimation = 7,
    StartAction = 8,
    ProgressScript = 9,
}

[System.Serializable]
public class RxSequenceRtNode
{
    public RxSequenceRtAction action;
    public float delayBefore;
    public float delayAfter;
    public float time;
    public Vector3 value; // adjust
    public ParameterHandler parameterHandler;
    [FormerlySerializedAs("startValue")] 
    [ConditionalField(nameof(parameterHandler),new object[]{ParameterHandler.None},true)] public Vector3 parameter; // adjust
    public IntInt indexValue;
    public bool once;
    public bool parallel;
    public int curve = -1;
    public AnimationSpace space;
    public ActionScriptTriggerType triggerType;

    public enum AnimationSpace
    {
        Local,
        Anchor,
        World
    }
    
    public enum ParameterHandler
    {
        None,
        SetStart,
        AddPingPong,
    }
}


[System.Serializable]
public class RxSequenceRtNodeCollection
{
    [ArrayElementTitle("action")] [SerializeField]
    public RxSequenceRtNode[] nodes;
}

[System.Serializable]
public class RxSequenceRtNodeCollectionKey : RxSequenceRtNodeCollection
{
    public string key;
}

public class RxSequenceRectTransform : BaseRxAnimator
{
    private bool loop = false;
    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private RxSequenceRtNodeCollection action;
    [SerializeField] private RxSequenceRtNodeCollectionKey[] otherActions;
    private int _overrideNode;

    private RxSequenceRtNodeCollection Node()
    {
        if (_overrideNode == 0)
            return action;
        return otherActions[_overrideNode - 1];
    }

    [SerializeField] private AnimationCurve[] curves;
    [SerializeField] private RectTransform[] targets;
    [SerializeField] private RxSetPosAnimator[] triggersPos;
    [SerializeField] private ProgressScript[] progressScripts;
    [SerializeField] private BaseBaseActionScript[] actionScripts;
    [SerializeField] private GameObject[] turnOffs;
    [SerializeField] private BaseRxAnimator[] anims;

    public RectTransform[] Targets => targets;
    private Vector3[] _injectValues;

    public void TurnOff()
    {
        gameObject.SetActive(false);
        foreach (var turnOff in turnOffs)
            turnOff.SetActive(false);
    }

    public RxSequenceRectTransform Inject(Vector3[] injects)
    {
        _injectValues = injects;
        return this;
    }

    public override bool HasIn()
    {
        return true;
    }

    public override void Animator_In()
    {
        base.Animator_In();
        _overrideNode = 0;
    }

    [MyButtonInt(nameof(Animator_In2))] [SerializeField]
    private int testNode;

    public void Animator_In2(int overrideNode)
    {
        _overrideNode = overrideNode;
        base.Animator_In();
    }

    private void SetPos(RectTransform rt, RxSequenceRtNode.AnimationSpace space, Vector2 value)
    {
        switch (space)
        {
            case RxSequenceRtNode.AnimationSpace.Local:
                rt.localPosition = value;
                break;
            case RxSequenceRtNode.AnimationSpace.Anchor:
                rt.anchoredPosition = value;
                break;
            case RxSequenceRtNode.AnimationSpace.World:
                rt.position = value;
                break;
        }
    }

    protected override IEnumerator ThisDoAnimate_In()
    {
        if (settings.deactiveAfterOut)
            gameObject.SetActive(true);

        var thisNode = Node();
        if (thisNode.nodes.Length == 0)
            Debug.LogError("not valid nodes");
        else
        {
            bool[] skips = new bool[thisNode.nodes.Length];
            do
            {
                if (!gameObject.activeInHierarchy)
                    break;
                for (var i = 0; i < thisNode.nodes.Length; i++)
                {
                    if (skips[i])
                        continue;

                    var node = thisNode.nodes[i];
                    if (node.once)
                        skips[i] = true;

                    if (node.delayBefore > 0)
                        yield return new WaitForSeconds(node.delayBefore);

                    float t = 0;
                    bool move = false;
                    Vector3 targetPos = node.value;
                    Vector3 startPos = Vector3.zero;
                    switch (node.action)
                    {
                        case RxSequenceRtAction.Move:
                            move = !node.parallel;
                            if (node.parameterHandler == RxSequenceRtNode.ParameterHandler.SetStart)
                                SetPos(rectTransform, node.space, node.parameter);
                            switch (node.space)
                            {
                                case RxSequenceRtNode.AnimationSpace.Local:
                                    startPos = rectTransform.localPosition;
                                    break;
                                case RxSequenceRtNode.AnimationSpace.Anchor:
                                    startPos = rectTransform.anchoredPosition;
                                    break;
                                case RxSequenceRtNode.AnimationSpace.World:
                                    startPos = rectTransform.position;
                                    break;
                            }

                            if (node.parallel)
                            {
                                switch (node.space)
                                {
                                    case RxSequenceRtNode.AnimationSpace.Local:
                                        if (node.curve >= 0)
                                            rectTransform.DOLocalMove(targetPos, node.time).SetEase(curves[node.curve]);
                                        else
                                            rectTransform.DOLocalMove(targetPos, node.time);
                                        break;
                                    case RxSequenceRtNode.AnimationSpace.Anchor:
                                        if (node.curve >= 0)
                                            rectTransform.DOAnchorPos(targetPos, node.time).SetEase(curves[node.curve]);
                                        else
                                            rectTransform.DOAnchorPos(targetPos, node.time);
                                        break;
                                    case RxSequenceRtNode.AnimationSpace.World:
                                        if (node.curve >= 0)
                                            rectTransform.DOMove(targetPos, node.time).SetEase(curves[node.curve]);
                                        else
                                            rectTransform.DOMove(targetPos, node.time);
                                        break;
                                }
                            }

                            break;
                        case RxSequenceRtAction.ScaleTo:
                            if (node.parameterHandler == RxSequenceRtNode.ParameterHandler.SetStart)

                                rectTransform.localScale = node.parameter;
                            if (node.time <= 0)
                                rectTransform.localScale = node.value;
                            else
                            {
                                if (!node.parallel)
                                {
                                    Vector2 startValue = rectTransform.localScale;
                                    while (t < node.time)
                                    {
                                        var tValue = Vector3.Lerp(startValue, node.value, node.curve >= 0
                                            ? curves[node.curve].Evaluate(t / node.time)
                                            : t / node.time);
                                        rectTransform.localScale = tValue;
                                        yield return null;
                                        t += Time.deltaTime;
                                    }

                                    rectTransform.localScale = node.value;
                                }
                                else
                                {
                                    if (node.curve >= 0)
                                        rectTransform.DOScale(targetPos, node.time).SetEase(curves[node.curve])
                                            .SetDelay(node.delayBefore);
                                    else
                                        rectTransform.DOScale(targetPos, node.time)
                                            .SetDelay(node.delayBefore);
                                }
                            }

                            break;
                        case RxSequenceRtAction.RotateTo:
                            if (node.parameterHandler == RxSequenceRtNode.ParameterHandler.SetStart)

                                rectTransform.localEulerAngles = node.parameter;
                            if (node.time <= 0)
                                rectTransform.localEulerAngles = node.value;
                            else
                            {
                                Vector2 startValue = rectTransform.localEulerAngles;
                                while (t < node.time)
                                {
                                    var tValue = Vector3.Lerp(startValue, node.value, node.curve >= 0
                                        ? curves[node.curve].Evaluate(t / node.time)
                                        : t / node.time);
                                    rectTransform.localEulerAngles = tValue;
                                    yield return null;
                                    t += Time.deltaTime;
                                }

                                rectTransform.localEulerAngles = node.value;
                            }

                            break;
                        case RxSequenceRtAction.StartAction:
                            actionScripts[node.indexValue.value].StartAction(node.triggerType, node.indexValue.value2);
                            break;
                        case RxSequenceRtAction.MoveToTarget:
                            if (node.parameterHandler == RxSequenceRtNode.ParameterHandler.SetStart)

                                SetPos(rectTransform, node.space, node.parameter);
                            move = true;
                            startPos = rectTransform.localPosition;
                            targetPos = rectTransform.parent.InverseTransformPoint(
                                            targets[node.indexValue.value].position) + node.value;
                            break;
                        case RxSequenceRtAction.MoveToInject:
                            if (node.parameterHandler == RxSequenceRtNode.ParameterHandler.SetStart)
                                SetPos(rectTransform, node.space, node.parameter);
                            move = true;
                            startPos = rectTransform.localPosition;
                            targetPos = rectTransform.parent.InverseTransformPoint(
                                            _injectValues[node.indexValue.value]) + node.value;
                            break;
                        case RxSequenceRtAction.TriggerInjectScript:
                            triggersPos[node.indexValue.value].gameObject.SetActive(true);
                            triggersPos[node.indexValue.value]
                                .SetPos(_injectValues[node.indexValue.value2] + node.value).Animator_In();
                            break;
                        case RxSequenceRtAction.StartAnimation:
                            anims[node.indexValue.value].Animator_In();
                            break;
                        case RxSequenceRtAction.ProgressScript:
                            progressScripts[node.indexValue.value].Init();
                            while (t < node.time)
                            {
                                var tValue = Mathf.Lerp(0, 1,
                                    node.curve >= 0
                                        ? curves[node.curve].Evaluate(t / node.time)
                                        : t / node.time);
                                progressScripts[node.indexValue.value].OnChanged(tValue);
                                yield return null;
                                t += Time.deltaTime;
                            }
                            progressScripts[node.indexValue.value].OnChanged(1);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    if (move)
                    {
                        if (node.time <= 0)
                            SetPos(rectTransform, node.space, targetPos);
                        else
                        {
                            while (t < node.time)
                            {
                                var tValue = Vector3.Lerp(startPos, targetPos,
                                    node.curve >= 0
                                        ? curves[node.curve].Evaluate(t / node.time)
                                        : t / node.time);
                                SetPos(rectTransform, node.space, tValue);
                                yield return null;
                                t += Time.deltaTime;
                            }

                            SetPos(rectTransform, node.space, targetPos);
                        }
                    }

                    if (node.delayAfter > 0)
                        yield return new WaitForSeconds(node.delayAfter);
                }
            } while (loop);
        }
    }

    protected override IEnumerator ThisDoAnimate_Out()
    {
        yield return null;
    }

    protected override IEnumerator ThisDoAnimate_InThenOut()
    {
        yield return ThisDoAnimate_In();
    }

    public override bool HasOut()
    {
        return true;
    }
}