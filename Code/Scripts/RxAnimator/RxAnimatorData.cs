
using System;
using DG.Tweening;
using UnityEngine;

[Serializable]
public class RxAnimatorFade
{
    public bool active;

    [ConditionalField(nameof(active), new object[] {true}, false, false)]
    public bool turnOffRayCast = true;

    [ConditionalField(nameof(active), new object[] {true}, false, false)]
    public float value;

    [ConditionalField(nameof(active), new object[] {true}, false, false)]
    public float time = .3f;

    [ConditionalField(nameof(active), new object[] {true}, false, false)]
    public float delay;

    [ConditionalField(nameof(active), new object[] {true}, false, false)]
    public float beforeAfterDelay;

    [ConditionalField(nameof(active), new object[] {true}, false, false)]
    public Ease ease = Ease.Linear;


    [ConditionalField(nameof(active), new object[] {true}, false, false)]
    public bool useCurve;

    [ConditionalField(nameof(active), new object[] {true}, false, false)]
    public AnimationCurve curve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1));

    public static readonly string[] Display =
    {
        nameof(active),
        nameof(turnOffRayCast),
        nameof(value),
        nameof(time),
        nameof(delay),
        nameof(delay),
        nameof(beforeAfterDelay),
        nameof(ease),
    };
}


[Serializable]
public class RxAnimatorMove
{
    public bool active;

    // Move In
    [ConditionalField(nameof(active), new object[] {true}, false, false)]
    public AnimatorMoveType type;

    [ConditionalField(nameof(active), new object[] {true}, false, false)]
    public Vector3 value;

    [ConditionalField(nameof(active), new object[] {true}, false, false)]
    public float time = .3f;

    [ConditionalField(nameof(active), new object[] {true}, false, false)]
    public float delay;

    [ConditionalField(nameof(active), new object[] {true}, false, false)]
    public float beforeAfterDelay;

    [ConditionalField(nameof(active), new object[] {true}, false, false)]
    public Ease ease = Ease.Linear;

    [ConditionalField(nameof(active), new object[] {true}, false, false)]
    public LocationEnum locationEnum;

    [ConditionalField(nameof(active), new object[] {true}, false, false)]
    public bool useCurve;

    [ConditionalField(nameof(active), new object[] {true}, false, false)]
    public AnimationCurve curve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1));

    public static readonly string[] Display =
    {
        nameof(active),
        nameof(type),
        nameof(value),
        nameof(time),
        nameof(delay),
        nameof(beforeAfterDelay),
        nameof(ease),
        nameof(locationEnum),
    };

    public Vector3 GetTarget(Transform transform, bool local,Vector3 startPos)
    {
        switch (type)
        {
            case AnimatorMoveType.SpecificPosition:
                return value.RemoveZ();
            case AnimatorMoveType.ParentPosition:
                return local ? Vector3.zero : transform.parent.position.RemoveZ();
            case AnimatorMoveType.CurrentPosition:
                return local ? transform.localPosition.RemoveZ() : transform.position.RemoveZ();
            case AnimatorMoveType.LocationManager:
                var location = RootView.rootView.screenRoot.GetLocation(locationEnum);
                if (location != null)
                {
                    if (local)
                        return location.localPosition.RemoveZ();
                    return location.position.RemoveZ();
                }
                else
                {
                    Debug.LogError("not found location");
                    return Vector3.zero;
                }
            case AnimatorMoveType.AddPosition:
                return startPos + value;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
    
    public Vector3 GetTargetRt(RectTransform rt, bool local,Vector3 startPos)
    {
        switch (type)
        {
            case AnimatorMoveType.SpecificPosition:
                return value.RemoveZ();
            case AnimatorMoveType.ParentPosition:
                return local ? Vector3.zero : rt.parent.position.RemoveZ();
            case AnimatorMoveType.CurrentPosition:
                return local ? rt.localPosition.RemoveZ() : rt.position.RemoveZ();
            case AnimatorMoveType.LocationManager:
                return RootView.rootView.screenRoot.GetLocationManager()
                           .GetRelativeLocalPosition(locationEnum, rt.parent) + value;
            case AnimatorMoveType.AddPosition:
                return startPos + value;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}

[Serializable]
public class RxAnimatorScale
{
    public bool active;

    [ConditionalField(nameof(active), new object[] {true}, false, false)]
    public Vector3 value;

    [ConditionalField(nameof(active), new object[] {true}, false, false)]
    public float time = .3f;

    [ConditionalField(nameof(active), new object[] {true}, false, false)]
    public float delay;

    [ConditionalField(nameof(active), new object[] {true}, false, false)]
    public float beforeAfterDelay;

    [ConditionalField(nameof(active), new object[] {true}, false, false)]
    public Ease ease = Ease.Linear;

    [ConditionalField(nameof(active), new object[] {true}, false, false)]
    public bool pop;

    [ConditionalField(nameof(active), new object[] {true}, false, false)]
    public bool useCurve;

    [ConditionalField(nameof(active), new object[] {true}, false, false)]
    public AnimationCurve curve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1));

    public static readonly string[] Display =
    {
        nameof(active),
        nameof(value),
        nameof(time),
        nameof(delay),
        nameof(beforeAfterDelay),
        nameof(ease),
        nameof(pop),
    };
}

