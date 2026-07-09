using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MouseScriptAlwaysOn : MonoBehaviour
{
    [SerializeField] private Sprite on;
    [SerializeField] private Sprite off;
    [SerializeField] private Image img;

    private void Awake()
    {
        (transform as RectTransform).anchorMin = Vector2.zero;
        (transform as RectTransform).anchorMax = Vector2.zero;
#if !UNITY_EDITOR
        gameObject.SetActive(false);
#endif
    }

    public int type;
    // Update is called once per frame
    void Update()
    {
        if (Camera.main != null)
        {
            var screenPoint = Input.mousePosition;
            // Debug.Log(Input.mousePosition);
            //   screenPoint.z = Camera.main.depth; //distance of the plane from the camera
            var x = Camera.main.ScreenToWorldPoint(screenPoint);
            // Debug.Log(x);
            var screenPoint2 = Camera.main.WorldToScreenPoint(x);
            (transform as RectTransform).anchoredPosition = type == 0 ? screenPoint2 : screenPoint;
            img.sprite = Input.GetMouseButton(0) ? @on : off;
        }
    }
}