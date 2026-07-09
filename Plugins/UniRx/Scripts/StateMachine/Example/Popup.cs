using System.Collections;
using System.Collections.Generic;
using HC.AI;
using UnityEngine;

public class Popup : StateMachine
{
    [ContextMenu("ChangePopupOut")]
    public void ChangePopupOut()
    {
        Transition<PopupOut>();
    }

    [ContextMenu("ChangePopupIn")]
    public void ChangePopupIn()
    {
        Transition<PopupIn>();
    }
}