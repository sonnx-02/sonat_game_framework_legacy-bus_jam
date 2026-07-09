using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class EditorFunctionExecute : MonoBehaviour {

    public UnityEvent MyEvent;

    public void Execute()
    {
        MyEvent.Invoke();
        gameObject.SetActive(false);
        gameObject.SetActive(true);
    }
}
