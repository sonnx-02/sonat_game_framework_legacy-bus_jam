using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIRayCastPosition : BaseFrameworkView
{
    [SerializeField] private Camera cam;
    [SerializeField] private ScreenRoot screenRoot;
    [SerializeField] private Collider hitCollider;
    [SerializeField] private Transform target;

    public Transform Target => target;

    public float RateFromBottom()
    {
        var viewPort = cam.WorldToViewportPoint(transform.position);
        return viewPort.y;
    }

    public void Cast()
    {
        bool hitPlane = cam.HitPositionCastFrom(transform.position,hitCollider, out Vector3 point);
        if (hitPlane)
        {
//            Debug.LogError(hit.point);
            if (target != null)
                target.position = point;
        }
    }

    public override void LogCustom()
    {
        base.LogCustom();
        Cast();


//      ray = cam.ScreenPointToRay(position);
//        
//      bool hitPlane2 = (hitCollider.Raycast(ray, out RaycastHit hit2, 1000));
//      if (hitPlane2)
//      {
//         Debug.LogError(hit2.point);
//      }
//      
//      if (Physics.Raycast(ray, out hit)) {
//         Transform objectHit = hit.transform;
//            
//         // Do something with the object that was hit by the raycast.
//      }

//      Vector2 localPos;
//      RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas, screenPos, Camera.main, out localPos);
    }

    public void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var ray = cam.ScreenPointToRay(Input.mousePosition);
            Debug.Log(Input.mousePosition);
            bool hitPlane = (hitCollider.Raycast(ray, out RaycastHit hit, 1000));
            if (hitPlane)
            {
                if (target != null)
                    target.position = hit.point;
            }
        }

    }
}