using UnityEngine;
using UnityEngine.UI;

public class ButtonNextLevel : BaseActionScript
{
    public Button reinitLevel;
    protected override void DoAction()
    {
        base.DoAction();
        reinitLevel.onClick.Invoke();
        ScreenRoot.popupManager.GetPopup((int)InScreenPopupEnum.Win).gameObject.SetActive(false);
    }
}