using System.Collections;
using System.Collections.Generic;
using HC.AI;
using UniRx;
using UnityEngine;

public class PopupIn : BaseState
{
  protected override void OnEnter(Unit u)
  {
    base.OnEnter(u);
    Debug.Log("enter PopupIn");
  }
}
