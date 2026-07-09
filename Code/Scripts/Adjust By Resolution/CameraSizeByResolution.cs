using System;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.Serialization;

public abstract class PresetSettingView : BaseFrameworkView, IActionScript
{
    // Presettttttttttttttttttttttttt
    [MyButtonRange(0, 10, nameof(ApplyPreset), null)] [SerializeField]
    private int testApplyPreset;

    [MyButtonRange(0, 10, nameof(AddPresetTo), null)] [SerializeField]
    private int testAddPreset;
    [SerializeField] protected int currentPresetIndex;
    [SerializeField] [DisplayAsDisable] protected int currentIndex;


    public Vector2 source;
    public Vector3 desAndX;

    public override void LogCustom()
    {
        base.LogCustom();
        
        Debug.LogError(DuongExtensions.Remap(source.x,source.y,desAndX.x,desAndX.y,desAndX.z));
        
    }

    public virtual void ApplySetting(int current, int index)
    {
        currentPresetIndex = current;
        currentIndex = index;
        ApplyPreset(index);
    }

    public virtual void ApplyPreset(int index)
    {
        currentIndex = index;
       // CurrentPresets[preset].Apply(transform, pos, scale, rotate);
    }

    public virtual void AddPresetTo(int index)
    {
//        var transform1 = transform;
//        if (index < CurrentPresets.Count)
//            CurrentPresets[index] = new TransformConfigData()
//            {
//                aspect = Aspect,
//                position = transform1.localPosition,
//                scale = transform1.localScale,
//                quaternion = transform1.rotation,
//                camSize = sourceCamera != null
//                    ? (sourceCamera.orthographic ? sourceCamera.orthographicSize : sourceCamera.fieldOfView)
//                    : 5
//            };
//        else
//            AddPreset();
//      
//        this.SetDirty();
    }
    
    public virtual void AddPreset()
    {
//        var transform1 = transform;
//        CurrentPresets.Add(new TransformConfigData()
//        {
//            aspect = Aspect,
//            position = transform1.localPosition,
//            scale = transform1.localScale,
//            quaternion = transform1.rotation,
//            camSize = sourceCamera != null
//                ? (sourceCamera.orthographic ? sourceCamera.orthographicSize : sourceCamera.fieldOfView)
//                : 5
//        });
//        this.SetDirty();
    }

    public abstract void StartAction();
}


public abstract class ByResolution : PresetSettingView
{
    [FormerlySerializedAs("c")]
    [FormerlySerializedAs("cam")] 
    [SerializeField] protected Camera sourceCamera;
    [SerializeField] protected bool applyOnEnable;

    private void OnEnable()
    {
        if (applyOnEnable)
            Apply();
    }

    [SerializeField] protected ByResolution[] others = new ByResolution[0];


    protected virtual Camera Cam
    {
        get
        {
            #if UNITY_EDITOR
                if(!Application.isPlaying)
                  return  FindObjectOfType<RootView>().screenRoot.playerCam;
            #endif
            if (sourceCamera != null)
                return sourceCamera;
            return RootView.rootView.screenRoot.playerCam;
        }
    }

    protected float Aspect => Cam.pixelWidth * 1f / Cam.pixelHeight;

    [MyButton(nameof(AddCurrent), nameof(AddCurrentOther), nameof(Apply))]
    public int testFunction2;

    public virtual void AddCurrentOther()
    {
        AddCurrent();
        foreach (var byResolution in others)
            byResolution.AddCurrent();
    }

    public abstract void AddCurrent();
    public abstract void Apply();

    public override void StartAction()
    {
        Apply();

    }
}


[RequireComponent(typeof(Camera))]
public class CameraSizeByResolution : ByResolution
{
    public bool usePos;
    public List<CameraSizeByResolutionCondition> conditions;

    [System.Serializable]
    public class CameraSizeByResolutionCondition
    {
        public float aspect;
        public float size;
        public float posY;
        public float addPosYIosNotch;
    }

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
            if(others[i] is CanvasScalerByResolution)
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
            conditions.Add(new CameraSizeByResolutionCondition()
            {
                aspect = Aspect,
                size = Cam.orthographicSize,
                posY = transform.localPosition.y
            });
        }
        else
        {
            condition.size = Cam.orthographicSize;
            condition.posY = transform.localPosition.y;
        }

        this.SetDirty();
    }

    void Apply(CameraSizeByResolutionCondition condition)
    {
        GetComponent<Camera>().orthographicSize = condition.size;
        FpsDebugController.message = Aspect + "/ CameraAspect" + GetComponent<Camera>().aspect + "/" +
                                     condition.aspect + "/" + condition.size;

        if (usePos)
        {
            var localPosition = transform.localPosition;
            localPosition = new Vector3(localPosition.x, condition.posY, localPosition.z);
#if UNITY_IOS
                if(HaveNotch())
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

    public CameraSizeByResolutionCondition NearestCondition(float epsilon = 0.001f)
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