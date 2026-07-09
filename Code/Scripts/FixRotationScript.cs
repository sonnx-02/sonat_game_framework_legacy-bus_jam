using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixRotationScript : MonoBehaviour
{
   
    // Update is called once per frame
    void LateUpdate()
    {
        transform.eulerAngles = Vector3.zero;
    }
}
