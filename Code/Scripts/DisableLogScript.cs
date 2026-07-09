using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableLogScript : MonoBehaviour
{
    public bool logEnable ;
    // Start is called before the first frame update
    void Awake()
    {
#if !UNITY_EDITOR
        Debug.unityLogger.logEnabled = PlayerPrefs.GetInt("logEnabled",logEnable ? 1 : 0) == 1;
#endif
    }

    [ContextMenu("test")]
    void test()
    {
        Debug.unityLogger.logEnabled = true;
    }
}
