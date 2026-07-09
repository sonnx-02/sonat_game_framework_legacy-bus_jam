using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ToggleScriptTriggerActionScript : ToggleScript
{
    public MonoBehaviour[] scripts;
    
    protected override void LoadComponent()
    {
      var s =  GetComponents<MonoBehaviour>();
      scripts = s.Where(x => x is IActionScript).ToArray();
    }
    
    protected override void Trigger(bool val)
    {
        if(val)
            foreach (var monoBehaviour in scripts)
            {
                if (monoBehaviour is IActionScript)
                {
                    (monoBehaviour as IActionScript).StartAction();
                }
            }
    }
}
