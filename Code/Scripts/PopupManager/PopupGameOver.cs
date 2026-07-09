using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupGameOver : BasePopup
{
    
    #if UNITY_EDITOR
    [MyButton(nameof(AddActionScoreTween))] [SerializeField]
    private int addHandler;

    
    public void AddActionScoreTween()
    {
        gameObject.AddComponent<DependencyGameOverScoreTween>();
    }
    #endif
    
    public override void OnShowHandler(WhenPopupClose whenPopupClosed)
    {
        base.OnShowHandler(whenPopupClosed);
    }
}
