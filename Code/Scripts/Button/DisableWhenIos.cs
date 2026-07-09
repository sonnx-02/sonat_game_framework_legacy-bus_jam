using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableWhenIos : MonoBehaviour
{

    [SerializeField] private bool whenIos;
    [SerializeField] private bool whenAndroid;
    // Start is called before the first frame update
    private void Start()
    {
        #if !UNITY_EDITOR && UNITY_IOS
            gameObject.SetActive(whenIos);
        #endif
        
#if !UNITY_EDITOR && UNITY_ANDROID
            gameObject.SetActive(whenAndroid);
#endif
    }
}
