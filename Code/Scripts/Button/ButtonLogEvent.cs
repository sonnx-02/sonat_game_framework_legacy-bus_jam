using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonLogEvent : MonoBehaviour
{
    public string eventName;
    public string paramName;
    public string paramValue;

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Button>().onClick.AddListener(() =>
        {
            if(!string.IsNullOrEmpty(eventName))
                Kernel.Resolve<FireBaseController>().LogEvent(eventName,new LogParameter(paramName,paramValue));
        });
    }

   
}
