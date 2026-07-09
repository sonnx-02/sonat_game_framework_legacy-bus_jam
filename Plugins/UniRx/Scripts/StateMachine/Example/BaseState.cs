using System.Collections;
using System.Collections.Generic;
using HC.AI;
using UniRx;
using UnityEngine;

public abstract class BaseState : State
{
    [SerializeField] private bool enableOnUpdate = true;

    public virtual bool EnableOnUpdate => enableOnUpdate;
    private void Start()
    {
        BeginStream.Subscribe(OnEnter);
        if(EnableOnUpdate)
            UpdateStream.Subscribe(OnUpdate);
        //      FixedUpdateStream.Subscribe(_ => Debug.Log("State FixedUpdate"));
        //      LateUpdateStream.Subscribe(_ => Debug.Log("State LateUpdate"));
        EndStream.Subscribe(OnExit);
        //      OnDrawGizmosStream.Subscribe(_ => Debug.Log("State OnDrawGizmos"));
        //     OnGUIStream.Subscribe(_ => Debug.Log("State OnGUI"));
    }

    protected virtual void OnEnter(Unit u)
    {
        
    }
    
    protected virtual  void OnUpdate(Unit u)
    {
        
    }
    
    protected virtual  void OnExit(Unit u)
    {
        
    }
}
