using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasPositionFromWorld3D : MonoBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField] private bool startOnEnable;
    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private Transform copyFrom;
    [SerializeField] private Bool3 copy = new Bool3(true, true, true);
    [SerializeField] private Vector2 offset;

    [MyButton(nameof(SetPos))] [SerializeField]
    private int myFunction;

    public void SetPosFromTransform(Transform target)
    {
        ScreenRoot screenRoot = null;
#if UNITY_EDITOR
        screenRoot = FindObjectOfType<ScreenRoot>();
#endif

        if (screenRoot == null)
            screenRoot = RootView.rootView.screenRoot;

        screenRoot.UpdateSize();
        rectTransform.anchoredPosition = screenRoot.WorldObjectScreenPosition(target.position) + offset;
        if (cam == null)
        {
            var position = rectTransform.position;
            var position1 = target.position;
            target.position
                = new Vector3(
                    copy.value1 ? position.x : position1.x,
                    copy.value2 ? position.y : position1.y,
                    copy.value3 ? position.z : position1.z
                );
        }
    }

    private void SetPos()
    {
        SetPosFromTransform(copyFrom);
    }

    private void Reset()
    {
        rectTransform = GetComponent<RectTransform>();
    }
}