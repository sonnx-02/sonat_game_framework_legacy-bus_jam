
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocationManager : BaseView
{
    public List<LocationItem> locations;
    private readonly Dictionary<LocationEnum,RectTransform> _dictionary = new Dictionary<LocationEnum, RectTransform>();

    
    protected override void Register()
    {
        base.Register();
        locations.ForEach(x =>
        {
            if(x != null)
                _dictionary.Add(x.locationEnum, x.transform as RectTransform);
            else
            {
                Debug.LogError("null",gameObject);
            }
        });
    }


    public Vector3 GetCanvasPosition(LocationEnum key)
    {
        return DuongExtensions.WorldToUISpace(RootView.rootView.screenRoot.canvas, _dictionary[key].position);
    }
    
    public RectTransform GetLocation(LocationEnum key)
    {
        CheckRegister();
        if(_dictionary.ContainsKey(key))
            return _dictionary[key];
        return null;
    }
    
    public Vector3 GetCanvasPosition(Vector3 worldPosition)
    {
        return DuongExtensions.WorldToUISpace(RootView.rootView.screenRoot.canvas, worldPosition);
    }
    
    public Vector3 GetRelativeLocalPosition(LocationEnum key,Transform parent)
    {
        return (parent as RectTransform).InverseTransformPoint(_dictionary[key].position);
    }
    
    
    public RectTransform TestRectTransform;
    [ContextMenu("Test")]
    public void Test()
    {
        Debug.Log(TestRectTransform.position + "" + DuongExtensions.WorldToUISpace(RootView.rootView.screenRoot.canvas, TestRectTransform.position));
    }
}