using UnityEngine;
using UniRx;
using HC.AI;


[DisallowMultipleComponent]
public class Example : State
{
    #region event

    private void Start()
    {
        BeginStream.Subscribe(_ => Debug.Log("State Begin"));
   //     UpdateStream.Subscribe(_ => Debug.Log("State Update"));
  //      FixedUpdateStream.Subscribe(_ => Debug.Log("State FixedUpdate"));
  //      LateUpdateStream.Subscribe(_ => Debug.Log("State LateUpdate"));
        EndStream.Subscribe(_ => Debug.Log("State End"));
  //      OnDrawGizmosStream.Subscribe(_ => Debug.Log("State OnDrawGizmos"));
   //     OnGUIStream.Subscribe(_ => Debug.Log("State OnGUI"));
    }

    #endregion
}