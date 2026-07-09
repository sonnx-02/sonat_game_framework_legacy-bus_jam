using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PopupTest : BasePopup
{
    public bool skipPassword;
    public List<int> password;
    public GameObject activeWhenFill;
    public GameObject activeWhenNotFill;

   [SerializeField] private List<int> temp;
   
    protected override void Register()
    {
        base.Register();
        temp = password.ToArray().ToList();
    }

    public override void OnShowHandler(WhenPopupClose whenPopupClosed)
    {
        RootView.testEnable.BoolValue = true;
        base.OnShowHandler(whenPopupClosed);
        CheckRegister();
        if (skipPassword)
            temp = new List<int>();
        activeWhenFill.SetActive(temp != null && temp.Count == 0);
        activeWhenNotFill.SetActive(!activeWhenFill.activeSelf);
    }

    public void AddCharacter(int character)
    {
        if (temp != null && temp.Count > 0)
        {
            if (temp[0] == character)
            {
                temp.RemoveAt(0);
                activeWhenFill.gameObject.SetActive(temp != null && temp.Count == 0);
                activeWhenNotFill.SetActive(!activeWhenFill.activeSelf);
            }
            else
            {
                temp = password.ToArray().ToList();
            }
        }
    }
}
