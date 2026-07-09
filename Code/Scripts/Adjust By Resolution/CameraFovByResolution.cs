using System;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.Serialization;

[System.Serializable]
public class CameraFovByResolutionCondition
{
    public float aspect;
    public float fov;
    public float posY;
    public float addPosYIosNotch;
}

[RequireComponent(typeof(Camera))]
public class CameraFovByResolution : ByResolution
{
    public bool usePos;

    [ArrayElementTitle("aspect")] [SerializeField]
    public List<CameraFovByResolutionCondition> conditions;

#if UNITY_EDITOR
    [SerializeField] protected ByResolution[] onEditor = new ByResolution[0];
    [SerializeField] protected ToggleScript[] onEditor2;
#endif

    public override void Start()
    {
        base.Start();
        if (conditions.Count > 0)
            Apply();

        for (var i = 0; i < others.Length; i++)
        {
            if (others[i] is CanvasScalerByResolution)
                Debug.LogError("remove this, cause conflict");
        }
    }


    public override void Apply()
    {
        var condition = conditions[0];
        for (int i = 1; i < conditions.Count; i++)
            if (Mathf.Abs(conditions[i].aspect - Aspect) < Mathf.Abs(condition.aspect - Aspect))
                condition = conditions[i];
        Apply(condition);


        if (others != null)
            foreach (var transformByResolution in others)
                transformByResolution.Apply();

#if UNITY_EDITOR
        foreach (var byResolution in onEditor)
            byResolution.Apply();
        onEditor2.OnChanged(true);
        if (!Application.isPlaying)
        {
            gameObject.SetActive(false);
            gameObject.SetActive(true);
        }
#endif
    }

    public override void AddCurrent()
    {
        if (others == null)
            others = new ByResolution[0];
        foreach (var transformByResolution in others)
            transformByResolution.AddCurrent();

        var condition = NearestCondition();
        if (condition == null)
        {
            conditions.Add(new CameraFovByResolutionCondition()
            {
                aspect = Aspect,
                fov = Cam.fieldOfView,
                posY = transform.localPosition.y
            });
        }
        else
        {
            condition.fov = Cam.fieldOfView;
            condition.posY = transform.localPosition.y;
        }

        this.SetDirty();
    }

    void Apply(CameraFovByResolutionCondition condition)
    {
        GetComponent<Camera>().fieldOfView = condition.fov;
        FpsDebugController.message = Aspect + "/ CameraAspect" + GetComponent<Camera>().aspect + "/" +
                                     condition.aspect + "/" + condition.fov;

        if (usePos)
        {
            var localPosition = transform.localPosition;
            localPosition = new Vector3(localPosition.x, condition.posY, localPosition.z);
#if UNITY_IOS
            if (HaveNotch())
                localPosition = new Vector3(localPosition.x, condition.posY, localPosition.z);
            else
                localPosition =
                    new Vector3(localPosition.x, condition.posY + condition.addPosYIosNotch, localPosition.z);
#else
            localPosition = new Vector3(localPosition.x, condition.posY, localPosition.z);
#endif
            transform.localPosition = localPosition;
        }
    }

    public static bool HaveNotch()
    {
        return Screen.safeArea.height < Screen.height;
    }

    private CameraFovByResolutionCondition NearestCondition(float epsilon = 0.001f)
    {
        var aspect = Cam.pixelWidth * 1f / Cam.pixelHeight;

        if (conditions.Count == 0)
            return null;

        var condition = conditions[0];
        for (int i = 1; i < conditions.Count; i++)
            if (Mathf.Abs(conditions[i].aspect - aspect) < Mathf.Abs(condition.aspect - aspect))
                condition = conditions[i];

        if (Mathf.Abs(aspect - condition.aspect) < epsilon)
            return condition;
        else
            return null;
    }
}