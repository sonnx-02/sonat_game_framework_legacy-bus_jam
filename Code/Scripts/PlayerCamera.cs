using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UniRx;
using UnityEngine;
using UnityEngine.Serialization;


[System.Serializable]
public class PlayerCameraSetting
{
    public Vector3 localPosition;
    public Quaternion rotation;
    public float fieldOfView;
    public AnimationCurve fovCurve = AnimationCurve.Linear(0,0,1,1);
    public static PlayerCameraSetting Copy(Camera cam)
    {
        var transform = cam.transform;
        return new PlayerCameraSetting()
        {
            localPosition = transform.localPosition,
            rotation = transform.rotation,
            fieldOfView = cam.fieldOfView,
            fovCurve = AnimationCurve.Linear(0,0,1,1)
        };
    }

    public void Apply(Camera cam)
    {
        var transform = cam.transform;
        transform.localPosition = localPosition;
        transform.rotation = rotation;
        cam.fieldOfView = fieldOfView;
    }
}

[Serializable]
public class PlayerCameraSettingByScreen
{
    public float aspect;
    [FormerlySerializedAs("steps")] public PlayerCameraSetting[] settings;
}


public class PlayerCamera : ByResolution
{
    public Camera cam;
    protected override Camera Cam => cam;

    public List<PlayerCameraSettingByScreen> conditions;

    private PlayerCameraSettingByScreen _currentSetting;
   


    protected override void Register()
    {
        base.Register();
        _currentSetting = NearestCondition(int.MaxValue);
        progress.Subscribe(SetProgress);
    }
    


    [SerializeField] [MyButton(nameof(Clear))]
    private int currentIndex2;

    private void Clear()
    {
        _currentSetting = null;
    }

   [MyButtonRange(0, 4, nameof(AddCurrentByIndex), null,"Add ")] public int testAdd;
   [MyButtonRange(0, 4, nameof(ApplyCurrentByIndex), null,"Apply ")] public int testApply;
    
    private void AddCurrentByIndex(int index)
    {
        currentIndex2 = index;
        AddCurrent();
    }
    
    private void ApplyCurrentByIndex(int index)
    {
        currentIndex2 = index;
        Apply();
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
            conditions.Add(new PlayerCameraSettingByScreen()
            {
                aspect = Aspect,
                settings = new[]
                {
                    PlayerCameraSetting.Copy(Cam)
                }
            });
        }
        else
        {
            condition.settings[currentIndex] = PlayerCameraSetting.Copy(Cam);
        }

        this.SetDirty();
    }

    public override void Apply()
    {
        if(_currentSetting == null)
            _currentSetting = NearestCondition(int.MaxValue);
        _currentSetting.settings[currentIndex].Apply(Cam);
    }

    public void SetProgress(float progressValue)
    {
        var index = Mathf.Clamp(Mathf.FloorToInt(progressValue), 0, _currentSetting.settings.Length - 2);
        var from = _currentSetting.settings[index];
        var to = _currentSetting.settings[index + 1];
        Cam.transform.localPosition = Vector3.Lerp(from.localPosition, to.localPosition, progressValue - index);
        Cam.transform.rotation = Quaternion.Lerp(from.rotation, to.rotation, progressValue - index);
        Cam.fieldOfView = Mathf.Lerp(from.fieldOfView, to.fieldOfView, to.fovCurve.Evaluate(progressValue - index));
    }
    
    public void ApplySetting(int index)
    {
        if(_currentSetting == null)
            _currentSetting = NearestCondition(int.MaxValue);
        _currentSetting.settings[index].Apply(Cam);
    }

    public FloatReactiveProperty progress;

    private PlayerCameraSettingByScreen NearestCondition(float epsilon = 0.001f)
    {
        var aspect = Cam.pixelWidth * 1f / Cam.pixelHeight;
        if (conditions.Count == 0)
        {
            UIDebugLog.LogError("not found any setting");
            return null;
        }

        var condition = conditions[0];
        for (int i = 1; i < conditions.Count; i++)
            if (Mathf.Abs(conditions[i].aspect - aspect) < Mathf.Abs(condition.aspect - aspect))
                condition = conditions[i];

        if (Mathf.Abs(aspect - condition.aspect) < epsilon)
            return condition;
        return null;
    }
    
    // _____________________________


//    [MyButtonRange(0, 4, "Copy", "copy")] public int testCopy;
//
//    [MyButtonRange(0, 4, "ApplySetting", "apply")]
//    public int testApply;
//

//
//    public void Copy(int index)
//    {
//        settings[index] = PlayerCameraSetting.Copy(cam);
//    }
}