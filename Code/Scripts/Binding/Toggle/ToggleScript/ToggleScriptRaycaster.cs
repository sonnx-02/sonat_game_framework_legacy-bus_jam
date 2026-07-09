using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleScriptRaycaster : ToggleScript
{
    [SerializeField] private GraphicRaycaster graphicRaycaster;
    // Start is called before the first frame update
    protected override void LoadComponent()
    {
        graphicRaycaster = GetComponent<GraphicRaycaster>();
    }

   

    protected override void Trigger(bool val)
    {
        graphicRaycaster.enabled = val;
    }

}
