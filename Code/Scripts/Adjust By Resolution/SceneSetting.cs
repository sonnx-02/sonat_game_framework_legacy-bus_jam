using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ScenceSettingTargetConfig
{
    public bool active = true;
    public Vector3 position;
    public Vector3 scale;
    public Quaternion rotation;
}

[System.Serializable]
public abstract class ScenceSettingTargetBase
{
    public abstract void Apply(int i);
    public abstract void Add(int i);
}

[System.Serializable]
public class ScenceSettingTarget : ScenceSettingTargetBase
{
    public Transform target;
   // [HideInInspector] 
    public List<ScenceSettingTargetConfig> configs;
    public bool enableActive = true;

    public override void Apply(int i)
    {
        if(target == null)
            return;
        var config = configs[i];
        if(enableActive)
            target.gameObject.SetActive(config.active);
        target.localPosition = config.position;
        target.localScale = config.scale;
        target.rotation = config.rotation;
 
    }

    public override void Add(int i)
    {
        ScenceSettingTargetConfig config = null;
        while (i >= configs.Count)
        {
            config = new ScenceSettingTargetConfig();
            configs.Add(config);
        }

        config = configs[i];
        config.position = target.localPosition;
        config.scale = target.localScale;
        config.rotation = target.rotation;
        config.active = target.gameObject.activeSelf;
    }
}

public class SceneSetting : BaseFrameworkView
{
    [ArrayElementTitleByName(nameof(ScenceSettingTarget.target))]
    public ScenceSettingTarget[] targets;
    public SceneSettingTargetScriptBase[] others;
    public TransformByResolution cameraSettings;

    [MyButtonRange(0,10,nameof(ApplyPreset),null,"Apply")] [SerializeField]
    private int testApplyPreset;
    [MyButtonRange(0,10,nameof(AddPresetTo),null,"Add")] [SerializeField]
    private int testAddPreset;


    public int currentSetting;
    public void ApplyPreset(int index)
    {
        currentSetting = index;
        foreach (var scenceSettingTarget in targets)
            scenceSettingTarget.Apply(index);
        foreach (var scenceSettingTarget in others)
            scenceSettingTarget.Apply(index);
        if (cameraSettings != null)
            cameraSettings.ApplySetting(index, 0);
    }

    
    public void ApplyPreset(int index,bool cam = true)
    {
        currentSetting = index;
        foreach (var scenceSettingTarget in targets)
            scenceSettingTarget.Apply(index);
        foreach (var scenceSettingTarget in others)
            scenceSettingTarget.Apply(index);
        if (cameraSettings != null && cam)
            cameraSettings.ApplySetting(index, 0);
    }

    public void AddPresetTo(int index)
    {
        foreach (var scenceSettingTarget in targets)
            scenceSettingTarget.Add(index);
        foreach (var scenceSettingTarget in others)
            scenceSettingTarget.Add(index);
        this.SetDirty();
    }
}