using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeactiveOnDevice : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
#if !UNITY_EDITOR
        gameObject.SetActive(false);
#endif
    }

    
}
