using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RxAnimatorUpdatePos : MonoBehaviour
{
    [SerializeField] private bool updateX;
    [SerializeField] private bool updateY;
    [SerializeField] private bool clamp;
    [SerializeField] private Vector2 clampX;

    public void SetLocalPos(Vector3 pos)
    {
        if (!updateX)
            pos.x = transform.localPosition.x;
        if (!updateY)
            pos.y = transform.localPosition.y;

        if (clamp)
        {
            pos.x = Mathf.Clamp(pos.x,clampX.x, clampX.y);
        }

        transform.localPosition = pos;
    }
}
