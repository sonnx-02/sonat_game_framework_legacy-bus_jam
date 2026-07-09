using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class ImageColorChange
{
    public Image[] targets;
    public TMP_Text[] targets2;
    public Color colorOn = Color.white;
    public Color colorOff = new Color(0.7f,.7f,.7f);

    public void Toggle(bool value)
    {
        foreach (var target in targets)
        {
            if(target != null)
                target.color = value ? colorOn : colorOff;
            #if UNITY_EDITOR
            else 
                Debug.LogError("null on targets");
            #endif
        }

        foreach (var target in targets2)
        {
            if(target != null)
                target.color = value ? colorOn : colorOff;
#if UNITY_EDITOR
            else 
                Debug.LogError("null on targets2");
#endif
        }
    }
}

public class ToggleScriptInteractable : ToggleScript
{

    [SerializeField] private Button button;
    [SerializeField] private Image[] targets;
    [SerializeField] private Material grayMaterial;
    [SerializeField] private Sprite onSprite;
    [SerializeField] private Sprite offSprite;
    [SerializeField] private ImageColorChange colorChange;

    
    protected override void Trigger(bool val)
    {
        button.interactable = val;
        if (grayMaterial != null)
            foreach (var target in targets)
                target.material = val ?  null : grayMaterial;
        if (onSprite != null && offSprite != null)
            foreach (var target in targets)
                target.sprite = val ?  onSprite : offSprite;
        colorChange.Toggle(val);
    }


    protected override void LoadComponent()
    {
        button = GetComponent<Button>();
        colorChange = new ImageColorChange();
        colorChange.targets = GetComponentsInChildren<Image>();
        colorChange.targets2 = GetComponentsInChildren<TMP_Text>();
    }
}
