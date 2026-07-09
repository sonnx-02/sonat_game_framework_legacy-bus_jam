using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[System.Serializable]
public struct From3DToCanvasPoolDescriptor
{
    [IndexAsEnum("From3DToCanvasEnum")]
    public int key;
    public From3DToCanvas prefab;
    public int prePool;
}

public class From3DToCanvas : ParameterOnRentPoolItem
{
    [SerializeField] protected int index;
    public override int Index => index;
    
    public RectTransform rectTransform;
    [SerializeField] private Vector2 offset;

    public InvokeScript[] startScripts;

    [SerializeField] private float delayToAnimIn;
    [SerializeField] private BaseRxAnimator[] rxAnimators;

    public void InvokeStartScripts()
    {
        foreach (var invokeScript in startScripts)
            invokeScript.StartScript();
    }


    public override void OnRent()
    {
        base.OnRent();
        delayToAnimIn.Timer(() =>
        {
            foreach (var baseRxAnimator in rxAnimators)
                baseRxAnimator.Animator_In();
        });
    }

    public void SetPosFromWorld(Vector3 worldPos)
    {
        rectTransform.anchoredPosition = RootView.rootView.screenRoot.WorldObjectScreenPosition(worldPos) + offset;
    }

    private void UpdatePos(Camera inputCamera, Vector3 worldPosition, Vector2 canvasSize)
    {
        rectTransform.anchoredPosition = inputCamera.WorldToAnchorPos(worldPosition,canvasSize) + offset;
    }
    
    // ___________________________ test
    public bool enableTest;
    [ConditionalField(nameof(enableTest))]
    [SerializeField] private Transform target3D;
    [ConditionalField(nameof(enableTest))]
    [SerializeField] private Camera cam;
    [ConditionalField(nameof(enableTest))]
    [SerializeField] private Canvas canvas;
    [ConditionalField(nameof(enableTest))]
    [MyButton(nameof(Test),nameof(TestRuntime))]
    public bool test;
    public void Test()
    {
        UpdatePos(cam,target3D.position,canvas.GetComponent<RectTransform>().sizeDelta);
    }
    public void TestRuntime()
    {
        rectTransform.anchoredPosition = RootView.rootView.screenRoot.WorldObjectScreenPosition(target3D.position) + offset;
    }
    // test
    public override void SetParams(Point3Float para)
    {
        
    }
}