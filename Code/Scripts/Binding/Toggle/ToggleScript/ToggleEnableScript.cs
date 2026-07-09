using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleEnableScript : ToggleScript
{
    [SerializeField] private MonoBehaviour[] monoBehaviours;
    [SerializeField] private Collider[] colliders;
    // Start is called before the first frame update
    protected override void LoadComponent()
    {
       var x  = GetComponents<MonoBehaviour>();
       monoBehaviours = new MonoBehaviour[1];
       monoBehaviours[0] = x[1];
    }

    [MyButtonInt(nameof(LoadAt))]
    [SerializeField] private int testLoad;

    private void LoadAt(int load)
    {
        var x  = GetComponents<MonoBehaviour>();
        monoBehaviours = new MonoBehaviour[1];
        monoBehaviours[0] = x[load];
    }

    protected override void Trigger(bool val)
    {
        foreach (var monoBehaviour in monoBehaviours)
        {
#if UNITY_EDITOR
            if (monoBehaviour == null)
                Debug.LogError("null on" + gameObject, gameObject);
#endif
            if (monoBehaviour != null)
                monoBehaviour.enabled = val;
        }
        foreach (var monoBehaviour in colliders)
        {
            monoBehaviour.enabled = val;
        }
    }

}
