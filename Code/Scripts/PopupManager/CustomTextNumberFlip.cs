using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CustomTextNumberFlip : ProgressScript
{
    [SerializeField] private Sprite[] numberTops;
    [SerializeField] private Sprite[] numberBots;

    [SerializeField] private Image bot;
    [SerializeField] private Image botNext;
    [SerializeField] private Image top;
    [SerializeField] private Image topNext;

    [SerializeField] private Vector3 eulerRotateBot;
    [SerializeField] private Vector3 eulerRotateTop;

 
    protected override void Trigger(float val)
    {
        base.Trigger(val);
        int intVal = Mathf.FloorToInt(val);
        if (intVal < 0) return;
        botNext.sprite = numberBots[intVal];
        topNext.sprite = numberTops[intVal+1];
        bot.sprite = numberBots[intVal+1];
        top.sprite = numberTops[intVal];

        float interval = 1- (val - intVal);
        var lerp1 = interval * 2f;
        var lerp2 = (interval - 0.5f)*2;

        if (lerp1 < 1 && lerp1 > 0)
        {
            topNext.gameObject.SetActive(true);
            topNext.transform.localEulerAngles = Vector3.Lerp(Vector3.zero, eulerRotateTop,lerp1);
        }
        else
        {
            topNext.gameObject.SetActive(false);
        }
        
        if (lerp2 > 0 && lerp2 <= 1)
        {
            botNext.gameObject.SetActive(true);
            botNext.transform.localEulerAngles = Vector3.Lerp(Vector3.zero, eulerRotateTop,lerp1);
        }
        else
        {
            botNext.gameObject.SetActive(false);
        }
        
        botNext.transform.localEulerAngles = Vector3.Lerp(eulerRotateBot,Vector3.zero, lerp2);
    }
}
