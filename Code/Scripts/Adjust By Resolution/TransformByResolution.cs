using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;


[System.Serializable]
public class TransformConfigData
{
    public float aspect;
    public Vector3 position;
    public Vector3 scale;
    public Quaternion quaternion;
    public float camSize;

    public void Apply(Transform transform, bool pos = true, bool scal = true, bool rotate = true)
    {
        if (pos)
            transform.localPosition = position;
        if (scal)
            transform.localScale = scale;
        if (rotate)
            transform.rotation = quaternion;
    }

    public void Apply(Transform transform, TransformConfigData add, bool pos = true, bool scal = true,
        bool rotate = true)
    {
        if (pos)
            transform.localPosition = position + add.position;
        if (scal)
            transform.localScale = scale + add.scale;
        if (rotate)
            transform.rotation = quaternion * add.quaternion;
    }


    public void Apply(Transform transform, Camera camera, bool orthoganic, bool pos = true, bool scal = true,
        bool rotate = true)
    {
        if (pos)
            transform.localPosition = position;
        if (scal)
            transform.localScale = scale;
        if (rotate)
            transform.rotation = quaternion;

        if (orthoganic)
        {
            camera.orthographicSize = camSize;
        }
        else
        {
            camera.fieldOfView = (int) camSize;
        }
    }
}


[System.Serializable]
public class ListTransformConfigData
{
    public string name;
    public List<TransformConfigData> list = new List<TransformConfigData>();
}

public class TransformByResolution : ByResolution
{
    [SerializeField] protected bool disable;
    [SerializeField] protected bool pos = true;
    [SerializeField] protected bool scale = true;
    [SerializeField] protected bool rotate = true;
    [SerializeField] protected List<TransformConfigData> conditions = new List<TransformConfigData>();
    [SerializeField] protected TransformConfigData whenIosNotch;
    [SerializeField] private bool testIosNotch;
    public override void Start()
    {
        base.Start();
        if (transform is RectTransform)
            Debug.LogError("use " + nameof(AnchorPositionByResolution) + " instead");
        if (conditions.Count > 0)
            Apply();
    }

    private void Apply(TransformConfigData condition)
    {
        if (!disable)
        {
#if UNITY_EDITOR
            if (testIosNotch)
            {
                condition.Apply(transform, whenIosNotch, pos, scale, rotate);
                return;
            }
#endif

#if UNITY_IOS
      var notch = DuongExtensions.HaveNotch();
      if(notch)
            condition.Apply(transform,whenIosNotch ,pos, scale, rotate);
        else 
    condition.Apply(transform, pos, scale, rotate);
#else
            condition.Apply(transform, pos, scale, rotate);
#endif
        }
    }

    public override void Apply()
    {
        var condition = conditions[0];
        for (int i = 1; i < conditions.Count; i++)
            if (Mathf.Abs(conditions[i].aspect - Aspect) < Mathf.Abs(condition.aspect - Aspect))
                condition = conditions[i];
        Apply(condition);

        foreach (var transformByResolution in others)
            transformByResolution.Apply();
    }

    public override void AddCurrent()
    {
        var condition = NearestCondition();
        var transform1 = transform;
        if (condition == null)
        {
            conditions.Add(new TransformConfigData()
            {
                aspect = Aspect,
                position = transform1.localPosition,
                scale = transform1.localScale,
                quaternion = transform1.rotation,
            });
        }
        else
        {
            condition.position = transform1.localPosition;
            condition.scale = transform1.localScale;
            condition.quaternion = transform1.rotation;
        }
    }


    public TransformConfigData NearestCondition(float epsilon = 0.001f)
    {
        if (conditions == null)
            conditions = new List<TransformConfigData>();
        if (conditions.Count == 0)
            return null;

        var condition = conditions[0];
        for (int i = 1; i < conditions.Count; i++)
            if (Mathf.Abs(conditions[i].aspect - Aspect) < Mathf.Abs(condition.aspect - Aspect))
                condition = conditions[i];

        if (Mathf.Abs(Aspect - condition.aspect) < epsilon)
            return condition;
        return null;
    }


    [MyButtonVector3(nameof(TestTweenCamera))] [SerializeField]
    protected Vector3 defaultTween = new Vector3(0, 1, 0.5f);

    [SerializeField] private ListTransformConfigData[] listPresets;

    protected List<TransformConfigData> CurrentPresets
    {
        get
        {
            if (currentPresetIndex < listPresets.Length)
                return listPresets[currentPresetIndex].list;
            return null;
        }
    }


    public override void ApplyPreset(int index)
    {
        base.ApplyPreset(index);
        if (CurrentPresets != null)
        {
            if (index > CurrentPresets.Count)
                index = CurrentPresets.Count - 1;
            CurrentPresets[index].Apply(transform, pos, scale, rotate);
        }
    }

    public override void AddPresetTo(int index)
    {
        var transform1 = transform;
        if (index < CurrentPresets.Count)
            CurrentPresets[index] = new TransformConfigData()
            {
                aspect = Aspect,
                position = transform1.localPosition,
                scale = transform1.localScale,
                quaternion = transform1.rotation,
                camSize = sourceCamera != null
                    ? (sourceCamera.orthographic ? sourceCamera.orthographicSize : sourceCamera.fieldOfView)
                    : 5
            };
        else
            AddPreset();

        this.SetDirty();
    }

    public override void AddPreset()
    {
        var transform1 = transform;
        CurrentPresets.Add(new TransformConfigData()
        {
            aspect = Aspect,
            position = transform1.localPosition,
            scale = transform1.localScale,
            quaternion = transform1.rotation,
            camSize = sourceCamera != null
                ? (sourceCamera.orthographic ? sourceCamera.orthographicSize : sourceCamera.fieldOfView)
                : 5
        });
        this.SetDirty();
    }


    protected void TestTweenCamera(Vector3 vec)
    {
        TweenCamera(new IntIntFloat(vec)).Subscribe();
    }


    public IObservable<Unit> IoDefaultTween()
    {
        return TweenCamera(new IntIntFloat(defaultTween));
    }

    private IEnumerator Null()
    {
        yield return null;
    }

    public IObservable<Unit> IoTween(Vector3 tween)
    {
        return TweenCamera(new IntIntFloat(tween));
    }

    [SerializeField] protected AnimationCurve curve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1));
    [SerializeField] private bool turnOffTween;

    protected virtual IObservable<Unit> TweenCamera(IntIntFloat tween)
    {
        var from = CurrentPresets[tween.value];
        var to = CurrentPresets[tween.value2];
        var tf = transform;
        if (!turnOffTween)
        {
            return Observable.FromCoroutine(() => DuongExtensions.IEDoValue(0, 1, tween.value3, data =>
            {
                if (pos)
                    tf.localPosition = Vector3.Lerp(from.position, to.position, data);
                if (scale)
                    tf.localScale = Vector3.Lerp(from.scale, to.scale, data);
                if (rotate)
                    tf.rotation = Quaternion.Lerp(from.quaternion, to.quaternion, data);
                if (sourceCamera != null)
                    if (sourceCamera.orthographic)
                        sourceCamera.orthographicSize = Mathf.Lerp(from.camSize, to.camSize, data);
                    else
                        sourceCamera.fieldOfView = (int) Mathf.Lerp(from.camSize, to.camSize, data);
            }, curve));
        }

        if (pos)
            tf.localPosition = to.position;
        if (scale)
            tf.localScale = to.scale;
        if (rotate)
            tf.rotation = to.quaternion;
        if (sourceCamera != null)
            if (sourceCamera.orthographic)
                sourceCamera.orthographicSize = to.camSize;
            else
                sourceCamera.fieldOfView = (int) to.camSize;
        return DuongExtensions.IoExecuteInNextFrame(null);
    }
}