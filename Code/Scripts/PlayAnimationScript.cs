using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayAnimationScript : MonoBehaviour
{
    [FormerlySerializedAs("TriggerName")] [SerializeField]   protected string triggerName;
    [FormerlySerializedAs("After")] [SerializeField] protected float after;
    
    public void Invoke()
    {
        Invoke(nameof(PlayAnimation),after);
    }

    private void PlayAnimation()
    {
        
    }
    
}
