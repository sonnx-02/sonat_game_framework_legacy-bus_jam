using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BasePopupDependency: BaseScreenDependency
{
    public void SetParent(BasePopup p)
    {
        ParentPopup = p;
    }
    protected BasePopup ParentPopup;
    public virtual bool ReadyToShow() => false;
}
