using UnityEngine.UI;
using UnityEngine;

public class ButtonPlay : BaseButton
{
    public Button fakeBtnPlay;

    protected override void OnClick()
    {
        base.OnClick();
        OnClickHandler();
    }

    protected virtual void OnClickHandler()
    {
        PlayerData.currentLevel.Value++;
        fakeBtnPlay.onClick.Invoke();
    }
}
