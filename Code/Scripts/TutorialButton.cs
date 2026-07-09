using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialButton : BaseFrameworkView
{
    [IndexAsEnum(BuiltInEnumType.TutorialButtonEnum)]
    [SerializeField] private int buttonEnum;
    [SerializeField] private Button button;
    [SerializeField] private bool isInPopup;
    [SerializeField] private Vector2 offset;
    [SerializeField] private bool timeScaleZero;
    [SerializeField] private bool disableInput;
    [SerializeField] private float delayToStart;
    [SerializeField] private bool disableOtherRaycaster = true;

    public bool DisableOtherRaycaster => disableOtherRaycaster;
    public int ButtonEnum => buttonEnum;
    public Button Button => button;
    public Vector2 Offset => offset;

    public float DelayToStart => delayToStart;

    public bool TimeScaleZero => timeScaleZero;

    public bool DisableInput => disableInput;

    public void Focus(bool b)
    {
        if (b)
        {
            gameObject.AddComponent<Canvas>();
            gameObject.AddComponent<GraphicRaycaster>();
        }
        else
        {
            Destroy(gameObject.GetComponent<GraphicRaycaster>());
            Destroy(gameObject.GetComponent<Canvas>());
        }
    }
}