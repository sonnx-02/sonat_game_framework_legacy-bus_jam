using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WideScreenAdjuster : MonoBehaviour
{
    
    public float Ratio = 0.65f;
    public float Default = 0.5625f;// => 9/16

    void Awake()
    {
        SetPos();
    }

    public void SetPos()
    {
        var aspect = Camera.main.pixelWidth * 1f / Camera.main.pixelHeight;
        if (Ratio < aspect)
        {
            var ratioFitter = gameObject.AddComponent<AspectRatioFitter>();
            ratioFitter.aspectMode = AspectRatioFitter.AspectMode.WidthControlsHeight;
            ratioFitter.aspectRatio = Default;
            transform.localScale = new Vector3(aspect, aspect, 0);
        }
    }


    public void FitParent()
    {
        var rt = transform as RectTransform;

        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.anchoredPosition = Vector2.zero;
        rt.sizeDelta = Vector2.zero;
    }
}
