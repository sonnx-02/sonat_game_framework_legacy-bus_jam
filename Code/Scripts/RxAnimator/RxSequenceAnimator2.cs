using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;


public enum RxSequenceAction2
{
    SetPosition = 0,
    Move = 1,
    ScaleTo = 2,
    RotateTo = 3,
    TriggerActionTrue = 4,
    TriggerActionFalse = 5,
    MoveToTarget = 6,
    MoveToInject = 7,
    TriggerInjectScript = 8,
    RunProgressScript = 9,
    SetProgressScript = 10,
    StartAnimation = 11,
    AnchorPosZero = 12,
}

[System.Serializable]
public class RxSequenceNode2
{
    public RxSequenceAction2 action;
    public float delayBefore;
    public float delayAfter;
    public float time;
    public bool setStartValue; // set start value or move from current
    public Vector3 startValue; // adjust
    public Vector3 value3; // adjust
    public Vector3Int indexes;
    public bool once;
    public int curve = -1;
}

[System.Serializable]
public class RxSequenceNode2Collection
{
    public string key;

    [ArrayElementTitle("action")] [SerializeField]
    public RxSequenceNode2[] nodes;
}


public class RxSequenceAnimator2 : BaseRxAnimator
{
    [SerializeField] private bool loop = true;

    [HideInInspector] [DisplayAsDisable] [SerializeField] private RxSequenceNode2[] nodes; // obsolete


    [SerializeField] private RxSequenceNode2Collection action;
    [SerializeField] private RxSequenceNode2Collection[] otherActions;
    private int _overrideNode;

    private RxSequenceNode2Collection Node()
    {
        if (_overrideNode == 0)
            return action;
        return otherActions[_overrideNode - 1];
    }

    [SerializeField] private AnimationCurve[] curves;
    [SerializeField] private Transform[] targets;
    [SerializeField] private RectTransform[] rtTargets;
    [SerializeField] private ToggleScript[] triggers;
    [SerializeField] private RxSetPosAnimator[] triggersPos;
    [SerializeField] private ProgressScript[] progressScripts;
    [SerializeField] private BaseBooleanActionScript[] actionScripts;
    [SerializeField] private GameObject[] turnOffs;
    [SerializeField] private RxAnimator[] anims;
    [SerializeField] private RectTransform rt;

    public Transform[] Targets => targets;
    private Vector3[] _injectValues;

    [SerializeField] private bool isLocalPos;

    public void TurnOff()
    {
        gameObject.SetActive(false);
        foreach (var turnOff in turnOffs)
            turnOff.SetActive(false);
    }

    public RxSequenceAnimator2 Inject(Vector3[] injects)
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
    private int testsequence;

    public void Animator_In2(int overrideNode)
    {
        _overrideNode = overrideNode;
        base.Animator_In();
    }

    protected override IEnumerator ThisDoAnimate_In()
    {
        if (settings.deactiveAfterOut)
            gameObject.SetActive(true);

        var thisNode = Node();
        if (thisNode.nodes.Length == 0)
        {
            Debug.LogError("not valid nodes");
        }
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
                    Vector3 vec;
                    switch (node.action)
                    {
                        case RxSequenceAction2.SetPosition:
                            if (isLocalPos)
                                transform.localPosition = node.value3;
                            else
                                transform.position = node.value3;
                            break;
                        case RxSequenceAction2.Move:

                            if (node.setStartValue)
                            {
                                if (isLocalPos)
                                    transform.localPosition = node.startValue;
                                else
                                    transform.position = node.startValue;
                            }

                            if (isLocalPos)
                                vec = transform.localPosition;
                            else
                                vec = transform.position;
                            while (t < node.time)
                            {
                                if (isLocalPos)
                                    transform.localPosition = Vector3.Lerp(vec, node.value3, t / node.time);
                                else
                                    transform.position = Vector3.Lerp(vec, node.value3, t / node.time);
                                yield return null;
                                t += Time.deltaTime;
                            }

                            if (isLocalPos)
                                transform.localPosition = node.value3;
                            else
                                transform.position = node.value3;

                            break;
                        case RxSequenceAction2.ScaleTo:

                            if (node.setStartValue)
                                transform.localScale = node.startValue;

                            vec = transform.localScale;
                            while (t < node.time)
                            {
                                transform.localScale = Vector3.Lerp(vec, node.value3, t / node.time);
                                yield return null;
                                t += Time.deltaTime;
                            }

                            transform.localScale = node.value3;
                            break;
                        case RxSequenceAction2.RotateTo:
                            if (node.setStartValue)
                                transform.eulerAngles = node.startValue;
                            vec = transform.eulerAngles;
                            while (t < node.time)
                            {
                                transform.eulerAngles = Vector3.Lerp(vec, node.value3, t / node.time);
                                yield return null;
                                t += Time.deltaTime;
                            }

                            transform.eulerAngles = node.value3;
                            break;
                        case RxSequenceAction2.TriggerActionTrue:
                            if (node.indexes.x < triggers.Length)
                                triggers[node.indexes.x].OnChanged(true);
                            break;
                        case RxSequenceAction2.TriggerActionFalse:
                            if (node.indexes.x < triggers.Length)
                                triggers[node.indexes.x].OnChanged(false);
                            break;
                        case RxSequenceAction2.MoveToTarget:
                            if (rt != null)
                            {
                                vec = rt.anchoredPosition;
                                var targetPos = rt.parent.InverseTransformPoint(rtTargets[node.indexes.x].position) + node.value3;
                              //  var target2 = rtTargets[node.indexes.x].position + node.value3;
                                while (t < node.time)
                                {
                                    t += Time.deltaTime;
                                    rt.anchoredPosition = Vector3.Lerp(vec, targetPos, t / node.time);
                                    yield return null;
                                }
                                rt.anchoredPosition = targetPos;
                            }
                            else
                            {
                                var transform1 = transform;
                                vec = isLocalPos ? transform1.localPosition : transform1.position;
                                var target = targets[node.indexes.x].position + node.value3;
                                while (t < node.time)
                                {
                                    transform.position = Vector3.Lerp(vec, target, t / node.time);
                                    yield return null;
                                    t += Time.deltaTime;
                                }
                                transform.position = target;
                            }
                            break;
                        case RxSequenceAction2.MoveToInject:
                            if (rt != null)
                            {
                                if (node.setStartValue)
                                    rt.anchoredPosition = node.startValue;

                                vec = rt.anchoredPosition;
                                var target2 = _injectValues[node.indexes.x] + node.value3;
                                while (t < node.time)
                                {
                                    t += Time.deltaTime;
                                    rt.anchoredPosition = Vector3.Lerp(vec, target2, t / node.time);
                                    yield return null;
                                }
                                rt.anchoredPosition = target2;
                                break;
                            }
                            else
                            {
                                if (node.setStartValue)
                                {
                                    if (isLocalPos)
                                        transform.localPosition = node.startValue;
                                    else
                                        transform.position = node.startValue;
                                }

                            
                                vec = isLocalPos ? transform.localPosition : transform.position;
                                var target2 = _injectValues[node.indexes.x] + node.value3;
                                while (t < node.time)
                                {
                                    t += Time.deltaTime;
                                    if (isLocalPos)
                                        transform.position = Vector3.Lerp(vec, target2, t / node.time);
                                    else
                                        transform.localPosition = Vector3.Lerp(vec, target2, t / node.time);
                                    yield return null;
                                }

                                if (isLocalPos)
                                    transform.position = target2;
                                else
                                    transform.localPosition = target2;
                                break;
                            }
                        case RxSequenceAction2.TriggerInjectScript:
                            triggersPos[node.indexes.x].gameObject.SetActive(true);
                            triggersPos[node.indexes.x].SetPos(_injectValues[node.indexes.y] + node.value3)
                                .Animator_In();
                            break;
                        case RxSequenceAction2.RunProgressScript:
                            var progressScript = progressScripts[node.indexes.x];
                            if (node.setStartValue)
                                progressScript.OnChanged(node.startValue.x);
                            
                            if (node.curve >= 0)
                            {
                                var curve = curves[node.curve];
                                while (t < node.time)
                                {
                                    progressScript.OnChanged(curve.Evaluate(t / node.time));
                                    yield return null;
                                    t += Time.deltaTime;
                                }

                                progressScript.OnChanged(1);
                            }
                            else
                            {
                                while (t < node.time)
                                {
                                    progressScript.OnChanged(t / node.time);
                                    yield return null;
                                    t += Time.deltaTime;
                                }
                                progressScript.OnChanged(1);
                            }

                            break;
                        case RxSequenceAction2.SetProgressScript:
                             progressScripts[node.indexes.x].OnChanged(node.value3.x);
                             break;
                        case RxSequenceAction2.StartAnimation:
                            anims[node.indexes.x].Animator_In();
                            break;
                        case RxSequenceAction2.AnchorPosZero:
                            rt.localPosition = Vector3.zero;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
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